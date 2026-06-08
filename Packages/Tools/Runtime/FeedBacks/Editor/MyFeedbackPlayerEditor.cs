using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MyFeedbackPlayer))]
public class MyFeedbackPlayerEditor : Editor
{
    #region References

    protected MyFeedbackPlayer _target;
    protected SerializedProperty _feedbacksProperty;
    protected VisualElement _root;
    protected ListView _listView;
    protected Label _listTitle;

    public StyleSheet StyleSheetBase;
    public StyleSheet StyleSheetControls;
    public StyleSheet StyleSheetFeedbacksList;
    public StyleSheet StyleSheetFoldouts;
    public StyleSheet StyleSheetSettings;
    public StyleSheet StyleSheetLightSkin;

    // Props gérées dans le header — jamais affichées dans le corps du feedback
    protected static readonly HashSet<string> _headerPropNames = new HashSet<string>
    {
        "Active", "IsExpanded", "Label", "Owner"
    };

    // Cache statique : la Reflection ne tourne qu'une seule fois par session Editor
    private static List<(string path, Type type)> _feedbackTypes;

    #endregion

    #region Lifecycle

    /// <summary>
    /// Appelé quand l'Inspector s'ouvre ou que la sélection change.
    /// On récupère les références et on s'abonne à l'Undo.
    /// </summary>
    protected virtual void OnEnable()
    {
        _target = (MyFeedbackPlayer)target;
        _feedbacksProperty = serializedObject.FindProperty("Feedbacks");
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    /// <summary>
    /// Appelé quand l'Inspector se ferme ou que la sélection change.
    /// On se désabonne de l'Undo pour éviter les fuites mémoire.
    /// </summary>
    protected virtual void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    /// <summary>
    /// Appelé par Unity quand l'utilisateur fait Ctrl+Z / Ctrl+Y.
    /// On resynchronise le serializedObject et on reconstruit la ListView
    /// pour refléter l'état après l'annulation.
    /// </summary>
    private void OnUndoRedo()
    {
        if (serializedObject == null || serializedObject.targetObject == null) return;
        serializedObject.Update();
        _feedbacksProperty = serializedObject.FindProperty("Feedbacks");
        RefreshListView();
    }

    #endregion

    #region Create GUI

    /// <summary>
    /// Point d'entrée Unity pour construire l'Inspector en UITK.
    /// Appelé une seule fois à l'ouverture de l'Inspector.
    /// On construit l'arbre visuel complet ici.
    /// </summary>
    public override VisualElement CreateInspectorGUI()
    {
        _root = new VisualElement();
        _root.AddToClassList("mmf-editor");

        // Application des stylesheets MMF (assignées dans l'Inspector de l'Editor lui-même)
        if (StyleSheetBase != null) _root.styleSheets.Add(StyleSheetBase);
        if (StyleSheetControls != null) _root.styleSheets.Add(StyleSheetControls);
        if (StyleSheetFeedbacksList != null) _root.styleSheets.Add(StyleSheetFeedbacksList);
        if (StyleSheetFoldouts != null) _root.styleSheets.Add(StyleSheetFoldouts);
        if (StyleSheetSettings != null) _root.styleSheets.Add(StyleSheetSettings);
        if (!EditorGUIUtility.isProSkin && StyleSheetLightSkin != null)
            _root.styleSheets.Add(StyleSheetLightSkin);

        // Titre "N FEEDBACKS"
        _listTitle = new Label();
        _listTitle.AddToClassList("mm-feedbacks-list-title");
        UpdateListTitle();
        _root.Add(_listTitle);

        // Liste des feedbacks
        BuildListView();
        _root.Add(_listView);

        // Barre du bas : boutons debug + ajout de feedback
        _root.Add(BuildBottomBar());

        return _root;
    }

    #endregion

    #region ListView

    /// <summary>
    /// Construit la ListView qui affiche les feedbacks.
    /// 
    /// Pourquoi une ListView plutôt qu'une boucle for ?
    /// - makeItem/bindItem : Unity réutilise les éléments visuels au lieu de les recréer,
    ///   ce qui est plus performant avec beaucoup de feedbacks.
    /// - reorderable = true : le drag & drop pour réordonner est géré nativement par Unity,
    ///   pas besoin de coder les boutons Move Up/Down manuellement.
    /// - DynamicHeight : chaque feedback peut avoir une hauteur différente selon ses propriétés.
    /// </summary>
    protected virtual void BuildListView()
    {
        _listView = new ListView();
        _listView.AddToClassList("mm-feedbacks-list");
        _listView.AddToClassList("mm-body-all");

        _listView.itemsSource = _target.Feedbacks;
        _listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        _listView.reorderable = true;
        _listView.reorderMode = ListViewReorderMode.Animated;
        _listView.selectionType = SelectionType.None;

        // makeItem : crée un conteneur vide qui sera réutilisé par Unity
        _listView.makeItem = () => new VisualElement();

        // bindItem : remplit ce conteneur avec le feedback à l'index donné
        _listView.bindItem = (element, index) =>
        {
            element.Clear();
            if (index >= _target.Feedbacks.Count) return;
            element.Add(DrawFeedbackItem(index));
        };

        // unbindItem : vide le conteneur quand Unity le recycle pour un autre index
        _listView.unbindItem = (element, index) => element.Clear();

        // Après un drag & drop de reorder, on sauvegarde et on rafraîchit
        _listView.itemIndexChanged += (oldIndex, newIndex) =>
        {
            Undo.RecordObject(_target, "Reorder Feedback");
            serializedObject.ApplyModifiedProperties();
            RefreshListView();
        };
    }

    /// <summary>
    /// Resynchronise la ListView avec l'état actuel de _target.Feedbacks.
    /// Utilisé après chaque modification (ajout, suppression, reorder, undo).
    /// Rebuild() demande à Unity de rebinder tous les éléments visibles.
    /// </summary>
    protected virtual void RefreshListView()
    {
        serializedObject.Update();
        _listView.itemsSource = _target.Feedbacks;
        _listView.Rebuild();
        UpdateListTitle();
    }

    /// <summary>
    /// Met à jour le label "N FEEDBACKS" au-dessus de la liste.
    /// </summary>
    protected virtual void UpdateListTitle()
    {
        if (_listTitle != null)
            _listTitle.text = $"{_feedbacksProperty.arraySize} FEEDBACKS";
    }

    #endregion

    #region Draw Feedback Item

    /// <summary>
    /// Construit l'élément visuel complet pour un feedback à l'index donné.
    /// Structure :
    ///   group
    ///     ├── header [colorBar | checkbox | label | timing | menuButton]
    ///     └── inspector (contenu dépliable avec les groupes de propriétés)
    /// </summary>
    protected virtual VisualElement DrawFeedbackItem(int index)
    {
        SerializedProperty elementProp = _feedbacksProperty.GetArrayElementAtIndex(index);
        MyFeedback feedback = _target.Feedbacks[index];

        // Conteneur principal — la classe mm-mmf-group est ciblée par le CSS MMF
        VisualElement group = new VisualElement();
        group.AddToClassList("mm-mmf-group");
        group.style.marginBottom = 1;
        group.style.borderBottomWidth = 1;
        group.style.borderBottomColor = Color.black;

        // ── HEADER ──────────────────────────────────────────────────────────
        VisualElement header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.alignItems = Align.Center;
        header.style.backgroundColor = new Color(0f, 0f, 0f, 0.3f);
        header.style.height = 21; // hauteur fixe pour que la colorBar ne suive pas le contenu
        group.Add(header);

        // Barre colorée gauche — couleur définie par feedback.FeedbackColor
        // Hauteur fixe : elle représente le feedback, pas son contenu déplié
        VisualElement colorBar = new VisualElement();
        colorBar.AddToClassList("mm-feedback-left-border");
        colorBar.style.backgroundColor = feedback.FeedbackColor;
        colorBar.style.width = 6;
        colorBar.style.height = 21;
        colorBar.style.flexShrink = 0;
        header.Add(colorBar);

        // Checkbox Active — lie directement la propriété sérialisée
        PropertyField activeField = new PropertyField(elementProp.FindPropertyRelative("Active"), "");
        activeField.AddToClassList("mm-feedback-active-toggle");
        activeField.style.marginLeft = 2;
        activeField.style.marginRight = 2;
        header.Add(activeField);

        // Label du nom du feedback (en majuscules)
        Label feedbackLabel = new Label(feedback.Label.ToUpper());
        feedbackLabel.AddToClassList("mm-feedback-foldout-label");
        feedbackLabel.style.flexGrow = 1;
        feedbackLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        feedbackLabel.style.paddingLeft = 4;
        feedbackLabel.style.paddingTop = 2;
        feedbackLabel.style.paddingBottom = 2;
        header.Add(feedbackLabel);

        // Label de durée (utilise feedback.Duration défini dans MyFeedback)
        Label timingLabel = new Label($"{feedback.Duration:0.00}s");
        timingLabel.AddToClassList("mm-feedback-timing-label");
        timingLabel.style.position = Position.Relative;
        timingLabel.style.right = StyleKeyword.Auto;
        timingLabel.style.top = StyleKeyword.Auto;
        timingLabel.style.marginRight = 4;
        header.Add(timingLabel);

        // Bouton menu contextuel (⋮) — ouvre Duplicate/Move/Remove
        Button menuButton = new Button();
        menuButton.text = "⋮";
        menuButton.AddToClassList("mm-feedback-contextual-menu-button");
        menuButton.style.position = Position.Relative;
        menuButton.style.right = StyleKeyword.Auto;
        menuButton.clicked += () => OpenContextMenu(index, menuButton);
        header.Add(menuButton);

        // ── CONTENU DÉPLIABLE ────────────────────────────────────────────────
        // Affiché ou caché selon feedback.IsExpanded
        VisualElement inspector = new VisualElement();
        inspector.style.paddingLeft = 8;
        inspector.style.paddingRight = 6;
        inspector.style.paddingTop = 4;
        inspector.style.paddingBottom = 8;
        inspector.style.backgroundColor = new Color(1f, 1f, 1f, 0.05f);
        inspector.style.display = feedback.IsExpanded ? DisplayStyle.Flex : DisplayStyle.None;
        group.Add(inspector);

        // Remplissage des groupes de propriétés via Reflection
        FillInspectorProperties(inspector, elementProp, index);

        // Clic sur le header pour plier/déplier
        // On ignore les clics venant du checkbox et du bouton menu (ils ont leur propre action)
        header.RegisterCallback<ClickEvent>(evt =>
        {
            if (evt.target == activeField || evt.target == menuButton) return;
            Undo.RecordObject(_target, "Toggle Feedback Expand");
            feedback.IsExpanded = !feedback.IsExpanded;
            inspector.style.display = feedback.IsExpanded ? DisplayStyle.Flex : DisplayStyle.None;
            EditorUtility.SetDirty(_target);
        });

        return group;
    }

    #endregion

    #region Fill Inspector Properties

    /// <summary>
    /// Lit les champs sérialisés du feedback via Reflection et les regroupe
    /// selon leur attribut [InspectorGroup].
    ///
    /// Pourquoi la Reflection et pas SerializedProperty directement ?
    /// SerializedProperty ne connaît pas les attributs C# — elle voit juste
    /// les champs. La Reflection permet de lire [InspectorGroup] sur chaque
    /// champ et de créer dynamiquement les bons foldouts, exactement comme MMF.
    /// Résultat : ajouter un [InspectorGroup] sur un champ suffit,
    /// rien à changer dans l'editor.
    /// </summary>
    protected virtual void FillInspectorProperties(VisualElement inspector, SerializedProperty elementProp, int index)
    {
        Type feedbackType = _target.Feedbacks[index].GetType();

        // 1. Collecte tous les champs dans l'ordre base -> dérivé avec leur attribut de groupe
        var fieldList = new List<(string propName, InspectorGroupAttribute groupAttr)>();
        CollectFieldsWithGroups(feedbackType, fieldList);

        // 2. Construit la liste ordonnée de groupes
        var groupOrder = new List<string>();
        var groupMap = new Dictionary<string, (InspectorGroupAttribute attr, List<string> propNames)>();
        string currentGroupName = null;

        foreach (var (propName, groupAttr) in fieldList)
        {
            // On ignore les props gérées dans le header
            if (_headerPropNames.Contains(propName)) continue;

            // Nouveau groupe explicite : on crée une entrée dans la map
            if (groupAttr != null && groupAttr.GroupName != currentGroupName)
            {
                currentGroupName = groupAttr.GroupName;
                if (!groupMap.ContainsKey(currentGroupName))
                {
                    groupOrder.Add(currentGroupName);
                    groupMap[currentGroupName] = (groupAttr, new List<string>());
                }
            }

            // Aucun groupe défini encore : groupe "Settings" par défaut
            if (currentGroupName == null)
            {
                currentGroupName = "Settings";
                if (!groupMap.ContainsKey(currentGroupName))
                {
                    groupOrder.Add(currentGroupName);
                    groupMap[currentGroupName] = (null, new List<string>());
                }
            }

            // Ce champ appartient au groupe courant
            groupMap[currentGroupName].propNames.Add(propName);
        }

        // 3. Crée les foldouts dans l'ordre de déclaration
        foreach (string groupName in groupOrder)
        {
            var (attr, propNames) = groupMap[groupName];
            bool isOpen = attr == null || !attr.ClosedByDefault;
            Color barColor = Colors.GetColorAt(attr?.GroupColorIndex ?? 1);

            VisualElement groupContent = CreateMMFGroup(groupName, isOpen, inspector, barColor);

            foreach (string propName in propNames)
            {
                SerializedProperty prop = elementProp.FindPropertyRelative(propName);
                if (prop == null) continue;

                PropertyField field = new PropertyField(prop);
                field.label = ObjectNames.NicifyVariableName(propName);
                field.Bind(serializedObject);
                groupContent.Add(field);
            }
        }
    }

    /// <summary>
    /// Remonte la hiérarchie de classes de dérivé -> base, puis inverse
    /// pour obtenir les champs dans l'ordre base -> dérivé.
    /// 
    /// Pourquoi remonter manuellement ?
    /// GetFields() avec BindingFlags.DeclaredOnly ne retourne que les champs
    /// du type exact, pas ceux des classes parentes. On doit donc parcourir
    /// la chaîne d'héritage nous-mêmes.
    /// </summary>
    private void CollectFieldsWithGroups(Type type, List<(string, InspectorGroupAttribute)> result)
    {
        var typeChain = new List<Type>();
        Type t = type;
        while (t != null && t != typeof(object))
        {
            typeChain.Add(t);
            if (t == typeof(MyFeedback)) break;
            t = t.BaseType;
        }
        typeChain.Reverse(); // base d'abord -> dérivé ensuite

        foreach (Type currentType in typeChain)
        {
            FieldInfo[] fields = currentType.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                // Ignore les champs non-sérialisés par Unity
                if (!fi.IsPublic && fi.GetCustomAttribute<SerializeField>() == null) continue;
                if (fi.GetCustomAttribute<HideInInspector>() != null) continue;
                if (fi.GetCustomAttribute<NonSerializedAttribute>() != null) continue;

                InspectorGroupAttribute groupAttr = fi.GetCustomAttribute<InspectorGroupAttribute>();
                result.Add((fi.Name, groupAttr));
            }
        }
    }

    #endregion

    #region Create MMF Group

    /// <summary>
    /// Crée un foldout style MMF avec une barre colorée verticale à gauche.
    ///
    /// Structure : wrapper [flex-row]
    ///               ├── bar (3px, couleur de l'attribut, s'étire avec le foldout)
    ///               └── foldout (prend le reste de la largeur)
    ///
    /// La barre s'étire grâce à alignSelf: Stretch — elle suit la hauteur
    /// du foldout qu'il soit plié ou déplié, donnant le rendu MMF caractéristique.
    /// borderLeftWidth = 0 sur le foldout car le USS mm-foldout en ajoute un
    /// par défaut — notre barre colorée le remplace.
    /// </summary>
    protected VisualElement CreateMMFGroup(string groupName, bool expanded, VisualElement parent, Color barColor)
    {
        VisualElement wrapper = new VisualElement();
        wrapper.style.flexDirection = FlexDirection.Row;
        wrapper.style.marginTop = 3;

        VisualElement bar = new VisualElement();
        bar.style.width = 3;
        bar.style.flexShrink = 0;
        bar.style.backgroundColor = barColor;
        bar.style.alignSelf = Align.Stretch;
        wrapper.Add(bar);

        Foldout foldout = new Foldout();
        foldout.AddToClassList("mm-foldout");
        foldout.text = groupName;
        foldout.value = expanded;
        foldout.style.flexGrow = 1;
        foldout.style.borderLeftWidth = 0;

        Toggle toggle = foldout.Q<Toggle>();
        if (toggle != null) toggle.AddToClassList("mm-foldout-toggle");

        wrapper.Add(foldout);
        parent.Add(wrapper);

        VisualElement content = foldout.Q<VisualElement>("unity-content");
        return content ?? foldout;
    }

    protected VisualElement CreateMMFGroup(string groupName, bool expanded, VisualElement parent)
        => CreateMMFGroup(groupName, expanded, parent, Colors.GetColorAt(1));

    #endregion

    #region Bottom Bar

    /// <summary>
    /// Construit la barre du bas avec :
    /// - Les boutons debug (Initialize / Play / Stop) utilisables en Play Mode
    /// - Le bouton "Add New Feedback" qui ouvre le menu de sélection
    ///
    /// Pourquoi les boutons debug ?
    /// Ils permettent de tester les feedbacks directement depuis l'Inspector
    /// sans avoir à chercher dans la scène ou écrire du code de test.
    /// Ils sont désactivés hors Play Mode car Init() et Play() nécessitent
    /// que le MonoBehaviour soit actif (Awake doit avoir tourné).
    /// </summary>
    protected virtual VisualElement BuildBottomBar()
    {
        VisualElement root = new VisualElement();

        // Boutons debug
        VisualElement debugBar = new VisualElement();
        debugBar.style.flexDirection = FlexDirection.Row;
        debugBar.style.marginBottom = 2;

        Button initButton = new Button(() =>
        {
            foreach (var fb in _target.Feedbacks) fb.Init(_target);
        })
        { text = "Initialize" };

        Button playButton = new Button(() => _target.Play()) { text = "▶ Play" };
        playButton.style.backgroundColor = new StyleColor(new Color32(100, 123, 37, 255));

        Button stopButton = new Button(() =>
        {
            foreach (var fb in _target.Feedbacks)
                if (fb.Active) fb.Init(_target);
        })
        { text = "■ Stop" };

        // Désactivés hors Play Mode
        initButton.SetEnabled(Application.isPlaying);
        playButton.SetEnabled(Application.isPlaying);
        stopButton.SetEnabled(Application.isPlaying);

        debugBar.Add(initButton);
        debugBar.Add(playButton);
        debugBar.Add(stopButton);
        root.Add(debugBar);

        // Bouton Add New Feedback
        VisualElement addBar = new VisualElement();
        addBar.AddToClassList("mm-bottom-bar");
        Button addButton = new Button(() => AddFeedbackMenu()) { text = "Add New Feedback" };
        addButton.AddToClassList("mm-add-feedback-button");
        addBar.Add(addButton);
        root.Add(addBar);

        return root;
    }

    #endregion

    #region Context Menu

    /// <summary>
    /// Menu contextuel ouvert par le bouton ⋮ sur chaque feedback.
    /// Toutes les actions sont enregistrées dans l'Undo pour pouvoir
    /// être annulées avec Ctrl+Z.
    /// </summary>
    protected void OpenContextMenu(int index, VisualElement target)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Duplicate"), false, () =>
        {
            Undo.RecordObject(_target, "Duplicate Feedback");
            _feedbacksProperty.InsertArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            RefreshListView();
        });

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Move Up"), false, () =>
        {
            if (index > 0)
            {
                Undo.RecordObject(_target, "Move Feedback Up");
                _feedbacksProperty.MoveArrayElement(index, index - 1);
                serializedObject.ApplyModifiedProperties();
                RefreshListView();
            }
        });

        menu.AddItem(new GUIContent("Move Down"), false, () =>
        {
            if (index < _feedbacksProperty.arraySize - 1)
            {
                Undo.RecordObject(_target, "Move Feedback Down");
                _feedbacksProperty.MoveArrayElement(index, index + 1);
                serializedObject.ApplyModifiedProperties();
                RefreshListView();
            }
        });

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Remove"), false, () =>
        {
            Undo.RecordObject(_target, "Remove Feedback");
            _feedbacksProperty.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            RefreshListView();
        });

        menu.DropDown(target.worldBound);
    }

    #endregion

    #region Add Feedback Menu

    /// <summary>
    /// Découverte automatique de tous les types de feedbacks disponibles
    /// via Reflection sur tous les assemblies chargés.
    ///
    /// Pourquoi un cache statique (_feedbackTypes) ?
    /// La Reflection sur tous les assemblies est coûteuse. On la fait
    /// une seule fois par session Editor et on met le résultat en cache.
    /// Le cache est invalidé au prochain rechargement de domaine (recompilation).
    ///
    /// Seuls les types avec [FeedbackPath] apparaissent dans le menu.
    /// Les classes abstraites ou sans attribut sont ignorées.
    /// </summary>
    private static List<(string path, Type type)> GetFeedbackTypes()
    {
        if (_feedbackTypes != null) return _feedbackTypes;

        _feedbackTypes = new List<(string, Type)>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(MyFeedback))) continue;
                string path = FeedbackPathAttribute.GetPath(type);
                if (string.IsNullOrEmpty(path)) continue;
                _feedbackTypes.Add((path, type));
            }
        }

        // Tri alphabétique : les sous-menus ("Audio/...") sont regroupés automatiquement
        _feedbackTypes.Sort((a, b) => string.Compare(a.path, b.path, StringComparison.Ordinal));
        return _feedbackTypes;
    }

    /// <summary>
    /// Ouvre le menu "Add New Feedback" et instancie le type choisi.
    /// Activator.CreateInstance() crée une instance sans constructeur spécifique —
    /// c'est pourquoi tous les feedbacks doivent avoir un constructeur sans paramètres
    /// (ce qui est le cas par défaut en C# si aucun constructeur n'est défini).
    /// </summary>
    protected void AddFeedbackMenu()
    {
        GenericMenu menu = new GenericMenu();
        var types = GetFeedbackTypes();

        if (types.Count == 0)
        {
            menu.AddDisabledItem(new GUIContent("No feedbacks found — add [FeedbackPath] attribute"));
        }
        else
        {
            foreach (var (path, type) in types)
            {
                Type capturedType = type;
                menu.AddItem(new GUIContent(path), false, () =>
                {
                    Undo.RecordObject(_target, "Add Feedback");
                    _target.Feedbacks.Add((MyFeedback)Activator.CreateInstance(capturedType));
                    serializedObject.ApplyModifiedProperties();
                    RefreshListView();
                });
            }
        }

        menu.ShowAsContext();
    }

    #endregion
}
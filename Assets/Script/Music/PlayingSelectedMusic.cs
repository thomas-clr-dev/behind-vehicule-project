using UnityEngine;

public class PlayingSelectedMusic : MonoBehaviour
{
    #region SerializeField
    [Header("Selection")]
    [Tooltip("Index de l'AudioSource à jouer (0 = premier enfant)")]
    [SerializeField] private int _selectedAudioSourceIndex = 0;

    [Header("Contrôles")]
    [Tooltip("Volume pour l'AudioSource active")]
    [SerializeField] private float _activeVolume = 1f;
    #endregion

    #region Private Fields
    private AudioSource[] _childAudioSources;
    private int _previousIndex = -1;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        RefreshAudioSources();
        ApplySelection();
    }

    private void Update()
    {
        if (_selectedAudioSourceIndex != _previousIndex)
        {
            ApplySelection();
        }
    }
    #endregion

    #region Public Methods
    public void RefreshAudioSources()
    {
        _childAudioSources = GetComponentsInChildren<AudioSource>();

        if (_childAudioSources.Length == 0)
        {
            Debug.LogWarning("Aucune AudioSource trouvée dans les enfants!");
        }
        else
        {
            Debug.Log($"Trouvé {_childAudioSources.Length} AudioSource(s)");
        }
    }

    public void ApplySelection()
    {
        if (_childAudioSources == null || _childAudioSources.Length == 0)
        {
            RefreshAudioSources();
            if (_childAudioSources.Length == 0) return;
        }

        _selectedAudioSourceIndex = Mathf.Clamp(_selectedAudioSourceIndex, 0, _childAudioSources.Length - 1);
        _previousIndex = _selectedAudioSourceIndex;

        for (int i = 0; i < _childAudioSources.Length; i++)
        {
            AudioSource audioSource = _childAudioSources[i];

            if (i == _selectedAudioSourceIndex)
            {
                audioSource.mute = false;
                audioSource.volume = _activeVolume;
                
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                
                Debug.Log($"✅ Lecture: {audioSource.gameObject.name}");
            }
            else
            {
                audioSource.mute = true;
            }
        }
    }

    public void SelectByIndex(int index)
    {
        _selectedAudioSourceIndex = index;
        ApplySelection();
    }

    public void SelectNext()
    {
        _selectedAudioSourceIndex++;
        if (_selectedAudioSourceIndex >= _childAudioSources.Length)
        {
            _selectedAudioSourceIndex = 0;
        }
        ApplySelection();
    }

    public void SelectPrevious()
    {
        _selectedAudioSourceIndex--;
        if (_selectedAudioSourceIndex < 0)
        {
            _selectedAudioSourceIndex = _childAudioSources.Length - 1;
        }
        ApplySelection();
    }

    public AudioSource[] GetChildAudioSources()
    {
        return _childAudioSources;
    }

    public int GetSelectedIndex()
    {
        return _selectedAudioSourceIndex;
    }
    #endregion
}

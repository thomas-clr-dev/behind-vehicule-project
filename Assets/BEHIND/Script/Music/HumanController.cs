using UnityEngine;

public class HumanController : MonoBehaviour
{
    #region SerializeField
    [Header("Movement Settings")]
    [Tooltip("Vitesse de d�placement du joueur")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("Input Settings")]
    [Tooltip("Utiliser ZQSD (true) ou WASD (false)")]
    [SerializeField] private bool _useAZERTY = true;
    #endregion

    #region Private Fields
    private Vector2 _moveInput;
    private Rigidbody _rigidbody;
    private CharacterController _characterController;
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Essayer de r�cup�rer un Rigidbody
        _rigidbody = GetComponent<Rigidbody>();

        // Sinon essayer CharacterController
        if (_rigidbody == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

        if (_rigidbody == null && _characterController == null)
        {
            Debug.LogWarning("Aucun Rigidbody ou CharacterController trouv�! Ajoutez-en un pour le mouvement.");
        }
    }

    private void Update()
    {
        // R�cup�rer les inputs
        GetInputs();
    }

    private void FixedUpdate()
    {
        // Appliquer le mouvement (appel� � taux fixe pour la physique)
        Move();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// R�cup�re les inputs du clavier et de la manette
    /// </summary>
    private void GetInputs()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // Input clavier (ZQSD ou WASD)
        if (_useAZERTY)
        {
            // AZERTY : ZQSD
            if (Input.GetKey(KeyCode.Z)) vertical += 1f;   // Avant
            if (Input.GetKey(KeyCode.S)) vertical -= 1f;   // Arri�re
            if (Input.GetKey(KeyCode.Q)) horizontal -= 1f; // Gauche
            if (Input.GetKey(KeyCode.D)) horizontal += 1f; // Droite
        }
        else
        {
            // QWERTY : WASD
            if (Input.GetKey(KeyCode.W)) vertical += 1f;   // Avant
            if (Input.GetKey(KeyCode.S)) vertical -= 1f;   // Arri�re
            if (Input.GetKey(KeyCode.A)) horizontal -= 1f; // Gauche
            if (Input.GetKey(KeyCode.D)) horizontal += 1f; // Droite
        }

        // Input manette (joystick gauche)
        // Utilise les axes par d�faut de Unity
        float gamepadHorizontal = Input.GetAxis("Horizontal"); // Joystick horizontal
        float gamepadVertical = Input.GetAxis("Vertical");     // Joystick vertical

        // Combiner clavier et manette (priorit� � la manette si utilis�e)
        if (Mathf.Abs(gamepadHorizontal) > 0.1f || Mathf.Abs(gamepadVertical) > 0.1f)
        {
            horizontal = gamepadHorizontal;
            vertical = gamepadVertical;
        }

        _moveInput = new Vector2(horizontal, vertical);

        // Normaliser pour �viter la vitesse diagonale excessive
        if (_moveInput.magnitude > 1f)
        {
            _moveInput.Normalize();
        }
    }

    /// <summary>
    /// Applique le mouvement au joueur
    /// </summary>
    private void Move()
    {
        if (_moveInput.magnitude < 0.01f) return; // Pas de mouvement

        // Calculer la direction de mouvement
        Vector3 moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);

        // Appliquer le mouvement selon le composant disponible
        if (_characterController != null)
        {
            // Utiliser CharacterController
            _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);
        }
        else if (_rigidbody != null)
        {
            // Utiliser Rigidbody (avec v�locit� pour un mouvement plus fluide)
            Vector3 velocity = moveDirection * _moveSpeed;
            velocity.y = _rigidbody.linearVelocity.y; // Conserver la gravit�
            _rigidbody.linearVelocity = velocity;
        }
        else
        {
            // Fallback : transform.position (pas recommand� pour la physique)
            transform.position += moveDirection * _moveSpeed * Time.deltaTime;
        }
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (_moveInput.magnitude > 0.01f)
        {
            Gizmos.color = Color.green;
            Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);
            Gizmos.DrawRay(transform.position, direction * 2f);
        }
    }
    #endregion
}
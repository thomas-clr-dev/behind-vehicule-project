using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Version simplifiée pour activer des zones sans l'héritage CharacterAbility.
/// </summary>
public class CharacterButtonActivation : MonoBehaviour
{
    //[Header("Settings")]
    //public bool AbilityPermitted = true;
    //[Tooltip("Si vrai, le personnage ne pourra pas sauter (nécessite une ref au contrôleur)")]
    //public bool PreventJumpInButtonActivatedZone = true;

    //[Header("Status (Read Only)")]
    //public bool InButtonActivatedZone;
    //public bool InButtonAutoActivatedZone;
    //public ButtonActivated ButtonActivatedZone;

    //protected bool _activating = false;
    //protected Animator _animator;

    //// Remplacement du InputManager de MM par les entrées classiques Unity
    //// Tu peux modifier ces strings pour correspondre à ton Input Manager
    //public string InteractButtonName = "Interact";

    //protected virtual void Awake()
    //{
    //    _animator = GetComponentInChildren<Animator>();
    //}

    //protected virtual void Update()
    //{
    //    if (!AbilityPermitted) return;

    //    HandleInput();
    //}

    ///// <summary>
    ///// Remplace le HandleInput de l'Ability
    ///// </summary>
    //protected virtual void HandleInput()
    //{
    //    if (InButtonActivatedZone && (ButtonActivatedZone != null))
    //    {
    //        bool buttonPressed = false;

    //        // On simplifie la détection d'input
    //        switch (ButtonActivatedZone.InputType)
    //        {
    //            case ButtonActivated.InputTypes.Default:
    //                buttonPressed = Input.GetButtonDown(InteractButtonName);
    //                break;

    //            case ButtonActivated.InputTypes.Button:
    //                buttonPressed = Input.GetButtonDown(ButtonActivatedZone.InputButton);
    //                break;

    //            case ButtonActivated.InputTypes.Key:
    //                buttonPressed = Input.GetKeyDown(ButtonActivatedZone.InputKey);
    //                break;
    //        }

    //        if (buttonPressed)
    //        {
    //            ButtonActivation();
    //        }
    //    }
    //}

    ///// <summary>
    ///// Tries to activate the button activated zone
    ///// </summary>
    //protected virtual void ButtonActivation()
    //{
    //    if (InButtonActivatedZone && ButtonActivatedZone != null)
    //    {
    //        // Vérification simplifiée du sol (optionnel, selon ton contrôleur)
    //        // if (ButtonActivatedZone.CanOnlyActivateIfGrounded && !IsGrounded()) return;

    //        // Si c'est auto-activé, on ne fait rien manuellement
    //        if (ButtonActivatedZone.AutoActivation && !ButtonActivatedZone.AutoActivationAndButtonInteraction)
    //        {
    //            return;
    //        }

    //        // On lance l'action sur la zone
    //        ButtonActivatedZone.TriggerButtonAction();

    //        // On gère l'animation
    //        StartCoroutine(TriggerAnimation());
    //    }
    //}

    //private System.Collections.IEnumerator TriggerAnimation()
    //{
    //    _activating = true;
    //    if (_animator != null)
    //    {
    //        _animator.SetBool("Activating", true);

    //        // Si la zone a un paramètre d'animation spécial
    //        if (!string.IsNullOrEmpty(ButtonActivatedZone.AnimationTriggerParameterName))
    //        {
    //            _animator.SetTrigger(ButtonActivatedZone.AnimationTriggerParameterName);
    //        }
    //    }

    //    yield return new WaitForSeconds(0.1f); // Petite pause pour l'anim

    //    _activating = false;
    //    if (_animator != null)
    //    {
    //        _animator.SetBool("Activating", false);
    //    }
    //}

    //public virtual void ResetFlags()
    //{
    //    InButtonActivatedZone = false;
    //    ButtonActivatedZone = null;
    //    InButtonAutoActivatedZone = false;
    //}

    //// Gestion de la mort (si tu as un script de Santé à part)
    //public void OnDeath()
    //{
    //    ResetFlags();
    //}
}
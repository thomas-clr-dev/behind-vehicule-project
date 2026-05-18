using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class MyFeedback
{
    public bool Active = true;
    public string Label = "Effet";
    public float Delay = 0f;
    public Color FeedbackColor = Color.cyan;
    public bool IsExpanded = false; // Pour le pliage/dépliage
    public float Duration = 1f; // Durée de l'effet, utile pour les timelines ou les effets qui ont une durée



    [HideInInspector] public MyFeedbackPlayer Owner;

    public virtual void Init(MyFeedbackPlayer owner)
    {
        Owner = owner;
    }

    public virtual void Play()
    {
        if (!Active) return;

        Owner.StartCoroutine(PlayWithDelay());
    }

    private IEnumerator PlayWithDelay()
    {
        if (Delay > 0) yield return new WaitForSeconds(Delay);
        CustomPlay();
    }

    protected abstract void CustomPlay();

}

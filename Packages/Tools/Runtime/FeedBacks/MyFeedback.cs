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
    public bool IsExpanded = false;
    public float Duration = 1f;

    public bool Loop = false;
    public float DelayBetweenLoops = 0f;

    [HideInInspector] public MyFeedbackPlayer Owner;

    private Coroutine _playCoroutine;

    public virtual void Init(MyFeedbackPlayer owner)
    {
        Owner = owner;
    }

    public virtual void Play()
    {
        if (!Active) return;
        _playCoroutine = Owner.StartCoroutine(PlayWithDelay());
    }

    public virtual void Stop()
    {
        if (_playCoroutine != null)
        {
            Owner.StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }
        CustomStop();
    }

    public virtual void Reset()
    {
        Stop();
        CustomReset();
    }

    private IEnumerator PlayWithDelay()
    {
        if (Delay > 0) yield return new WaitForSeconds(Delay);

        do
        {
            Debug.Log($"Playing feedback: {Label}");
            CustomPlay();
            if (Loop)
            {
                yield return new WaitForSeconds(Duration + DelayBetweenLoops);
            }
        }
        while (Loop);

        _playCoroutine = null;
    }

    protected abstract void CustomPlay();
    protected virtual void CustomStop() { }
    protected virtual void CustomReset() { }
}
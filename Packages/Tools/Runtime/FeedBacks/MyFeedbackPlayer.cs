using System.Collections.Generic;
using UnityEngine;

public class MyFeedbackPlayer : MonoBehaviour
{
    [SerializeReference]
    public List<MyFeedback> Feedbacks = new List<MyFeedback>();


    private void Awake()
    {
        foreach (var feedback in Feedbacks)
            feedback.Init(this);
    }

    public void Play()
    {
        foreach (var feedback in Feedbacks)
            feedback.Play();
    }

    public void Stop()
    {
        foreach (var feedback in Feedbacks)
            feedback.Stop();
    }

    public void Reset()
    {
        foreach (var feedback in Feedbacks)
            feedback.Reset();
    }
}
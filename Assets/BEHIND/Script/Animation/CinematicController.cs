using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private UnityEvent OnCinematicStart;
    [SerializeField] private UnityEvent OnCinematicEnd;

    public void StartCinematic()
    {
        OnCinematicStart.Invoke();
        director.Play();
        director.stopped += OnDirectorStopped;
    }

    private void OnDirectorStopped(PlayableDirector d)
    {
        director.stopped -= OnDirectorStopped;
        OnCinematicEnd.Invoke();
    }
}
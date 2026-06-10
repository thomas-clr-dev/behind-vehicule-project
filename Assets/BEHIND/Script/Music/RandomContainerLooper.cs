using System.Collections;
using UnityEngine;

public class RandomContainerLooper : MonoBehaviour
{
    [Header("CLIPS DE MUSIQUE")]
    public AudioClip[] clips;

    [Header("PARAMETRES")]
    [Range(0, 1f)] public float volume = 1f;
    [Range(0, 5f)] public float fadeDuration = 1.5f;

    [HideInInspector] public AudioSource[] players;

    int current = 0;
    int lastClipIndex = -1;
    Coroutine loopRoutine;

    void Awake()
    {
        CreateSource();
        CreateSource();
        players = GetComponentsInChildren<AudioSource>();
    }

    void Start()
    {
        loopRoutine = StartCoroutine(PlayLoop());
    }

    void CreateSource()
    {
        Transform source = new GameObject("Source (pas toucher)").transform;
        source.parent = transform;
        source.localPosition = source.localEulerAngles = Vector3.zero;
        source.localScale = Vector3.one;
        AudioSource a = source.gameObject.AddComponent<AudioSource>();
        a.loop = false;
        a.spatialBlend = 0;
        a.volume = 0f;
        a.playOnAwake = false;
    }

    AudioClip GetRandomClip()
    {
        if (clips.Length == 1) return clips[0];
        int index;
        do { index = Random.Range(0, clips.Length); }
        while (index == lastClipIndex);
        lastClipIndex = index;
        return clips[index];
    }

    IEnumerator PlayLoop()
    {
        AudioSource active = players[current];
        active.clip = GetRandomClip();
        active.volume = 0f;
        active.Play();

        // Fade in initial
        float t = 0f;
        while (t < fadeDuration)
        {
            active.volume = Mathf.Lerp(0f, volume, t / fadeDuration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        active.volume = volume;

        while (true)
        {
            float waitTime = active.clip.length - fadeDuration - 0.1f;
            if (waitTime > 0f)
                yield return new WaitForSecondsRealtime(waitTime);


            int newIndex = current == 0 ? 1 : 0;
            AudioSource next = players[newIndex];
            next.clip = GetRandomClip();
            next.volume = 0f;
            next.Play();

            // Crossfade simultané
            t = 0f;
            while (t < fadeDuration)
            {
                float ratio = t / fadeDuration;
                active.volume = Mathf.Lerp(volume, 0f, ratio);
                next.volume = Mathf.Lerp(0f, volume, ratio);
                t += Time.unscaledDeltaTime;
                yield return null;
            }


            active.volume = 0f;
            active.Stop();
            active.clip = null;
            next.volume = volume;

            current = newIndex;
            active = players[current];
        }
    }

    public void Stop()
    {
        if (loopRoutine != null) StopAllCoroutines();
        foreach (var p in players) { p.Stop(); p.clip = null; p.volume = 0f; }
    }

    public void Pause()
    {
        foreach (var p in players) if (p.clip != null) p.Pause();
    }

    public void Resume()
    {
        foreach (var p in players) if (p.clip != null) p.Play();
    }
}
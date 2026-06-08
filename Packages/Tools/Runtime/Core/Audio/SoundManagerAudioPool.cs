using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundManagerAudioPool
{
    protected List<AudioSource> _pool = new List<AudioSource>();
    protected Transform _parent;

    // -------------------------
    // Fill
    // -------------------------

    public virtual void FillAudioSourcePool(int size, Transform parent)
    {
        _parent = parent;

        if (size <= 0 || _pool.Count >= size) return;

        for (int i = 0; i < size; i++)
            CreateNewSource(i, false);
    }

    protected virtual AudioSource CreateNewSource(int index, bool active)
    {
        GameObject obj = new GameObject($"PooledAudioSource_{index}");
        obj.transform.SetParent(_parent);
        AudioSource source = obj.AddComponent<AudioSource>();
        obj.SetActive(active);
        _pool.Add(source);
        return source;
    }

    // -------------------------
    // Get
    // -------------------------

    public virtual AudioSource GetAvailableAudioSource(bool poolCanExpand, Transform parent)
    {
        foreach (AudioSource source in _pool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }

        if (poolCanExpand)
            return CreateNewSource(_pool.Count, true);

        return null;
    }

    // -------------------------
    // AutoDisable
    // -------------------------

    /// <summary>
    /// Désactive l'AudioSource une fois la lecture terminée.
    /// Supporte les sous-clips (playbackTime + playbackDuration) comme le MMSoundManager.
    /// </summary>
    public virtual IEnumerator AutoDisableAudioSource(float duration, AudioSource source, AudioClip clip,
        bool doNotAutoRecycleIfNotDonePlaying = false, float playbackTime = 0f, float playbackDuration = 0f)
    {
        if (source == null) yield break;

        // Durée effective : si playbackDuration est défini, on joue seulement cette portion
        float waitDuration = (playbackDuration > 0f) ? playbackDuration : duration;

        // On attend que le son démarre vraiment (cas des PlayDelayed)
        if (clip != null)
        {
            float timeout = 2f;
            float waited = 0f;
            while (!source.isPlaying && waited < timeout)
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(waitDuration);

        // Si le clip a changé entre-temps, on ne touche pas à cette source
        if (clip != null && source.clip != clip)
            yield break;

        // Si on veut attendre que le son soit vraiment fini avant de recycler
        if (doNotAutoRecycleIfNotDonePlaying && source.isPlaying)
        {
            while (source.isPlaying)
                yield return null;
        }

        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
    }

    // -------------------------
    // Free
    // -------------------------

    public virtual bool FreeSound(AudioSource sourceToStop)
    {
        foreach (AudioSource source in _pool)
        {
            if (source == sourceToStop)
            {
                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }

    // -------------------------
    // Cleanup
    // -------------------------

    /// <summary>
    /// Retire de la liste les sons qui ont fini de jouer naturellement.
    /// À appeler depuis SoundManager pour éviter que _sounds grossisse indéfiniment.
    /// </summary>
    public virtual void CleanUp(List<SoundManagerSound> sounds)
    {
        sounds.RemoveAll(s => s.Source == null ||
                             (!s.Source.isPlaying && !s.Source.loop && !s.Source.gameObject.activeInHierarchy));
    }
}
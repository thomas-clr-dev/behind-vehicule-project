using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundManagerAudioPool 
{
    protected List<AudioSource> _pool = new List<AudioSource>();
    protected Transform _parent;

    public virtual void FillAudioSourcePool(int size, Transform parent)
    {
        _parent = parent;
        for (int i = 0; i < size; i++)
        {
            CreateNewSource();
        }
    }

    protected virtual AudioSource CreateNewSource()
    {
        GameObject obj = new GameObject("PooledAudioSource");
        obj.transform.SetParent(_parent);
        AudioSource source = obj.AddComponent<AudioSource>();
        obj.SetActive(false);
        _pool.Add(source);
        return source;
    }

    // Renommé GetSource -> GetAvailableAudioSource pour correspondre à ton PlaySound()
    public AudioSource GetAvailableAudioSource(bool poolCanExpand, Transform parent)
    {
        foreach (var source in _pool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }

        if (poolCanExpand)
        {
            return CreateNewSource();
        }

        return null;
    }

    public virtual IEnumerator AutoDisableAudioSource(float duration, AudioSource source, AudioClip clip)
    {
        yield return new WaitForSecondsRealtime(duration);

        if (source.clip == clip)
        {
            source.Stop();
            source.gameObject.SetActive(false);
        }
    }

    public virtual bool FreeSound(AudioSource sourceToStop)
    {
        foreach (AudioSource source in _pool)
        {
            if (source == sourceToStop)
            {
                source.Stop();
                source.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }



}

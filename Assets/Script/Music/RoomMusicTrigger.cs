using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RoomMusicTrigger : MonoBehaviour
{
    #region References
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float inTargetVolume = 1f;
    [SerializeField] private float outTargetVolume = 0f;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;
    #endregion

    private void Start()
    {
        if (musicSource == null)
        {

            musicSource = GetComponent<AudioSource>();

            if (musicSource == null)
            {
                Debug.LogError("No AudioSource found on RoomMusicTrigger.");
            }
        }
        musicSource.volume = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            musicSource.Play();
            StartCoroutine(FadeInMusic());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOutMusic());
        }
    }

    private IEnumerator FadeInMusic()
    {
        float startVolume = musicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, inTargetVolume, elapsedTime / fadeInDuration);
            yield return null;
        }

        musicSource.volume = inTargetVolume;
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, outTargetVolume, elapsedTime / fadeOutDuration);
            yield return null;
        }
        musicSource.volume = outTargetVolume;
        musicSource.Pause();
    }
}

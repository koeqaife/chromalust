using System;
using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip music;
    public float fadeDuration = 3f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.Play();
    }

    void Update()
    {
        if (audioSource.time >= audioSource.clip.length - fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, (audioSource.clip.length - audioSource.time) / fadeDuration);
        }
        else if (audioSource.time <= fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, audioSource.time / fadeDuration);
        }
    }
}


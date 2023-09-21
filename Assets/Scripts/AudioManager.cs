using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] sounds;
    public AudioSource source;

    private bool isAudioPaused = false;

    private Vector2 previousWindowSize;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (HasWindowResized())
        {
            PauseAudioSources();
        }
        else
        {
            if (!isAudioPaused)
            {
                PlaySounds();
            }
        }

        RecordWindowSize();
    }

    private bool HasWindowResized()
    {
        Vector2 currentWindowSize = new Vector2(Screen.width, Screen.height);
        return currentWindowSize != previousWindowSize;
    }

    private void RecordWindowSize()
    {
        previousWindowSize = new Vector2(Screen.width, Screen.height);
    }

    private void PlaySounds()
    {
        source.UnPause();
    }

    private void PauseAudioSources()
    {
        source.Pause();
    }

    public void Swipe()
    {
        source.clip = sounds[0];
        source.PlayOneShot(source.clip);
    }

    public void SwipeResist()
    {
        source.clip = sounds[1];
        source.PlayOneShot(source.clip);
    }

    public void SwipeResistBorder()
    {
        source.clip = sounds[2];
        source.PlayOneShot(source.clip);
    }

    public void FruitCrush()
    {
        source.clip = sounds[Random.Range(3, 7)];
        source.PlayOneShot(source.clip);
    }

    public void FruitFall()
    {
        source.clip = sounds[Random.Range(7, 9)];
        source.PlayOneShot(source.clip);
    }

    public void Bubble()
    {
        source.clip = sounds[9];
        source.PlayOneShot(source.clip);
    }

    public void PowerUpCreation()
    {
        source.clip = sounds[10];
        source.PlayOneShot(source.clip);
    }

    public void StarbaleBreak()
    {
        source.clip = sounds[Random.Range(11, 13)];
        source.PlayOneShot(source.clip);
    }

    public void FruitBreak()
    {
        source.clip = sounds[13];
        source.PlayOneShot(source.clip);
    }

    public void BoxBreak()
    {
        source.clip = sounds[14];
        source.PlayOneShot(source.clip);
    }

    public void MarbleBreak()
    {
        source.clip = sounds[15];
        source.PlayOneShot(source.clip);
    }
}

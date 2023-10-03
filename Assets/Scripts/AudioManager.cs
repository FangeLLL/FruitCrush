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
        source.clip = sounds[Random.Range(4, 7)];
        source.PlayOneShot(source.clip);
    }

    public void FruitFall()
    {
        source.clip = sounds[7];
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

    public void StrawBaleBreak()
    {
        source.clip = sounds[11];
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

    public void HudEntry()
    {
        source.clip = sounds[16];
        source.PlayOneShot(source.clip);
    }

    public void HudOut()
    {
        source.clip = sounds[17];
        source.PlayOneShot(source.clip);
    }

    public void MenuClick()
    {
        source.clip = sounds[18];
        source.PlayOneShot(source.clip);
    }

    public void MenuClickReturn()
    {
        source.clip = sounds[19];
        source.PlayOneShot(source.clip);
    }

    public void MetalBreak()
    {
        source.clip = sounds[20];
        source.PlayOneShot(source.clip);
    }

    public void ObjectBreak()
    {
        source.clip = sounds[21];
        source.PlayOneShot(source.clip);
    }
}

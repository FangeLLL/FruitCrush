using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioClip[] sounds;
    public AudioSource source;

    private bool isAudioPaused = false;

    private Vector2 previousWindowSize;

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

    public void MuteSoundEffects()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>();
        source.volume = 0;
    }

    public void UnMuteSoundEffects()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>();
        source.volume = 1;
    }

    public void MuteMusics()
    {
        GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
        source.volume = 0;
    }

    public void UnMuteMusics()
    {
        GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
        source.volume = 1;
    }

}

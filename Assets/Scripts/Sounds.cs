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
        if(source != null)
            source.UnPause();
    }

    private void PauseAudioSources()
    {
        if (source != null)
            source.Pause();
    }

    public void MuteSoundEffects()
    {
        source = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>();
        source.mute = true;
    }

    public void UnMuteSoundEffects()
    {
        source = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>();
        source.mute = false;
    }

    public void MuteMusics()
    {
        source = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
        source.mute = true;
    }

    public void UnMuteMusics()
    {
        source = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
        source.mute = false;
    }

}

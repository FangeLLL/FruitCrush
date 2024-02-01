using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : Sounds
{
    private float musicFadeDuration = 0.3f;    

    //public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        source = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();


        /*if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            source = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }*/
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            InGameMusicOne();
        else
            MainMenuMusic();
    }

    private void MainMenuMusic()
    {
        source.volume = 1.0f;
        source.loop = true;
        source.clip = sounds[0];
        source.PlayOneShot(source.clip);
    }

    private void InGameMusicOne()
    {
        source.volume = 1.0f;
        source.loop = true;
        source.clip = sounds[1];
        source.PlayOneShot(source.clip);
    }

    public void FadeOutMainMenuMusic()
    {
        /*if (source.volume > 0f)
        {
            Debug.Log("TESTTEST");
            source.volume -= Time.deltaTime / musicFadeDuration;
        }
        else
        {
            source.Stop();
            FadeInInGameMusic();
                
        }*/
        //source.Stop();
        //FadeInInGameMusic();
    }

    public void FadeOutInGameMusic()
    {
        /*if (source.volume > 0f)
        {
            source.volume -= Time.deltaTime / musicFadeDuration;
        }
        else
        {
            source.Stop();
            FadeInMainMenuMusic();
        }*/
        //source.Stop();
        //FadeInMainMenuMusic();
    }

    private void FadeInInGameMusic()
    {
        //InGameMusicOne();
        /*if (source.volume <= 1f)
        {                      
            source.volume += Time.deltaTime / musicFadeDuration;
        }*/
    }

    private void FadeInMainMenuMusic()
    {
        //MainMenuMusic();
        /*if (source.volume <= 1f)
        {
            source.volume += Time.deltaTime / musicFadeDuration;
        }*/
    }
}

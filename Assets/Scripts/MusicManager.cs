using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : Sounds
{

    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            source = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        MainMenuMusic();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            source.Stop();
            InGameMusicOne();
        }
    }

    private void MainMenuMusic()
    {
        source.loop = true;
        source.clip = sounds[0];
        source.PlayOneShot(source.clip);
    }

    private void InGameMusicOne()
    {
        source.clip = sounds[1];
        source.PlayOneShot(source.clip);
    }

    /*private void FadeOutMusic()
    {
        if (source.volume > 0f && isExitingMainMenu)
        {
            source.volume -= Time.deltaTime / musicFadeDuration;
        }
        else
        {
            source.Stop();
            isExitingMainMenu = false;
            oneTime = false;
        }
    }

    private void FadeInMusic()
    {
        InGameMusicOne();
        if (source.volume <= 1f && isExitingMainMenu)
        {                      
            source.volume += Time.deltaTime / musicFadeDuration;
        }
        else
        {
            isEnteredLevels = false;
        }
    }*/
}

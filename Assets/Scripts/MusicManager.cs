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
            source.volume = 0;
        }
    }

    public void MainMenuMusic()
    {
        source.loop = true;
        source.clip = sounds[0];
        source.PlayOneShot(source.clip);
    }
}

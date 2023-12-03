using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Sounds
{
    [SerializeField] private AudioSource powerUpGainSource;
    [SerializeField] private AudioSource hoeSource;
    [SerializeField] private AudioSource harvesterSource;
    private void Awake()
    {
        source = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>();        
    }

    private void Update()
    {
        if(source.mute)
        {
            powerUpGainSource.mute = true;
            hoeSource.mute=true;
            harvesterSource.mute=true;
        }
        else
        {
            powerUpGainSource.mute = false;
            hoeSource.mute=false;
            harvesterSource.mute=false;
        }
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

    public void GlassHit()
    {
        source.clip = sounds[22];
        source.PlayOneShot(source.clip);
    }

    public void GlassShatter()
    {
        source.clip = sounds[23];
        source.PlayOneShot(source.clip);
    }

    public void GrassBreak()
    {
        source.clip = sounds[24];
        source.PlayOneShot(source.clip);
    }

    public void Harvester()
    {
        harvesterSource.clip = sounds[25];
        harvesterSource.PlayOneShot(harvesterSource.clip);
    }

    public void Pickaxe()
    {
        source.clip = sounds[26];
        source.PlayOneShot(source.clip);
    }

    public void JellyBreak()
    {
        source.clip = sounds[27];
        source.PlayOneShot(source.clip);
    }

    public void UnknownSoundEffect()
    {
        source.clip = sounds[28];
        source.PlayOneShot(source.clip);
    }

    public void PowerUpGain()
    {
        powerUpGainSource.clip = sounds[29];
        powerUpGainSource.PlayOneShot(powerUpGainSource.clip);
    }
    public void PowerUpGain2()
    {
        powerUpGainSource.clip = sounds[30];
        powerUpGainSource.PlayOneShot(powerUpGainSource.clip);
    }
}

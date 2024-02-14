using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Sounds
{
    [SerializeField] private AudioSource powerUpGainSource;
    [SerializeField] private AudioSource hoeSource;
    [SerializeField] private AudioSource harvesterSource;
    private string mainMenu = "MainMenu";
    private bool[] isSoundPlaying = new bool[31];
    
    /*
     * ALL INDEXES REFERENCE A SOUND EFFECT
     * 
     * ----------------------------------------
     * 
     * 0 => FRUIT FALL
     * 1 => BUBBLE
     * 2 => POWER UP CREATION
     * 3 => STRAWBALE BREAK
     * 4 => FRUIT BREAK
     * 5 => BOX BREAK
     * 6 => MARBLE BREAK
     * 7 => HUD ENTRY
     * 8 => HUD OUT
     * 9 => MENU CLICK
     * 10 => MENU CLICK RETURN
     * 11 => METAL BREAK
     * 12 => OBJECT BREAK
     * 13 => GLASS HIT
     * 14 => GLASS SHATTER
     * 15 => GLASS BREAK
     * 16 => HARVESTER
     * 17 => PICKAXE
     * 18 => JELLY BREAK
     * 19 => UNKNOWN SOUND EFFECT
     * 20 => POWER UP GAIN
     * 21 => POWER UP GAIN 2
     * 22 => BOX TOUCH
     * 23 => BUSH CRUSH
     * 24 => OPEN NI NOOR
     * 25 => TOUCH GRASS
     * 26 => TOUCH HONEY
     * 27 => UI SOUND EFFECT
     * 28 => UI SOUND EFFECT 2
     * 29 => OBJECT TOUCH
     */

    private void Awake()
    {
        source = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>();        
    }

    private void Update()
    {
        if(!SceneManager.GetSceneByName(mainMenu).isLoaded)
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
        
    }

    private IEnumerator WaitForSound(int index)
    {
        yield return new WaitForSeconds(0.1f);
        isSoundPlaying[index] = false;
    }

    public void SoundController(string sound)
    {
        switch (sound)
        {
            case "StrawBaleBreak":
                StrawBaleBreak();
                break;
            case "BoxBreak":
                BoxBreak();
                break;
            case "GrassBreak":
                GrassBreak();
                break;
            case "MarbleBreak":
                MarbleBreak();
                break;
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
        if (!isSoundPlaying[0])
        {
            isSoundPlaying[0] = true;
            StartCoroutine(WaitForSound(0));
            source.clip = sounds[7];
            source.PlayOneShot(source.clip);
        }       
    }   

    public void Bubble()
    {
        if (!isSoundPlaying[1])
        {
            isSoundPlaying[1] = true;
            StartCoroutine(WaitForSound(1));
            source.clip = sounds[9];
            source.PlayOneShot(source.clip);
        }        
    }

    public void PowerUpCreation()
    {
        if (!isSoundPlaying[2])
        {
            isSoundPlaying[2] = true;
            StartCoroutine(WaitForSound(2));
            source.clip = sounds[10];
            source.PlayOneShot(source.clip);
        }        
    }

    public void StrawBaleBreak()
    {
        if (!isSoundPlaying[3])
        {
            isSoundPlaying[3] = true;
            StartCoroutine(WaitForSound(3));
            source.clip = sounds[11];
            source.PlayOneShot(source.clip);
        }
        
    }

    public void FruitBreak()
    {
        if (!isSoundPlaying[4])
        {
            isSoundPlaying[4] = true;
            StartCoroutine(WaitForSound(4));
            source.clip = sounds[13];
            source.PlayOneShot(source.clip);
        }       
    }

    public void BoxBreak()
    {
        if (!isSoundPlaying[5])
        {
            isSoundPlaying[5] = true;
            StartCoroutine(WaitForSound(5));
            // 14 is not used.
            source.clip = sounds[31];
            source.PlayOneShot(source.clip);
        }        
    }

    public void MarbleBreak()
    {
        if (!isSoundPlaying[6])
        {
            isSoundPlaying[6] = true;
            StartCoroutine(WaitForSound(6));
            source.clip = sounds[15];
            source.PlayOneShot(source.clip);
        }       
    }

    public void HudEntry()
    {
        if (!isSoundPlaying[7])
        {
            isSoundPlaying[7] = true;
            StartCoroutine(WaitForSound(7));
            source.clip = sounds[16];
            source.PlayOneShot(source.clip);
        }        
    }

    public void HudOut()
    {
        if (!isSoundPlaying[8])
        {
            isSoundPlaying[8] = true;
            StartCoroutine(WaitForSound(8));
            source.clip = sounds[17];
            source.PlayOneShot(source.clip);
        }        
    }

    public void MenuClick()
    {
        if (!isSoundPlaying[9])
        {
            isSoundPlaying[9] = true;
            StartCoroutine(WaitForSound(9));
            source.clip = sounds[18];
            source.PlayOneShot(source.clip);
        }       
    }

    public void MenuClickReturn()
    {
        if (!isSoundPlaying[10])
        {
            isSoundPlaying[10] = true;
            StartCoroutine(WaitForSound(10));
            source.clip = sounds[19];
            source.PlayOneShot(source.clip);
        }       
    }

    public void MetalBreak()
    {
        if (!isSoundPlaying[11])
        {
            isSoundPlaying[11] = true;
            StartCoroutine(WaitForSound(11));
            source.clip = sounds[20];
            source.PlayOneShot(source.clip);
        }        
    }

    public void ObjectBreak()
    {
        if (!isSoundPlaying[12])
        {
            isSoundPlaying[12] = true;
            StartCoroutine(WaitForSound(12));
            source.clip = sounds[21];
            source.PlayOneShot(source.clip);
        }        
    }

    public void GlassHit()
    {
        if (!isSoundPlaying[13])
        {
            isSoundPlaying[13] = true;
            StartCoroutine(WaitForSound(13));
            source.clip = sounds[22];
            source.PlayOneShot(source.clip);
        }        
    }

    public void GlassShatter()
    {
        if (!isSoundPlaying[14])
        {
            isSoundPlaying[14] = true;
            StartCoroutine(WaitForSound(14));
            source.clip = sounds[23];
            source.PlayOneShot(source.clip);
        }       
    }

    public void GrassBreak()
    {
        if (!isSoundPlaying[15])
        {
            isSoundPlaying[15] = true;
            StartCoroutine(WaitForSound(15));
            source.clip = sounds[24];
            source.PlayOneShot(source.clip);
        }       
    }

    public void Harvester()
    {
        if (!isSoundPlaying[16])
        {
            isSoundPlaying[16] = true;
            StartCoroutine(WaitForSound(16));
            source.clip = sounds[25];
            source.PlayOneShot(source.clip);
        }        
    }

    public void Pickaxe()
    {
        if (!isSoundPlaying[17])
        {
            isSoundPlaying[17] = true;
            StartCoroutine(WaitForSound(17));
            source.clip = sounds[26];
            source.PlayOneShot(source.clip);
        }       
    }

    public void JellyBreak()
    {
        if (!isSoundPlaying[18])
        {
            isSoundPlaying[18] = true;
            StartCoroutine(WaitForSound(18));
            source.clip = sounds[27];
            source.PlayOneShot(source.clip);
        }      
    }

    public void UnknownSoundEffect()
    {
        if (isSoundPlaying[19])
        {
            isSoundPlaying[19] = false;
            StartCoroutine(WaitForSound(19));
            source.clip = sounds[28];
            source.PlayOneShot(source.clip);
        }       
    }

    public void PowerUpGain()
    {
        if (!isSoundPlaying[20])
        {
            isSoundPlaying[20] = true;
            StartCoroutine(WaitForSound(20));
            powerUpGainSource.clip = sounds[29];
            powerUpGainSource.PlayOneShot(powerUpGainSource.clip);
        }       
    }

    public void PowerUpGain2()
    {
        if (!isSoundPlaying[21])
        {
            isSoundPlaying[21] = true;
            StartCoroutine(WaitForSound(21));
            powerUpGainSource.clip = sounds[30];
            powerUpGainSource.PlayOneShot(powerUpGainSource.clip);
        }       
    }

    public void BoxTouch()
    {
        if (!isSoundPlaying[22])
        {
            isSoundPlaying[22] = true;
            StartCoroutine(WaitForSound(22));
            source.clip = sounds[Random.Range(32, 34)];
            source.PlayOneShot(source.clip);
        }        
    }

    public void BushCrush()
    {
        if (!isSoundPlaying[23])
        {
            isSoundPlaying[23] = true;
            StartCoroutine(WaitForSound(23));
            source.clip = sounds[34];
            source.PlayOneShot(source.clip);
        }        
    }

    /// <summary>
    /// This function nopens ný noor.
    /// </summary>
    public void OpenDoor()
    {
        if (!isSoundPlaying[24])
        {
            isSoundPlaying[24] = true;
            StartCoroutine(WaitForSound(24));
            source.clip = sounds[35];
            source.PlayOneShot(source.clip);
        }        
    }

    public void TouchGrass()
    {
        if (!isSoundPlaying[25])
        {
            isSoundPlaying[25] = true;
            StartCoroutine(WaitForSound(25));
            source.clip = sounds[36];
            source.PlayOneShot(source.clip);
        }
        
    }

    public void TouchHoney()
    {
        if (!isSoundPlaying[26])
        {
            isSoundPlaying[26] = true;
            StartCoroutine(WaitForSound(26));
            source.clip = sounds[37];
            source.PlayOneShot(source.clip);
        }        
    }

    public void UISoundEffect()
    {
        if (!isSoundPlaying[27])
        {
            isSoundPlaying[27] = true;
            StartCoroutine(WaitForSound(27));
            source.clip = sounds[57];
            source.PlayOneShot(source.clip);
        }       
    }

    public void UISoundEffect2()
    {
        if (!isSoundPlaying[28])
        {
            isSoundPlaying[28] = true;
            StartCoroutine(WaitForSound(28));
            source.clip = sounds[58];
            source.PlayOneShot(source.clip);
        }        
    }

    public void ObjectTouch()
    {
        if (!isSoundPlaying[29])
        {
            isSoundPlaying[29] = true;
            StartCoroutine(WaitForSound(29));
            source.clip = sounds[60];
            source.PlayOneShot(source.clip);
        }        
    }

}

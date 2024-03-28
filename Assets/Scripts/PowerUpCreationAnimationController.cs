using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCreationAnimationController : MonoBehaviour
{
    private int isPowerUpCreated = Animator.StringToHash("isPowerUpCreated");
    [SerializeField] private GameObject TNTShadows;
    public void PowerUpCreationStop()
    {
        GetComponent<Animator>().SetBool(isPowerUpCreated, false);
    } 

    public void TNTShadowController()
    {
        TNTShadows.SetActive(true);
    }
}

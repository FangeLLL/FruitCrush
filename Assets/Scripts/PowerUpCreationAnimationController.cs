using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCreationAnimationController : MonoBehaviour
{
    private int isPowerUpCreated = Animator.StringToHash("isPowerUpCreated");
    public void PowerUpCreationStop()
    {
        GetComponent<Animator>().SetBool(isPowerUpCreated, false);
    } 

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationLengthTest : MonoBehaviour
{

    float time;
    bool trigger = false;

    public void Update()
    {
        if (trigger)
        {
            time += Time.deltaTime;
        }
    }

    public void StartAnimation()
    {
        time = 0;
        trigger = true;
    }

    public void FinishAnimation()
    {
        trigger= false;
        Debug.Log(time);
    }
}

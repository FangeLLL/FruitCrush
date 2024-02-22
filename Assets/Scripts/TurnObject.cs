using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnObject : MonoBehaviour
{
    private Vector3 rotationSpeed = new Vector3(0, 0, -500); // Rotation speed in degrees per second

    void FixedUpdate()
    {
        // Rotate the object continuously based on the rotationSpeed
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

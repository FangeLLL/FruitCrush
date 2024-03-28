using ObjectSpeedAndTimeWaitingNameSpace;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ExplosionAreaScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExplosionAreaGrowing());
    }

    private IEnumerator ExplosionAreaGrowing()
    {
        float elapsedTime = 0f;
        float growDuration = ObjectSpeedAndTimeWaitingLibrary.twoDiscoBallMergeExplosionDuration;
        float targetColliderSize = 20f; // Change this value as needed

        while (elapsedTime < growDuration)
        {
            // Calculate the interpolation factor (0 to 1) based on elapsed time and duration
            float t = Mathf.Lerp(0f, targetColliderSize, elapsedTime / growDuration);
            // Interpolate the scale gradually from initialScale to targetScale
            GetComponent<CircleCollider2D>().radius = t;
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        Destroy(gameObject);
    }
}

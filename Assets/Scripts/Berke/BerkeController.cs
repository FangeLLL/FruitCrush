using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerkeController : MonoBehaviour
{
    GameObject berke;
    Collider2D berke_yarrak;
    Animator berke_got;

    bool kalkik;

    private void Start()
    {
        berke = GetComponent<GameObject>();
        berke_yarrak = GetComponent<Collider2D>();
        berke_got = GetComponent<Animator>();
    }

    private void Update()
    {
        if (berke_got == kalkik && berke_yarrak != kalkik)
        {
            StartEdging();
        }
    }

    private void StartEdging()
    {
        Destroy(berke);
    }
}

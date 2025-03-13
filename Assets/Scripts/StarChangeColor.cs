using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarChangeColor : MonoBehaviour
{
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void onLightSwitch(bool f)
    {
        sr.color = f? Color.yellow:Color.white;
    }
}

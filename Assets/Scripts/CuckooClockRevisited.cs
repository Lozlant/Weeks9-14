using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CuckooClockRevisited : MonoBehaviour
{
    [Range(0f, 3f)]
    public float speed;
    public Transform hourHand;
    float degree;
    public UnityEvent onAHourGone;

    private void Update()
    {
        float degreechange = 15f * Time.deltaTime*speed;
        degree += degreechange;
        hourHand.Rotate(0,0,-degreechange);

        if(degree >= 30f)
        {
            onAHourGone.Invoke();
            Debug.Log("Play chime");
            degree -= 30f;
        }
    }
}

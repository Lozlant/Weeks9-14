using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    float speed=5f;
    public int direction = 1;
    private void Update()
    {
        transform.Translate(speed*direction*Time.deltaTime,0,0);
        float screenPosX = Camera.main.WorldToScreenPoint(transform.position).x;
        if (screenPosX>= Screen.width || screenPosX <= 0)
        {
            direction *= -1;
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }
    public void Go()
    {
        speed = 5f;
    }
    public void Stop()
    {
        speed = 0f;
    }
}

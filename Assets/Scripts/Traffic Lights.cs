using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrafficLights : MonoBehaviour
{
    public UnityEvent Stop;
    public UnityEvent Go;

    public SpriteRenderer trafficLight;

    private void Start()
    {
        GoTrafficLight();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Stop.Invoke();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Go.Invoke();
        }
    }

    public void GoTrafficLight()
    {
        trafficLight.color = Color.green;
    }
    public void StopTrafficLight()
    {
        trafficLight.color= Color.red;
    }
}

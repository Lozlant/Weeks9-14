using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerSwitch : MonoBehaviour
{
    public UnityEvent<bool> LightSwitch;
    public GameObject prefab;
    float timer;
    public float timeLimit=2f;

    bool isOn=false;

    private void Start()
    {
        timer = timeLimit;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isOn = !isOn;
            LightSwitch.Invoke(isOn);
        }

        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            timer = 0;
            GameObject star = Instantiate(prefab);
            star.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0));
            star.transform.localScale = Vector3.one * 0.5f;

            LightSwitch.AddListener(star.GetComponent<StarChangeColor>().onLightSwitch);
        }
    }

    
    
}

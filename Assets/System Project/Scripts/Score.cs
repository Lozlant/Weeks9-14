using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Score : MonoBehaviour
{
    TextMeshProUGUI text;
    public UnityEvent onUpgrade;
    int score;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void killEnemy()
    {
        changeScore(1);
        if(score%5==0)
        {
            onUpgrade.Invoke();
        }
    }

    void changeScore(int change)
    {
        score += change;
        score=Mathf.Clamp(score, 0, score);
        text.text = "Score: " + score;
    }

}

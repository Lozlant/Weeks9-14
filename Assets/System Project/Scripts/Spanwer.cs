using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spanwer : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Tank tank;
    public Score score;
    public float enemySpawnInterval;

    void Start()
    {
       StartCoroutine(startSpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator startSpawnEnemy()
    {
        float time = enemySpawnInterval;
        while (true)
        {
            time += Time.deltaTime;
            if (time >= enemySpawnInterval)
            {
                time = 0;
                spawnEnemy();
            }
            yield return null;
        }
    }
    void spawnEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        enemy.onClick.AddListener(tank.pointToTaret);
        enemy.onDie.AddListener(tank.enemyDie);
        enemy.onBeKilled.AddListener(score.killEnemy);
    }
}

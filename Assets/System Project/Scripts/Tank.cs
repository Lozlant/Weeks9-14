using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public Transform gun;
    public float speed;
    public GameObject missePrefab;
    public float missleSpeed;
    public Transform missleSpawnPoint;
    public float missleSpawnInterval;
    Enemy enemy;
    Coroutine shooting;
    Coroutine rotating;

    // Start is called before the first frame update
    void Start()
    {
        transform.position=Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pointToTaret(Enemy enemy)
    {
        if(this.enemy!=null)
        {
            this.enemy.loseTarget();
            this.enemy.onDie.RemoveListener(enemyDie);
        }
        this.enemy = enemy;
        Vector3 target =enemy.transform.position;
        Vector3 direction = target-gun.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (shooting != null)
        {
            StopCoroutine(shooting);
        }
        shooting = StartCoroutine(startSpawnMissle());

        if (rotating!=null)
        {
            StopCoroutine(rotating);
        }
        rotating=StartCoroutine(rotateToTarget(angle));

        
        //gun.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator rotateToTarget(float target)
    {
        float startAngle = gun.rotation.eulerAngles.z;
        float rotateAngle = target - startAngle;
        if (rotateAngle > 180)rotateAngle = -360;
        if (rotateAngle < -180)rotateAngle += 360;

        int direction = rotateAngle < 0? -1 : 1;

        float hasroate = 0;
        while (Mathf.Abs(rotateAngle - hasroate) > 1f)
        {
            hasroate += direction*speed * Time.deltaTime;
            gun.transform.rotation = Quaternion.Euler(0, 0, startAngle + hasroate);
            yield return null;
        }

        gun.transform.rotation = Quaternion.Euler(0, 0, startAngle + rotateAngle);
    }
    IEnumerator startSpawnMissle()
    {
        float time = missleSpawnInterval;
        while (true)
        {
            time += Time.deltaTime;
            if (time >= missleSpawnInterval)
            {
                time = 0;
                spawnMissle();
            }
            yield return null;
        }
    }
    void spawnMissle()
    {
        GameObject missle = Instantiate(missePrefab,missleSpawnPoint.position,gun.rotation);
        StartCoroutine(missleflying(missle.transform));
        
    }
    IEnumerator missleflying(Transform missle)
    {
        float time = 0;
        Enemy target = enemy;
        SpriteRenderer enemysr = target.GetComponent<SpriteRenderer>();
        while (target == null || !enemysr.bounds.Contains(missle.position))
        {
            time += Time.deltaTime;
            if (time >= 10f)
            {
                Destroy(missle.gameObject);
                break;
            }
            missle.transform.Translate(missleSpeed*transform.right*Time.deltaTime);
            yield return null;
        }
        Destroy(missle.gameObject);
    }

    public void enemyDie()
    {
        StopCoroutine(shooting);
        enemy=null;
    }

}

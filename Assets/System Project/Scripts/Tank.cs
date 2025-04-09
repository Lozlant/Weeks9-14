using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
    Coroutine lasering;
    Coroutine upgrading;
    public bool isUpgrade;

    public UnityEvent onAttack;

    public LineRenderer laser;
    public float laserDamageInterval;
    public float upgradeTime;
    public float upgradeSpeedAdd;

    Color basecolor;
    public Color upgradeColor;

    // Start is called before the first frame update
    void Start()
    {
        transform.position=Vector3.zero;
        basecolor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pointToTaret(Enemy enemy)
    {
        if(enemy == this.enemy)return;
        if(this.enemy!=null)
        {
            this.enemy.loseTarget();
            this.enemy.onDie.RemoveListener(enemyDie);
            onAttack.RemoveListener(enemy.takeDamage);
        }
        if(enemy == null) return;
        this.enemy = enemy;
        onAttack.AddListener(enemy.takeDamage);

        Vector3 target =enemy.transform.position;
        Vector3 direction = target-gun.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        
        if (rotating!=null)
        {
            StopCoroutine(rotating);
        }
        stopAttack();

        rotating =StartCoroutine(rotateToTarget(angle));

        
        //gun.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void stopAttack()
    {
        if (shooting != null)
        {
            StopCoroutine(shooting);
        }
        if (lasering != null)
        {
            StopCoroutine(lasering);
            laser.positionCount = 0;
        }
    }
    IEnumerator rotateToTarget(float target)
    {
        target=setAngle(target);
        float startAngle = setAngle(gun.rotation.eulerAngles.z);
        float rotateAngle = setAngle(target - startAngle);

        int direction = rotateAngle < 0? -1 : 1;

        float hasroate = 0;
        while (Mathf.Abs(rotateAngle)>=Mathf.Abs(hasroate))
        {
            hasroate += direction*speed * Time.deltaTime;
            gun.transform.rotation = Quaternion.Euler(0, 0, startAngle + hasroate);
            yield return null;
        }

        gun.transform.rotation = Quaternion.Euler(0, 0, startAngle + rotateAngle);
        if(enemy!=null)
        {
            if (isUpgrade)
            {
                lasering = StartCoroutine(laserAttack());
            }
            else
            {
                shooting = StartCoroutine(startSpawnMissle());
            }
        }
        
    }

    //使得angle的角度统一在-180-180
    float setAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
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
        if(enemy==null) return;
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
        if (target != null) onAttack.Invoke();
        Destroy(missle.gameObject);
    }

    public void enemyDie()
    {
        stopAttack();
        enemy=null;
    }

    public void Upgrade()
    {
        stopAttack();
        isUpgrade=true;
        pointToTaret(enemy);

        if(upgrading!=null)
        {
            StopCoroutine(upgrading);
        }
        upgrading=StartCoroutine(upgradeTimer());
        GetComponent<SpriteRenderer>().color=upgradeColor;
        speed += upgradeSpeedAdd;
    }

    IEnumerator laserAttack()
    {
        //if (enemy == null) StopCoroutine(lasering);
        laser.positionCount = 2;
        laser.SetPosition(0, missleSpawnPoint.position);
        laser.SetPosition(1, enemy.transform.position);

        float time = laserDamageInterval;
        while(isUpgrade)
        {
            time += Time.deltaTime;
            if(time >= laserDamageInterval)
            {
                time = 0;
                onAttack.Invoke();
            }
            yield return null;
        }
    }
    IEnumerator upgradeTimer()
    {
        float time = 0;
        while(time<upgradeTime)
        {
            time+= Time.deltaTime;
            yield return null;
        }
        isUpgrade = false;
        GetComponent<SpriteRenderer>().color = basecolor;
        speed -= upgradeSpeedAdd;
        pointToTaret(enemy);
    }

}

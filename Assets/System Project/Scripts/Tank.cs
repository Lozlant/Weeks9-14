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

    void Start()
    {
        transform.position=Vector3.zero;
        basecolor = GetComponent<SpriteRenderer>().color;
    }


    //would call when Onclick raised
    public void pointToTaret(Enemy enemy)
    {
        //subscribe the new enemy, remove the last one
        if (enemy == this.enemy)return;
        if(this.enemy!=null)
        {
            this.enemy.loseTarget();
            this.enemy.onDie.RemoveListener(enemyDie);
            onAttack.RemoveListener(enemy.takeDamage);
        }
        else return;
        this.enemy = enemy;
        onAttack.AddListener(enemy.takeDamage);

        //caculate the rotate
        Vector3 target =enemy.transform.position;
        Vector3 direction = target-gun.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        
        if (rotating!=null)
        {
            StopCoroutine(rotating);
        }
        stopAttack();

        //start rotate process
        rotating =StartCoroutine(rotateToTarget(angle));
    }

    // a methods to stop all kind of attack
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
    //a rotate progress, gun to target
    IEnumerator rotateToTarget(float target)
    {
        target=setAngle(target);
        float startAngle = setAngle(gun.rotation.eulerAngles.z);
        float rotateAngle = setAngle(target - startAngle);

        // choose the closer direction
        int direction = rotateAngle < 0? -1 : 1;

        float hasroate = 0;
        while (Mathf.Abs(rotateAngle)>=Mathf.Abs(hasroate))
        {
            hasroate += direction*speed * Time.deltaTime;
            gun.transform.rotation = Quaternion.Euler(0, 0, startAngle + hasroate);
            yield return null;
        }

        gun.transform.rotation = Quaternion.Euler(0, 0, startAngle + rotateAngle);

        //start attack
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

    //Makes the angles of angle uniform at -180 to 180
    float setAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
    }

    //missle spawn per interval time
    IEnumerator startSpawnMissle()
    {
        //spawn at start
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
        //every missle has its own flying process
        StartCoroutine(missleflying(missle.transform));
        
    }
    IEnumerator missleflying(Transform missle)
    {
        float time = 0;
        Enemy target = enemy;
        SpriteRenderer enemysr = target.GetComponent<SpriteRenderer>();

        //missle won't stop or lose althought it lose target
        while (target == null || !enemysr.bounds.Contains(missle.position))
        {
            time += Time.deltaTime;
            //if flying for a long time,destroy!
            if (time >= 10f)
            {
                Destroy(missle.gameObject);
                break;
            }
            missle.transform.Translate(missleSpeed*transform.right*Time.deltaTime);
            yield return null;
        }

        //here missle reach the target, raise the onAttack to make enemy take the damage
        if (target != null) onAttack.Invoke();
        Destroy(missle.gameObject);
    }

    //if enemyDie(enemy's onDie event raise),stop the attack and reset the record enemy
    public void enemyDie()
    {
        stopAttack();
        enemy=null;
    }

    //if score raise the onUpgrade event,call this
    public void Upgrade()
    {
        stopAttack();
        //set the status record variable
        isUpgrade = true;
        //recall the click methods to renew the attackway
        pointToTaret(enemy);

        //call a timer corotine to stop the upgrade after certain time
        if(upgrading!=null)
        {
            StopCoroutine(upgrading);
        }
        upgrading=StartCoroutine(upgradeTimer());

        //change the tank color when upgrade starting
        GetComponent<SpriteRenderer>().color=upgradeColor;
        speed += upgradeSpeedAdd;//and add the rotateSpeed
    }

    //the laser atack
    IEnumerator laserAttack()
    {
        //show the laser between gun and enemy
        laser.positionCount = 2;
        laser.SetPosition(0, missleSpawnPoint.position);
        laser.SetPosition(1, enemy.transform.position);

        //take damage interval
        float time = laserDamageInterval;
        while(isUpgrade)//if is not in upgrade state, the attack will stop
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

    //timer to finish the upgrade state after certain time 
    IEnumerator upgradeTimer()
    {
        float time = 0;
        while(time<upgradeTime)
        {
            time+= Time.deltaTime;
            yield return null;
        }
        isUpgrade = false;
        //reset the tank variable's 
        GetComponent<SpriteRenderer>().color = basecolor;
        speed -= upgradeSpeedAdd;

        //recall the click methods to renew the attackway
        pointToTaret(enemy);
    }

}

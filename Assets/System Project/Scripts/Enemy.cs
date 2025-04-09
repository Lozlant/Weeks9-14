using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public UnityEvent<Enemy> onClick;
    public UnityEvent onDie;
    public UnityEvent onBeKilled;

    public float speed = 1f;
    public int maxhealth=3;

    int health;

    public TextMeshProUGUI hpText;


    Vector2 direction;
    
    SpriteRenderer sr;

    private void Start()
    {
        health = maxhealth;
        randomPositionAtEdge();

        sr = GetComponent<SpriteRenderer>();
        direction = (Vector3.zero-transform.position).normalized;
    }

    void randomPositionAtEdge()
    {
        int edge = Random.Range(0, 4);
        Vector3 screenPos;
        switch (edge)
        {
            case 0:
                screenPos = new Vector2(Random.Range(0f, Screen.width), Screen.height);
                break;
            case 1:
                screenPos = new Vector2(Random.Range(0f, Screen.width), 0f);
                break;
            case 2:
                screenPos = new Vector2(0f, Random.Range(0f, Screen.height));
                break;
            default:
                screenPos = new Vector2(Screen.width, Random.Range(0f, Screen.height));
                break;
        }
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }
    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        if (sr.bounds.Contains(mousePos) && Input.GetMouseButtonDown(0))
        {
            target.SetActive(true);
            onClick.Invoke(this);
        }

        transform.Translate(direction*speed*Time.deltaTime);
        if(Vector3.Distance(transform.position, Vector3.zero) < 2f)
        {
            Die();
        }
        
    }
    public void loseTarget()
    {
        target.SetActive(false);
    }

    public void takeDamage()
    {
        int damage = 1;
        health -= damage;
        hpText.text = "HP:" + health;
        if(health <= 0)
        {
            Die();
            onBeKilled.Invoke();
        }
    }

    void Die()
    {
        onDie.Invoke();
        Destroy(gameObject);
    }
}

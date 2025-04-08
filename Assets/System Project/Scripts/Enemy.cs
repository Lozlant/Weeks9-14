using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public UnityEvent<Enemy> onClick;
    public UnityEvent onDie;

    public float speed = 1f;

    Vector2 direction;
    
    SpriteRenderer sr;

    private void Start()
    {
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
            onDie.Invoke();
            Destroy(gameObject);
        }
        
    }
    public void loseTarget()
    {
        target.SetActive(false);
    }
}

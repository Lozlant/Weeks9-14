using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerEvents : MonoBehaviour
{
    public GameObject starPrefab;
    public SpriteRenderer mainstar;
    public void PointerEnter()
    {
        mainstar.color = Color.yellow;
    }
    public void PointerExist()
    {
        mainstar.color= Color.white;
    }
    public void PointerClick()
    {
        GameObject star=Instantiate(starPrefab);
        star.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0));
        star.transform.localScale = Vector3.one * 0.1f;
    }
}

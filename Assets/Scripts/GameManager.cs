using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] Enemies;

    [SerializeField]TMP_Text timeScaleText;

    private void Start()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        }
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
        timeScaleText.text = "Time scale: " + scale;
    }
}

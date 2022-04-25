using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviour
{
    GameObject Health;
    HealthSystem healthSystem;
    // Start is called before the first frame update
    void Start()
    {
        Health = GameObject.Find("Health");
        healthSystem = Health.GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            healthSystem.TakeDamage(-10);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            healthSystem.TakeDamage(10);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int health = 100;
    Scrollbar Scrollbar;
    // Start is called before the first frame update
    void Start()
    {
        Scrollbar = GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hchange(int change)
    {
        if (health<0)
        {
            health = 0;
        }
        if (health>100)
        {
            health = 100;
        }

        health += change;
        Scrollbar.size = health/100f;
        if(Scrollbar.size == 0)
        {
            Debug.Log("Konec");
        }
    }
}

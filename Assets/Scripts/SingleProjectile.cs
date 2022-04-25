using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleProjectile : MonoBehaviour
{
    [SerializeField] int projectileDamage;

    private void Awake()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.GetComponent<HealthSystem>().Hchange(projectileDamage);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleProjectile : MonoBehaviour
{
    GameObject Player;

    Vector3 playerPosition;

    [SerializeField] float projectileSpeed;

    [SerializeField] int projectileDamage;

    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
        Player = GameObject.Find("TestPlayer");
        playerPosition = Player.transform.position - transform.position;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.GetComponent<HealthSystem>().Hchange(projectileDamage);
        }
    }
}

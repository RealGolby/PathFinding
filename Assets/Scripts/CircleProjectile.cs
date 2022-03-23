using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleProjectile : MonoBehaviour
{
    GameObject Player;
    [SerializeField] float projectileSpeed;

    [SerializeField] int projectileDamage;

    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.Find("Player");
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * projectileSpeed * Time.deltaTime);
        //rb.AddForce(Vector3.forward * projectileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.GetComponent<HealthSystem>().Hchange(projectileDamage);
        }
    }
}

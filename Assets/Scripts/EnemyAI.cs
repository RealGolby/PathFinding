using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObject Player;

    [Header("Enemy")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject enemyGO;
    [SerializeField] float jumpForce;
    [SerializeField] float FollowDistance;

    public float EnemySpeed = 4f;
    float enemyDistance;

    void FixedUpdate()
    {
        if (this.gameObject.transform.name=="Enemy")
        {
            enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
            if (FollowDistance > enemyDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, EnemySpeed * Time.fixedDeltaTime);
            }
        }  
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0,jumpForce)); 
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.gameObject.transform.name =="jumpCheck")
        {
            if (collision.transform.CompareTag("Obstacles"))
            {
                Jump();
                Debug.Log("Jump");
            }
        }
    }
}

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GameObject Player;

    [Header("Enemy")]
    Rigidbody2D rb;
    SpriteRenderer sr;
    [SerializeField] float jumpForce;
    [SerializeField] float followDistance;
    [SerializeField] float attackDistance;

    public float EnemySpeed = 4f;
    float enemyDistance;

    bool isGrounded;
    bool isChasing;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        MoveEnemy();
    }
    private void Update()
    {
        Attack();
        Jump();
        updateEnemyFace();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .2f, groundMask);
    }
    void updateEnemyFace()
    {
        if (isChasing)
        {
            if (Player.transform.position.x > transform.position.x)
            {
                sr.flipX = false;
            }
            else if (Player.transform.position.x < transform.position.x)
            {
                sr.flipX = true;
            }
        }
    }

    void MoveEnemy()
    {
            enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
            if (followDistance > enemyDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, EnemySpeed * Time.fixedDeltaTime);
                isChasing = true;
            }
            else if (followDistance < enemyDistance)
            {
                isChasing = false;
            }
    }

    void Jump()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x + 1.1f, transform.position.y), Vector2.left, .5f);
        if (hitLeft)
        {
            if (hitLeft.collider.transform.tag != "Enemy" && hitLeft.collider.transform.tag != "Player")
            {
                if (isGrounded)
                {
                    //rb.AddForce(new Vector2(5f, jumpForce));
                    rb.velocity = new Vector2(rb.velocity.x + .5f, jumpForce);
                    sr.flipX = false;
                }

            }
        }
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x - 1.1f, transform.position.y), Vector2.right, .5f);
        if (hitRight)
        {
            if (hitRight.collider.transform.tag != "Enemy" && hitRight.collider.transform.tag != "Player")
            {
                if (isGrounded)
                {
                    sr.flipX = true;
                    //rb.AddForce(new Vector2(-5f, jumpForce));
                    rb.velocity = new Vector2(rb.velocity.x + -.5f, jumpForce);
                }
            }
        }
    }

    void Attack()
    {
        enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
        if (attackDistance > enemyDistance)
        {
            Debug.Log("Attack player!");
        }
    }
}
using System.Collections;
using UnityEngine;

enum EnemyFace { Left, Right }

public class EnemyAI : MonoBehaviour
{
    GameObject Player;

    Rigidbody2D rb;
    SpriteRenderer sr;
    [SerializeField] float jumpForceY;
    [SerializeField] float jumpForceX;
    [SerializeField] float followDistance;

    public float EnemySpeed = 4f;
    float enemyDistance;
    float enemyDistanceX;
    [SerializeField] float minEnemyDistance;

    bool isDead;
    bool isGrounded;
    bool isChasing;
    bool isAttacking;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    HealthSystem playerHealth;

    public float Health = 100;
    [SerializeField] GameObject healthBar;

    EnemyFace enemyFace;

    [Header("Attaking")]
    [SerializeField] bool dashyAttack;
    [SerializeField] int enemyDamage;
    [SerializeField] float attackDistance;

    private void Start()
    {
        playerHealth = FindObjectOfType<HealthSystem>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            MoveEnemy();
            Jump();
            AttackPlayer();
        }
    }
    private void Update()
    {
        if (!isDead)
        {
            UpdateEnemyFace();
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .2f, groundMask);
    }
    void UpdateEnemyFace()
    {
        if (isChasing)
        {
            if (Player.transform.position.x > transform.position.x)
            {
                enemyFace = EnemyFace.Right;
                sr.flipX = false;
            }
            else if (Player.transform.position.x < transform.position.x)
            {
                enemyFace = EnemyFace.Left;
                sr.flipX = true;
            }
        }
    }


    void MoveEnemy()
    {
        enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
        enemyDistanceX = Mathf.Abs(transform.position.x - Player.transform.position.x);

        if (followDistance > enemyDistance && enemyDistanceX !>= minEnemyDistance)
        {
                if (enemyFace == EnemyFace.Right) transform.Translate(new Vector2(EnemySpeed * Time.fixedDeltaTime, 0));
                else if (enemyFace == EnemyFace.Left) transform.Translate(new Vector2(-EnemySpeed * Time.fixedDeltaTime, 0));

                //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, EnemySpeed * Time.fixedDeltaTime);
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
                    rb.velocity = new Vector2(rb.velocity.x + jumpForceX, jumpForceY);
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
                    rb.velocity = new Vector2(rb.velocity.x + -jumpForceX, jumpForceY);
                }
            }
        }

    }

    void AttackPlayer()
    {
        enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
        if (attackDistance > enemyDistance && !isAttacking)
        {
            //takeDamage(15);
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(Random.Range(.2f, .4f));
        if (dashyAttack)
        {
            if (enemyFace == EnemyFace.Left)
            {
                rb.AddForce(new Vector2(-200f, 100));
                yield return new WaitForSeconds(.3f);

                playerHealth.Hchange(-enemyDamage);
                TakeDamage(15);
                Debug.Log("Attack player!");
                yield return new WaitForSeconds(.1f);

                rb.AddForce(new Vector2(120f, 40));
                yield return new WaitForSeconds(1.5f);
                isAttacking = false;
            }
            else if (enemyFace == EnemyFace.Right)
            {
                rb.AddForce(new Vector2(200f, 100));
                yield return new WaitForSeconds(.3f);

                playerHealth.Hchange(-enemyDamage);
                TakeDamage(15);
                Debug.Log("Attack player!");
                yield return new WaitForSeconds(.1f);

                rb.AddForce(new Vector2(-120f, 40));
                yield return new WaitForSeconds(1.5f);
                isAttacking = false;
            }
        }
        else
        {
            Debug.Log("Attack player!");
            playerHealth.Hchange(-enemyDamage);
            yield return new WaitForSeconds(2f);
            isAttacking = false;
        }

    }

    public void TakeDamage(int damageAmount)
    {
        this.Health -= damageAmount;
        this.healthBar.transform.localScale = new Vector3((Health / 100f) * 2f, .25f, 1f);
        if (Health <= 0)
        {
            isDead = true;
            rb.velocity = Vector2.zero;
            healthBar.transform.localScale = Vector3.zero;
            Debug.Log("Enemy Died");
            Object.Destroy(gameObject,.5f);
        }
    }

}
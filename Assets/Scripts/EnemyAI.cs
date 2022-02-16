using System.Collections;
using UnityEngine;

enum EnemyFace { Left, Right }

enum EnemyState { Idle, Wander, Chase, Attack, Dead }
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

    bool isGrounded;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    HealthSystem playerHealth;

    public float Health = 100;
    [SerializeField] GameObject healthBar;

    EnemyFace enemyFace;
    [SerializeField] EnemyState enemyState;

    [Header("Attaking")]
    [SerializeField] bool dashyAttack;
    [SerializeField] float dashyAttackJumpX;
    [SerializeField] float dashyAttackJumpY;

    [SerializeField] int enemyDamage;
    [SerializeField] float attackDistance;

    [SerializeField] float attackIntervalMin;
    [SerializeField] float attackIntervalMax;

    [Header("Wandeing")]
    [SerializeField] bool canWander;
    bool canGoLeft;
    bool canGoRight;
    bool wanderWalk;
    bool wandering;

    private void Start()
    {
        enemyState = EnemyState.Idle;
        playerHealth = FindObjectOfType<HealthSystem>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        if (enemyState != EnemyState.Dead)
        {
            ChasePlayer();
            Jump();
            AttackPlayer();
            if (enemyState != EnemyState.Chase && canWander)
            {
                Wander();
            }
        }
    }
    private void Update()
    {
        if (enemyState != EnemyState.Dead)
        {
            UpdateEnemyFace();
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .2f, groundMask);
    }

    void Wander()
    {
        if(enemyState != EnemyState.Wander && !wanderWalk && !wandering) StartCoroutine(Wandering());

        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x + +.6f, transform.position.y - .5f), Vector2.down, .5f, groundMask);
        if (hitLeft.collider == null)
        {
            canGoLeft = false;
            /*wanderWalk = false;           DODELAT ABY ENEMY NEPADAL
            wandering = false;
            StopCoroutine(Wandering());
            enemyState = EnemyState.Idle;*/
        }
        else
        {
            canGoLeft = true;
        }
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x - .6f, transform.position.y - .5f), Vector2.down, .5f, groundMask);
        if (hitRight.collider == null)
        {
            canGoRight = false;
            /*wanderWalk = false;           DODELAT ABY ENEMY NEPADAL
            wandering = false;
            StopCoroutine(Wandering());
            enemyState = EnemyState.Idle;*/
        }
        else
        {
            canGoRight = true;
        }

        if (wanderWalk)
        {
            MoveEnemy();
        }
    }

    IEnumerator Wandering()
    {
        wandering = true;
        yield return new WaitForSeconds(Random.Range(1,3));
        if(Random.Range(0,2) == 0)
        {
            int side = Random.Range(0,2);
            if(side == 0 && canGoLeft)
            {
                enemyFace = EnemyFace.Left;
                wanderWalk = true;
                enemyState = EnemyState.Wander;
                yield return new WaitForSeconds(Random.Range(1, 4));
                    wanderWalk = false;
                    enemyState = EnemyState.Idle;
                    wandering = false;
            }
            else if(side == 1 && canGoRight)
            {
                enemyFace = EnemyFace.Right;
                wanderWalk = true;
                enemyState = EnemyState.Wander;
                yield return new WaitForSeconds(Random.Range(1,4));
                    wanderWalk = false;
                    enemyState = EnemyState.Idle;
                    wandering = false;
            }
        }
        else
        {
            wandering = false;
        }
    }


    void UpdateEnemyFace()
    {
        if (enemyFace == EnemyFace.Right) sr.flipX = false;
        else if (enemyFace == EnemyFace.Left) sr.flipX = true;
    }

    void ChasePlayer()
    {
        enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
        enemyDistanceX = Mathf.Abs(transform.position.x - Player.transform.position.x);

        if (followDistance > enemyDistance && enemyDistanceX! >= minEnemyDistance)
        {
            if (enemyState != EnemyState.Attack)
            {
                StopCoroutine(Wandering());
                wandering = false;
                wanderWalk = false;
                MoveEnemy();
                enemyState = EnemyState.Chase;
                SetEnemyFace();
            }

        }
        /*else if (followDistance < enemyDistance)
        {
            enemyState = EnemyState.Idle;
        }*/

    }

    void SetEnemyFace()
    {
        if (Player.transform.position.x > transform.position.x)
        {
            enemyFace = EnemyFace.Right;
        }
        else if (Player.transform.position.x < transform.position.x)
        {
            enemyFace = EnemyFace.Left;
        }
    }

    void MoveEnemy()
    {
        if (enemyFace == EnemyFace.Right) transform.Translate(new Vector2(EnemySpeed * Time.fixedDeltaTime, 0));
        else if (enemyFace == EnemyFace.Left) transform.Translate(new Vector2(-EnemySpeed * Time.fixedDeltaTime, 0));

        //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, EnemySpeed * Time.fixedDeltaTime);
    }

    void Jump()
    {
        if(enemyState != EnemyState.Wander)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x + 1.1f, transform.position.y), Vector2.left, .5f);
            if (hitLeft)
            {
                if (hitLeft.collider.transform.tag != "Enemy" && hitLeft.collider.transform.tag != "Player")
                {
                    if (isGrounded)
                    {
                        //rb.AddForce(new Vector2(5f, jumpForce));
                        rb.velocity = new Vector2(rb.velocity.x + -jumpForceX, jumpForceY);
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
                        rb.velocity = new Vector2(rb.velocity.x + jumpForceX, jumpForceY);
                    }
                }
            }
        }
    }

    void AttackPlayer()
    {
        enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
        if (attackDistance > enemyDistance && enemyState != EnemyState.Attack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        enemyState = EnemyState.Attack;
        yield return new WaitForSeconds(Random.Range(attackIntervalMin, attackIntervalMax));
        if (dashyAttack)
        {
            if (enemyFace == EnemyFace.Left)
            {
                rb.AddForce(new Vector2(-dashyAttackJumpX, dashyAttackJumpY));
                yield return new WaitForSeconds(.3f);

                playerHealth.Hchange(-enemyDamage);
                TakeDamage(15);
                Debug.Log("Attack player!");
                yield return new WaitForSeconds(.1f);

                rb.AddForce(new Vector2(dashyAttackJumpX / 2, dashyAttackJumpY / 2));
                yield return new WaitForSeconds(.5f);
                enemyState = EnemyState.Idle;
            }
            else if (enemyFace == EnemyFace.Right)
            {
                rb.AddForce(new Vector2(dashyAttackJumpX, dashyAttackJumpY));
                yield return new WaitForSeconds(.3f);

                playerHealth.Hchange(-enemyDamage);
                TakeDamage(15);
                Debug.Log("Attack player!");
                yield return new WaitForSeconds(.1f);

                rb.AddForce(new Vector2(-dashyAttackJumpX / 2, dashyAttackJumpY / 2));
                yield return new WaitForSeconds(.5f);
                enemyState = EnemyState.Idle;
            }
        }
        else
        {
            Debug.Log("Attack player!");
            playerHealth.Hchange(-enemyDamage);
            yield return new WaitForSeconds(2f);
            enemyState = EnemyState.Chase;
        }

    }

    void TakeDamage(int damageAmount)
    {
        this.Health -= damageAmount;
        this.healthBar.transform.localScale = new Vector3((Health / 100f) * 2f, .25f, 1f);
        if (Health <= 0)
        {
            enemyState = EnemyState.Dead;
            rb.velocity = Vector2.zero;
            healthBar.transform.localScale = Vector3.zero;
            Debug.Log("Enemy Died");
            Object.Destroy(gameObject, .5f);
        }
    }
}
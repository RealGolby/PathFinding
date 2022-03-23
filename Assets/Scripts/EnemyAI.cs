using System.Collections;
using UnityEngine;

enum EnemyFace { Left, Right }

enum EnemyState { Idle, Wander, Chase, Attack, Dead }

enum EnemyAttackType { Normal, Dash}

public class EnemyAI : MonoBehaviour
{
    GameObject Player;

    Rigidbody2D rb;
    SpriteRenderer sr;

    [Header("Movement")]

    [SerializeField] float jumpForceY;
    [SerializeField] float jumpForceX;
    [SerializeField] float followDistance;
    [SerializeField, Tooltip("Jak moc se enemy muze priblizit k hraci")]
    float minEnemyDistance;

    public float EnemySpeed = 4f;
    float enemyDistance;
    float enemyDistanceX;

    [SerializeField]float jumpDetectionOffset;

    [SerializeField] float wanderFallOffsetX;
    [SerializeField]float wanderFallOffsetY;

    bool isGrounded;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    HealthSystem playerHealth;

    public float EnemyHealth = 100;
    [SerializeField] GameObject healthBar;

    [SerializeField] EnemyFace enemyFace;
    [SerializeField] EnemyState enemyState;

    [Header("Attaking")]
    [SerializeField] EnemyAttackType attackType;
    public int enemyDamage;
    [SerializeField] float enemyAttackDistance;
    [SerializeField] float attackIntervalMin;
    [SerializeField] float attackIntervalMax;

    [Space(10)]
    [SerializeField] float dashAttackJumpX;
    [SerializeField] float dashAttackJumpY;
    [Space(10)]

    [Header("Wandeing")]
    [SerializeField] 
    bool canWander;

    [SerializeField, Tooltip("minimalni cas ve kterym se enemy bude rozhodovat jestli ma sam chodit")]
    float minTimeBetweenWander;
    [SerializeField, Tooltip("maximalni cas ve kterym se enemy bude rozhodovat jestli ma sam chodit")]
    float maxTimeBetweenWander;

    [SerializeField, Tooltip("nejmensi cas po ktery bude enemak sam chodit")]
    float minWanderTime;
    [SerializeField, Tooltip("nejvyssi cas po ktery bude enemak sam chodit")]
    float maxWanderTime;
    
    bool canGoLeft;
    bool canGoRight;
    bool wanderWalk;
    bool wandering;

    void Start()
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
            if (enemyState != EnemyState.Chase && canWander && enemyState != EnemyState.Attack)
            {
                Wander();
            }
        }
    }
    void Update()
    {
        if (enemyState != EnemyState.Dead)
        {
            UpdateEnemyFace();
        }
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .2f, groundMask);
    }

    #region Movement
    void Wander()
    {
        if (enemyState != EnemyState.Wander && !wanderWalk && !wandering && enemyState != EnemyState.Attack) StartCoroutine(Wandering());

        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + wanderFallOffsetX, transform.position.y - wanderFallOffsetY), Vector2.down, .5f, groundMask);
        Debug.DrawRay(new Vector2(transform.position.x + wanderFallOffsetX, transform.position.y - wanderFallOffsetY), Vector2.down / 2);
        if (hitRight.collider == null)
        {
            canGoRight = false;
        }
        else
        {
            canGoRight = true;
        }
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - wanderFallOffsetX, transform.position.y - wanderFallOffsetY), Vector2.down, .5f, groundMask);
        Debug.DrawRay(new Vector2(transform.position.x - wanderFallOffsetX, transform.position.y - wanderFallOffsetY), Vector2.down / 2);
        if (hitLeft.collider == null)
        {
            canGoLeft = false;
        }
        else
        {
            canGoLeft = true;
        }

        if (wanderWalk)
        {
            MoveEnemy();
        }
    }

    IEnumerator Wandering()
    {
        wandering = true;
        yield return new WaitForSeconds(Random.Range(minTimeBetweenWander, maxTimeBetweenWander));
        if (Random.Range(0, 2) == 0)
        {
            int side = Random.Range(0, 2);
            if (side == 0 && canGoLeft)
            {
                enemyFace = EnemyFace.Left;
                wanderWalk = true;
                enemyState = EnemyState.Wander;
                yield return new WaitForSeconds(Random.Range(minWanderTime, maxWanderTime));
                wanderWalk = false;
                enemyState = EnemyState.Idle;
                wandering = false;
            }
            else if (side == 1 && canGoRight)
            {
                enemyFace = EnemyFace.Right;
                wanderWalk = true;
                enemyState = EnemyState.Wander;
                yield return new WaitForSeconds(Random.Range(minWanderTime, maxWanderTime));
                wanderWalk = false;
                enemyState = EnemyState.Idle;
                wandering = false;
            }
            else
            {
                StopCoroutine(Wandering());
                wandering = false;
                wanderWalk = false;
                enemyState = EnemyState.Idle;
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
                if (enemyState == EnemyState.Wander)
                {
                    StopCoroutine(Wandering());
                    wandering = false;
                    wanderWalk = false;
                }
                MoveEnemy();
                enemyState = EnemyState.Chase;
                SetEnemyFace();
            }

        }
        else if (followDistance < enemyDistance && enemyState != EnemyState.Attack && enemyState == EnemyState.Chase)
        {
            enemyState = EnemyState.Idle;
        }

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
        float EnemySpeedLeft = EnemySpeed;
        float EnemySpeedRight = EnemySpeed;
        if (enemyState == EnemyState.Wander)
        {
            if (!canGoLeft)
            {
                EnemySpeedLeft = 0;
            }
            else if (!canGoRight)
            {
                EnemySpeedRight = 0;
            }
        }
        if (enemyFace == EnemyFace.Right) transform.Translate(new Vector2(EnemySpeedRight * Time.fixedDeltaTime, 0));
        else if (enemyFace == EnemyFace.Left) transform.Translate(new Vector2(-EnemySpeedLeft * Time.fixedDeltaTime, 0));
    }

    void Jump()
    {
        if (enemyState != EnemyState.Wander && enemyState != EnemyState.Attack && enemyState != EnemyState.Idle)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x + jumpDetectionOffset, transform.position.y), Vector2.left, .5f);
            if (hitLeft)
            {
                if (hitLeft.collider.transform.tag != "Enemy" && hitLeft.collider.transform.tag != "Player")
                {
                    if (isGrounded)
                    {
                        rb.velocity = new Vector2(rb.velocity.x + -jumpForceX, jumpForceY);
                        sr.flipX = false;
                    }

                }
            }
            RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x - jumpDetectionOffset, transform.position.y), Vector2.right, .5f);
            if (hitRight)
            {
                if (hitRight.collider.transform.tag != "Enemy" && hitRight.collider.transform.tag != "Player")
                {
                    if (isGrounded)
                    {
                        sr.flipX = true;
                        rb.velocity = new Vector2(rb.velocity.x + jumpForceX, jumpForceY);
                    }
                }
            }
        }
    }
    #endregion

    #region Attacking
    void AttackPlayer()
    {
        enemyDistance = Vector3.Distance(transform.position, Player.transform.position);
        if (enemyAttackDistance > enemyDistance && enemyState != EnemyState.Attack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        enemyState = EnemyState.Attack;
        yield return new WaitForSeconds(Random.Range(attackIntervalMin, attackIntervalMax));
        if (attackType == EnemyAttackType.Dash)
        {
            if (enemyFace == EnemyFace.Left)
            {
                rb.AddForce(new Vector2(-dashAttackJumpX, dashAttackJumpY));
                yield return new WaitForSeconds(.3f);

                playerHealth.Hchange(-enemyDamage);
                TakeDamage(15);
                Debug.Log("Attack player!");
                yield return new WaitForSeconds(.1f);

                rb.AddForce(new Vector2(dashAttackJumpX / 2, dashAttackJumpY / 2));
                yield return new WaitForSeconds(.5f);
                enemyState = EnemyState.Idle;
            }
            else if (enemyFace == EnemyFace.Right)
            {
                rb.AddForce(new Vector2(dashAttackJumpX, dashAttackJumpY));
                yield return new WaitForSeconds(.3f);

                playerHealth.Hchange(-enemyDamage);
                TakeDamage(15);
                Debug.Log("Attack player!");
                yield return new WaitForSeconds(.1f);

                rb.AddForce(new Vector2(-dashAttackJumpX / 2, dashAttackJumpY / 2));
                yield return new WaitForSeconds(.5f);
                enemyState = EnemyState.Idle;
            }
        }
        else
        {
            Debug.Log("Attack player!");
            playerHealth.Hchange(-enemyDamage);
            yield return new WaitForSeconds(2f);
            enemyState = EnemyState.Idle;
        }
    }
    void TakeDamage(int damageAmount)
    {
        EnemyHealth -= damageAmount;
        healthBar.transform.localScale = new Vector3((EnemyHealth / 100f) * 2f, .25f, 1f);
        if (EnemyHealth <= 0)
        {
            enemyState = EnemyState.Dead;
            rb.velocity = Vector2.zero;
            healthBar.transform.localScale = Vector3.zero;
            Debug.Log("Enemy Died");
            Destroy(gameObject, .5f);
        }
    }

    #endregion
}
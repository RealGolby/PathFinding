using System.Collections;
using UnityEngine;

enum BossState
{
    Idle, Chase, Attack, Transforming, Dead
}

enum BossPhase
{
    First, Second
}

enum BossFace
{
    Left, Right
}

enum SecondPhaseAttacks
{
    Single, Circle, Shotgun, Laser
}

public class BossAI : MonoBehaviour
{
    [SerializeField] BossPhase bossPhase;
    [SerializeField] BossState bossState;
    [SerializeField] BossFace bossFace;

    SecondPhaseAttacks secondPhaseAttacks;

    Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    BoxCollider2D bc;
    [SerializeField] Animator Anim;

    public float MaxBossHealth;
    public float BossHealth;

    [SerializeField] GameObject healthBar;

    [SerializeField] float bossSpeed;
    [SerializeField] float followDistance;

    GameObject Player;

    [SerializeField] float bossAttackDistance;

    [SerializeField, Tooltip("Jak moc se boss muze priblizit k hraci")]
    float minBossDistance;

    HealthSystem playerHealth;

    public Vector3 ArenaCenter;

    [SerializeField] Sprite secondPhaseSprite;

    void Start()
    {
        BossHealth = MaxBossHealth;

        bossPhase = BossPhase.First;
        bossState = BossState.Idle;

        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        Player = GameObject.Find("Player");
        playerHealth = FindObjectOfType<HealthSystem>();

        secondPhaseAttacks = SecondPhaseAttacks.Single;
    }
    void Update()
    {
        if (bossState != BossState.Dead && bossPhase == BossPhase.First)
        {
            updateBossFace();
        }
        else if (bossState != BossState.Dead && bossPhase == BossPhase.Second)
        {
            moveBossToCenter();
        }
    }

    private void FixedUpdate()
    {
        if (bossState != BossState.Dead)
        {
            AttackPlayer();
            if (bossPhase == BossPhase.First)
            {
                ChasePlayer();
            }
        }
    }

    void MoveEnemy()
    {
        float BossSpeedLeft = bossSpeed;
        float BossSpeedRight = bossSpeed;

        if (bossFace == BossFace.Right) transform.Translate(new Vector2(BossSpeedRight * Time.fixedDeltaTime, 0));
        else if (bossFace == BossFace.Left) transform.Translate(new Vector2(-BossSpeedLeft * Time.fixedDeltaTime, 0));
    }

    float bossDistance;
    float bossDistanceX;
    void ChasePlayer()
    {
        bossDistance = Vector3.Distance(transform.position, Player.transform.position);
        bossDistanceX = Mathf.Abs(transform.position.x - Player.transform.position.x);

        if (followDistance > bossDistance && bossDistanceX! >= minBossDistance)
        {
            if (bossState != BossState.Attack)
            {
                MoveEnemy();
                bossState = BossState.Chase;
                setBossFace();
            }

        }
        else if (followDistance < bossDistance && bossState != BossState.Attack && bossState == BossState.Chase)
        {
            bossState = BossState.Idle;
        }

    }
    void updateBossFace()
    {
        if (bossFace == BossFace.Right) spriteRenderer.flipX = false;
        else if (bossFace == BossFace.Left) spriteRenderer.flipX = true;
    }

    void setBossFace()
    {
        if (Player.transform.position.x > transform.position.x)
        {
            bossFace = BossFace.Right;
        }
        else if (Player.transform.position.x < transform.position.x)
        {
            bossFace = BossFace.Left;
        }
    }

    void AttackPlayer()
    {
        bossDistance = Vector3.Distance(transform.position, Player.transform.position);
        if (bossAttackDistance > bossDistance && bossState != BossState.Attack && bossPhase == BossPhase.First)
        {
            StartCoroutine(AttackFirstPhase());
        }
        else if (bossPhase == BossPhase.Second && bossState != BossState.Attack)
        {
            StartCoroutine(AttackSecondPhase());
        }
    }

    void moveBossToCenter()
    {
        transform.position = Vector3.MoveTowards(transform.position, ArenaCenter, 10 * Time.deltaTime);
    }

    IEnumerator AttackFirstPhase()
    {
        bossState = BossState.Attack;
        yield return new WaitForSeconds(Random.Range(1, 2));

        Debug.Log("Attack player!");
        playerHealth.TakeDamage(15);
        TakeDamage(200);
        yield return new WaitForSeconds(2f);
        bossState = BossState.Idle;
    }
    #region SecondPhaseAttacks


    IEnumerator AttackSecondPhase()
    {
        bossState = BossState.Attack;
        yield return new WaitForSeconds(Random.Range(1, 3));
        int attack = Random.Range(0, 4);
        switch (attack.ToString())
        {
            case "0":
                StartCoroutine(SecondPhaseSingleAttack());
                break;
            case "1":
                StartCoroutine(SecondPhaseCircleAttack());
                break;
            case "2":
                StartCoroutine(SecondPhaseShotgunAttack());
                break;
            case "3":
                StartCoroutine(SecondPhaseLaserAttack());
                break;
        }
    }
    [Header("SecondPhaseSingleAttack")]
    [SerializeField] GameObject singleProjectile;
    [SerializeField] float singleProjectileSpeed;
    [SerializeField] float singleTimeBetweenShots;
    [SerializeField] float singleProjectilesToShoot;
    IEnumerator SecondPhaseSingleAttack()
    {
        for (int i = 0; i < singleProjectilesToShoot; i++)
        {
            Vector3 difference = Player.transform.position - transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            float distance = difference.magnitude;
            Vector2 direction = difference / distance;
            direction.Normalize();
            GameObject projectile = Instantiate(singleProjectile, transform);
            projectile.transform.position = ArenaCenter + new Vector3(0,2,0);
            projectile.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * singleProjectileSpeed;
            yield return new WaitForSeconds(singleTimeBetweenShots);
        }
        bossState = BossState.Idle;
    }
    [Header("SecondPhaseShotgunAttack")]
    [SerializeField] int shotgunProjectilesPerShoot;
    [SerializeField] int shotgunProjectilesToShoot;
    [SerializeField] float shotgunTimeBetweenShots;
    [SerializeField] float shotgunProjectileSpeed;
    [SerializeField] GameObject shotgunProjectile;
    IEnumerator SecondPhaseShotgunAttack()
    {
        for (int i = 0; i < shotgunProjectilesToShoot; i++)
        {
            Vector3 difference = Player.transform.position - transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            float distance = difference.magnitude;
            Vector2 direction = difference / distance;
            direction.Normalize();
            for (int x = -1; x < shotgunProjectilesPerShoot - 1; x++)
            {
                GameObject projectileInstance = Instantiate(shotgunProjectile, transform);
                projectileInstance.transform.position = ArenaCenter + new Vector3(0, 2, 0); ;
                projectileInstance.transform.rotation = Quaternion.Euler(0, 0, rotationZ + x * 35);
                projectileInstance.GetComponent<Rigidbody2D>().velocity = direction * shotgunProjectileSpeed;
            }
            yield return new WaitForSeconds(shotgunTimeBetweenShots);
        }
        bossState = BossState.Idle;
    }

    [Header("SecondPhaseCircleAttack")]
    [SerializeField] float circleProjectilesToShoot = 15;
    [SerializeField] GameObject circleProjectile;
    IEnumerator SecondPhaseCircleAttack()
    {
        int rndAngle = Random.Range(0, 360);
        for (int i = 0; i < circleProjectilesToShoot; i++)
        {
            float point = i / circleProjectilesToShoot;
            float angle = (point * Mathf.PI * 2);
            float x = Mathf.Sin(angle) * 1;
            float y = Mathf.Cos(angle) * 1;
            Vector3 pos = new Vector3(x, y, 0) + ArenaCenter + new Vector3(0, 2, 0); ;
            GameObject projectile = Instantiate(circleProjectile, pos, Quaternion.Euler(0, 0, -Mathf.Rad2Deg * angle + rndAngle));
        }
        yield return new WaitForSeconds(.5f);
        bossState = BossState.Idle;
    }
    [Header("SecondPhaseLaserAttack")]
    [SerializeField] GameObject laserProjectile;
    public float LaserProjectileSpeed;
    public float LaserProjectileIncreaseRate;
    [SerializeField] float laserProjectileLiveTime;
    IEnumerator SecondPhaseLaserAttack()
    {
        int rnd = Random.Range(0, 2);
        GameObject laserInstance = Instantiate(laserProjectile, transform);
        Laser laser = laserInstance.GetComponentInChildren<Laser>();
        if (rnd == 0)
        {
            laserInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));

            laser.CanRotate = true;
            laser.RotateClockwise = false;
        }
        else if (rnd == 1)
        {
            laserInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
            laser.CanRotate = true;
            laser.RotateClockwise = true;
        }
        yield return new WaitForSeconds(laserProjectileLiveTime);
        Destroy(laserInstance);
        bossState = BossState.Idle;
    }

    #endregion

    void TakeDamage(int damageAmount)
    {
        BossHealth -= damageAmount;
        healthBar.transform.localScale = new Vector3((BossHealth / 100f) * 2f, .25f, 1f);
        if (BossHealth <= MaxBossHealth / 2 && bossPhase == BossPhase.First)
        {
            Debug.Log("Boss has entered second phase!");
            Anim.transform.GetComponent<SpriteRenderer>().sprite = secondPhaseSprite;
            Anim.transform.localPosition = Vector3.zero;
            Anim.SetBool("SecondPhase",true);
            rb.bodyType = RigidbodyType2D.Static;
            rb.gravityScale = 0;
            bossPhase = BossPhase.Second;
            bc.isTrigger = true;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 5, 0), 50 * Time.deltaTime);
        }
        if (BossHealth <= 0)
        {
            bossState = BossState.Dead;
            rb.velocity = Vector2.zero;
            healthBar.transform.localScale = Vector3.zero;
            Debug.Log("Boss Ded");
            Destroy(gameObject, 2f);
        }
    }
}

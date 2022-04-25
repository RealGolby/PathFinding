using UnityEngine;

public class Laser : MonoBehaviour
{
    BossAI bossAI;
    public bool RotateClockwise;
    public bool CanRotate;

    [SerializeField] int laserDamage;
    private void Start()
    {
        bossAI = FindObjectOfType<BossAI>();
        transform.parent.position = bossAI.ArenaCenter + new Vector3(0,1.8f,0);


    }
    void Update()
    {
        if (transform.localScale.y <= 30)
        {
            transform.localScale += new Vector3(0, bossAI.LaserProjectileIncreaseRate * Time.deltaTime, 0);
            transform.localPosition = new Vector3(0, transform.localScale.y / 2, 0);
        }

        if (RotateClockwise && CanRotate)
        {
            transform.parent.Rotate(new Vector3(0, 0, -bossAI.LaserProjectileSpeed * Time.deltaTime));
        }
        else if(!RotateClockwise && CanRotate)
        {
            transform.parent.Rotate(new Vector3(0, 0, bossAI.LaserProjectileSpeed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            FindObjectOfType<HealthSystem>().TakeDamage(laserDamage);
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}

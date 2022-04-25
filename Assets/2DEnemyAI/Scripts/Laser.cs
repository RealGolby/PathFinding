using UnityEngine;

public class Laser : MonoBehaviour
{
    BossAI bossAI;
    public bool RotateClockwise;
    public bool CanRotate;
    private void Start()
    {
        bossAI = FindObjectOfType<BossAI>();
        transform.parent.position = bossAI.ArenaCenter + new Vector3(0,1,0);


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
            transform.parent.Rotate(new Vector3(0, 0, -bossAI.LaserProjectileSpeed));
        }
        else if(!RotateClockwise && CanRotate)
        {
            transform.parent.Rotate(new Vector3(0, 0, bossAI.LaserProjectileSpeed));
        }

    }
}

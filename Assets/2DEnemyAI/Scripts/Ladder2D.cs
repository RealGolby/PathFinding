//Copyright @2018 sharpcoderr.com
//You are free to use this script in free or commercial projects
//Selling the source code of this script is not allowed

using UnityEngine;

public class Ladder2D : MonoBehaviour
{
    Collider2D ladderCollider;
    [HideInInspector]
    public Vector3 boundsCenter;

    void Start()
    {
        //t = transform;
        ladderCollider = GetComponent<Collider2D>();
        if (ladderCollider)
        {
            ladderCollider.isTrigger = true;
            ladderCollider.gameObject.layer = 2; //Set ladder collider layer to IgnoreRaycast
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ladderCollider)
            {
                boundsCenter = ladderCollider.bounds.center;
            }
            other.SendMessage("AssignLadder", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage("RemoveLadder", this, SendMessageOptions.DontRequireReceiver);
        }
    }
}

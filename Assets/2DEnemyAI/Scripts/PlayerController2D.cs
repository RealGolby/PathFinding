//Copyright @2018 sharpcoderr.com
//You are free to use this script in free or commercial projects
//Selling the source code of this script is not allowed

using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    //Move player in 2D space
    public float maxSpeed = 2.57f;
    public float jumpHeight = 6.47f;
    public float playerHP = 100;

    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public float moveDirection = 0;
    [HideInInspector]
    public Rigidbody2D r2d;
    [HideInInspector]
    public Collider2D mainCollider;
    [HideInInspector]
    public Vector2 playerDimensions;
    [HideInInspector]
    public bool isGrounded = false;
    //Check every collider except Player and Ignore Raycast
    LayerMask layerMask = ~(1 << 2 | 1 << 8); //Make sure our player has Layer 8

    [HideInInspector]
    public Ladder2D currentLadder;
    List<Ladder2D> allLadders = new List<Ladder2D>();
    float moveDirectionY = 0;
    float distanceFromLadder;
    [HideInInspector]
    public bool isAttachedToLadder = false;
    bool ladderGoingDown = false;
    //bool isMovingOnLadder = false;
    [HideInInspector]
    public bool canGoDownOnLadder = false;
    [HideInInspector]
    public bool canClimbLadder = false;

    //Bot movement directions
    [HideInInspector]
    public bool isBot = false;
    [HideInInspector]
    public float botMovement = 0;
    [HideInInspector]
    public float botVerticalMovement = 0;
    [HideInInspector]
    public bool botJump = false;
    [HideInInspector]
    public Transform t;
    [HideInInspector]
    public int selectedWeaponTmp = 0;

    float gravityScale;

    static bool ignoreCollision = false;

    // Use this for initialization
    void Start()
    {
        r2d = GetComponent<Rigidbody2D>();
        r2d.freezeRotation = true;
        mainCollider = GetComponent<Collider2D>();
        t = transform;

        gravityScale = r2d.gravityScale;
        selectedWeaponTmp = -100;

        facingRight = t.localScale.x > 0;

        //sr = GetComponent<SpriteRenderer>();
        playerDimensions = BotController2D.ColliderDimensions(GetComponent<Collider2D>());

        gameObject.layer = 8; //Setting this instance layer to 8 (If there is no such layer, add one and call it "Player")
        if (!ignoreCollision)
        {
            //Ignore collision between player and bots
            Physics2D.IgnoreLayerCollision(8, 8);

            ignoreCollision = true;
        }
    }

    void OnDisable()
    {
        r2d.bodyType = RigidbodyType2D.Static;
        r2d.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBot)
        {
            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) &&
                (isGrounded || r2d.velocity.x > 0.01f || isAttachedToLadder))
            {
                moveDirection = Input.GetKey(KeyCode.A) ? -1 : 1;
            }
            else
            {
                if (isGrounded || r2d.velocity.magnitude < 0.01f)
                    moveDirection = 0;
            }
        }
        else
        {
            if (botMovement != 0 && (isGrounded || r2d.velocity.x > 0.01f))
            {
                moveDirection = botMovement < 0 ? -1 : 1;
            }
            else
            {
                if (isGrounded || r2d.velocity.magnitude < 0.01f)
                    moveDirection = 0;
            }
        }

        //Change facing position
        if (moveDirection != 0)
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
            }
        }

        if (facingRight)
        {
            if (t.localScale.x < 0)
            {
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if (t.localScale.x > 0)
            {
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }

        //Vector2 velocityTmp = r2d.velocity;
        bool canGoDownTmp = false;

        // LADDER CONTROL START
        if (currentLadder)
        {
            distanceFromLadder = Mathf.Abs(currentLadder.boundsCenter.x - t.position.x);
            canClimbLadder = distanceFromLadder < 0.34f;
            if (!isAttachedToLadder)
            {
                if (canClimbLadder)
                {
                    if (currentLadder.boundsCenter.y > t.position.y)
                    {
                        if (!isBot)
                        {
                            if (Input.GetKey(KeyCode.W))
                            {
                                isAttachedToLadder = true;
                            }
                        }
                        else
                        {
                            if (botVerticalMovement > 0)
                            {
                                isAttachedToLadder = true;
                            }
                        }
                    }
                    if (currentLadder.boundsCenter.y < t.position.y)
                    {
                        if (!isBot)
                        {
                            if (Input.GetKey(KeyCode.S))
                            {
                                isAttachedToLadder = true;
                            }
                        }
                        else
                        {
                            if (botVerticalMovement < 0)
                            {
                                isAttachedToLadder = true;
                            }
                        }

                        canGoDownTmp = true;
                    }
                }

                if (isAttachedToLadder)
                {
                    r2d.gravityScale = 0;
                    moveDirection = 0;
                    moveDirectionY = 0;
                }
            }
            else
            {
                //Make our collider trigger if we stand on top of the ladder (To prevent collision with the ground while going down)
                mainCollider.isTrigger = currentLadder.boundsCenter.y < t.position.y; 

                //Ladder movement
                if ((!isBot && Input.GetKey(KeyCode.W)) || (isBot && botVerticalMovement > 0))
                {
                    moveDirectionY = 3.97f;
                    ladderGoingDown = false;
                    //For sound controller
                    //isMovingOnLadder = true;
                }
                else if ((!isBot && Input.GetKey(KeyCode.S)) || (isBot && botVerticalMovement < 0))
                {
                    moveDirectionY = -3.97f;
                    ladderGoingDown = true;
                    if (!mainCollider.isTrigger && isGrounded)
                    {
                        //RemoveLadder(currentLadder);
                        isAttachedToLadder = false;
                        mainCollider.isTrigger = false;
                        r2d.gravityScale = gravityScale;
                        moveDirectionY = 0;
                    }
                    //For sound controller
                    //isMovingOnLadder = true;
                }
                else
                {
                    //isMovingOnLadder = false;
                    moveDirectionY = 0;
                }
            }

            if (distanceFromLadder > playerDimensions.x * 2)
            {
                RemoveLadder(currentLadder);
            }
        }
        canGoDownOnLadder = canGoDownTmp;
        // LADDER CONTROL END

        if (!isBot)
        {
            //Jumping
            if (Input.GetKeyDown(KeyCode.W))
            {
                Jump();
            }
        }
        else
        {
            if (botJump)
            {
                botJump = false;
                Jump();
            }
        }

        if (!isBot)
        {
            //Weapon firing
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Attack();
            }
        }
    }

    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0.1f, 0);
        //Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, 0.25f, layerMask);

        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, 0.25f, 0), isGrounded ? Color.green : Color.red);

        //Apply player velocity
        r2d.velocity = new Vector2((moveDirection) * maxSpeed, isAttachedToLadder ? moveDirectionY : r2d.velocity.y);
    }

    void AssignLadder(Ladder2D ladderTmp)
    {
        currentLadder = ladderTmp;
        allLadders.Add(ladderTmp);
    }

    void RemoveLadder(Ladder2D ladderTmp)
    {
        //print("On trigger out");
        allLadders.Remove(ladderTmp);
        if (currentLadder == ladderTmp)
        {
            currentLadder = null;

            if (allLadders.Count > 0)
            {
                currentLadder = allLadders[allLadders.Count - 1];
            }
        }

        if (isAttachedToLadder && !currentLadder)
        {
            isAttachedToLadder = false;
            //r2d.bodyType = RigidbodyType2D.Dynamic;
            mainCollider.isTrigger = false;

            r2d.gravityScale = gravityScale;
            r2d.velocity = Vector3.zero;

            if (!ladderGoingDown)
            {
                r2d.velocity = new Vector2(r2d.velocity.x, 1.47f);
            }
            ladderGoingDown = false;
        }
    }

    public void Jump()
    {
        if (isGrounded && !isAttachedToLadder)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
            //Tip: Play jump sound here
        }
    }

    public void Attack()
    {
        print(gameObject.name + " is Attacking");

        //Tip: Write your attack function here (ex. Raycast toward the enemy to inflict the damage)
    }
}
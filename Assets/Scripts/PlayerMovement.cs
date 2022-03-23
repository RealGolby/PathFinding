using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float dashForceSide;
    public float dashForceUp;
    public float jumpForce;
    private GroundDetector Dscript;
    private float dragToRemember;
    private bool canDash;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Dscript = GameObject.Find("Ground detector").GetComponent<GroundDetector>();
        dragToRemember = rb.drag;
    }

    // Update is called once per frame
    void Update()
    {
        if (Dscript.IsOnGround())
        {
            canDash = true;
        }


        if (Input.GetKey(KeyCode.A) && Dscript.IsOnGround())
        {
            rb.AddForce(new Vector2(-Time.deltaTime * speed, 0));
        }
        if (Input.GetKeyDown(KeyCode.A) && Dscript.IsOnGround() == false && canDash)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(-dashForceSide, dashForceUp), ForceMode2D.Impulse);
            canDash = false;
        }


        if (Input.GetKey(KeyCode.D) && Dscript.IsOnGround())
        {
            rb.AddForce(new Vector2(Time.deltaTime * speed, 0));
        }
        if (Input.GetKeyDown(KeyCode.D) && Dscript.IsOnGround() == false && canDash)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(dashForceSide, dashForceUp), ForceMode2D.Impulse);
            canDash = false;
        }

        if (Dscript.IsOnGround() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        if (Dscript.IsOnGround())
        {
            rb.drag = dragToRemember;
        }
        else
        {
            rb.drag = 0;
        }
    }
}
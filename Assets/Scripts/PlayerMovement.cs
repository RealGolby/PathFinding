using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    bool isGrounded;

    [SerializeField] LayerMask mask;

    [SerializeField] Transform groundCheck;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        float x = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3 (x * Time.deltaTime * 8, 0, 0));
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,.15f, mask);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0, 800));
            }
        }
    }
}
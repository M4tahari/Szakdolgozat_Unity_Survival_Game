using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Mover : Fighter
{
    public float maxHealthPoints;
    public float currentHealthPoints;
    protected Vector2 moveDelta;
    public float walkSpeed = 1.25f;
    public float sprintSpeed = 2.5f;
    public float currentSpeed = 1.25f;
    protected bool isFatigued;
    protected bool isSprinting;
    public float exponentialPenalty = 1f;
    public float fatigueTimer = 0f;
    public float ySpeed = 1.25f;
    public float jumpForce = 2.0f;
    protected Rigidbody2D rb;
    public bool facingRight;
    protected bool isGrounded;
    protected bool canJump;
    protected virtual void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }
    protected virtual void UpdateMotor(Vector2 input)
    {
        moveDelta = new Vector2(input.x * currentSpeed, input.y * ySpeed);
        Vector2 oldVelocity = rb.velocity;
        oldVelocity.x = moveDelta.x;
        rb.velocity = oldVelocity;

        if(facingRight && moveDelta.x < 0)
        {
            Flip();
        }

        else if(!facingRight && moveDelta.x > 0)
        {
            Flip();
        }
    }
    protected void Flip()
    {
        this.facingRight = !this.facingRight;
        Vector3 scale = this.transform.localScale;
        scale.x *= -1;
        this.transform.localScale = scale;
    }
    protected virtual void Jump()
    {
        if (isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
            canJump = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            isGrounded = false;
        }
    }
}

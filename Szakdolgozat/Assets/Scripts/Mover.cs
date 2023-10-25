using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Mover : Interactable
{
    public float maxHealthPoints;
    public float currentHealthPoints;
    protected Vector3 moveDelta;
    public float walkSpeed;
    public float sprintSpeed;
    public float currentSpeed;
    protected Vector3 pushDirection;
    public float pushRecoverySpeed;
    public float pushForce;
    protected float lastImmune;
    public float damageImmunityTime;
    protected bool isFatigued;
    protected bool isSprinting;
    public float exponentialPenalty = 1f;
    public float fatigueTimer = 0f;
    public float ySpeed;
    public float jumpForce;
    protected Rigidbody2D rb;
    public bool facingRight;
    protected bool isGrounded;
    protected bool canJump;
    protected virtual void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }
    protected virtual void UpdateMotor(Vector3 input)
    {
        moveDelta = new Vector3(input.x * currentSpeed, input.y * ySpeed, 0);
        Vector3 oldVelocity = rb.velocity;
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

        moveDelta += pushDirection;

        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);
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
    public virtual void ReceiveDamage(float damage, Vector3 attackerPosition, float force)
    {
        if(Time.time - lastImmune > damageImmunityTime)
        {
            lastImmune = Time.time;
            currentHealthPoints -= damage;
            pushDirection = (transform.position - attackerPosition).normalized * force;
        }

        if(currentHealthPoints <= 0)
        {
            currentHealthPoints = 0;
            Death();
        }
    }
    protected virtual void Death()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Mover, Persistance
{
    public GameObject staminaBar;
    public GenerateMap map;
    public float hittingTimer;
    public float pickupRadius;
    private void Awake()
    {
        stamina = totalStamina;
        this.transform.position = new Vector3((map.mapSize * 0.32f) / 2, map.surfaceLevel + map.heightAddition + 1, 0);
    }
    private void Update()
    {
        Sprint();

        if (staminaBar != null)
        {
            staminaBar.transform.localScale = new Vector2(stamina / totalStamina, staminaBar.transform.localScale.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            canJump = true;
        }
    }
    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        UpdateMotor(new Vector2(x, y));

        if(canJump == true)
        {
            Jump();
        }
    }

    public void LoadData(WorldState state)
    {
        this.transform.position = state.playerPos;
        currentSpeed = state.currentSpeed;
        exponentialPenalty = state.exponentialPenalty;
        jumpForce = state.jumpForce;
        pickupRadius = state.pickupRadius;
        radius = state.radius;
        sprintSpeed = state.sprintSpeed;
        stamina = state.stamina;
        staminaCost = state.staminaCost;
        staminaRecoveryRate = state.staminaRecoveryRate;
        totalStamina = state.totalStamina;
        walkSpeed = state.walkSpeed;
        ySpeed = state.ySpeed;
        fatigueTimer = state.fatigueTimer;
    }
    public void SaveData(ref WorldState state)
    {
        state.playerPos = this.transform.position;
        state.currentSpeed = this.currentSpeed;
        state.exponentialPenalty = this.exponentialPenalty;
        state.jumpForce = this.jumpForce;
        state.pickupRadius = this.pickupRadius;
        state.radius = this.radius;
        state.sprintSpeed = this.sprintSpeed;
        state.stamina = this.stamina;
        state.staminaCost = this.staminaCost;
        state.staminaRecoveryRate = this.staminaRecoveryRate;
        state.totalStamina = this.totalStamina;
        state.walkSpeed = this.walkSpeed;
        state.ySpeed = this.ySpeed;
        state.fatigueTimer = this.fatigueTimer;
    }
    protected override void Jump()
    {
        if (isGrounded == true && stamina > staminaCost*2)
        {
            rb.velocity = Vector2.up * jumpForce;
            stamina -= staminaCost*2;
            canJump = false;
        }
    }
    protected void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Horizontal") != 0)
        {
            if (stamina > 0 && !isFatigued)
            {
                currentSpeed = sprintSpeed;
                isSprinting = true;
            }

            else if (isSprinting || isFatigued)
            {
                currentSpeed = walkSpeed;
                isSprinting = false;
                exponentialPenalty = 1;
            }

            exponentialPenalty += Time.deltaTime / 20f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (isSprinting || isFatigued)
            {
                currentSpeed = walkSpeed;
                isSprinting = false;
            }
        }

        if (!Input.GetKey(KeyCode.LeftShift) && exponentialPenalty > 1)
        {
            exponentialPenalty -= Time.deltaTime / 20f;
            if (exponentialPenalty < 1)
            {
                exponentialPenalty = 1f;
            }
        }

        if (isSprinting)
        {
            stamina -= (Time.deltaTime * staminaCost * exponentialPenalty);
            
        }

        else if (!isFatigued)
        {
            if(Input.GetAxis("Horizontal") == 0)
            {
                stamina += Time.deltaTime * staminaRecoveryRate * 1.5f;
            }

            else
            {
                stamina += Time.deltaTime * staminaRecoveryRate;
            }
           
        }

        if (stamina <= 0f && fatigueTimer <= 5)
        {
            fatigueTimer += Time.deltaTime;
            isFatigued = true;
            currentSpeed = walkSpeed / 2;
        }

        else if (fatigueTimer >= 5)
        {
            stamina += Time.deltaTime * staminaRecoveryRate;
            isFatigued = false;
            fatigueTimer = 0;
            currentSpeed = walkSpeed;
        }

        if (stamina < 0f)
        {
            stamina = 0f;
        }

        if (stamina > 100f)
        {
            stamina = 100f;
        }
    }
}

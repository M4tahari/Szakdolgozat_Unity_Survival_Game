using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Mover, Persistance
{
    public GameObject staminaBar;
    public float hittingTimer;
    public float pickupRadius;
    public static float visibleBlocksRadius = 10.0f;
    public SerializableDictionary<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>> items;
    private void Awake()
    {
        stamina = totalStamina;
        if(InputTextHandler.mapSize >= 1000)
        {
            this.transform.position = new Vector3((InputTextHandler.mapSize * 0.32f) / 2, InputTextHandler.surfaceLevel + InputTextHandler.heightAddition + 1, 0);
        }

        else
        {
            this.transform.position = new Vector3((1000 * 0.32f) / 2, 0.2f + 21, 0);
        }
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
    public void LoadData(WorldState worldState, PlayerState playerState)
    {
        this.transform.position = worldState.playerPos;
        currentSpeed = playerState.currentSpeed;
        exponentialPenalty = playerState.exponentialPenalty;
        jumpForce = playerState.jumpForce;
        pickupRadius = playerState.pickupRadius;
        radius = playerState.radius;
        sprintSpeed = playerState.sprintSpeed;
        stamina = playerState.stamina;
        staminaCost = playerState.staminaCost;
        staminaRecoveryRate = playerState.staminaRecoveryRate;
        totalStamina = playerState.totalStamina;
        walkSpeed = playerState.walkSpeed;
        ySpeed = playerState.ySpeed;
        fatigueTimer = playerState.fatigueTimer;
        items = playerState.items;
    }
    public void SaveData(ref WorldState worldState, ref PlayerState playerState)
    {
        worldState.playerPos = this.transform.position;
        playerState.currentSpeed = this.currentSpeed;
        playerState.exponentialPenalty = this.exponentialPenalty;
        playerState.jumpForce = this.jumpForce;
        playerState.pickupRadius = this.pickupRadius;
        playerState.radius = this.radius;
        playerState.sprintSpeed = this.sprintSpeed;
        playerState.stamina = this.stamina;
        playerState.staminaCost = this.staminaCost;
        playerState.staminaRecoveryRate = this.staminaRecoveryRate;
        playerState.totalStamina = this.totalStamina;
        playerState.walkSpeed = this.walkSpeed;
        playerState.ySpeed = this.ySpeed;
        playerState.fatigueTimer = this.fatigueTimer;
        playerState.items = this.items;
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

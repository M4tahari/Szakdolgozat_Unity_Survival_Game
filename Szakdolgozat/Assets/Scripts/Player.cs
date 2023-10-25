using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Fighter, Persistance
{
    public float maxHungerPoints;
    public float maxHydrationPoints;
    public float currentHungerPoints;
    public float currentHydrationPoints;
    public GameObject healthBar;
    public GameObject hungerBar;
    public GameObject hydrationBar;
    public GameObject staminaBar;
    public float hittingTimer;
    public float pickupRadius;
    public static float visibleBlocksRadius = 10.0f;
    public SerializableDictionary<SerializableDictionary<Item, Destroyable>, SerializableDictionary<int, int>> items;
    private void Awake()
    {
        this.stamina = totalStamina;
        this.currentHealthPoints = maxHealthPoints;
        this.currentHungerPoints = maxHungerPoints;
        this.currentHydrationPoints = maxHydrationPoints;

        if(InputTextHandler.mapSize >= 1000)
        {
            this.transform.position = new Vector3((InputTextHandler.mapSize * 0.32f) / 2, InputTextHandler.surfaceLevel + InputTextHandler.heightAddition + 1, 0);
        }

        else
        {
            this.transform.position = new Vector3((1000 * 0.32f) / 2, InputTextHandler.surfaceLevel + InputTextHandler.heightAddition + 1, 0);
        }
    }
    protected override void Start()
    {
        base.Start();

        InvokeRepeating("DrainHungerAndHydration", 10, 10);
    }
    private void Update()
    {
        Sprint();

        if (healthBar != null)
        {
            healthBar.transform.localScale = new Vector2(currentHealthPoints / maxHealthPoints, healthBar.transform.localScale.y);
        }

        if (hungerBar != null)
        {
            hungerBar.transform.localScale = new Vector2(currentHungerPoints / maxHungerPoints, hungerBar.transform.localScale.y);
        }

        if (hydrationBar != null)
        {
            hydrationBar.transform.localScale = new Vector2(currentHydrationPoints / maxHydrationPoints, hydrationBar.transform.localScale.y);
        }

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

        UpdateMotor(new Vector3(x, y, 0));

        if(canJump == true)
        {
            Jump();
        }

        TakeDamageFromHungerOrDehydration();

        Death();
    }
    public void LoadData(WorldState worldState, PlayerState playerState)
    {
        this.transform.position = worldState.playerPos;
        maxHealthPoints = playerState.maxHealthPoints;
        currentHealthPoints = playerState.currentHealthPoints;
        maxHungerPoints = playerState.maxHungerPoints;
        currentHungerPoints = playerState.currentHungerPoints;
        maxHydrationPoints = playerState.maxHydrationPoints;
        currentHydrationPoints = playerState.currentHydrationPoints;
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
        playerState.maxHealthPoints = this.maxHealthPoints;
        playerState.currentHealthPoints = this.currentHealthPoints;
        playerState.maxHungerPoints = this.maxHungerPoints;
        playerState.currentHungerPoints = this.currentHungerPoints;
        playerState.maxHydrationPoints = this.maxHydrationPoints;
        playerState.currentHydrationPoints = this.currentHydrationPoints;
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
    protected void DrainHungerAndHydration()
    {
        if(this.currentHungerPoints < 0)
        {
            this.currentHungerPoints = 0;
        }

        if(this.currentHydrationPoints < 0)
        {
            this.currentHydrationPoints = 0;
        }

        if (this.currentHungerPoints > 0)
        {
            switch ((stamina / totalStamina) < 0.5)
            {
                case true:
                    this.currentHungerPoints -= 5;
                    break;

                case false:
                    this.currentHungerPoints--;
                    break;
            }
        }

        if (this.currentHydrationPoints > 0)
        {
            switch ((stamina / totalStamina) < 0.5)
            {
                case true:
                    this.currentHydrationPoints -= 5;
                    break;

                case false:
                    this.currentHydrationPoints -= 2;
                    break;
            }
        }
    }
    protected void TakeDamageFromHungerOrDehydration()
    {
        if((this.currentHungerPoints == 0 || this.currentHydrationPoints == 0) && this.currentHealthPoints > 0)
        {
            this.currentHealthPoints -= 2;
        }

        else if((this.currentHungerPoints == 0 && this.currentHydrationPoints == 0) && this.currentHealthPoints > 0)
        {
            this.currentHealthPoints -= 5;
        }
    }
    protected override void Death()
    {
        if(this.currentHealthPoints == 0)
        {
            currentHealthPoints = maxHealthPoints;
            currentHungerPoints = maxHungerPoints;
            currentHydrationPoints = maxHydrationPoints;
            stamina = totalStamina;
            this.transform.position = new Vector3((InputTextHandler.mapSize * 0.32f) / 2, InputTextHandler.surfaceLevel + InputTextHandler.heightAddition + 1, 0);
        }
    }
}

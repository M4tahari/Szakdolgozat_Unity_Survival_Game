using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termite : Fighter
{
    public GameObject maxHealthBar;
    public GameObject currentHealthBar;
    public Item foodToPickUp;
    public float foodValue;
    public float thirstValue;
    public InventoryManager inventoryManager;
    public float attackDamage;
    public float chaseLength;
    private bool chasing;
    private Transform playerTransform;
    private bool collidingWithThePlayer;
    public Vector3 castlePosition;
    public float castleMid;
    public float castleHeight;
    public Transform castPoint;
    private ContactFilter2D filter;
    CapsuleCollider2D termiteCollider;
    private Collider2D[] collisions = new Collider2D[10];
    private void Awake()
    {
        this.stamina = totalStamina;
        this.currentHealthPoints = maxHealthPoints;
    }
    protected override void Start()
    {
        base.Start();
        this.playerTransform = GameObject.Find("Player").transform;
        this.termiteCollider = this.GetComponent<CapsuleCollider2D>();
        Physics2D.IgnoreLayerCollision(10, 10);
    }
    private void Update()
    {
        this.playerTransform = GameObject.Find("Player").transform;

        if(chasing)
        {
            Sprint();
        }

        if (maxHealthBar != null && currentHealthBar != null)
        {
            if(this.currentHealthPoints < this.maxHealthPoints)
            {
                maxHealthBar.GetComponent<SpriteRenderer>().enabled = true;
                currentHealthBar.GetComponent<SpriteRenderer>().enabled = true;
            }

            else
            {
                maxHealthBar.GetComponent<SpriteRenderer>().enabled = false;
                currentHealthBar.GetComponent<SpriteRenderer>().enabled = false;
            }

            currentHealthBar.transform.localScale = new Vector2(currentHealthPoints / maxHealthPoints, currentHealthBar.transform.localScale.y);
        }
    }
    private void FixedUpdate()
    {
        if(Vector3.Distance(this.playerTransform.position, this.castlePosition) < chaseLength)
        {
            if(Vector3.Distance(this.playerTransform.position, this.castlePosition) < this.radius)
            {
                chasing = true;
            }

            if (chasing)
            {
                if(!collidingWithThePlayer)
                {
                    UpdateMotor((this.playerTransform.position - this.transform.position).normalized);
                }

                else
                {
                    DamageThePlayer();
                }
            }

            else
            {
                UpdateMotor(this.castlePosition - this.transform.position);

                if(this.currentHealthPoints < this.maxHealthPoints)
                {
                    this.currentHealthPoints = this.maxHealthPoints;
                }
            }
        }

        else
        {
            UpdateMotor(this.castlePosition - this.transform.position);
            chasing = false;

            if (this.currentHealthPoints < this.maxHealthPoints)
            {
                this.currentHealthPoints = this.maxHealthPoints;
            }
        }

        collidingWithThePlayer = false;
        termiteCollider.OverlapCollider(filter, collisions);

        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i] == null)
            {
                continue;
            }

            if (collisions[i].tag == "Player")
            {
                collidingWithThePlayer = true;
            }

            if (collisions[i].name == "WetlandsFloorBlock(Clone)" || collisions[i].name == "MudBlock(Clone)")
            {
                this.walkSpeed = 0.75f;
                this.sprintSpeed = 1.5f;
                this.currentSpeed = walkSpeed;
            }

            if (collisions[i].name == "SandBlock(Clone)" || collisions[i].name == "TermitePlainsFloorBlock(Clone)" ||
                collisions[i].name == "TermiteCastleWallBlock(Clone)" || collisions[i].name == "JungleFloorBlock(Clone)" ||
                collisions[i].name == "DirtBlock(Clone)")
            {
                this.walkSpeed = 1.25f;
                this.sprintSpeed = 2.5f;
                this.currentSpeed = walkSpeed;
            }

            collisions[i] = null;
        }

        bool shouldJump = blockIsInFront(1);

        if(shouldJump)
        {
            Jump();
        }
    }
    protected void Sprint()
    {
        if (stamina > 0)
        {
            currentSpeed = sprintSpeed;
            isSprinting = true;
        }

        else if (!isSprinting)
        {
            currentSpeed = walkSpeed;
            isSprinting = false;
            exponentialPenalty = 1;
        }

        exponentialPenalty += Time.deltaTime / 20f;
        
        if (!isSprinting)
        {
            currentSpeed = walkSpeed;
            isSprinting = false;
        }

        if (exponentialPenalty > 5)
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

        else
        {
            stamina += Time.deltaTime * staminaRecoveryRate;
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
    protected override void Jump()
    {
        if (isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
            canJump = false;
        }
    }
    private bool blockIsInFront(float distance)
    {
        bool inFront = false;
        float castDist = distance;

        if (!facingRight)
        {
            castDist = -distance;
        }

        Vector2 endPosition = castPoint.position + Vector3.right * castDist;

        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPosition, 1 << LayerMask.NameToLayer("GroundBlock"));

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("WetlandsFloorBlock"))
            {
                inFront = true;
            }

            else
            {
                inFront = false;
            }
        }

        return inFront;
    }
    private void DamageThePlayer()
    {
        Player player = GameObject.Find("Player").GetComponent<Player>();

        if(player != null && player.currentHealthPoints > 0)
        {
            player.ReceiveDamage(this.attackDamage, this.transform.position, this.pushForce);
        }
    }
    public void PickupFood()
    {
        var random = new System.Random();
        double multipleChance = random.NextDouble();

        if (multipleChance > 0.33f && multipleChance <= 0.66f)
        {
            inventoryManager.AddFood(foodToPickUp, foodValue, thirstValue);
        }

        else if (multipleChance > 0.66f)
        {
            inventoryManager.AddFood(foodToPickUp, foodValue, thirstValue);
            inventoryManager.AddFood(foodToPickUp, foodValue, thirstValue);
        }
    }
    protected override void Death()
    {
        PickupFood();
        base.Death();
    }
}

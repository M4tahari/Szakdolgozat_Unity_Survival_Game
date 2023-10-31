using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Capybara : Mover
{
    public GameObject maxHealthBar;
    public GameObject currentHealthBar;
    public float runningLength;
    private bool runningAway;
    private Transform playerTransform;
    public Vector3 startingPosition;
    public Transform castPoint;
    private ContactFilter2D filter;
    CapsuleCollider2D capybaraCollider;
    private Collider2D[] collisions = new Collider2D[10];
    private void Awake()
    {
        this.currentHealthPoints = maxHealthPoints;
    }
    protected override void Start()
    {
        base.Start();
        this.playerTransform = GameObject.Find("Player").transform;
        this.startingPosition = this.transform.position;
        this.capybaraCollider = this.GetComponent<CapsuleCollider2D>();
        Physics2D.IgnoreLayerCollision(10, 10);
    }
    private void Update()
    {
        this.playerTransform = GameObject.Find("Player").transform;
        this.startingPosition = this.transform.position;

        if (runningAway)
        {
            Sprint();
        }

        if (maxHealthBar != null && currentHealthBar != null)
        {
            if (this.currentHealthPoints < this.maxHealthPoints)
            {
                maxHealthBar.SetActive(true);
                currentHealthBar.SetActive(true);
            }

            currentHealthBar.transform.localScale = new Vector2(currentHealthPoints / maxHealthPoints, currentHealthBar.transform.localScale.y);
        }
    }
    private void FixedUpdate()
    {
        bool shouldJump = false;

        if (Vector3.Distance(this.playerTransform.position, this.startingPosition) < runningLength)
        {
            if (Vector3.Distance(this.playerTransform.position, this.startingPosition) < this.radius)
            {
                runningAway = true;
            }

            if (runningAway)
            {
                UpdateMotor(-(this.playerTransform.position - this.transform.position).normalized);
                shouldJump = blockIsInFront(1);
            }

            else
            {
                UpdateMotor(this.startingPosition - this.transform.position);
            }
        }

        else
        {
            UpdateMotor(this.startingPosition - this.transform.position);
            runningAway = false;
            shouldJump = false;
        }

        capybaraCollider.OverlapCollider(filter, collisions);

        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i] == null)
            {
                continue;
            }

            if (collisions[i].name == "WetlandsFloorBlock(Clone)" || collisions[i].name == "MudBlock(Clone)")
            {
                this.walkSpeed = 1f;
                this.sprintSpeed = 2f;
                this.currentSpeed = walkSpeed;
            }

            if (collisions[i].name == "SandBlock(Clone)" || collisions[i].name == "TermitePlainsFloorBlock(Clone)" ||
                collisions[i].name == "TermiteCastleWallBlock(Clone)" || collisions[i].name == "JungleFloorBlock(Clone)" ||
                collisions[i].name == "DirtBlock(Clone)")
            {
                this.walkSpeed = 0.75f;
                this.sprintSpeed = 1.5f;
                this.currentSpeed = walkSpeed;
            }

            collisions[i] = null;
        }

        if (shouldJump)
        {
            Jump();
        }
    }
    protected void Sprint()
    {
        currentSpeed = sprintSpeed;
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

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("WetlandsFloorBlock"))
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
}

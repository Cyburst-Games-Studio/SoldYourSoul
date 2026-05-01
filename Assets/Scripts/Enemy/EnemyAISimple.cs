using UnityEngine;

public class EnemyAISimple : Enemy
{
    public bool canDoubleJump;
    private bool airJump;

    new void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        airJump = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(DistanceToPlayer() < detectionRadius)
        {
            Move();

            if (direction.y > 0.3)
            {
                Jump();
            }
        }
    }
    private void Update()
    {
        if (CanJump())
        {
            airJump = true;
        }
    }

    new void Jump()
    {
        if (CanJump())
        {
            rb.linearVelocityY = 0;
            rb.AddForceY(jumpForce);
        }
        else if (CanDoubleJump())
        {
            airJump = false;
            rb.linearVelocityY = 0;
            rb.AddForceY(jumpForce);
        }
    }

    bool CanDoubleJump()
    {
        if (!canDoubleJump) return false;           // if the enemy is able to double jump
        if (CanJump()) return false;                // if the enemy is not grounded
        if (!airJump) return false;                 // if the enemy has not used their mid air jump already
        if (rb.linearVelocityY >= 0) return false;  // if the enemy is already falling
        return true;
    }
}

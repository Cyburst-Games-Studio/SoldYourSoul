using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Basic Parameters")]
    public int enemyHealth = 10;
    public float detectionRadius = 12f;
    public float moveSpeed = 4;
    public float jumpForce = 3000;
    public int soulDropped = 4;
    public float iFrameScale = 0.2f;
    public Transform groundCheck;
    public LayerMask ground;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Transform player;
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public bool canDamage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        canDamage = true;
    }

    // handles taking damage from player weapons and projectiles
    // lowers the health of this enemy by that of the object dealing damage
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerDamage")
        {
            if (canDamage)
            {
                // get the reference of the opposing object's Weapon script
                try
                {
                    PlayerWeaponBehavior weapon = collision.GetComponent<PlayerWeaponBehavior>();
                    int damage = weapon.weaponDamage;

                    // check if the weapon is a melee weapon to apply the effect of Strength upgrade
                    if (!weapon.ranged && PlayerMaster.PM.playerAb.HasAbility("Strength"))
                    {
                        damage = Mathf.RoundToInt(damage * 1.2f);
                    }

                    // apply damage to the enemy
                    enemyHealth -= damage;
                    canDamage = false;
                    DeathProcedure();

                    // apply iFrames
                    if (enemyHealth > 0)
                    {
                        StartCoroutine(ApplyIFrames(damage * iFrameScale));
                    }
                }
                catch
                {
                    Debug.Log("cannot deal damage due to an error");
                    canDamage = true;
                }
            }
        }
    }

    // handles basic death procedures
    // despawns the enemy and awards the player with health upon defeat
    private void DeathProcedure()
    {
        if(enemyHealth <= 0)
        {
            PlayerMaster.PM.playerDh.AddHealth(soulDropped, true);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ApplyIFrames(float timer)
    {
        yield return new WaitForSeconds(timer);
        canDamage = true;
    }

    // gets the distance to the player to apply further logic in future subclasses
    public float DistanceToPlayer()
    {
        direction = player.position - transform.position;
        direction.x = Mathf.Clamp(direction.x, -1, 1); direction.y = Mathf.Clamp(direction.y, -1, 1);

        return Vector2.Distance(transform.position, player.position);
    }

    // moves the enemy around
    public void Move()
    {
        rb.linearVelocityX = moveSpeed * direction.x;
    }

    // determines if the enemy is grounded and can therfor jump
    public bool CanJump()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
    }

    // performs a jump similar to the player
    public void Jump()
    {
        if (CanJump())
        {
            rb.linearVelocityY = 0;
            rb.AddForceY(jumpForce);
        }
    }
}

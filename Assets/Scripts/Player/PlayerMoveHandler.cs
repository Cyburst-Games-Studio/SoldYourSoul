using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 dir;
    private CameraFollowBehavior cam;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 0.2f;
    [SerializeField] float jumpForce = 2f;
    // jumping and double jumping
    private Transform groundCheck;
    [SerializeField] LayerMask ground;
    private bool canDoubleJump;

    // knockback when hit by enemies
    [HideInInspector] public bool stunned;
    [SerializeField] private Vector2 knockbackForce;

    // implementing coyoteTime
    float coyoteTimer;
    float coyoteTimeTo = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("Player/GroundCheck").transform;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraFollowBehavior>();
        canDoubleJump = true;
    }

    void FixedUpdate()
    {
        // move the player based on the given input
        if (!stunned)
        {
            rb.linearVelocityX = dir.x * (PlayerMaster.PM.playerAb.HasAbility("Speedrunner") ? moveSpeed * 1.2f: moveSpeed);
            transform.localScale = new Vector3(dir.x != 0 ? Mathf.Round(dir.x) : transform.localScale.x, 1, 1);
        }
    }

    private void Update()
    {
        // update the coyote time, a period of time when the player has dropped off a ledge but can still jump
        if (CanJump())
        {
            coyoteTimer = coyoteTimeTo;
            canDoubleJump = true;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    void OnMove(InputValue value)
    {
        dir = value.Get<Vector2>();
        // Normalize vectors
        dir.x = Mathf.Round(dir.x);
        dir.y = Mathf.Round(dir.y);
        cam.SetPanAngle(dir.y);
    }

    // handle the jump input
    void OnJump()
    {
        if(!stunned)
            if (CanJump() || (!CanJump() && coyoteTimer >= 0f))
            {
                Jump();
            }
            else if (PlayerMaster.PM.playerAb.HasAbility("Double Jump") && !CanJump() && canDoubleJump)
            {
                canDoubleJump = false;
                Jump();
            }
    }
    void Jump()
    {
        coyoteTimer = -1f;
        rb.linearVelocityY = 0;
        rb.AddForceY(PlayerMaster.PM.playerAb.HasAbility("High Jump") ? jumpForce * 1.2f : jumpForce);
    }
    
    bool CanJump()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
    }

    public void Knockback(int dmg)
    {
        stunned = true;
        rb.AddForce(Vector2.up * knockbackForce.y + (Vector2.left * transform.localScale.x) * dmg * knockbackForce.x);
        Invoke("UnStun", 0.5f);
    }
    void UnStun()
    {
        stunned = false;
    }
}

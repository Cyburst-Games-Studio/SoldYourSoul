using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMoveHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 dir;
    private CameraFollowBehavior cam;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 2f;
    [SerializeField] float chargeSpeed = 15;

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

    // impletmenting the CHARGE upgrade
    float currentSpeed;

    // implementing the PHASE upgrade
    bool phased;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("Player/GroundCheck").transform;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraFollowBehavior>();
        canDoubleJump = true;
        currentSpeed = moveSpeed;
        phased = false;
    }

    void FixedUpdate()
    {
        // move the player based on the given input
        if (!stunned)
        {
            rb.linearVelocityX = dir.x * (PlayerMaster.PM.playerAb.HasAbility("Speedrunner") ? currentSpeed * 1.2f: currentSpeed);

            // flip the player when needed
            transform.localScale = new Vector3(dir.x != 0 ? Mathf.Round(dir.x) : transform.localScale.x, 1, 1);
            GameObject[] childWeapons = GameObject.FindGameObjectsWithTag("PlayerWeapon");
            foreach (GameObject w in childWeapons)
            {
               w.transform.localScale = new Vector3(Mathf.Abs(w.transform.localScale.x) * transform.localScale.x, w.transform.localScale.y, 1);
            }
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

    // get the movement direction
    public void OnMove(InputValue value)
    {
        dir.x = value.Get<float>();
    }
    // perform a short dash by
    // - instantly increase the speed
    // - gradually reduce that speed until it returns to normal
    public IEnumerator Charge()
    {
        if(currentSpeed > moveSpeed * 2) yield break;

        currentSpeed = chargeSpeed;

        while(currentSpeed > moveSpeed)
        {
            yield return new WaitForFixedUpdate();
            currentSpeed -= 0.1f;
        }
        currentSpeed = moveSpeed;
    }

    // Get the look angle
    public void OnLook(InputValue value)
    {
        // Get normalized vector
        dir.y = Mathf.Round(value.Get<float>());
        cam.SetPanAngle(dir.y);
    }

    // handle the jump input
    public void OnJump()
    {
        if (stunned) return;

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

    public IEnumerator Phase()
    {
        if (phased) yield break;

        phased = true;
        Camera pointCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        Vector3 mousePos = pointCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector3 dir = transform.position - mousePos;

        // draw the raycast
        RaycastHit2D[] ray = Physics2D.RaycastAll(transform.position, dir, Vector2.Distance(transform.position, mousePos));

        if (ray.Length < 3)
        {
            transform.localPosition = mousePos;
        }


        yield return new WaitForSeconds(2.0f);
        phased = false;
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

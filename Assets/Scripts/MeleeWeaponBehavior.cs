using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeWeaponBehavior : MonoBehaviour
{
    private Transform player;
    private Camera cam;
    [SerializeField] private float maxHoverDistance = 1.2f;
    [SerializeField] private Vector3 worldOffset;
    [Header("Weapon Stats")]
    public int weaponDamage;
    public float weaponCooldown;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        // get mouse position
        Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        // move but clamp to stay near the player
        Vector3 dir = mousePos - player.position;
        Vector3 targetPos = player.position + Vector3.ClampMagnitude(dir + worldOffset, maxHoverDistance);

        transform.position = targetPos;

        // find the distance for later
        float dist = Vector3.Distance(player.position, targetPos);

        // point toward the mouse
        Vector3 lookDir = mousePos - transform.position;
        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // flip the weapon over if needed
        // if the weapon is on the outer edge
        if (dist >= (maxHoverDistance * 0.98))
        {
            Vector3 scale = transform.localScale;
            // flip the y scale if
            if (player.localScale.x < 0 && rotZ < -90 || player.localScale.x > 0 && rotZ > 90)
            {
                transform.localScale = new Vector3(scale.x, -Mathf.Abs(scale.y), 1);
            }
            else if(player.localScale.x < 0 && rotZ > 90 || player.localScale.x > 0 && rotZ < -90)
            {
                transform.localScale = new Vector3(scale.x, Mathf.Abs(scale.y), 1);
            }
        }
    }
}


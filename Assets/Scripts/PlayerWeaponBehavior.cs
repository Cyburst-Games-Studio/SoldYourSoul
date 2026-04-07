using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeaponBehavior : MonoBehaviour
{
    private Transform player;
    private Camera cam;

    // Parameters for how the weapon object interacts with the world around the player
    [Header("Player Interaction")]
    [SerializeField] private float maxHoverDistance = 1.2f;
    [SerializeField] private Vector3 worldOffset;
    [SerializeField] private float forwardRotOffset;

    // Weapon stats
    [Header("Weapon Stats")]
    public int weaponDamage;
    public float weaponCooldown;
    public Animation anim;
    private bool onCooldown;

    // handles ranged weapons, can leave blank in inspector for melee weapons
    [SerializeField] private bool ranged;
    public GameObject projectile;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        onCooldown = false;
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
        transform.rotation = Quaternion.Euler(0, 0, rotZ + forwardRotOffset);

        // flip the weapon over if needed
        // if the weapon is on the outer edge
        Vector3 scale = transform.localScale;

        // flip the weapon when depending on which side of the player it is on
        if (player.localScale.x < 0)
        {
            scale.x = -Mathf.Abs(scale.x);
            forwardRotOffset = Mathf.Abs(forwardRotOffset);
        }
        else if (player.localScale.x > 0)
        {
            scale.x = Mathf.Abs(scale.x);
            forwardRotOffset = -Mathf.Abs(forwardRotOffset);
        }

        if (dist >= maxHoverDistance * .98)
        {
            if (transform.localPosition.x < 0 && player.localScale.x > 0 || transform.localPosition.x > 0 && player.localScale.x < 0)
            {
                scale.y = -Mathf.Abs(scale.y);
                forwardRotOffset = Mathf.Abs(forwardRotOffset);
            }
            else if (transform.localPosition.x > 0 && player.localScale.x > 0 || transform.localPosition.x < 0 && player.localScale.x < 0)
            {
                scale.y = Mathf.Abs(scale.y);
                forwardRotOffset = -Mathf.Abs(forwardRotOffset);
            }
        }
        else if (scale.y < 0)
        {
            scale.y = Mathf.Abs(scale.y);
        }

        // set the scale of the weapon object
        transform.localScale = scale;
    }

    public void OnAttack()
    {
        // do nothing if the weapon is on cooldown
        if (onCooldown) return;

        // if the weapon is a ranged weapon, spawn the associated projectile before continuing
        if (ranged)
        {
            Instantiate(projectile);
        }

        // play the associated animation, then place the weapon on cooldown for the set amount of time
        anim.Play();
        onCooldown = true;
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(weaponCooldown);
        onCooldown = false;
    }
}

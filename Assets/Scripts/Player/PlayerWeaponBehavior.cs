using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeaponBehavior : MonoBehaviour
{
    private Transform player;
    private Camera cam;

    // Parameters for how the weapon object interacts with the world around the player
    [Header("Player Interaction")]
    [SerializeField] private float hoverDistance = 1.2f;
    [SerializeField] private float forwardRotOffset;

    // Weapon stats
    [Header("Weapon Stats")]
    public int weaponDamage;
    public float weaponCooldown;
    public Animation anim;
    private bool onCooldown;
    private bool isAttacking;

    // handles ranged weapons, can leave blank in inspector for melee weapons
    public bool ranged;
    public GameObject projectile;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        onCooldown = false;
        isAttacking = false;

        if (ranged) gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isAttacking)
        {
            // get mouse position
            Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            // set the weapon a fixed distance from the player
            Vector3 dir = (mousePos - player.position).normalized;
            Vector3 targetPos = player.position + dir * hoverDistance;
            transform.position = targetPos;

            // set up the direction for the weapon to point
            Vector3 lookDir = mousePos - player.position;
            float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            // flip the weapon over if needed
            Vector3 scale = transform.localScale;

            // if the weapon is behind the player, flip it
            if (transform.localPosition.x < 0)
            {
                scale.y = -Mathf.Abs(scale.y);
                forwardRotOffset = Mathf.Abs(forwardRotOffset);
            }
            else
            {
                scale.y = Mathf.Abs(scale.y);
                forwardRotOffset = -Mathf.Abs(forwardRotOffset);
            }

            // adjust the direction based on the direction of the player
            scale.y *= (player.localScale.x * Mathf.Abs(player.localScale.x)); // the equation forces the result to be either 1 or -1, just in case the player's scale is altered.
            forwardRotOffset *= (player.localScale.x * Mathf.Abs(player.localScale.x));

            // set the scale of the weapon object as well as the rotation
            transform.localScale = scale;
            transform.rotation = Quaternion.Euler(0, 0, rotZ + forwardRotOffset);
        }
    }

    public IEnumerator OnAttack(bool meleeAttack)
    {
        // make sure this weapon does the correct behavior
        // either attack or hide depending on with button was pressed
            //if(meleeAttack && ranged)     // hide
            //if(meleeAttack && !ranged)    // appear
            //if(!meleeAttack && ranged)    // appear
            //if(!meleeAttack && !ranged)   // hide
        
        // this statment covers all cases
        if(meleeAttack == ranged)
        {
            // hide the weapon and break
            this.gameObject.SetActive(false);
            yield break;
        }
        else
        {
            // make the object appear and continue on
            this.gameObject.SetActive(true);
        }

        // do nothing if the weapon is on cooldown
        if (onCooldown) yield break;

        // if the weapon is a ranged weapon, spawn the associated projectile before continuing
        if (ranged)
        {
            // get mouse position
            Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            // set the weapon a fixed distance from the player
            Vector3 dir = (mousePos - player.position).normalized;

            // set up the direction for the weapon to point
            Vector3 lookDir = mousePos - player.position;
            float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            GameObject proj = Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, rotZ-90));
            proj.GetComponent<WeaponProjectileBehavior>().damage = weaponDamage;
            proj.GetComponent<WeaponProjectileBehavior>().dir = dir;
        }

        // play the associated animation, then place the weapon on cooldown for the set amount of time
        //isAttacking = true;
        //anim.Play();
        //while (anim.isPlaying)
        //{
        //    yield return null;
        //}
        //isAttacking = false;
        onCooldown = true;
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        // check for ranged weapon and for the QUICK SHOTS upgrade to reduce the length of the cooldown
        if (ranged && PlayerMaster.PM.playerAb.HasAbility("Quick Shots"))
        {
            yield return new WaitForSeconds(weaponCooldown * 0.75f);
            onCooldown = false;
            yield break;
        }

        // if this part is reached, then the player does not have the QUICK SHOTS upgrade, so wait for the usual amount of time
        yield return new WaitForSeconds(weaponCooldown);
        onCooldown = false;
    }
}

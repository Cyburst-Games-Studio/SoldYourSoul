using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Parameters")]
    [SerializeField] private int enemyHealth;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int soulDropped;
    private Rigidbody2D rb;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerDamage")
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

                DeathProcedure();
            }
            catch
            {
                Debug.Log("cannot deal damage due to an error");
            }
        }
    }

    private void DeathProcedure()
    {
        if(enemyHealth <= 0)
        {
            PlayerMaster.PM.playerDh.AddHealth(soulDropped, true);
        }
    }
}

using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerDamageHandler : MonoBehaviour
{


    [Header("Player Health")]
    public int playerHealth;
    public TMP_Text healthText;
    [HideInInspector] bool invincible;

    private void Start()
    {
        invincible = false;
        UpdateHealthUI();
    }

    public void SetHealth(int hp)
    {
        playerHealth = hp;
    }


    // recovers the player's health by a set amount
    public void AddHealth(int amount, bool enemyDefeat)
    {
        if (enemyDefeat)
        {
            // increase the player's health, accounting for the VAMPIRIC VITALITY upgrade, which increases health gained
            playerHealth += PlayerMaster.PM.playerAb.HasAbility("Vampiric Vitality") ? Mathf.CeilToInt(amount * 1.2f) : amount;
            return;
        }

        playerHealth += amount;
    }

    // Handle collisions with enemies
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            if (!invincible)
            {
                int amount = GameMasterHandler.gm.difficulty;
                invincible = true;

                if (PlayerMaster.PM.playerAb.HasAbility("Armor"))
                {
                    amount--;
                    if (amount < 0) amount = 0;
                }
                    
                playerHealth -= amount;


                PlayerMaster.PM.playerMh.Knockback(GameMasterHandler.gm.difficulty);
                DeathProcedure();
                UpdateHealthUI();
                Invoke("UnInvincible", 2.0f);
            }
        }
    }

    public void UpdateHealthUI()
    {
        healthText.text = playerHealth.ToString();
    }

    void UnInvincible()
    {
        invincible = false;
    }

    void OnMelee()
    {
        Debug.Log("Melee");
    }

    void OnRanged()
    {
        Debug.Log("Ranged");
    }

    void DeathProcedure()
    {
        if(playerHealth <= 0)
        {
            if(PlayerMaster.PM.playerAb.HasAbility("Undying Will")){
                SetHealth(GameMasterHandler.gm.difficulty + 1);
                int i = PlayerMaster.PM.playerAb.FindAbility("Undying Will");
                PlayerMaster.PM.playerAb.DestroyAbility(i);
                PlayerMaster.PM.playerAb.UpdateAbilityDisplay(i);
            }
            else
            {
                Debug.Log("You are DEAD!!!!");
            }
        }
    }
}

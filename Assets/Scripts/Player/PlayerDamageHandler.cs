using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerDamageHandler : MonoBehaviour
{


    [Header("Player Health")]
    public int playerHealth;
    public TMP_Text healthText;
    [HideInInspector] public bool invincible;

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
    private IEnumerator OnCollisionStay2D(Collision2D collision)
    {
        // early returns
        // if the collided object is not an enemy or the player is invulnerable, skip damage
        if (!collision.gameObject.tag.Equals("Enemy")) yield break;
        if (invincible) yield break;

        // get the damage based off the game's current difficulty
        int amount = GameMasterHandler.gm.difficulty;
        invincible = true;

        // reduce the incoming damage if the player has the ARMOR upgrade
        if (PlayerMaster.PM.playerAb.HasAbility("Armor"))
        {
            amount--;
            if (amount < 0) amount = 0;
        }

        // reduce the player health
        playerHealth -= amount;

        // apply knockback
        PlayerMaster.PM.playerMh.Knockback(GameMasterHandler.gm.difficulty);
        // run the deathprocedure
        DeathProcedure();
        // update UI
        UpdateHealthUI();
        // return the player to a vulnerable state after x seconds
        yield return new WaitForSeconds(2.0f);
        invincible = false;
    }

    private IEnumerator OnTriggerStay2D(Collider2D collision)
    {
        // early returns
        // if the collided object is not an enemy or the player is invulnerable, skip damage
        if (!collision.gameObject.tag.Equals("Enemy")) yield break;
        if (invincible) yield break;

        // get the damage based off the game's current difficulty
        int amount = GameMasterHandler.gm.difficulty;
        invincible = true;

        // reduce the incoming damage if the player has the ARMOR upgrade
        if (PlayerMaster.PM.playerAb.HasAbility("Armor"))
        {
            amount--;
            if (amount < 0) amount = 0;
        }

        // reduce the player health
        playerHealth -= amount;

        // apply knockback
        PlayerMaster.PM.playerMh.Knockback(GameMasterHandler.gm.difficulty);
        // run the deathprocedure
        DeathProcedure();
        // update UI
        UpdateHealthUI();
        // return the player to a vulnerable state after x seconds
        yield return new WaitForSeconds(2.0f);
        invincible = false;
    }

    public void UpdateHealthUI()
    {
        healthText.text = playerHealth.ToString();
    }

    public void OnMelee()
    {
        PlayerWeaponBehavior[] weapons = GetComponentsInChildren<PlayerWeaponBehavior>(true);

        foreach (PlayerWeaponBehavior pwb in weapons)
        {
            StartCoroutine(pwb.OnAttack(true));
        }
    }

    public void OnRanged()
    {
        PlayerWeaponBehavior[] weapons = GetComponentsInChildren<PlayerWeaponBehavior>(true);

        foreach (PlayerWeaponBehavior pwb in weapons)
        {
            StartCoroutine(pwb.OnAttack(false));
        }
    }

    void DeathProcedure()
    {
        if(playerHealth <= 0)
        {
            if(PlayerMaster.PM.playerAb.HasAbility("Undying Will")){
                SetHealth(GameMasterHandler.gm.difficulty + 1);
                int i = PlayerMaster.PM.playerAb.FindAbilityByName("Undying Will");
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

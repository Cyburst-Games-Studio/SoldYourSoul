using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopHandler : MonoBehaviour
{
    //[HideInInspector]
    public PlayerAbility[] abilitiesForSale = new PlayerAbility[4];

    [Header("UI Information")]
    [SerializeField] private TMP_Text[] shopNameText;
    [SerializeField] private TMP_Text[] shopCostText;
    [SerializeField] private Image      shopInfoIcon;
    [SerializeField] private TMP_Text   shopInfoName;
    [SerializeField] private TMP_Text   shopInfoText;
    [SerializeField] private GameObject shopCanvas;

    [Header("Static Shops")]
    [Tooltip("Setting this overrides standard generation, allowing for specific upgrades to be placed in this shop"), SerializeField] 
    private bool StaticShop;

    [Tooltip("Use the name of any abilities you want to spawn in this shop."), SerializeField]
    private string[] StaticAbilities;

    int activeChoice = -1;
    Coroutine shopRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeChoice = -1;
        shopRoutine = null;

        gameObject.GetComponentInChildren<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        if (!StaticShop)
        {
            // initialize the ability list for sale
            for (int i = 0; i < abilitiesForSale.Length; i++)
            {
                bool selected = false;
                while (!selected)
                {
                    int randAb = Random.Range(0, GameMasterHandler.gm.playerAbilities.Count);

                    abilitiesForSale[i].Set(GameMasterHandler.gm.playerAbilities[randAb]);

                    // roll to keep the ability
                    if (abilitiesForSale[i].rarity > Random.Range(0, 26))
                    {
                        selected = true;
                    }

                    // check other slots for duplicate entries
                    for (int j = 0; j < abilitiesForSale.Length; j++)
                    {
                        if (j != i && abilitiesForSale[j].abilityName.Equals(abilitiesForSale[i].abilityName))
                        {
                            selected = false;
                            break;
                        }
                    }
                }
            }
        }
        // sets the shop according to editor input if the shop is set to static
        else
        {
            for (int i = 0; i < StaticAbilities.Length; i++)
            {
                abilitiesForSale[i].Set(GameMasterHandler.gm.FindAbilityByName(StaticAbilities[i]));
            }
        }

        // assign the buttons for each shop ability
        for (int i = 0; i < shopNameText.Length; i++)
        {
            try 
            {  
                shopNameText[i].text = abilitiesForSale[i].abilityName; 
                shopCostText[i].text = abilitiesForSale[i].cost.ToString();
            }
            catch { }  
        }

        shopCanvas.SetActive(false);
    }

    // updates the info texts to match the item the player is currently viewing
    public void UpdateInfoText(int index)
    {
        activeChoice = index;
        shopInfoIcon.sprite = abilitiesForSale[index].icon;
        shopInfoIcon.color = Color.white;
        shopInfoName.text = abilitiesForSale[index].abilityName;
        shopInfoText.text = abilitiesForSale[index].description;
    }

    // handles purchasing of abilities and upgrades of the player, handling
    // all situations
    public void PurchaseAbility()
    {
        // stop current routine if present
        if (shopRoutine != null) StopCoroutine(shopRoutine);

        // run the shop routine
        shopRoutine = StartCoroutine(PurchaseAbilityRoutine());  
    }

    IEnumerator PurchaseAbilityRoutine()
    {
        // check if a choice is selected in the shop
        if (activeChoice < 0)
        {
            yield break;
        }

        // check if the player has enough to purchase
        if (PlayerMaster.PM.playerDh.playerHealth <= abilitiesForSale[activeChoice].cost)
        {
            shopInfoText.text = "You do not have enough soul to purchase this.";
            yield break;
        }

        // check if the player already has this upgrade
        for (int i = 0; i < PlayerMaster.PM.playerAb.abilityList.Length; i++)
        {
            if (PlayerMaster.PM.playerAb.abilityList[i].abilityName.Equals(abilitiesForSale[activeChoice].abilityName))
            {
                shopInfoText.text = "You already own this.";
                yield break;
            }
        }

        // check if the player has a conflicting upgrade
        // the player can only have 1 melee weapon, 1 ranged weapon, and 2 specials at a time
        if (abilitiesForSale[activeChoice].attribute.Equals('m') || abilitiesForSale[activeChoice].attribute.Equals('r'))
        {
            foreach (PlayerAbility ab in PlayerMaster.PM.playerAb.abilityList)
            {
                // if the attribute of the selected upgrade for sale
                if (ab.attribute.Equals(abilitiesForSale[activeChoice].attribute))
                {
                    // prompt the player to remove the conflicting upgrade
                    shopInfoName.text = "Conflict detected!";
                    shopInfoText.text = ab.abilityName + " conflicts with the selected upgrade, please remove";
                    shopInfoIcon.color = Color.clear;

                    PlayerMaster.PM.playerAb.SetPromptState(true, ab.attribute);

                    while (PlayerMaster.PM.playerAb.promptState)
                    {
                        yield return null;
                    }

                    // finally, add the ability
                    PlayerMaster.PM.playerAb.AddAbility(abilitiesForSale[activeChoice]);
                    CompletePurchase();

                    yield break;
                }
            }
        }
        
        // find an open slot to place the ability
        // if no such slot exists, prompt the player to delete an existing ability
        if (PlayerMaster.PM.playerAb.AddAbility(abilitiesForSale[activeChoice]) == -1)
        {
            // no slot was found, player must discard an ability to make space for the new one
            shopInfoName.text = "You have too many upgrades";
            shopInfoText.text = "Please select one to discard";
            shopInfoIcon.color = Color.clear;
            PlayerMaster.PM.playerAb.SetPromptState(true);

            while (PlayerMaster.PM.playerAb.promptState)
            {
                yield return null;
            }

            // finally, add the ability
            PlayerMaster.PM.playerAb.AddAbility(abilitiesForSale[activeChoice]);
        }
        // complete the transaction
        CompletePurchase();

    }

    void CompletePurchase()
    {
        // complete the transaction
        PlayerMaster.PM.playerDh.playerHealth -= abilitiesForSale[activeChoice].cost;
        PlayerMaster.PM.playerDh.UpdateHealthUI();

        shopInfoName.text = string.Empty;
        shopInfoText.text = "Thank you for your purchase\n\n";
        shopInfoIcon.color = Color.clear;
        shopRoutine = null;
    }

    // handles opening and closing the shop menu when the player gets close
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            shopCanvas.SetActive(true);
            shopInfoName.text = "To Begin";
            shopInfoText.text = "Click on any item to view details or purchase";
            shopInfoIcon.color = Color.clear;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            activeChoice = -1;
            shopCanvas.SetActive(false);

            if(shopRoutine != null) StopCoroutine(shopRoutine);
            shopRoutine = null;

            PlayerMaster.PM.playerAb.SetPromptState(false);
        }
    }
}

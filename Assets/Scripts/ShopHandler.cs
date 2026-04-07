using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopHandler : MonoBehaviour
{
    public PlayerAbility[] abilitiesForSale = new PlayerAbility[4];

    [SerializeField] private TMP_Text[] shopNameText;
    [SerializeField] private TMP_Text[] shopCostText;

    [SerializeField] private Image shopInfoIcon;
    [SerializeField] private TMP_Text shopInfoName;
    [SerializeField] private TMP_Text shopInfoText;
    [SerializeField] private GameObject shopCanvas;

    int activeChoice = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeChoice = -1;

        gameObject.GetComponentInChildren<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        // initialize the ability list for sale
        for (int i = 0; i < abilitiesForSale.Length; i++)
        {
            bool selected = false;
            while (!selected)
            {
                abilitiesForSale[i].Set(GameMasterHandler.gm.playerAbilities[Random.Range(1, GameMasterHandler.gm.playerAbilities.Length)]);

                // roll to keep the ability
                if (abilitiesForSale[i].rarity > Random.Range(0, 26))
                {
                    selected = true;
                }

                // check other slots for duplicate entries
                for(int j = 0; j < abilitiesForSale.Length; j++)
                {
                    if (j != i && abilitiesForSale[j].name.Equals(abilitiesForSale[i].name))
                    {
                        selected = false;
                        break;
                    }
                }
            }
        }
        // assign the buttons for each shop ability
        for (int i = 0; i < shopNameText.Length; i++)
        {
            try 
            {  
                shopNameText[i].text = abilitiesForSale[i].name; 
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
        shopInfoName.text = abilitiesForSale[index].name;
        shopInfoText.text = abilitiesForSale[index].description;
    }

    // handles purchasing of abilities and upgrades of the player, handling
    // all situations
    public void PurchaseAbility()
    {
        StartCoroutine(PurchaseAbilityPayLoad());  
    }

    IEnumerator PurchaseAbilityPayLoad()
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
            if (PlayerMaster.PM.playerAb.abilityList[i].name.Equals(abilitiesForSale[activeChoice].name))
            {
                shopInfoText.text = "You already own this.";
                yield break;
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
            PlayerMaster.PM.playerAb.inPrompt = true;

            while (PlayerMaster.PM.playerAb.inPrompt)
            {
                yield return null;
            }

            // re-add the ability
            PlayerMaster.PM.playerAb.AddAbility(abilitiesForSale[activeChoice]);
        }
        // complete the transaction
        PlayerMaster.PM.playerDh.playerHealth -= abilitiesForSale[activeChoice].cost;
        PlayerMaster.PM.playerDh.UpdateHealthUI();

        shopInfoName.text = string.Empty;
        shopInfoText.text = "Thank you for your purchase\n\n";
        shopInfoIcon.color = Color.clear;

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
            shopCanvas.SetActive(false);
            StopCoroutine(PurchaseAbilityPayLoad());
            PlayerMaster.PM.playerAb.inPrompt = false;
        }
    }
}

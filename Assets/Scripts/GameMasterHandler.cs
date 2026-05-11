using UnityEngine;
using System.Collections.Generic;

public class GameMasterHandler : MonoBehaviour
{
    [HideInInspector]
    public static GameMasterHandler gm;

    [Header("General")]
    public int difficulty;      // the game's difficulty - the higher the number, the harder the game

    [Header("Player Abilities")]
    public List<PlayerAbility> playerAbilities = new();

    //[Header("Statistics Handling")]

    void Awake()
    {
        // singleton behavior
        if (GameMasterHandler.gm != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            GameMasterHandler.gm = this;
        }

        // This object will carry between scenes
        DontDestroyOnLoad(this.gameObject);

        // load abilities from Resources
        PlayerAbilityScriptableObject[] abList = Resources.LoadAll<PlayerAbilityScriptableObject>("PlayerAbilities/");

        foreach(PlayerAbilityScriptableObject ab in abList)
        {
            playerAbilities.Add(ab.playerAbility);
        }
    }

    // finds an ability in the list by its given name
    public PlayerAbility FindAbilityByName(string searchName)
    {
        foreach (PlayerAbility ab in playerAbilities)
        {
            if (ab.abilityName == searchName)
            {
                return ab;
            }
        }

        return new PlayerAbility();
    }
    
}

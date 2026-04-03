using UnityEngine;

public class GameMasterHandler : MonoBehaviour
{
    [HideInInspector]
    public static GameMasterHandler gm;

    [Header("General")]
    public int difficulty;      // the game's difficulty - the higher the number, the harder the game

    [Header("Player Abilities")]
    public PlayerAbility[] playerAbilities;

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
    }
}

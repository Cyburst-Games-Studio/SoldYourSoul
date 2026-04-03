using UnityEngine;

public class PlayerMaster : MonoBehaviour
{
    [HideInInspector] public static PlayerMaster PM;

    public PlayerAbilityManager playerAb;
    public PlayerDamageHandler playerDh;
    public PlayerMoveHandler playerMh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // singleton behavior
        if (PlayerMaster.PM != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            PlayerMaster.PM = this;
        }
    }
}

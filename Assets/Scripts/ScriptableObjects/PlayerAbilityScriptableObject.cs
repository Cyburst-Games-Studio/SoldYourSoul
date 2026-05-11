using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAbilityObject", menuName = "ScriptableObjects/PlayerAbilityObject")]
public class PlayerAbilityScriptableObject : ScriptableObject
{
    /* A SPECIAL THANKS TO THOSE WHO CONTRIBUTED IDEAS FOR PLAYER UPGRADES
     *  @Charbrose
     *  @Benny_Blue
     */

    // stores a PlayerAbility object whose details are outlined in its class
    public PlayerAbility playerAbility;
}

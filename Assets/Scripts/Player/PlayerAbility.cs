using UnityEngine;

[System.Serializable]
public class PlayerAbility
{
    public string name;         // the name used in menus
    public string description;  // short description used in menus
    public char attribute;      // the type of ability it is (m=melee;r=ranged;p=passive;a=active)
    public short rarity;       // controls how rare the ability is in shops. The higer the number, the more common. caps at 25
    public ushort cost;         // the cost of the ability in shops.
    public Sprite icon;         // an icon for the ability to make distinguishing at a glance easier

    public PlayerAbility()
    {
        name = string.Empty;
        description = string.Empty;
        attribute = char.MinValue;
        rarity = -1;
        cost = 0;
        icon = null;
    }

    // returns true is a desired ability is present
    public bool IsAbility(string nm)
    {
        return name.Equals(nm);
    }

    // returns the type of ability
    public char getType()
    {
        return attribute;
    }

    public void Set(PlayerAbility ab)
    {
        name = ab.name;
        description = ab.description;
        attribute = ab.attribute;
        rarity = ab.rarity;
        cost = ab.cost;
        icon = ab.icon;
    }

    public void Clear()
    {
        name = string.Empty;
        description = string.Empty;
        attribute = char.MinValue;
        rarity = -1;
        cost = 0;
        icon = null;
    }
}

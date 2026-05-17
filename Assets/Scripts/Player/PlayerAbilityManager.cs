using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbilityManager : MonoBehaviour
{
    public PlayerAbility[] abilityList = new PlayerAbility[5];
    public GameObject[] abilitydisplays = new GameObject[5];
    [HideInInspector] public bool promptState;
    private char attributeConflict;

    private void Start()
    {
        promptState = false;
        attributeConflict = ' ';
        for(int i = 0; i < abilityList.Length; i++)
        {
            abilityList[i] = new PlayerAbility();
        }
    }

    public int AddAbility(PlayerAbility ab)
    {
        for(int i = 0; i < abilityList.Length; i++)
        {
            if (abilityList[i].abilityName == string.Empty)
            {
                // add the ability to the list
                abilityList[i].Set(ab);
                UpdateAbilityDisplay(i);

                // check if the current upgrade is a weapon object, then instantiate it from resources
                if (abilityList[i].attribute == 'm' || abilityList[i].attribute == 'r')
                {
                    GameObject weapon = Resources.Load<GameObject>("Weapons/" + abilityList[i].abilityName);
                    Instantiate(weapon, transform.position, Quaternion.identity, transform);
                }

                return 0;
            }
        }
        return -1;
    }

    public void PromptToRemoveAbility(int index)
    {
        // if the player is not prompted, do nohting
        if (!promptState) return;

        // check if the ability matches potential conflict
        if (!attributeConflict.Equals(' ') && !abilityList[index].attribute.Equals(attributeConflict)) return;

        DestroyAbility(index);
    }

    public void SetPromptState(bool state)
    {
        promptState = state;
    }
    public void SetPromptState(bool state, char attributeToFind)
    {
        promptState = state;
        attributeConflict = attributeToFind;
    }

    public void DestroyAbility(int index)
    {
        // check if the current upgrade is a weapon, then remove it 
        if (abilityList[index].attribute == 'm' || abilityList[index].attribute == 'r')
        {
            Destroy(transform.Find(abilityList[index].abilityName + "(Clone)").gameObject);
        }

        // clear out the slot
        abilityList[index].Clear();
        SetPromptState(false);
    }

    public void UpdateAbilityDisplay(int index)
    {
        abilitydisplays[index].GetComponent<Image>().sprite = abilityList[index].icon;
        abilitydisplays[index].GetComponentInChildren<TMP_Text>().text = abilityList[index].abilityName;
    }

    public bool HasAbility(string name)
    {
        foreach (PlayerAbility ab in abilityList)
        {
            if (ab.IsAbility(name)) return true;
        }
        return false;
    }

    public int FindAbilityByName(string name)
    {
        for (int i = 0; i < abilityList.Length; i++)
        {
            if (abilityList[i].IsAbility(name)) return i;
        }
        return -1;
    }
    
    void OnSpecial1()
    {
        StartCoroutine(PlayerMaster.PM.playerMh.Phase());
    }
}

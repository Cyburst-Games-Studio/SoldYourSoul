using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbilityManager : MonoBehaviour
{
    public PlayerAbility[] abilityList = new PlayerAbility[5];
    public GameObject[] abilitydisplays = new GameObject[5];
    [HideInInspector] public int indexToRemove;
    [HideInInspector] public bool inPrompt;

    private void Start()
    {
        inPrompt = false;
        for(int i = 0; i < abilityList.Length; i++)
        {
            abilityList[i] = new PlayerAbility();
        }
    }

    public int AddAbility(PlayerAbility ab)
    {
        for(int i = 0; i < abilityList.Length; i++)
        {
            if (abilityList[i].name == string.Empty)
            {
                abilityList[i].Set(ab);
                UpdateAbilityDisplay(i);
                return 0;
            }
        }
        return -1;
    }

    public void RemoveAbility(int slot)
    {
        if (inPrompt)
        {
            abilityList[slot].Clear();
            inPrompt = false;
        }
    }

    public void DestroyAbility(int index)
    {
        abilityList[index].Clear();
    }

    public void UpdateAbilityDisplay(int index)
    {
        abilitydisplays[index].GetComponent<Image>().sprite = abilityList[index].icon;
        abilitydisplays[index].GetComponentInChildren<TMP_Text>().text = abilityList[index].name;
    }
    public void PromptToRemoveAbility()
    {
        inPrompt = true;
    }

    public bool HasAbility(string name)
    {
        foreach (PlayerAbility ab in abilityList)
        {
            if (ab.IsAbility(name)) return true;
        }
        return false;
    }

    public int FindAbility(string name)
    {
        for (int i = 0; i < abilityList.Length; i++)
        {
            if (abilityList[i].IsAbility(name)) return i;
        }
        return -1;
    }
}

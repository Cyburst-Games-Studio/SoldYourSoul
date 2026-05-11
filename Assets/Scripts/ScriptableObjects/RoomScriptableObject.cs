using UnityEngine;

[CreateAssetMenu(fileName = "RoomObject", menuName = "ScriptableObjects/RoomObject")]
public class RoomScriptableObject : ScriptableObject
{
    public GameObject room;
    public int weight;
}

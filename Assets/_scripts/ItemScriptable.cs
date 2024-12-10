using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Iventory/Item", order = 1)]
public class ItemScriptable : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;
}

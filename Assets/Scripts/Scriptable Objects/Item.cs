using UnityEngine;

[CreateAssetMenu(fileName = "Item.Asset", menuName = "Item")]
public class Item : ScriptableObject
{
    [Header("General Information")]
    public string itemName;
    public int itemValue;

    [Header("Item ID")] 
    public int itemID;

    [Header("Sprite References")] 
    public Sprite itemIcon;
}

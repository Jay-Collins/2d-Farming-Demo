using UnityEngine;

[CreateAssetMenu(fileName = "Crop.Asset", menuName = "Crop")]
public class Crop : ScriptableObject
{
    [Header("General Information")]
    public string cropName;
    public int stageDays;

    [Header("ID Of Seed Tool")] 
    public int seedToolID;

    [Header("Sprite References")] 
    public Sprite cropIcon;
    public Sprite stage1;
    public Sprite stage2;
    public Sprite stage3;
    public Sprite stage4;

    [Header("Multiple Harvests")] 
    public bool multipleHarvests;
}

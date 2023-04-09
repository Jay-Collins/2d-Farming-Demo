using UnityEngine;

[CreateAssetMenu(fileName = "Crop.Asset", menuName = "Crop")]
public class Crop : ScriptableObject
{
    [Header("General Information")]
    public string cropName;
    public int StageDays;

    [Header("Sprite References")]
    public Sprite stage1;
    public Sprite Stage2;
    public Sprite Stage3;
    public Sprite Stage4;

    [Header("Multiple Harvests")] 
    public bool multipleHarvests;
}

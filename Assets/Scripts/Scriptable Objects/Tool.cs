using UnityEngine;

[CreateAssetMenu(fileName = "Tool.Asset", menuName = "Tool")]
public class Tool : ScriptableObject
{
    [Header("General Information")]
    public string toolName;
    public int toolID;

    [Header("Sprite References")] 
    public Sprite toolIcon;
}

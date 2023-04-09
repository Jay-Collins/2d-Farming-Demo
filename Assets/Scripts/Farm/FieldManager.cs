using System;
using UnityEngine;

public struct FieldPerameters
{
    public bool watered;
    public ScriptableObject crop;
    public Sprite sprite;
    public Sprite plantSprite;
    public bool multipleHarvests;
}
    
public class FieldManager : MonoSingleton<FieldManager>
{
    public FieldPerameters[,] field = new FieldPerameters[9, 15];
    
    [Header("References")] 
    [SerializeField] private Sprite _dirtSprite;
    [SerializeField] private Sprite _wetDirtSprite;
    [SerializeField] private Sprite _seededDirtSprite;
    [SerializeField] private Sprite _seededWetDirtSprite;

    public const float _tileSize = 0.16f;

    private void Start()
    {
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                // set each cells default parameters. 
                field[i, j].watered = false;
                field[i, j].sprite = _dirtSprite;
                field[i, j].crop = null;
                field[i, j].plantSprite = null;
                field[i, j].multipleHarvests = false; 

                // create game object with a SpriteRenderer then assign the sprite parameter sprite
                GameObject fieldTile = new GameObject("Field Cell(" + i + "," + j + ")");
                fieldTile.AddComponent<SpriteRenderer>().sprite = field[i, j].sprite;
                
                // move new game objects into correct positions
                fieldTile.transform.position = new Vector3(transform.position.x + j * _tileSize, transform.position.y + i * _tileSize,0);
            }
        }
    }
}

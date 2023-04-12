using UnityEngine;

public class FieldManager : MonoSingleton<FieldManager>
{
    // field array
    public GameObject[,] field = new GameObject[9, 15];
    
    // sprite references needed for the new field tile game objects
    [Header("References")] 
    [SerializeField] public Sprite dirtSprite;
    [SerializeField] public Sprite wetDirtSprite;
    [SerializeField] public Sprite seededDirtSprite;
    [SerializeField] public Sprite seededWetDirtSprite;
    [SerializeField] public Sprite cursorSprite;

    // the pixel size of tiles along the X axis
    public const float _tileSize = 0.16f; 

    private void Start()
    {
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                // create game object with a SpriteRenderer then assign the sprite parameter sprite
                GameObject fieldTile = new GameObject("Field Cell(" + i + "," + j + ")");
                fieldTile.transform.parent = gameObject.transform;
                var fieldTileClass = fieldTile.AddComponent<FieldTile>();
                Vector2Int fieldPos = new Vector2Int(i, j);
                fieldTileClass.OnCreation(fieldPos);

                // move new game objects into correct positions
                fieldTile.transform.position = new Vector3(transform.position.x + j * _tileSize, transform.position.y + i * _tileSize,0);
                
                // assign to array
                field[i,j] = fieldTile;
            }
        }
    }
}

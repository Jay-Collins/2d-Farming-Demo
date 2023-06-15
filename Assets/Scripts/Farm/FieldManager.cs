using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoSingleton<FieldManager>
 {
     // field array
     public GameObject[,] field = new GameObject[15, 21];
     
     // sprite references needed for the new field tile game objects
     [Header("References")] 
     [SerializeField] public Sprite dirtSprite;
     [SerializeField] public Sprite tilledDirtSprite;
     [SerializeField] public Sprite wetDirtSprite;
     [SerializeField] public Sprite seededDirtSprite;
     [SerializeField] public Sprite seededWetDirtSprite;
     [SerializeField] public Sprite cursorSprite;
     
     [Header("Crop Scriptable Objects")]
     [SerializeField] public List<ScriptableObject> _cropsAssetFiles;
 
     // the pixel size of tiles along the X axis
     public const float _tileSize = 0.16f; 
 
     private void Start()
     {
         for (int i = 1; i < field.GetLength(0); i++) // rows
         {
             for (int j = 1; j < field.GetLength(1); j++) // columns
             {
                 // skips tile creation for first and last rows and columns
                 // these "bleed" tiles in the array allow for easy cursor calculation
                 // i > 2 skips the fist 3 rows, i < field.GetLength(0) - 3 skips the last 3 rows
                 // j > skips the first 3 columns, j < field.GetLength(1) - 1 skips the last column
                 if (i > 2 && i < field.GetLength(0) - 3 && j > 2 && j < field.GetLength(1) - 3)
                 {
                     // create game object with a SpriteRenderer then assign the sprite parameter sprite
                     GameObject fieldTile = new GameObject("Field Cell(" + i + "," + j + ")");
                     fieldTile.transform.parent = gameObject.transform;
                     var fieldTileClass = fieldTile.AddComponent<FieldTile>();
                     Vector2Int fieldPos = new Vector2Int(i, j);
                     fieldTileClass.OnCreation(fieldPos);
 
                     // move new game objects into correct positions
                     fieldTile.transform.position = new Vector3(transform.position.x +j * _tileSize, transform.position.y + i * _tileSize,0);
                 
                     // assign to array
                     field[i, j] = fieldTile;
                 }
             }
         }
     }
     
     private void OnDrawGizmos()
     {
         Gizmos.color = Color.red;

         // Loop through field multidimensional array
         for (int i = 0; i < field.GetLength(0); i++) // rows
         {
             for (int j = 0; j < field.GetLength(1); j++) // columns
             {
                 // Check if its a skipped row or column
                 if (i <= 2 || i >= field.GetLength(0) - 3 || j <= 2 || j >= field.GetLength(1) - 3)
                 {
                     // draw a wire cube on in the places where it didnt generate a field tile
                     Gizmos.DrawWireCube(new Vector3(transform.position.x + j * _tileSize, transform.position.y + i * _tileSize, 0f), new Vector3(_tileSize, _tileSize, 0f));
                 }
             }
         }
     }
 }

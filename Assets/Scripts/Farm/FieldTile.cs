using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldTile : MonoBehaviour
{
    private SpriteRenderer _groundRenderer;
    private SpriteRenderer _plantRenderer;
    private SpriteRenderer _cursorRenderer;

    private Vector2Int _fieldPos;

    // scriptable object of the data for the crop
    private ScriptableObject _crop;
    
    // if tile has been watered
    private bool _watered;
    
    // days without being watered
    private int _dehydration;
    
    private void OnEnable()
    {
        PlayerFarming.showCursor += ShowCursor;
    }
    
    public void OnCreation(Vector2Int arrayPos)
    {
        // stores location within array
        _fieldPos = arrayPos;
        
        // creates child objects for the ground, the cursor, and the plant
        // then applies the default settings for each
        var plantTile = new GameObject("Plant Tile");
        plantTile.transform.parent = gameObject.transform;
        _plantRenderer = plantTile.AddComponent<SpriteRenderer>();
        
        var cursorTile = new GameObject("Cursor Tile");
        cursorTile.transform.parent = gameObject.transform;
        _cursorRenderer = cursorTile.AddComponent<SpriteRenderer>();
        _cursorRenderer.sprite = FieldManager.instance.cursorSprite;
        _cursorRenderer.sortingOrder = 1;
        _cursorRenderer.enabled = false;
        
        var groundTile = new GameObject("Ground Tile");
        groundTile.transform.parent = gameObject.transform;
        _groundRenderer = groundTile.AddComponent<SpriteRenderer>();
        _groundRenderer.sprite = FieldManager.instance.dirtSprite;
    }

    private void NewDay()
    {
        
    }

    // tells the game object to enable or disable the cursor game objects sprite renderer 
    public void ShowCursor(List<Vector2Int> pattern)
    {
        if (pattern.Contains(_fieldPos))
        {
            _cursorRenderer.enabled = true;
            Debug.Log("Show Cursor" + _fieldPos);
        }
        else
        {
            Debug.Log("Hide Cursor");
            _cursorRenderer.enabled = false;
        }
    }

    public bool CursorOn()
    {
        return _cursorRenderer.enabled;
    }
}

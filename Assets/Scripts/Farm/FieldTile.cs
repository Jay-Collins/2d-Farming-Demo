using System;
using UnityEngine;

public class FieldTile : MonoBehaviour
{
    private SpriteRenderer _groundRenderer;
    private SpriteRenderer _plantRenderer;
    private SpriteRenderer _cursorRenderer;

    private void OnEnable()
    {
        PlayerFarming.hideCursor += HideCursor;
    }

    // scriptable object of the data for the crop
    private ScriptableObject _crop;
    
    // if tile has been watered
    private bool _watered;
    
    // days without being watered
    private int _dehydration;
    
    public void OnCreation()
    {
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
    public void ShowCursor() => _cursorRenderer.enabled = true;
    public void HideCursor() => _cursorRenderer.enabled = false;
}

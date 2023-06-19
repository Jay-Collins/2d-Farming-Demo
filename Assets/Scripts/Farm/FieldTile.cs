using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldTile : MonoBehaviour
{
    private SpriteRenderer _groundRenderer;
    private SpriteRenderer _plantRenderer;
    private SpriteRenderer _cursorRenderer;

    private BoxCollider2D _collider; // the field tilers collider, needed for picking crop
    private Crop _crop; // scriptable object of the data for the crop planted on the tile
    private Vector2Int _fieldPos; // fields position in the array
    
    private bool _watered; // if tile has been watered that day
    private bool _tilled; // if tile has been tilled
    private bool _seeded; // if the tile has seeds
    private bool _plantGrowing; // if the tile has a plant growing
    private int _dehydration; // days without being watered
    private int _plantStage; // stage of the plants growth cycle
    
    
    private void OnEnable()
    {
        PlayerFarming.showCursor += ShowCursor;
        PlayerFarming.usedTool += ToolWasUsed;
        GameManager.newDay += OnNewDay;
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

        _collider = gameObject.AddComponent<BoxCollider2D>();
        _collider.size = new Vector2(0.16f, 0.16f);
        _collider.enabled = false;
    }

    private void OnNewDay()
    {
        if (_watered)
        {
            _watered = false;
            _dehydration = 0;

            if (_seeded)
            {
                _plantGrowing = true;
                _seeded = false;
            }
            
            if (_plantGrowing)
            {
                _plantStage += 1;
                
                switch (_plantStage)
                {
                    case 1:
                        _plantRenderer.enabled = true;
                        _plantRenderer.sprite = _crop.stage1;
                        break;
                    case 2:
                        _plantRenderer.sprite = _crop.stage2;
                        break;
                    case 3:
                        _plantRenderer.sprite = _crop.stage3;
                        break;
                    case 4:
                        _plantRenderer.sprite = _crop.stage4;
                        gameObject.layer = LayerMask.NameToLayer("Interactable");
                        gameObject.tag = "PickableCrop";
                        _collider.enabled = true;
                        break;
                }
            }
            
            _groundRenderer.sprite = FieldManager.instance.tilledDirtSprite;
        }
            
        if (!_watered && _plantGrowing || _seeded)
        {
            _dehydration += 1;
            if (_dehydration >= 3)
            {
                _seeded = false;
                _plantGrowing = false;
                _tilled = false;
            }
        }
    }

    // tells the game object to enable or disable the cursor game objects sprite renderer 
    private void ShowCursor(List<Vector2Int> pattern)
    {
        if (pattern.Contains(_fieldPos))
        {
            _cursorRenderer.enabled = true;
        }
        else
        {
            _cursorRenderer.enabled = false;
        }
    }

    private void ToolWasUsed(List<Vector2Int> pattern, int toolID)
    {
        switch (toolID)
        {
            case 0: // empty hands, nothing happens
                break;
            //==========================================================================================================
            case 1: // watering can, water the tile
                if (!_tilled) return; // if not tilled watering can does nothing

                if (pattern.Contains(_fieldPos) && !_watered && !_seeded)
                {
                    // if tile is not watered and does not have seeds
                    _watered = true;
                    _groundRenderer.sprite = FieldManager.instance.wetDirtSprite;
                }
                else if (pattern.Contains(_fieldPos) && !_watered && _seeded)
                {
                    // if the tile is not watered and does have seeds
                    _watered = true;
                    _groundRenderer.sprite = FieldManager.instance.seededWetDirtSprite;
                }
                else if (pattern.Contains(_fieldPos) && !_watered && _plantGrowing)
                {
                    // if the tile is not watered and has a plant growing
                    _watered = true;
                    _groundRenderer.sprite = FieldManager.instance.wetDirtSprite;
                }

                break;
            // =========================================================================================================
            case 2: // hoe, till the tile
                if (pattern.Contains(_fieldPos) && !_tilled)
                {
                    _tilled = true;
                    _groundRenderer.sprite = FieldManager.instance.tilledDirtSprite;
                }
                else if (pattern.Contains(_fieldPos) && _tilled && _crop == null)
                {
                    // reset tile if already tilled and a plant is not growing
                    _seeded = false;
                    _watered = false;
                    _tilled = false;
                    _groundRenderer.sprite = FieldManager.instance.dirtSprite;
                }

                break;
            // =========================================================================================================
            // all tool ID's from 3 on are different types of seeds 
            case >= 3:
                if (pattern.Contains(_fieldPos) && !_seeded && !_plantGrowing && _tilled && PlayerInventory.instance.GetTools().Any(tool => tool.toolID == toolID))
                {
                    var crops = FieldManager.instance._cropsAssetFiles;

                    foreach (Crop crop in crops)
                    {
                        if (crop.seedToolID == toolID)
                        {
                            PlayerInventory.instance.RemoveCurrentTool(toolID);
                            _crop = crop;
                            _seeded = true;
                            _groundRenderer.sprite = _watered ? FieldManager.instance.seededWetDirtSprite : FieldManager.instance.seededDirtSprite;
                        }
                    }
                }

                break;
        }
    }

    public void PickPlant()
    {
        GameManager.disablePlayerMovement.Invoke(); // disable movement
        ResetTile();
        var holdableItemPrefab = GameManager.instance._holdableItemPrefab.GetComponent<HoldableItem>();
        var instantiatedObject = Instantiate(holdableItemPrefab, transform.position, 
            Quaternion.identity);

        instantiatedObject.itemData = _crop.correspondingItem; // enable movement
        PlayerPickUp.instance.PickupItem();
    }

    private void ResetTile()
    {
        // Leaves the ground tilled and watered if already so
        _collider.enabled = false;
        _groundRenderer.sprite = FieldManager.instance.tilledDirtSprite;
        _plantRenderer.sprite = null;
        _seeded = false;
        _plantGrowing = false;
        _dehydration = 0;
        _plantStage = 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFarming : MonoSingleton<PlayerFarming>
{
    public static Action<List<Vector2Int>> showCursor;
    public static Action<List<Vector2Int>, int> usedTool;

    private List<Vector2Int> _cursorPositions;
    private Vector2Int _currentCell;
    
    private bool _displayCursor;
    private bool _useTool;
    private int _direction;
    private int _toolID;
    

    private void OnEnable()
    {
        InputManager.walkStarted += DisplayCursor;
        InputManager.walkCanceled += HideCursor;
        InputManager.useTool += UseTool;
    }  
    
    private void Start()
    {
        _currentCell = DetermineCell();
        
    }

    private void Update()
    {
        if (_useTool)
            UsedTool();
    }

    private Vector2Int DetermineCell()
    {
        var playerPos = new Vector3(transform.position.x + FieldManager._tileSize / 2, transform.position.y, transform.position.z);
        var fieldPos = FieldManager.instance.transform.position;
        var relativePos = playerPos - fieldPos;

        var cellX = Mathf.FloorToInt(relativePos.x / FieldManager._tileSize);
        var cellY = Mathf.FloorToInt(relativePos.y / FieldManager._tileSize);

        var numberOfRows = FieldManager.instance.field.GetLength(0);
        var numberOfColumns = FieldManager.instance.field.GetLength(1);

        // check if the cell the player is standing on is within the bounds of the array
        var isOnCell = cellX >= 0 && cellX < numberOfColumns && cellY >= 0 && cellY < numberOfRows;

        if (isOnCell)
        {
            return new Vector2Int(cellY, cellX);
        }

        // when the player is not on a cell
        return new Vector2Int(-1, -1);
    }
    
    private void CheckCellDisplayCursor()
    {
        // run DetermineCell method to find out what cell player is standing on
        var newCell = DetermineCell();

        if (_currentCell != newCell)
        {
            _currentCell = newCell;
            CalculateCursor();
        }
    }
    
    public void CalculateCursor()
    {
        var numberOfRows = FieldManager.instance.field.GetLength(0);
        var numberOfColumns = FieldManager.instance.field.GetLength(1);
        
        // checks if the current cell (which is the cell under) is within the bounds of the array
        if (_currentCell.y >= 0 && _currentCell.y < numberOfColumns && _currentCell.x >= 0 && _currentCell.x < numberOfRows)
        {
            // cursorPosition is the list that gets sent with the Action to display the cursors
            // offsetCurrentCell stores the current cell under the player to be manipulated
            // solves issues with manipulating the current cell directly
            _cursorPositions = new List<Vector2Int>();
            var offsetCurrentCell = new Vector2Int(_currentCell.x, _currentCell.y);
            
            // display cursor for cells based on tool in hands
            switch (PlayerInventory.instance.GetEquippedTool().toolID)
            {
                case 0: // empty hands =================================================================================
                    // empty hands displays 1 cursor offset 1 tile in front of the player
                    // due to how the array was generated the x and y axis are swapped
                    _toolID = 0;
                    
                    switch (_direction)
                    {
                        case 0: offsetCurrentCell.x += 1; break; // up
                        case 1: offsetCurrentCell.x -= 1; break; // down
                        case 2: offsetCurrentCell.y -= 1; break; // left
                        case 3: offsetCurrentCell.y += 1; break; // right
                    }
                    _cursorPositions.Add(offsetCurrentCell);
                    
                    if (_cursorPositions.Count > 0)
                        showCursor.Invoke(_cursorPositions);
                    break;
                
                case 1: // watering can ================================================================================
                    // the rows and columns properly align a 3x3 grid based _currentCell position
                    _toolID = 1;
                    
                    for (int i = -1; i <= 1; i++) // rows
                    {
                        for (int j = -1; j <= 1; j++) // columns 
                        {
                            // offset the current cell for each part of the pattern, creating a 3x3 grid
                            offsetCurrentCell = new Vector2Int(_currentCell.x + i, _currentCell.y + j);

                            // check player direction and offset pattern accordingly
                            // due to how the array was generated the x and y axis are swapped
                            switch (_direction)
                            {
                                case 0: offsetCurrentCell.x += 2; break; // up
                                case 1: offsetCurrentCell.x -= 2; break; // down
                                case 2: offsetCurrentCell.y -= 2; break; // left
                                case 3: offsetCurrentCell.y += 2; break; // right
                            }
                            
                            _cursorPositions.Add(offsetCurrentCell);
                        }
                    }
 
                    if (_cursorPositions.Count > 0)
                        showCursor.Invoke(_cursorPositions);
                    break;
                
                case 2: // hoe =========================================================================================
                    // the hoe tool displays a 1x3 pattern so we check direction first and adjust it based on direction
                    // due to how the array was generated the x and y axis are swapped
                    _toolID = 2;
                    
                    switch (_direction)
                    {
                        case 0: // up
                            for (int i = -1; i <= 1; i++) // rows
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x + 1, _currentCell.y + i);
                                _cursorPositions.Add(offsetCurrentCell);
                            }
                            break;
                        
                        case 1: // down
                            for (int i = -1; i <= 1; i++) // rows
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x - 1, _currentCell.y + i);
                                _cursorPositions.Add(offsetCurrentCell);
                            }
                            break;
                        
                        case 2: // left
                            for (int j = -1; j <= 1; j++) // columns
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x +j, _currentCell.y -1);
                                _cursorPositions.Add(offsetCurrentCell);
                            }
                            break;

                        case 3: // right
                            for (int j = -1; j <= 1; j++) // columns
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x +j, _currentCell.y +1);
                                _cursorPositions.Add(offsetCurrentCell);
                            }
                            break;
                    }

                    if (_cursorPositions.Count > 0)
                    {
                        _cursorPositions.Reverse();
                        showCursor.Invoke(_cursorPositions);
                    }
                    break;
                
                case >= 3: // seeds =======================================================================================
                    // ===!!!      for the sake of this demo the watering can and seeds use the same pattern      !!!===
                    // ===!!!   in a final game these would level up for the watering can and change over time    !!!===
                    // ===!!!   the watering can section would end up looking vastly different in final product   !!!===
                    _toolID = PlayerInventory.instance.GetEquippedTool().toolID;
                    
                    // the rows and columns properly align a 3x3 grid based _currentCell position
                    for (int i = -1; i <= 1; i++) // rows
                    {
                        for (int j = -1; j <= 1; j++) // columns 
                        {
                            // offset the current cell for each part of the pattern, creating a 3x3 grid
                            offsetCurrentCell = new Vector2Int(_currentCell.x + i, _currentCell.y + j);

                            // seeds to not offset the pattern but rather keep the player in the middle
                            _cursorPositions.Add(offsetCurrentCell);
                        }
                    }
 
                    if (_cursorPositions.Count > 0)
                        showCursor.Invoke(_cursorPositions);
                    break;
            }
        }
        else 
        {
            showCursor.Invoke(new List<Vector2Int>());
        }
    }
    
    private void UseTool()
    {
        if (PlayerInventory.instance.CheckHands() || !_displayCursor) return; // return if item is in hands
        
        DisplayCursor();
        _useTool = true;
    }
    
    private void UsedTool()
    {
        usedTool.Invoke(_cursorPositions, _toolID);
        _useTool = false;
    }
    
    private void DisplayCursor()
    {
        _displayCursor = true;
        _direction = PlayerAnimation.instance.CheckDirection();
        StartCoroutine(WhileDisplayCursor());
    }

    private void HideCursor()
    {
        _displayCursor = false;
        StopCoroutine(WhileDisplayCursor());
        _currentCell = new Vector2Int(-1, -1);
        showCursor.Invoke(new List<Vector2Int>());
    }
    
    private IEnumerator WhileDisplayCursor()
    {
        while (_displayCursor)
        {
            CheckCellDisplayCursor();
            yield return new WaitForEndOfFrame();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFarming : MonoSingleton<PlayerFarming>
{
    public static Action<List<Vector2Int>> showCursor;

    private bool _displayCursor;
    private Vector2Int _currentCell;
    private int _direction;

    private void OnEnable()
    {
        InputManager.walkStarted += DisplayCursor;
        InputManager.walkCanceled += HideCursor;
    }

    private void Start()
    {
        _currentCell = DetermineCell();
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
            Debug.Log(_currentCell);
            CalculateCursor();
        }
    }
    
    private void CalculateCursor()
    {
        var numberOfRows = FieldManager.instance.field.GetLength(0);
        var numberOfColumns = FieldManager.instance.field.GetLength(1);
        
        // checks if the current cell (which is the cell under) is within the bounds of the array
        if (_currentCell.y >= 0 && _currentCell.y < numberOfColumns && _currentCell.x >= 0 && _currentCell.x < numberOfRows)
        {
            // cursorPosition is the list that gets sent with the Action to display the cursors
            // offsetCurrentCell stores the current cell under the player to be manipulated
            // solves issues with manipulating the current cell directly
            var cursorPositions = new List<Vector2Int>();
            var offsetCurrentCell = new Vector2Int(_currentCell.x, _currentCell.y);
            
            // display cursor for cells based on tool in hands
            switch (PlayerInventory.instance.GetTool())
            {
                case 0: // empty hands =================================================================================
                    // empty hands displays 1 cursor offset 1 tile in front of the player
                    // due to how the array was generated the x and y axis are swapped
                    switch (_direction)
                    {
                        case 0: offsetCurrentCell.x += 1; break; // up
                        case 1: offsetCurrentCell.x -= 1; break; // down
                        case 2: offsetCurrentCell.y -= 1; break; // left
                        case 3: offsetCurrentCell.y += 1; break; // right
                    }
                    cursorPositions.Add(offsetCurrentCell);
                    
                    if (cursorPositions.Count > 0)
                        showCursor.Invoke(cursorPositions);
                    break;
                
                case 1: // watering can ================================================================================
                    // the rows and columns properly align a 3x3 grid based _currentCell position
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
                            
                            cursorPositions.Add(offsetCurrentCell);
                        }
                    }
 
                    if (cursorPositions.Count > 0)
                        showCursor.Invoke(cursorPositions);
                    break;
                
                case 2: // hoe =========================================================================================
                    // the hoe tool displays a 1x3 pattern so we check direction first and adjust it based on direction
                    // due to how the array was generated the x and y axis are swapped
                    switch (_direction)
                    {
                        case 0: // up
                            for (int i = -1; i <= 1; i++) // rows
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x + 1, _currentCell.y + i);
                                cursorPositions.Add(offsetCurrentCell);
                            }
                            break;
                        
                        case 1: // down
                            for (int i = -1; i <= 1; i++) // rows
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x - 1, _currentCell.y + i);
                                cursorPositions.Add(offsetCurrentCell);
                            }
                            break;
                        
                        case 2: // left
                            for (int j = -1; j <= 1; j++) // columns
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x +j, _currentCell.y -1);
                                cursorPositions.Add(offsetCurrentCell);
                            }
                            break;

                        case 3: // right
                            for (int j = -1; j <= 1; j++) // columns
                            {
                                offsetCurrentCell = new Vector2Int(_currentCell.x +j, _currentCell.y +1);
                                cursorPositions.Add(offsetCurrentCell);
                            }
                            break;
                    }

                    if (cursorPositions.Count > 0)
                    {
                        Debug.Log(cursorPositions);
                        showCursor.Invoke(cursorPositions);
                    }
                    break;
                
                case 3: // seeds =======================================================================================
                    break;
            }
        }
        else 
        {
            showCursor.Invoke(new List<Vector2Int>());
        }
    }

    private void DisplayCursor(InputAction.CallbackContext context)
    {
        _displayCursor = true;
        _direction = PlayerAnimation.instance.CheckDirection();
        StartCoroutine(WhileDisplayCursor());
    }

    private void HideCursor(InputAction.CallbackContext context)
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

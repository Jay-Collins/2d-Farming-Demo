using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFarming : MonoSingleton<PlayerFarming>
{
    public static Action<List<Vector2Int>> showCursor;

    private int[,] _1x3Pattern = new int[1, 3];
    private int[,] _3x3Pattern = new int[3, 3];
    
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
            return _direction switch
            {
                0 => // up
                    new Vector2Int(cellY + 1, cellX),
                1 => // down
                    new Vector2Int(cellY - 1, cellX),
                2 => // left
                    new Vector2Int(cellY, cellX - 1),
                3 => // right
                    new Vector2Int(cellY , cellX + 1),
                _ => // default
                    new Vector2Int(-1, -1)
            };
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
    
    private void CalculateCursor()
    {
        Debug.Log("Calculating Cursor!");
        
        var numberOfRows = FieldManager.instance.field.GetLength(0);
        var numberOfColumns = FieldManager.instance.field.GetLength(1);
        
        // checks if the current cell (which is the cell in front of the player)
        // is within the bounds of the array
        if (_currentCell.y >= 0 && _currentCell.y < numberOfColumns && _currentCell.x >= 0 && _currentCell.x < numberOfRows)
        {
            // grab cell game objects FieldTile component
            // if fieldTile is not null and the cursor isn't already on
            // turn on cursor for cell player is on

            FieldTile fieldTile;
            switch (PlayerInventory.instance.GetTool())
            {
                case 0:
                    fieldTile = FieldManager.instance.field[_currentCell.x, _currentCell.y]?.GetComponent<FieldTile>();

                    if (fieldTile != null && !fieldTile.CursorOn())
                    {
                        showCursor.Invoke(new List<Vector2Int>{_currentCell});
                    }
                    break;
                case 1:
                    var cursorPositions = new List<Vector2Int>();
                    
                    // the rows and columns properly align 3x3 grid based _currentCell position
                    for (int i = -2; i <= 0; i++) // rows
                    {
                        for (int j = -1; j <= 1; j++) // columns
                        {
                            var newCurrentCell = new Vector2Int(_currentCell.x + i, _currentCell.y + j);
                            fieldTile = FieldManager.instance.field[newCurrentCell.x, newCurrentCell.y]?.GetComponent<FieldTile>();

                            if (fieldTile != null && !fieldTile.CursorOn())
                            {
                                cursorPositions.Add(newCurrentCell);
                            }
                        }
                    }
                    
                    if (cursorPositions.Count > 0)
                        showCursor.Invoke(cursorPositions);
                    break;
                case 2:
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

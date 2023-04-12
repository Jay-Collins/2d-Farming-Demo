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
        var playerPos = new Vector3(transform.position.x + (FieldManager._tileSize / 2), transform.position.y, transform.position.z);
        var fieldPos = FieldManager.instance.transform.position;
        var relativePos = playerPos - fieldPos;

        var cellX = Mathf.FloorToInt(relativePos.x / FieldManager._tileSize);
        var cellY = Mathf.FloorToInt(relativePos.y / FieldManager._tileSize);

        var isOnCell = cellX >= 0 && cellX < FieldManager.instance.field.GetLength(1) && cellY >= 0 && cellY < FieldManager.instance.field.GetLength(0);

        if (isOnCell)
        {
            return new Vector2Int(cellY, cellX);
        }
        else
        {
            return new Vector2Int(-1, -1);
        }
    }
    
    private void CheckCellDisplayCursor()
    {
        // run DetermineCell method to find out what cell player is standing on
        var newCell = DetermineCell();
        
        if (_currentCell != newCell)
        {
            _currentCell = newCell;
        
            // check direction 
            switch (_direction)
            {
                case 0: // up
                {
                    CalculateCursor();
                    break;
                }
                case 1: // down
                {
                    CalculateCursor();
                    break;
                }

                case 2: // left
                {
                    CalculateCursor();
                    break;
                }
                case 3: // right
                {
                    CalculateCursor();
                    break;
                }
            }
        }
    }

    private void CalculateCursor()
    {
        if (_currentCell.y != -1)
        {
            // grab cell game objects FieldTile component
            // turn on cursor for cell player is on
            var fieldTile = FieldManager.instance.field[_currentCell.x, _currentCell.y]?.GetComponent<FieldTile>();

            if (fieldTile != null && !fieldTile.CursorOn())
            {
                showCursor.Invoke(new List<Vector2Int>{_currentCell});
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

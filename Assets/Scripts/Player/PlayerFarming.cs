using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFarming : MonoSingleton<PlayerFarming>
{
    public static Action hideCursor;
    
    private bool _displayCursor;
    private Vector2Int _currentCell;

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

    public Vector2Int DetermineCellBelow()
    {
        var cursorPos = new Vector3(transform.position.x + (FieldManager._tileSize / 2), transform.position.y - FieldManager._tileSize, transform.position.z);
        var fieldPos = FieldManager.instance.transform.position;
        var relativePos = cursorPos - fieldPos;

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
        
        if (DetermineCell() != _currentCell)
        {
            hideCursor.Invoke();
        }
        
        // run DetermineCell method to find out what cell player is standing on
        _currentCell = DetermineCell();
        
        if (_currentCell.y != -1)
        {
            // grab cell game objects FieldTile component
            // turn on cursor for cell player is on
            var fieldTile = FieldManager.instance.field[_currentCell.x, _currentCell.y]?.GetComponent<FieldTile>();
            while (fieldTile)
            {
                fieldTile?.ShowCursor();
            }
        }
        else return;

        // check direction 
        switch (PlayerAnimation.instance.CheckDirection())
        {
            case 0: // up
            {
                
                break;
            }
            case 1: // down
            {
                
                break;
            }

            case 2: // left
            {
                

                break;
            }
            case 3: // right
            {
                

                break;
            }
        }
    }

    private void DisplayCursor(InputAction.CallbackContext context)
    {
        _displayCursor = true;
        StartCoroutine(WhileDisplayCursor());
    }

    private void HideCursor(InputAction.CallbackContext context)
    {
        _displayCursor = false;
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

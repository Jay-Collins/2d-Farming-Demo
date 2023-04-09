using System;
using UnityEngine;

public class PlayerFarming : MonoSingleton<PlayerFarming>
{
    public Vector2Int DetermineCell()
    {
        var playerPos = transform.position;
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
}

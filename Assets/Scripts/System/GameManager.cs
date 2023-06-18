using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public static Action newDay;
    public static Action enablePlayerMovement;
    public static Action disablePlayerMovement;
    
    public GameObject _holdableItemPrefab;
    
    private void OnEnable()
    {
        InputManager.advanceDay += NewDay;
    }

    private void NewDay()
    {
        newDay?.Invoke();
    }
}

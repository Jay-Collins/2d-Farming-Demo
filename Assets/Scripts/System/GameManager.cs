using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public static Action newDay;
    public static Action enablePlayerMovement;
    public static Action disablePlayerMovement;
    
    [SerializeField] private GameObject _holdableItemPrefab;
    
    private void OnEnable()
    {
        InputManager.advanceDay += NewDay;
    }

    private void NewDay()
    {
        newDay?.Invoke();
    }

    public void EnablePlayerMovement() => enablePlayerMovement?.Invoke();
    public void DisablePlayerMovement() => disablePlayerMovement?.Invoke();
}

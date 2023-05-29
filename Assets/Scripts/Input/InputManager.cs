using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Action<Vector2> movement;
    public static Action<InputAction.CallbackContext> interactStarted;
    public static Action<InputAction.CallbackContext> interactCanceled;
    public static Action<InputAction.CallbackContext> cancelStarted;
    public static Action<InputAction.CallbackContext> inventoryStarted;
    public static Action<InputAction.CallbackContext> walkStarted;
    public static Action<InputAction.CallbackContext> walkCanceled;
    public static Action<InputAction.CallbackContext> hotkey1Started;
    public static Action<InputAction.CallbackContext> hotkey2Started;
    public static Action<InputAction.CallbackContext> hotkey3Started;

    private PlayerInputActions _playerInput;

    private void OnEnable() => InitializeInputs();

    private void InitializeInputs()
    {
        // initialize action map and enable general inputs
        _playerInput = new PlayerInputActions();
        _playerInput.PlayerGeneralInputs.Enable();
        
        // initialize general inputs
        _playerInput.PlayerGeneralInputs.Interact.started += InteractStarted;
        _playerInput.PlayerGeneralInputs.Cancel.started += CancelStarted;
        _playerInput.PlayerGeneralInputs.Interact.started += InventoryStarted;
        _playerInput.PlayerGeneralInputs.Walk.started += WalkStarted;
        _playerInput.PlayerGeneralInputs.Walk.canceled += WalkCanceled;
        // initialize general inputs hotkeys
        _playerInput.PlayerGeneralInputs.Hotkey1.started += Hotkey1Started;
        _playerInput.PlayerGeneralInputs.Hotkey2.started += Hotkey2Started;
        _playerInput.PlayerGeneralInputs.Hotkey3.started += Hotkey3Started;
    }

    private void Update()
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            PlayerMovement();
    }
    
    //--- General Actions ---
    private void PlayerMovement()
    {
        var move = _playerInput.PlayerGeneralInputs.Movement.ReadValue<Vector2>();
        movement(move);
    }

    private void InteractStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled) 
            interactStarted(context);
    }

    private void InteractCanceled(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
        {
            interactCanceled(context);  
        }
    }

    private void CancelStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            cancelStarted(context);
    }

    private void InventoryStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            inventoryStarted(context);
    }

    private void WalkStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            walkStarted(context);
    }

    private void WalkCanceled(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            walkCanceled(context);
    }

    //  General Actions Hotkeys
    private void Hotkey1Started(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            hotkey1Started(context);
    }

    private void Hotkey2Started(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            hotkey2Started(context);
    }

    private void Hotkey3Started(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            hotkey3Started(context);
    }
}

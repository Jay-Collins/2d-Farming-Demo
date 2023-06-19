using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{
    public static Action<Vector2> movement;
    public static Action interactStarted;
    public static Action interactCanceled;
    public static Action cancelStarted;
    public static Action inventoryStarted;
    public static Action walkStarted;
    public static Action walkCanceled;
    public static Action cycleToolsUp;
    public static Action cycleToolsDown;
    public static Action cycleItemsUp;
    public static Action cycleItemsDown;
    public static Action useTool;
    public static Action advanceDay;
    public static Action viewControls;

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
        _playerInput.PlayerGeneralInputs.Invetory.started += InventoryStarted;
        _playerInput.PlayerGeneralInputs.Walk.started += WalkStarted;
        _playerInput.PlayerGeneralInputs.Walk.canceled += WalkCanceled;
        _playerInput.PlayerGeneralInputs.UseTool.started += UseToolStarted;
        
        _playerInput.PlayerGeneralInputs.CycleToolsUp.started += CycleToolsUpStarted;
        _playerInput.PlayerGeneralInputs.CycleToolsDown.started += CycleToolsDownStarted;
        _playerInput.PlayerGeneralInputs.CycleItemsUp.started += CycleItemsUpStarted;
        _playerInput.PlayerGeneralInputs.CycleItemsDown.started += CycleItemsDownStarted;

        _playerInput.PlayerGeneralInputs.AdvanceDay.started += AdvanceDayStarted;
        _playerInput.PlayerGeneralInputs.Controls.started += ViewControls;
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
            interactStarted();
    }

    private void InteractCanceled(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
        {
            interactCanceled();  
        }
    }

    private void CancelStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            cancelStarted();
    }

    private void InventoryStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            inventoryStarted();
    }

    private void WalkStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            walkStarted();
    }

    private void WalkCanceled(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            walkCanceled();
    }

    private void CycleToolsUpStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            cycleToolsUp();
    }
    
    private void CycleToolsDownStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            cycleToolsDown();
    }
    
    private void CycleItemsUpStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            cycleItemsUp();
    }
    
    private void CycleItemsDownStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            cycleItemsDown();
    }

    private void UseToolStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            useTool();
    }

    private void AdvanceDayStarted(InputAction.CallbackContext context)
    {
        if (_playerInput.PlayerGeneralInputs.enabled)
            advanceDay?.Invoke();
    }

    // enable and disable inputs
    public void EnableGeneralInputs()
    {
        _playerInput.PlayerGeneralInputs.Enable();
    }

    public void DisableGeneralInputs()
    {
        _playerInput.PlayerGeneralInputs.Disable();
    }
    
    // view controls
    public void ViewControls(InputAction.CallbackContext context)
    {
        viewControls?.Invoke();
    }
}

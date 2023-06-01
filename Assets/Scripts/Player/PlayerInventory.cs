using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoSingleton<PlayerInventory>
{
    private enum EquippedTool {None = 0, WateringCan = 1, Hoe = 2, Seeds = 3}

    private EquippedTool _equippedTool;

    private void OnEnable()
    {
        InputManager.hotkey1Started += QuickSlot1;
        InputManager.hotkey2Started += QuickSlot2;
        InputManager.hotkey3Started += QuickSlot3;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void QuickSlot1(InputAction.CallbackContext context)
    {
        _equippedTool = EquippedTool.None;
        Debug.Log("Equipped Nothing!");
    }

    private void QuickSlot2(InputAction.CallbackContext context)
    {
        _equippedTool = EquippedTool.WateringCan;
        Debug.Log("Equipped Watering Can!");
    }

    private void QuickSlot3(InputAction.CallbackContext context)
    {
        _equippedTool = EquippedTool.Hoe;
        Debug.Log("Equipped Hoe!");
    }

    public int GetTool()
    {
        return (int)_equippedTool;
    }
}

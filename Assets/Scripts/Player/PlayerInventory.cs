using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoSingleton<PlayerInventory>
{
    [SerializeField] private List<Item> _items;
    [SerializeField] private List<Tool> _tools;
    
    private GameObject _itemInHands; // item currently being held
    private Tool _equippedTool; // currently selected tool in Tools list inventory
    private Item _primaryItem; // currently selected item in Items list inventory 
    private int _equippedToolIndex;
    private int _primaryItemIndex;
    private bool _handsFull;
    
    private void OnEnable()
    {
        InputManager.cycleToolsUp += CycleToolsUp;
        InputManager.cycleToolsDown += CycleToolsDown;
        InputManager.cycleItemsUp += CycleItemsUp;
        InputManager.cycleItemsDown += CycleItemsDown;
        InputManager.inventoryStarted += PutItemAway;
    }

    private void Start()
    {
        _equippedTool = _tools[0];
        UIManager.instance.UpdateToolRotation(_equippedTool);
        _primaryItem = _items[0];
        UIManager.instance.UpdateItemRotation(_primaryItem);
    }
    
// TOOLS ===============================================================================================================
    private void CycleToolsUp() 
    {
        // increase index and wrap around if needed
        var nextIndex = (_equippedToolIndex + 1) % _tools.Count;

        // skip tools that are the same as the currently equipped tool
        var originalIndex = nextIndex;
        while (nextIndex != _equippedToolIndex && _tools[nextIndex] == _equippedTool)
        {
            nextIndex = (nextIndex + 1) % _tools.Count;
            
            if (nextIndex == originalIndex)
            {
                break;
            }
        }

        // update the equipped tool index and tool, refresh UI
        _equippedToolIndex = nextIndex;
        _equippedTool = _tools[_equippedToolIndex];
        
        UIManager.instance.UpdateToolRotation(_equippedTool);
        PlayerFarming.instance.CalculateCursor();
    }
    
    private void CycleToolsDown()
    {
        // increase index and wrap around if needed
        var nextIndex = (_equippedToolIndex - 1 + _tools.Count) % _tools.Count;

        // skip tools that are the same as the currently equipped tool
        var originalIndex = nextIndex;
        while (nextIndex != _equippedToolIndex && _tools[nextIndex] == _equippedTool)
        {
            nextIndex = (nextIndex - 1 + _tools.Count) % _tools.Count;
            
            if (nextIndex == originalIndex)
            {
                break;
            }
        }

        // update the equipped tool index and tool, refresh UI
        _equippedToolIndex = nextIndex;
        _equippedTool = _tools[_equippedToolIndex];
        
        UIManager.instance.UpdateToolRotation(_equippedTool);
        PlayerFarming.instance.CalculateCursor();
    }

    public void RemoveCurrentTool(int toolID)
    {
        foreach (var tool in _tools)
        {
            if (tool.toolID == toolID)
            { 
                _tools.Remove(tool); 
                break;
            }
        }
            
        if (!_tools.Any(tool => tool.toolID == toolID))
        {
            CycleToolsDown();
        }
            
        UIManager.instance.UpdateToolRotation(_equippedTool);
        PlayerFarming.instance.CalculateCursor();
    }
    
// ITEMS ===============================================================================================================
    private void CycleItemsUp()
    {
        // increase index and wrap around if needed
        var nextIndex = (_primaryItemIndex + 1) % _items.Count;

        // skip tools that are the same as the currently equipped tool
        var originalIndex = nextIndex;
        while (nextIndex != _primaryItemIndex && _items[nextIndex] == _primaryItem)
        {
            nextIndex = (nextIndex + 1) % _items.Count;
            
            if (nextIndex == originalIndex)
            {
                break;
            }
        }

        // update the equipped tool index and tool, refresh UI
        _primaryItemIndex = nextIndex;
        _primaryItem = _items[_primaryItemIndex];
        
        UIManager.instance.UpdateItemRotation(_primaryItem);
    }
    
    private void CycleItemsDown()
    {
        // increase index and wrap around if needed
        var nextIndex = (_primaryItemIndex - 1 + _items.Count) % _items.Count;

        // skip tools that are the same as the currently equipped tool
        var originalIndex = nextIndex;
        while (nextIndex != _primaryItemIndex && _items[nextIndex] == _primaryItem)
        {
            nextIndex = (nextIndex - 1 + _items.Count) % _items.Count;
            
            if (nextIndex == originalIndex)
            {
                break;
            }
        }

        // update the equipped tool index and tool, refresh UI
        _primaryItemIndex = nextIndex;
        _primaryItem = _items[_primaryItemIndex];

        UIManager.instance.UpdateItemRotation(_primaryItem);
    }
    
    public void PutItemInHands(GameObject item)
    {
        _handsFull = true;
        _itemInHands = item;
    }

    public void PutItemAway(InputAction.CallbackContext context)
    {
        if (_handsFull)
        {
            var item = _itemInHands.GetComponent<HoldableItem>().itemData;
            _items.Add(item);
            Destroy(_itemInHands);
            _handsFull = false;
            UIManager.instance.UpdateItemRotation(item);
        }
    }
    
// Return Functions ====================================================================================================
    public List<Tool> GetTools() => _tools;
    public List<Item> GetItems() => _items;
    public Tool GetEquippedTool() => _equippedTool;
    public Item GetItemInHands() => _itemInHands.gameObject.GetComponent<Item>();
    public bool CheckHands() => _handsFull;
}

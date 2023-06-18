using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoSingleton<PlayerInventory>
{
    [SerializeField] private SpriteRenderer _heldObject;
    [SerializeField] private List<Item> _items;
    [SerializeField] private List<Tool> _tools;

    private Item _itemInHands; // item currently being held
    private GameObject _objectInHands; // the actual game object representing the item in hands
    private Tool _equippedTool; // currently selected tool in Tools list inventory
    private Item _primaryItem; // currently selected item in Items list inventory 
    private int _equippedToolIndex;
    private int _primaryItemIndex;
    private int _money;
    private bool _handsFull;
    
    private void OnEnable()
    {
        InputManager.cycleToolsUp += CycleToolsUp;
        InputManager.cycleToolsDown += CycleToolsDown;
        InputManager.cycleItemsUp += CycleItemsUp;
        InputManager.cycleItemsDown += CycleItemsDown;
        InputManager.inventoryStarted += TakeOutItem;
    }

    private void Start()
    {
        _equippedTool = _tools[0];
        UIManager.instance.UpdateToolRotation(_equippedTool);
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
        if (_items.Count < 2)
        {
            UIManager.instance.UpdateItemRotation(_primaryItem); // UI still needs updated
            return; // only cycle if more than 2 items in the items list inventory
        }
        
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
        if (_items.Count < 2)
        {
            UIManager.instance.UpdateItemRotation(_primaryItem); // UI still needs updated
            return; // only cycle if more than 1 item in the items list inventory
        }
        

        // increase index and wrap around if needed
        var nextIndex = (_primaryItemIndex - 1 + _items.Count) % _items.Count;

        // skip items that are the same as the currently equipped items
        var originalIndex = nextIndex;
        while (nextIndex != _primaryItemIndex && _items[nextIndex] == _primaryItem)
        {
            nextIndex = (nextIndex - 1 + _items.Count) % _items.Count;
            
            if (nextIndex == originalIndex)
            {
                break;
            }
        }

        // update the equipped item index and tool, refresh UI
        _primaryItemIndex = nextIndex;
        _primaryItem = _items[_primaryItemIndex];

        UIManager.instance.UpdateItemRotation(_primaryItem);
    }
    
    public void PutItemInHands(Item item, GameObject heldObject)
    {
        InputManager.inventoryStarted -= TakeOutItem;
        InputManager.inventoryStarted += PutItemAway;
        _handsFull = true;
        _itemInHands = item;
        _objectInHands = heldObject;
    }

    public void TakeOutItem()
    {
        if (_items.Count <= 0) return; // if items inventory is empty just return

        _heldObject.sprite = _primaryItem.itemIcon;
        _itemInHands = _primaryItem;
        
        if (!_items.Contains(_itemInHands))
            CycleItemsDown();
        
        _items.Remove(_primaryItem);

        InputManager.inventoryStarted -= TakeOutItem;
        InputManager.inventoryStarted += PutItemAway;
    }
    
    public void PutItemAway()
    {
        var item = _itemInHands;
        
        // this sets the picked up item to be the primary selected item if inventory was empty
        
        if (_items.Count <= 0) 
        {
            _primaryItem = item;
            _items.Add(item);
        }
        else
        {
            // it also inserts the item into the inventory list next to the others like it
            int insertIndex = _items.FindIndex(i => i.itemID == item.itemID) + 1;
            _items.Insert(insertIndex, item);
        }

        _heldObject.sprite = null; // for when the player picks up from their bag
        if (_objectInHands != null)
            Destroy(_objectInHands); // for when the player picks up an item off the ground
        
        _handsFull = false;
        UIManager.instance.UpdateItemRotation(item);
        
        InputManager.inventoryStarted += TakeOutItem;
        InputManager.inventoryStarted -= PutItemAway;
    }
    
// MONEY ===============================================================================================================    
    private void AddMoney(int moneyAdded)
    {
        _money += moneyAdded;
        // tell UI to update money ui
    }

    private void RemoveMoney(int moneyRemoved)
    {
        _money -= moneyRemoved;
        // tell UI to update money ui
    }

// Return Functions ====================================================================================================
    public List<Tool> GetTools() => _tools;
    public List<Item> GetItems() => _items;
    public Tool GetEquippedTool() => _equippedTool;
    public Item GetItemInHands() => _itemInHands;
    public bool CheckHands() => _handsFull;
    public int CheckMoney() => _money;
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoSingleton<PlayerInventory>
{
    [SerializeField] private SpriteRenderer _storedHeldObject; // object used to display items above the players head
    [SerializeField] private List<Item> _items;
    [SerializeField] private List<Tool> _tools;

    private Item _itemInHands; // item currently being held
    private GameObject _objectPickedUp; // the actual game object representing the item the player picked up
    private Tool _equippedTool; // currently selected tool in Tools list inventory
    private Item _primaryItem; // currently selected item in Items list inventory 
    private int _equippedToolIndex;
    private int _primaryItemIndex;
    private int _money = 900;
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
        AudioManager.instance.PlayMenuWhistleSFX();
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
        AudioManager.instance.PlayMenuWhistleSFX();
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

    public void AddTool(Tool tool)
    {
        _tools.Add(tool);
        _equippedTool = tool;
        UIManager.instance.UpdateToolRotation(_equippedTool);
    }
    
// ITEMS ===============================================================================================================
    private void CycleItemsUp()
    {
        if (_items.Count < 2)
        {
            UIManager.instance.UpdateItemRotation(_primaryItem); // UI still needs updated
            return; // only cycle if more than 2 items in the items list inventory
        }
        
        if (_items.Count > 2)
        {
            AudioManager.instance.PlayMenuWhistleSFX();
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
        
        if (_items.Count > 2)
        {
            AudioManager.instance.PlayMenuWhistleSFX();
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

    public void PickedUpItem(Item item, GameObject heldObject)
    {
        InputManager.inventoryStarted -= TakeOutItem;
        InputManager.inventoryStarted += PutItemAway;
        
        _handsFull = true;
        PlayerAnimation.instance.SetHandsAnimation(_handsFull);
        AudioManager.instance.PlayPickupSFX();
        
        _objectPickedUp = heldObject;
        _itemInHands = item;
    }

    public void PutItemAway()
    {
        _primaryItem = _itemInHands;
        
        if (_items.Count <= 0)
        {
            // if the inventory is empty
            _items.Add(_itemInHands);
        }
        else
        {
            // if the inventory already has the same item
            int insertIndex = _items.FindIndex(i => i.itemID == _itemInHands.itemID) + 1;
            
            if (insertIndex == -1)
                _items.Add(_itemInHands);
            else
                _items.Insert(insertIndex , _itemInHands);
        }
        
        _itemInHands = null;
        UIManager.instance.UpdateItemRotation(_primaryItem);
        
        // set the sprite of the stored held object to null in case the item was taken from the inventory
        if (_storedHeldObject.sprite != null)
            _storedHeldObject.sprite = null;

        // set of object picked up to be null in case the player picked up and item off the ground
        if (_objectPickedUp != null)
            Destroy(_objectPickedUp);

        _handsFull = false;
        PlayerAnimation.instance.SetHandsAnimation(_handsFull);
        AudioManager.instance.PlayPickupSFX();
        
        UIManager.instance.UpdateItemRotation(_primaryItem);
        InputManager.inventoryStarted += TakeOutItem;
        InputManager.inventoryStarted -= PutItemAway;
    }

    private void TakeOutItem()
    {
        if (_items.Count <= 0) return;
        
        _handsFull = true;
        PlayerAnimation.instance.SetHandsAnimation(_handsFull);
        AudioManager.instance.PlayPickupSFX();
        
        _storedHeldObject.sprite = _primaryItem.itemIcon;
        _itemInHands = _primaryItem;
        
        
        if (!_items.Contains(_itemInHands))
            CycleItemsDown();

        _items.Remove(_primaryItem);
        UIManager.instance.UpdateItemRotation(_itemInHands);
        
        InputManager.inventoryStarted -= TakeOutItem;
        InputManager.inventoryStarted += PutItemAway;
    }

    public void SellItem()
    {
        _money += _itemInHands.itemValue;
        
        if (!_items.Contains(_itemInHands))
            CycleItemsDown();
        
        _handsFull = false;
        PlayerAnimation.instance.SetHandsAnimation(_handsFull);
        AudioManager.instance.PlayPickupSFX();
        
        _itemInHands = null;
        
        if (_storedHeldObject.sprite != null)
            _storedHeldObject.sprite = null;
        if (_objectPickedUp != null)
            Destroy(_objectPickedUp);
        if (_items.Count <= 0)
            _primaryItem = null;
        
        UIManager.instance.UpdateMoney();
        InputManager.inventoryStarted -= PutItemAway;
        InputManager.inventoryStarted += TakeOutItem;
    }

// MONEY ===============================================================================================================    
    private void AddMoney(int moneyAdded)
    {
        _money += moneyAdded;
        UIManager.instance.UpdateMoney();
    }

    public void RemoveMoney(int moneyRemoved)
    {
        _money -= moneyRemoved;
        UIManager.instance.UpdateMoney();
    }

// Return Functions ====================================================================================================
    public List<Tool> GetTools() => _tools;
    public List<Item> GetItems() => _items;
    public Tool GetEquippedTool() => _equippedTool;
    public Item GetItemInHands() => _itemInHands;
    public bool CheckHands() => _handsFull;
    public int CheckMoney() => _money;
}

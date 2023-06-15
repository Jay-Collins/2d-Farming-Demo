using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("References")]
    [SerializeField] private Image _toolSlot1Icon;
    [SerializeField] private Image _toolSlot2Icon;
    [SerializeField] private Image _toolSlot3Icon;
    
    [SerializeField] private Image _itemSlot1Icon;
    [SerializeField] private Image _itemSlot2Icon;
    [SerializeField] private Image _itemSlot3Icon;

    [SerializeField] private TMP_Text _toolCountText;
    [SerializeField] private TMP_Text _itemCountText;

    public void UpdateToolRotation(Tool tool)
    {
        // Find all tools and their counts
        var tools = PlayerInventory.instance.GetTools();
        var currentIndex = -1;
        var toolCount = 0;

        for (int i = 0; i < tools.Count; i++)
        {
            if (tool == tools[i])
            {
                toolCount++;
                if (currentIndex == -1)
                    currentIndex = i;
            }
        }

        // set the icons based on the current tools index and count
        if (toolCount > 0)
        {
            // find the index of the previous and next tools
            int previousIndex = currentIndex - 1;
            int nextIndex = currentIndex + 1;

            // handle wrap-around for previous and next indices
            if (previousIndex < 0)
                previousIndex = tools.Count - 1;
            if (nextIndex >= tools.Count)
                nextIndex = 0;

            // check if the next tool is the same as the current tool
            if (tools[nextIndex] == tool)
            {
                // if they are the same, find the index of the next different tool
                for (int i = (nextIndex + 1) % tools.Count; i != nextIndex; i = (i + 1) % tools.Count)
                {
                    if (tools[i] != tool)
                    {
                        nextIndex = i;
                        break;
                    }
                }
            }

            if (toolCount > 1)
            {
                _toolCountText.text = toolCount.ToString();
            }
            else
                _toolCountText.text = "";

            _toolSlot1Icon.sprite = tools[previousIndex].toolIcon; // tool before current tool
            _toolSlot2Icon.sprite = tools[currentIndex].toolIcon;  // the current tool
            _toolSlot3Icon.sprite = tools[nextIndex].toolIcon;     // tool after current tool
        }
    }
    
    public void UpdateItemRotation(Item item)
    {
        // Find all items and their counts
        var items = PlayerInventory.instance.GetItems();
        var currentIndex = -1;
        var itemCount = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (item == items[i])
            {
                itemCount++;
                if (currentIndex == -1)
                    currentIndex = i;
            }
        }

        // set the icons based on the current items index and count
        if (itemCount > 0)
        {
            // find the index of the previous and next items
            int previousIndex = currentIndex - 1;
            int nextIndex = currentIndex + 1;

            // handle wrap-around for previous and next indices
            if (previousIndex < 0)
                previousIndex = items.Count - 1;
            if (nextIndex >= items.Count)
                nextIndex = 0;

            // check if the next item is the same as the current item
            if (items[nextIndex] == item)
            {
                // if they are the same, find the index of the next different item
                for (int i = (nextIndex + 1) % items.Count; i != nextIndex; i = (i + 1) % items.Count)
                {
                    if (items[i] != item)
                    {
                        nextIndex = i;
                        break;
                    }
                }
            }
            
            if (itemCount > 1)
            {
                _itemCountText.text = itemCount.ToString();
            }
            else
                _toolCountText.text = "";

            _itemSlot1Icon.sprite = items[previousIndex].itemIcon; // tool before current tool
            _itemSlot2Icon.sprite = items[currentIndex].itemIcon;  // the current tool
            _itemSlot3Icon.sprite = items[nextIndex].itemIcon;     // tool after current tool
        }
    }
}

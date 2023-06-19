using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("References")] 
    [SerializeField] private GameObject _shopMenu;
    
    [SerializeField] private Image _toolSlot1Icon;
    [SerializeField] private Image _toolSlot2Icon;
    [SerializeField] private Image _toolSlot3Icon;
    
    [SerializeField] private Image _itemSlot1Icon;
    [SerializeField] private Image _itemSlot2Icon;
    [SerializeField] private Image _itemSlot3Icon;

    [SerializeField] private TMP_Text _toolCountText;
    [SerializeField] private TMP_Text _itemCountText;
    [SerializeField] private TMP_Text _moneyText;
    
    [SerializeField] private GameObject _controlsText;

    public Image _nightFade;

    private void OnEnable() => InputManager.viewControls += ViewControls;
    
    private void Start() => UpdateMoney();

    public void UpdateToolRotation(Tool tool)
    {
        // Find all tools and their counts
        var tools = PlayerInventory.instance.GetTools();
        var currentIndex = -1;
        var toolCount = 0;
        
        if (tools.Count <= 0)
        {
            // set background opacity
            var transparent = new Color(1, 1, 1, 0);
            _toolSlot1Icon.sprite = null; // item before current item
            _toolSlot1Icon.color = transparent;
            _toolSlot2Icon.sprite = null; // the current tool
            _toolSlot2Icon.color = transparent;
            _toolSlot3Icon.sprite = null; // item after current item
            _toolSlot3Icon.color = transparent;
            return;
        }

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

            // check how many of that tool are in the inventory, then update count ui if more than 1
            var count = PlayerInventory.instance.GetTools().Count(t => tool == t);
            _toolCountText.text = count > 1 ? count.ToString() : null;

            // set background opacity
            var opaque = new Color(1, 1, 1, 1);
            _toolSlot1Icon.sprite = tools[previousIndex].toolIcon; // tool before current tool
            _toolSlot1Icon.color = opaque;
            _toolSlot2Icon.sprite = tools[currentIndex].toolIcon;  // the current tool
            _toolSlot2Icon.color = opaque;
            _toolSlot3Icon.sprite = tools[nextIndex].toolIcon;     // tool after current tool
            _toolSlot3Icon.color = opaque;
        }
    }
    
    public void UpdateItemRotation(Item item)
    {
        // Find all items and their counts
        var items = PlayerInventory.instance.GetItems();
        var currentIndex = -1;
        var itemCount = 0;

        if (items.Count <= 0)
        {
            // set background opacity
            var transparent = new Color(1, 1, 1, 0);
            _itemSlot1Icon.sprite = null; // item before current item
            _itemSlot1Icon.color = transparent;
            _itemSlot2Icon.sprite = null; // the current tool
            _itemSlot2Icon.color = transparent;
            _itemSlot3Icon.sprite = null; // item after current item
            _itemSlot3Icon.color = transparent;
            return;
        }

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

            // handle wrap around for previous and next indices
            if (previousIndex < 0)
                previousIndex = items.Count - 1;
            if (nextIndex >= items.Count)
                nextIndex = 0;

            // check if the next item is the same as the current item
            if (items[nextIndex] == item)
            {
                // if they are the same find the index of the next different item
                for (int i = (nextIndex + 1) % items.Count; i != nextIndex; i = (i + 1) % items.Count)
                {
                    if (items[i] != item)
                    {
                        nextIndex = i;
                        break;
                    }
                }
            }
            
            // check how many of that tool are in the inventory, then update count ui if more than 1
            var count = PlayerInventory.instance.GetItems().Count(i => item == i);
            _itemCountText.text = count > 1 ? count.ToString() : null;

            // set background opacity
            var opaque = new Color(1, 1, 1, 1);
            _itemSlot1Icon.sprite = items[previousIndex].itemIcon; // item before current item
            _itemSlot1Icon.color = opaque;
            _itemSlot2Icon.sprite = items[currentIndex].itemIcon; // the current item
            _itemSlot2Icon.color = opaque;
            _itemSlot3Icon.sprite = items[nextIndex].itemIcon;    // item after current item
            _itemSlot3Icon.color = opaque;
        }
    }

    public void OpenShop()
    {
        _shopMenu.SetActive(true);
        GameManager.disablePlayerMovement?.Invoke();
        InputManager.instance.DisableGeneralInputs();
    }

    public void CloseShop()
    {
        _shopMenu.SetActive(false);
        GameManager.enablePlayerMovement?.Invoke();
        InputManager.instance.EnableGeneralInputs();
    }

    public void ViewControls() => StartCoroutine(ShowControlsText());
    

    private IEnumerator ShowControlsText()
    {
        _controlsText.SetActive(true);
        yield return new WaitForSeconds(10);
        _controlsText.SetActive(false);
    }

    public void UpdateMoney() => _moneyText.text = PlayerInventory.instance.CheckMoney().ToString();
}

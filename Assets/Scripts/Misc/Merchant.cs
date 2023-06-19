using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private List<Tool> _wares;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            InputManager.interactStarted += TalkedToMerchant;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            InputManager.interactStarted -= TalkedToMerchant;
    }

    private void TalkedToMerchant()
    {
        if (PlayerInventory.instance.CheckHands())
        {
            // player is holding an item to sell
            PlayerInventory.instance.SellItem();
        }
        else
        {
            UIManager.instance.OpenShop();
        }
    }

    public void CloseShop()
    {
        UIManager.instance.CloseShop();
    }

    public void BuyTurnipSeeds()
    {
        if (PlayerInventory.instance.CheckMoney() >= 100)
        {
            PlayerInventory.instance.AddTool(_wares[0]);
            PlayerInventory.instance.RemoveMoney(100);
            AudioManager.instance.PlayPickupSFX();
        }
    }

    public void BuyStrawberrySeed()
    {
        if (PlayerInventory.instance.CheckMoney() >= 80)
        {
            PlayerInventory.instance.AddTool(_wares[1]);
            PlayerInventory.instance.RemoveMoney(80);
            AudioManager.instance.PlayPickupSFX();
        }
    }

    public void BuyCabbageSeeds()
    {
        if (PlayerInventory.instance.CheckMoney() >= 150)
        {
            PlayerInventory.instance.AddTool(_wares[2]);
            PlayerInventory.instance.RemoveMoney(150);
            AudioManager.instance.PlayPickupSFX();
        }
    }

    public void BuyCauliflowerSeeds()
    {
        if (PlayerInventory.instance.CheckMoney() >= 150)
        {
            PlayerInventory.instance.AddTool(_wares[3]);
            PlayerInventory.instance.RemoveMoney(150);
            AudioManager.instance.PlayPickupSFX();
        }
    }
}

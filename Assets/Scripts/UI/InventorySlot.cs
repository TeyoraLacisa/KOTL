using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public GameObject emptySlot;
    
    private InventoryItem currentItem;
    
    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        
        if (item != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.color = Color.white;
            itemNameText.text = item.itemName;
            emptySlot.SetActive(false);
        }
        else
        {
            SetEmpty();
        }
    }
    
    public void SetEmpty()
    {
        currentItem = null;
        itemIcon.sprite = null;
        itemIcon.color = Color.clear;
        itemNameText.text = "";
        emptySlot.SetActive(true);
    }
    
    public void OnSlotClicked()
    {
        if (currentItem != null)
        {
            Debug.Log($"Предмет: {currentItem.itemName}\nОписание: {currentItem.description}");
            // Здесь можно добавить дополнительную логику при клике на предмет
        }
    }
}
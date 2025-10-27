using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;
    public Button closeButton;
    public TextMeshProUGUI inventoryTitle;
    
    [Header("Item Description")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemIconImage;
    
    private InventorySystem inventorySystem;
    
    void Start()
    {
        inventorySystem = InventorySystem.Instance;
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
    
    public void OpenInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            UpdateInventoryDisplay();
        }
    }
    
    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
    
    void UpdateInventoryDisplay()
    {
        if (inventoryGrid == null || inventorySlotPrefab == null) return;
        
        // Очищаем существующие слоты
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }
        
        // Создаем слоты
        for (int i = 0; i < inventorySystem.maxInventorySlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();
            
            if (i < inventorySystem.inventory.Count)
            {
                slotScript.SetItem(inventorySystem.inventory[i]);
            }
            else
            {
                slotScript.SetEmpty();
            }
        }
    }
    
    public void ShowItemDescription(InventoryItem item)
    {
        if (descriptionPanel != null && item != null)
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;
            itemIconImage.sprite = item.itemIcon;
            descriptionPanel.SetActive(true);
        }
    }
    
    public void HideItemDescription()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
}
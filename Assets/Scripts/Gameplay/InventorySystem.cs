using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite itemIcon;
    public string description;
    public bool isKeyItem;
    
    public InventoryItem(string name, Sprite icon, string desc, bool keyItem = false)
    {
        itemName = name;
        itemIcon = icon;
        description = desc;
        isKeyItem = keyItem;
    }
}

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxInventorySlots = 12;
    
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;
    public Button inventoryToggleButton;
    
    public List<InventoryItem> inventory = new List<InventoryItem>();
    private bool isInventoryOpen = false;
    
    public static InventorySystem Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad работает только для корневых объектов
            // Если нужно сохранить между сценами, сделайте этот объект корневым
        }
        else
        {
            Destroy(this);
        }
    }
    
    void Start()
    {
        if (inventoryToggleButton != null)
        {
            inventoryToggleButton.onClick.AddListener(ToggleInventory);
        }
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }
    
    public bool AddItem(InventoryItem item)
    {
        if (inventory.Count >= maxInventorySlots)
        {
            Debug.Log("Инвентарь полон!");
            return false;
        }
        
        inventory.Add(item);
        UpdateInventoryUI();
        Debug.Log($"Добавлен предмет: {item.itemName}");
        return true;
    }
    
    public bool RemoveItem(string itemName)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemName == itemName)
            {
                inventory.RemoveAt(i);
                UpdateInventoryUI();
                Debug.Log($"Удален предмет: {itemName}");
                return true;
            }
        }
        return false;
    }
    
    public bool HasItem(string itemName)
    {
        foreach (var item in inventory)
        {
            if (item.itemName == itemName)
                return true;
        }
        return false;
    }
    
    public InventoryItem GetItem(string itemName)
    {
        foreach (var item in inventory)
        {
            if (item.itemName == itemName)
                return item;
        }
        return null;
    }
    
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
        }
        
        if (isInventoryOpen)
        {
            Time.timeScale = 0f; // Пауза игры
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f; // Возобновление игры
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void UpdateInventoryUI()
    {
        if (inventoryGrid == null || inventorySlotPrefab == null) return;
        
        // Очищаем существующие слоты
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }
        
        // Создаем слоты для всех предметов
        for (int i = 0; i < maxInventorySlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();
            
            if (i < inventory.Count)
            {
                slotScript.SetItem(inventory[i]);
            }
            else
            {
                slotScript.SetEmpty();
            }
        }
    }
    
    void Update()
    {
        // Открытие инвентаря по клавише I
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
}
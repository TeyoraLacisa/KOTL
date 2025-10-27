using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleInventory : MonoBehaviour
{
    [Header("HUD Settings")]
    public bool showHUD = true;
    public int maxSlots = 3;
    public float slotSize = 80f;
    public Vector2 hudPosition = new Vector2(0, -150);
    
    [Header("Colors")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f);
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    
    // Система инвентаря
    private List<InventoryItem> inventory = new List<InventoryItem>();
    private int selectedSlot = 0;
    
    // HUD элементы
    private GameObject hudCanvas;
    private GameObject hudPanel;
    private List<Image> slotBackgrounds = new List<Image>();
    private List<Image> slotIcons = new List<Image>();
    
    void Start()
    {
        if (showHUD)
        {
            CreateHUD();
        }
        
        // Добавляем стартовые предметы
        AddStartingItems();
    }
    
    void CreateHUD()
    {
        // Создаем Canvas
        hudCanvas = new GameObject("SimpleInventory_Canvas");
        Canvas canvas = hudCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        
        CanvasScaler scaler = hudCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Создаем панель
        hudPanel = new GameObject("HUD_Panel");
        hudPanel.transform.SetParent(hudCanvas.transform, false);
        
        RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0);
        panelRect.anchorMax = new Vector2(0.5f, 0);
        panelRect.sizeDelta = new Vector2(maxSlots * slotSize + 40, slotSize + 40);
        panelRect.anchoredPosition = hudPosition;
        
        Image panelBg = hudPanel.AddComponent<Image>();
        panelBg.color = backgroundColor;
        
        // Создаем слоты
        for (int i = 0; i < maxSlots; i++)
        {
            CreateSlot(i);
        }
        
        Debug.Log("✅ SimpleInventory HUD создан!");
    }
    
    void CreateSlot(int index)
    {
        GameObject slot = new GameObject($"Slot_{index}");
        slot.transform.SetParent(hudPanel.transform, false);
        
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0, 0.5f);
        slotRect.anchorMax = new Vector2(0, 0.5f);
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);
        slotRect.anchoredPosition = new Vector2(20 + index * (slotSize + 10), 0);
        
        // Фон слота
        Image slotBg = slot.AddComponent<Image>();
        slotBg.color = normalColor;
        slotBackgrounds.Add(slotBg);
        
        // Иконка
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(slot.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.1f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.clear;
        iconImage.preserveAspect = true;
        slotIcons.Add(iconImage);
    }
    
    void AddStartingItems()
    {
        // Создаем стартовые предметы с простыми иконками
        InventoryItem diary = new InventoryItem("Дневник", CreateColoredIcon(new Color(0.6f, 0.4f, 0.2f)), "Старый дневник", true);
        InventoryItem lighter = new InventoryItem("Зажигалка", CreateColoredIcon(Color.red), "Металлическая зажигалка", true);
        InventoryItem key = new InventoryItem("Ключ", CreateColoredIcon(Color.yellow), "Ржавый ключ", true);
        
        // Добавляем в инвентарь
        AddItem(diary);
        AddItem(lighter);
        AddItem(key);
        
        Debug.Log("✅ Стартовые предметы добавлены!");
    }
    
    public void AddItem(InventoryItem item)
    {
        if (inventory.Count < maxSlots)
        {
            inventory.Add(item);
            UpdateHUD();
            Debug.Log($"✅ Добавлен предмет: {item.itemName}");
        }
        else
        {
            Debug.Log("❌ Инвентарь полон!");
        }
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
    
    public InventoryItem GetSelectedItem()
    {
        if (selectedSlot < inventory.Count)
        {
            return inventory[selectedSlot];
        }
        return null;
    }
    
    void UpdateHUD()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (i < inventory.Count)
            {
                slotIcons[i].sprite = inventory[i].itemIcon;
                slotIcons[i].color = inventory[i].itemIcon != null ? Color.white : Color.clear;
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color = Color.clear;
            }
        }
        
        UpdateSelection();
    }
    
    void UpdateSelection()
    {
        for (int i = 0; i < slotBackgrounds.Count; i++)
        {
            slotBackgrounds[i].color = (i == selectedSlot) ? selectedColor : normalColor;
        }
    }
    
    public void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < maxSlots)
        {
            selectedSlot = slotIndex;
            UpdateSelection();
            
            InventoryItem selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                Debug.Log($"Выбран предмет: {selectedItem.itemName}");
            }
        }
    }
    
    void Update()
    {
        // Выбор слотов
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        
        // Тестовые предметы
        if (Input.GetKeyDown(KeyCode.Q)) AddItem(new InventoryItem("Тест1", CreateColoredIcon(Color.cyan), "Тестовый предмет 1", true));
        if (Input.GetKeyDown(KeyCode.W)) AddItem(new InventoryItem("Тест2", CreateColoredIcon(Color.magenta), "Тестовый предмет 2", true));
        if (Input.GetKeyDown(KeyCode.E)) AddItem(new InventoryItem("Тест3", CreateColoredIcon(Color.green), "Тестовый предмет 3", true));
    }
    
    Sprite CreateColoredIcon(Color color)
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
}

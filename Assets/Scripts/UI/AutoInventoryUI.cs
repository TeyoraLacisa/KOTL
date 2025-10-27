using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Автоматически создает и управляет UI инвентаря
/// Просто добавьте этот скрипт на пустой GameObject
/// </summary>
public class AutoInventoryUI : MonoBehaviour
{
    [Header("Settings")]
    public int maxSlots = 12;
    public int columns = 4;
    public bool enableTestKeys = false; // Отключить тестовые клавиши
    
    [Header("HUD Settings")]
    public bool showHUD = true; // Показывать HUD на экране
    public int hudSlots = 3; // Количество слотов в HUD (только 3)
    public float hudSlotSize = 80f; // Размер слотов HUD
    public Vector2 hudPosition = new Vector2(0, -150); // Позиция HUD (снизу по центру)
    public Color selectedSlotColor = new Color(1f, 0.8f, 0f, 1f); // Цвет выбранного слота
    public Color normalSlotColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Цвет обычного слота
    
    [Header("Test Icons (optional)")]
    public Sprite diaryIcon;
    public Sprite lighterIcon;
    public Sprite keyIcon;
    
    private GameObject inventoryPanel;
    private Transform slotsParent;
    private List<SlotUI> slots = new List<SlotUI>();
    public List<InventoryItem> items = new List<InventoryItem>();
    private bool isOpen = false;
    
    // HUD элементы
    private GameObject hudCanvas;
    private GameObject hudPanel;
    private List<Image> hudSlotIcons = new List<Image>();
    private List<Image> hudSlotBackgrounds = new List<Image>();
    private int selectedSlot = 0; // Выбранный слот (0, 1, 2)
    
    public static AutoInventoryUI Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        CreateUI();
        CloseInventory();
        
        if (showHUD)
        {
            CreateHUD();
            InvokeRepeating(nameof(UpdateHUD), 0.1f, 0.1f);
        }
    }
    
    void CreateUI()
    {
        // Находим или создаем Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Auto_InventoryCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Создаем панель инвентаря
        inventoryPanel = new GameObject("Auto_InventoryPanel");
        inventoryPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = inventoryPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(600, 500);
        
        Image panelBg = inventoryPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Заголовок
        GameObject title = new GameObject("Title");
        title.transform.SetParent(inventoryPanel.transform, false);
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 50);
        titleRect.anchoredPosition = new Vector2(0, -25);
        
        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "ИНВЕНТАРЬ";
        titleText.fontSize = 32;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // Родитель для слотов
        GameObject slotsObj = new GameObject("Slots");
        slotsObj.transform.SetParent(inventoryPanel.transform, false);
        RectTransform slotsRect = slotsObj.AddComponent<RectTransform>();
        slotsRect.anchorMin = new Vector2(0, 0);
        slotsRect.anchorMax = new Vector2(1, 1);
        slotsRect.offsetMin = new Vector2(20, 20);
        slotsRect.offsetMax = new Vector2(-20, -70);
        
        GridLayoutGroup grid = slotsObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(110, 110);
        grid.spacing = new Vector2(10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        
        slotsParent = slotsObj.transform;
        
        // Создаем слоты
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(CreateSlot(i));
        }
        
        Debug.Log("✅ Автоматический UI инвентаря создан!");
    }
    
    SlotUI CreateSlot(int index)
    {
        GameObject slotObj = new GameObject($"Slot_{index}");
        slotObj.transform.SetParent(slotsParent, false);
        
        Image slotBg = slotObj.AddComponent<Image>();
        slotBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Иконка
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(slotObj.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.3f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = Color.clear;
        
        // Название
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(slotObj.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0.25f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 12;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        nameText.text = "";
        
        return new SlotUI { icon = iconImage, nameText = nameText };
    }
    
    public void AddItem(InventoryItem item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Инвентарь полон!");
            return;
        }
        
        // Проверяем, нет ли уже такого предмета
        if (HasItem(item.itemName))
        {
            Debug.Log($"Предмет '{item.itemName}' уже есть в инвентаре!");
            return;
        }
        
        items.Add(item);
        UpdateUI();
        Debug.Log($"✅ Добавлен: {item.itemName}");
        
        // Показываем уведомление в HUD
        if (InventoryHUD.Instance != null)
        {
            InventoryHUD.Instance.ShowItemNotification(item.itemName);
        }
        else if (SimpleInventoryHUD.Instance != null)
        {
            SimpleInventoryHUD.Instance.ShowNotification($"Получен: {item.itemName}");
        }
    }
    
    public bool HasItem(string itemName)
    {
        foreach (var item in items)
        {
            if (item.itemName == itemName)
                return true;
        }
        return false;
    }
    
    void UpdateUI()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                slots[i].icon.sprite = items[i].itemIcon;
                slots[i].icon.color = items[i].itemIcon != null ? Color.white : Color.clear;
                slots[i].nameText.text = items[i].itemName;
            }
            else
            {
                slots[i].icon.sprite = null;
                slots[i].icon.color = Color.clear;
                slots[i].nameText.text = "";
            }
        }
    }
    
    public void ToggleInventory()
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }
    
    void OpenInventory()
    {
        isOpen = true;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        UpdateUI();
    }
    
    void CloseInventory()
    {
        isOpen = false;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Открыть/закрыть инвентарь
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
        
        // Выбор предметов в HUD клавишами 1, 2, 3
        if (showHUD)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectSlot(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectSlot(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectSlot(2);
            }
        }
        
        // Тест - добавление предметов (только если включено)
        if (enableTestKeys)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                AddItem(KeyItems.CreateDiary(diaryIcon));
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                AddItem(KeyItems.CreateLighter(lighterIcon));
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                AddItem(KeyItems.CreateKey(keyIcon));
            }
        }
    }
    
    void CreateHUD()
    {
        // Создаем отдельный Canvas для HUD
        hudCanvas = new GameObject("HUD_Canvas");
        Canvas hudCanvasComp = hudCanvas.AddComponent<Canvas>();
        hudCanvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        hudCanvasComp.sortingOrder = 10; // Поверх всего
        
        CanvasScaler scaler = hudCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Создаем панель HUD
        hudPanel = new GameObject("HUD_Panel");
        hudPanel.transform.SetParent(hudCanvas.transform, false);
        
        RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0);
        panelRect.anchorMax = new Vector2(0.5f, 0);
        panelRect.sizeDelta = new Vector2(hudSlots * hudSlotSize + 40, hudSlotSize + 40);
        panelRect.anchoredPosition = hudPosition;
        
        Image panelBg = hudPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // Заголовок HUD
        GameObject hudTitle = new GameObject("Title");
        hudTitle.transform.SetParent(hudPanel.transform, false);
        RectTransform titleRect = hudTitle.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 20);
        titleRect.anchoredPosition = new Vector2(0, -10);
        
        TextMeshProUGUI titleText = hudTitle.AddComponent<TextMeshProUGUI>();
        titleText.text = "ПРЕДМЕТЫ";
        titleText.fontSize = 14;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // Создаем слоты HUD
        for (int i = 0; i < hudSlots; i++)
        {
            CreateHUDSlot(i);
        }
        
        Debug.Log("✅ HUD создан и отображается внизу по центру! Используйте клавиши 1, 2, 3 для выбора предметов.");
        Debug.Log($"HUD позиция: {hudPosition}, размер слотов: {hudSlotSize}, количество слотов: {hudSlots}");
    }
    
    void CreateHUDSlot(int index)
    {
        GameObject slot = new GameObject($"HUDSlot_{index}");
        slot.transform.SetParent(hudPanel.transform, false);
        
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0, 0.5f);
        slotRect.anchorMax = new Vector2(0, 0.5f);
        slotRect.sizeDelta = new Vector2(hudSlotSize, hudSlotSize);
        slotRect.anchoredPosition = new Vector2(20 + index * (hudSlotSize + 10), 0);
        
        // Фон слота
        Image slotBg = slot.AddComponent<Image>();
        slotBg.color = normalSlotColor;
        hudSlotBackgrounds.Add(slotBg);
        
        // Рамка
        GameObject border = new GameObject("Border");
        border.transform.SetParent(slot.transform, false);
        RectTransform borderRect = border.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(2, 2);
        borderRect.offsetMax = new Vector2(-2, -2);
        
        Image borderImage = border.AddComponent<Image>();
        borderImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        // Иконка предмета
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
        
        hudSlotIcons.Add(iconImage);
    }
    
    public void UpdateHUD()
    {
        if (hudSlotIcons.Count == 0) 
        {
            Debug.LogWarning("HUD слоты не созданы!");
            return;
        }
        
        for (int i = 0; i < hudSlotIcons.Count; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                hudSlotIcons[i].sprite = items[i].itemIcon;
                hudSlotIcons[i].color = items[i].itemIcon != null ? Color.white : Color.clear;
                Debug.Log($"HUD слот {i}: предмет {items[i].itemName}, иконка: {items[i].itemIcon != null}");
            }
            else
            {
                hudSlotIcons[i].sprite = null;
                hudSlotIcons[i].color = Color.clear;
            }
        }
        
        // Обновляем подсветку выбранного слота
        UpdateSelectedSlot();
    }
    
    void UpdateSelectedSlot()
    {
        if (hudSlotBackgrounds.Count == 0) return;
        
        for (int i = 0; i < hudSlotBackgrounds.Count; i++)
        {
            if (i == selectedSlot)
            {
                hudSlotBackgrounds[i].color = selectedSlotColor;
            }
            else
            {
                hudSlotBackgrounds[i].color = normalSlotColor;
            }
        }
    }
    
    public InventoryItem GetSelectedItem()
    {
        if (selectedSlot < items.Count && selectedSlot >= 0)
        {
            return items[selectedSlot];
        }
        return null;
    }
    
    public void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < hudSlots)
        {
            selectedSlot = slotIndex;
            UpdateSelectedSlot();
            
            InventoryItem selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                Debug.Log($"Выбран предмет: {selectedItem.itemName}");
            }
        }
    }
    
    private class SlotUI
    {
        public Image icon;
        public TextMeshProUGUI nameText;
    }
}


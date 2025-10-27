using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Простой HUD для быстрого отображения предметов
/// Автоматически создает UI при запуске
/// </summary>
public class SimpleInventoryHUD : MonoBehaviour
{
    [Header("HUD Settings")]
    public int maxSlots = 5;
    public float slotSize = 50f;
    public Vector2 position = new Vector2(-150, 150);
    
    [Header("Visual Settings")]
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public Color borderColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    
    private GameObject hudCanvas;
    private GameObject hudPanel;
    private List<Image> slotIcons = new List<Image>();
    private AutoInventoryUI inventoryUI;
    
    public static SimpleInventoryHUD Instance { get; private set; }
    
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
        inventoryUI = FindObjectOfType<AutoInventoryUI>();
        
        if (inventoryUI == null)
        {
            Debug.LogError("AutoInventoryUI не найден!");
            return;
        }
        
        CreateSimpleHUD();
        InvokeRepeating(nameof(UpdateHUD), 0.1f, 0.1f);
    }
    
    void CreateSimpleHUD()
    {
        // Создаем Canvas
        hudCanvas = new GameObject("SimpleHUD_Canvas");
        Canvas canvas = hudCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 3;
        
        CanvasScaler scaler = hudCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Создаем панель HUD
        hudPanel = new GameObject("HUD_Panel");
        hudPanel.transform.SetParent(hudCanvas.transform, false);
        
        RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1, 1);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.sizeDelta = new Vector2(maxSlots * slotSize + 20, slotSize + 20);
        panelRect.anchoredPosition = position;
        
        Image panelBg = hudPanel.AddComponent<Image>();
        panelBg.color = backgroundColor;
        
        // Создаем слоты
        for (int i = 0; i < maxSlots; i++)
        {
            CreateSlot(i);
        }
        
        Debug.Log("✅ Простой HUD создан!");
    }
    
    void CreateSlot(int index)
    {
        GameObject slot = new GameObject($"Slot_{index}");
        slot.transform.SetParent(hudPanel.transform, false);
        
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0, 0.5f);
        slotRect.anchorMax = new Vector2(0, 0.5f);
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);
        slotRect.anchoredPosition = new Vector2(10 + index * slotSize, 0);
        
        // Фон слота
        Image slotBg = slot.AddComponent<Image>();
        slotBg.color = backgroundColor;
        
        // Рамка
        GameObject border = new GameObject("Border");
        border.transform.SetParent(slot.transform, false);
        RectTransform borderRect = border.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        Image borderImage = border.AddComponent<Image>();
        borderImage.color = borderColor;
        
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
    
    void UpdateHUD()
    {
        if (inventoryUI == null || slotIcons.Count == 0) return;
        
        // Получаем предметы через рефлексию
        var itemsField = typeof(AutoInventoryUI).GetField("items", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (itemsField != null)
        {
            var items = itemsField.GetValue(inventoryUI) as List<InventoryItem>;
            
            if (items != null)
            {
                for (int i = 0; i < slotIcons.Count; i++)
                {
                    if (i < items.Count && items[i] != null)
                    {
                        slotIcons[i].sprite = items[i].itemIcon;
                        slotIcons[i].color = items[i].itemIcon != null ? Color.white : Color.clear;
                    }
                    else
                    {
                        slotIcons[i].sprite = null;
                        slotIcons[i].color = Color.clear;
                    }
                }
            }
        }
    }
    
    public void ShowNotification(string message)
    {
        Debug.Log($"🔔 HUD: {message}");
    }
    
    void Update()
    {
        // Переключение видимости HUD
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(!hudPanel.activeInHierarchy);
            }
        }
    }
}
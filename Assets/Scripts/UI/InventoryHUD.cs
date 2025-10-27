using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HUD для отображения предметов на экране во время игры
/// </summary>
public class InventoryHUD : MonoBehaviour
{
    [Header("HUD Settings")]
    public bool showMiniInventory = true;
    public bool showItemNotifications = true;
    public bool showItemCounter = true;
    
    [Header("Mini Inventory")]
    public int maxVisibleSlots = 5; // Максимум слотов на экране
    public float slotSize = 60f;
    public float slotSpacing = 10f;
    public Vector2 hudPosition = new Vector2(-200, 100); // Позиция относительно центра экрана
    
    [Header("Item Notifications")]
    public float notificationDuration = 3f;
    public float notificationFadeTime = 0.5f;
    public Vector2 notificationPosition = new Vector2(0, 200);
    
    [Header("Colors")]
    public Color slotBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color slotBorderColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color notificationTextColor = Color.white;
    public Color notificationBackgroundColor = new Color(0, 0, 0, 0.7f);
    
    private GameObject hudCanvas;
    private GameObject miniInventoryPanel;
    private GameObject notificationPanel;
    private List<GameObject> hudSlots = new List<GameObject>();
    private AutoInventoryUI inventoryUI;
    private Queue<string> notificationQueue = new Queue<string>();
    private bool isShowingNotification = false;
    
    public static InventoryHUD Instance { get; private set; }
    
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
            Debug.LogError("AutoInventoryUI не найден! HUD не может работать без системы инвентаря.");
            return;
        }
        
        CreateHUD();
        StartCoroutine(UpdateHUDCoroutine());
    }
    
    void CreateHUD()
    {
        // Создаем Canvas для HUD
        hudCanvas = new GameObject("InventoryHUD_Canvas");
        Canvas canvas = hudCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5; // Поверх игрового UI, но под меню
        
        CanvasScaler scaler = hudCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        hudCanvas.AddComponent<GraphicRaycaster>();
        
        // Создаем мини-инвентарь
        if (showMiniInventory)
        {
            CreateMiniInventory();
        }
        
        // Создаем панель уведомлений
        if (showItemNotifications)
        {
            CreateNotificationPanel();
        }
    }
    
    void CreateMiniInventory()
    {
        miniInventoryPanel = new GameObject("MiniInventory");
        miniInventoryPanel.transform.SetParent(hudCanvas.transform, false);
        
        RectTransform panelRect = miniInventoryPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1, 1);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.sizeDelta = new Vector2(maxVisibleSlots * (slotSize + slotSpacing), slotSize + 20);
        panelRect.anchoredPosition = hudPosition;
        
        // Фон мини-инвентаря
        Image panelBg = miniInventoryPanel.AddComponent<Image>();
        panelBg.color = slotBackgroundColor;
        
        // Создаем слоты
        for (int i = 0; i < maxVisibleSlots; i++)
        {
            GameObject slot = CreateHUDSlot(i);
            slot.transform.SetParent(miniInventoryPanel.transform, false);
            hudSlots.Add(slot);
        }
        
        // Заголовок
        GameObject title = new GameObject("Title");
        title.transform.SetParent(miniInventoryPanel.transform, false);
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 20);
        titleRect.anchoredPosition = new Vector2(0, 10);
        
        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "ИНВЕНТАРЬ";
        titleText.fontSize = 12;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
    }
    
    GameObject CreateHUDSlot(int index)
    {
        GameObject slot = new GameObject($"HUDSlot_{index}");
        
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0, 0.5f);
        slotRect.anchorMax = new Vector2(0, 0.5f);
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);
        slotRect.anchoredPosition = new Vector2(index * (slotSize + slotSpacing) + slotSize/2, 0);
        
        // Фон слота
        Image slotBg = slot.AddComponent<Image>();
        slotBg.color = slotBackgroundColor;
        
        // Рамка слота
        GameObject border = new GameObject("Border");
        border.transform.SetParent(slot.transform, false);
        RectTransform borderRect = border.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        Image borderImage = border.AddComponent<Image>();
        borderImage.color = slotBorderColor;
        borderImage.type = Image.Type.Sliced;
        
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
        
        // Индикатор количества (если нужно)
        if (showItemCounter)
        {
            GameObject counter = new GameObject("Counter");
            counter.transform.SetParent(slot.transform, false);
            RectTransform counterRect = counter.AddComponent<RectTransform>();
            counterRect.anchorMin = new Vector2(1, 1);
            counterRect.anchorMax = new Vector2(1, 1);
            counterRect.sizeDelta = new Vector2(20, 20);
            counterRect.anchoredPosition = new Vector2(-5, -5);
            
            Image counterBg = counter.AddComponent<Image>();
            counterBg.color = Color.red;
            
            TextMeshProUGUI counterText = counter.AddComponent<TextMeshProUGUI>();
            counterText.fontSize = 10;
            counterText.alignment = TextAlignmentOptions.Center;
            counterText.color = Color.white;
            counterText.text = "";
        }
        
        return slot;
    }
    
    void CreateNotificationPanel()
    {
        notificationPanel = new GameObject("NotificationPanel");
        notificationPanel.transform.SetParent(hudCanvas.transform, false);
        
        RectTransform notifRect = notificationPanel.AddComponent<RectTransform>();
        notifRect.anchorMin = new Vector2(0.5f, 1);
        notifRect.anchorMax = new Vector2(0.5f, 1);
        notifRect.sizeDelta = new Vector2(400, 60);
        notifRect.anchoredPosition = notificationPosition;
        
        Image notifBg = notificationPanel.AddComponent<Image>();
        notifBg.color = notificationBackgroundColor;
        
        TextMeshProUGUI notifText = notificationPanel.AddComponent<TextMeshProUGUI>();
        notifText.fontSize = 18;
        notifText.alignment = TextAlignmentOptions.Center;
        notifText.color = notificationTextColor;
        notifText.text = "";
        
        notificationPanel.SetActive(false);
    }
    
    IEnumerator UpdateHUDCoroutine()
    {
        while (true)
        {
            UpdateMiniInventory();
            yield return new WaitForSeconds(0.1f); // Обновляем каждые 0.1 секунды
        }
    }
    
    void UpdateMiniInventory()
    {
        if (!showMiniInventory || hudSlots.Count == 0) return;
        
        // Получаем предметы из инвентаря (через рефлексию, так как inventory приватный)
        var inventoryField = typeof(AutoInventoryUI).GetField("items", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (inventoryField != null)
        {
            var items = inventoryField.GetValue(inventoryUI) as List<InventoryItem>;
            
            if (items != null)
            {
                // Обновляем слоты
                for (int i = 0; i < hudSlots.Count; i++)
                {
                    GameObject slot = hudSlots[i];
                    Image icon = slot.transform.Find("Icon").GetComponent<Image>();
                    
                    if (i < items.Count && items[i] != null)
                    {
                        icon.sprite = items[i].itemIcon;
                        icon.color = items[i].itemIcon != null ? Color.white : Color.clear;
                    }
                    else
                    {
                        icon.sprite = null;
                        icon.color = Color.clear;
                    }
                }
            }
        }
    }
    
    public void ShowItemNotification(string itemName)
    {
        if (!showItemNotifications) return;
        
        notificationQueue.Enqueue($"Получен предмет: {itemName}");
        
        if (!isShowingNotification)
        {
            StartCoroutine(ShowNotificationCoroutine());
        }
    }
    
    IEnumerator ShowNotificationCoroutine()
    {
        isShowingNotification = true;
        
        while (notificationQueue.Count > 0)
        {
            string message = notificationQueue.Dequeue();
            
            if (notificationPanel != null)
            {
                TextMeshProUGUI text = notificationPanel.GetComponent<TextMeshProUGUI>();
                text.text = message;
                
                notificationPanel.SetActive(true);
                
                // Анимация появления
                CanvasGroup canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
                }
                
                // Fade in
                float fadeTime = notificationFadeTime;
                float elapsed = 0;
                while (elapsed < fadeTime)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeTime);
                    yield return null;
                }
                
                // Показываем уведомление
                yield return new WaitForSeconds(notificationDuration);
                
                // Fade out
                elapsed = 0;
                while (elapsed < fadeTime)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeTime);
                    yield return null;
                }
                
                notificationPanel.SetActive(false);
            }
        }
        
        isShowingNotification = false;
    }
    
    // Публичные методы для управления HUD
    public void ToggleMiniInventory()
    {
        if (miniInventoryPanel != null)
        {
            miniInventoryPanel.SetActive(!miniInventoryPanel.activeInHierarchy);
        }
    }
    
    public void SetHUDPosition(Vector2 position)
    {
        hudPosition = position;
        if (miniInventoryPanel != null)
        {
            RectTransform rect = miniInventoryPanel.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
        }
    }
    
    void Update()
    {
        // Переключение видимости мини-инвентаря
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleMiniInventory();
        }
    }
}
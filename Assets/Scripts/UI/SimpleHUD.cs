using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Простой HUD для отображения предметов
/// Добавьте этот скрипт на пустой GameObject
/// </summary>
public class SimpleHUD : MonoBehaviour
{
    [Header("HUD Settings")]
    public bool showHUD = true;
    public int maxSlots = 3;
    public float slotSize = 80f;
    public Vector2 position = new Vector2(0, -150);
    
    [Header("Colors")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f);
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    
    private GameObject canvas;
    private GameObject hudPanel;
    private List<GameObject> slots = new List<GameObject>();
    private List<Image> slotImages = new List<Image>();
    private List<Image> slotIcons = new List<Image>();
    private int selectedSlot = 0;
    
    public static SimpleHUD Instance { get; private set; }
    
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
        if (showHUD)
        {
            CreateHUD();
        }
    }
    
    void CreateHUD()
    {
        // Создаем Canvas
        canvas = new GameObject("HUD_Canvas");
        Canvas canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasComp.sortingOrder = 10;
        
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Создаем панель
        hudPanel = new GameObject("HUD_Panel");
        hudPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0);
        panelRect.anchorMax = new Vector2(0.5f, 0);
        panelRect.sizeDelta = new Vector2(maxSlots * slotSize + 40, slotSize + 40);
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
        slotRect.anchoredPosition = new Vector2(20 + index * (slotSize + 10), 0);
        
        // Фон слота
        Image slotBg = slot.AddComponent<Image>();
        slotBg.color = normalColor;
        slotImages.Add(slotBg);
        
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
        
        slots.Add(slot);
    }
    
    public void AddItem(string itemName, Sprite icon)
    {
        // Простое добавление предмета в первый свободный слот
        for (int i = 0; i < maxSlots; i++)
        {
            if (slotIcons[i].sprite == null)
            {
                slotIcons[i].sprite = icon;
                slotIcons[i].color = Color.white;
                Debug.Log($"✅ Добавлен предмет: {itemName} в слот {i}");
                break;
            }
        }
    }
    
    public void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < maxSlots)
        {
            selectedSlot = slotIndex;
            UpdateSlotColors();
            Debug.Log($"Выбран слот: {slotIndex}");
        }
    }
    
    void UpdateSlotColors()
    {
        for (int i = 0; i < slotImages.Count; i++)
        {
            if (i == selectedSlot)
            {
                slotImages[i].color = selectedColor;
            }
            else
            {
                slotImages[i].color = normalColor;
            }
        }
    }
    
    void Update()
    {
        // Выбор слотов клавишами 1, 2, 3
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
        
        // Тестовые предметы
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddItem("Дневник", CreateColoredIcon(new Color(0.6f, 0.4f, 0.2f)));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddItem("Зажигалка", CreateColoredIcon(Color.red));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddItem("Ключ", CreateColoredIcon(Color.yellow));
        }
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

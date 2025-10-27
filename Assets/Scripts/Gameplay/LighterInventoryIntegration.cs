using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Интеграция зажигалки с системой инвентаря
/// </summary>
public class LighterInventoryIntegration : MonoBehaviour
{
    [Header("Inventory Integration")]
    public int lighterSlotIndex = 1; // Индекс слота зажигалки (0, 1, 2)
    public GameObject lighterInHand; // Объект зажигалки в руке
    public Transform handPosition; // Позиция руки
    
    [Header("First Person Setup")]
    public FirstPersonCameraSetup cameraSetup;
    
    [Header("UI References")]
    public Slider fuelSlider;
    public Text fuelText;
    
    private LighterMechanics lighterMechanics;
    private bool isLighterSelected = false;
    private GameObject currentLighterInstance;
    
    void Start()
    {
        // Находим зажигалку в инвентаре
        FindLighterInInventory();
        
        // Создаем зажигалку в руке если её нет
        if (lighterInHand == null)
        {
            CreateLighterInHand();
        }
        
        // Настраиваем UI
        SetupUI();
    }
    
    void FindLighterInInventory()
    {
        // Ищем зажигалку в инвентаре
        InventorySystem inventory = InventorySystem.Instance;
        if (inventory != null)
        {
            for (int i = 0; i < inventory.inventory.Count; i++)
            {
                if (inventory.inventory[i].itemName == "Зажигалка")
                {
                    lighterSlotIndex = i;
                    Debug.Log($"✅ Зажигалка найдена в слоте {i}");
                    break;
                }
            }
        }
    }
    
    void CreateLighterInHand()
    {
        // Создаем объект зажигалки в руке
        lighterInHand = new GameObject("LighterInHand");
        
        // Настраиваем позицию относительно камеры от первого лица
        if (cameraSetup != null)
        {
            lighterInHand.transform.position = cameraSetup.GetLighterWorldPosition();
            lighterInHand.transform.rotation = cameraSetup.GetLighterWorldRotation();
        }
        else
        {
            lighterInHand.transform.SetParent(handPosition != null ? handPosition : transform);
            lighterInHand.transform.localPosition = Vector3.zero;
            lighterInHand.transform.localRotation = Quaternion.identity;
        }
        
        // Добавляем компонент механики зажигалки
        lighterMechanics = lighterInHand.AddComponent<LighterMechanics>();
        
        // Добавляем визуальную модель
        SimpleLighterVisual lighterVisual = lighterInHand.AddComponent<SimpleLighterVisual>();
        lighterMechanics.lighterVisual = lighterVisual;
        
        // Добавляем анимацию от первого лица
        FirstPersonLighterAnimation lighterAnimation = lighterInHand.AddComponent<FirstPersonLighterAnimation>();
        lighterMechanics.lighterAnimation = lighterAnimation;
        
        // Настраиваем UI ссылки
        lighterMechanics.fuelSlider = fuelSlider;
        lighterMechanics.fuelText = fuelText;
        
        // Скрываем зажигалку по умолчанию
        lighterInHand.SetActive(false);
        
        Debug.Log("✅ Зажигалка в руке создана!");
    }
    
    void SetupUI()
    {
        // Создаем UI для топлива если его нет
        if (fuelSlider == null || fuelText == null)
        {
            CreateFuelUI();
        }
    }
    
    void CreateFuelUI()
    {
        // Создаем Canvas для UI топлива
        GameObject fuelCanvas = new GameObject("FuelUI_Canvas");
        Canvas canvas = fuelCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5;
        
        CanvasScaler scaler = fuelCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Создаем панель топлива
        GameObject fuelPanel = new GameObject("FuelPanel");
        fuelPanel.transform.SetParent(fuelCanvas.transform, false);
        
        RectTransform panelRect = fuelPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.sizeDelta = new Vector2(200, 50);
        panelRect.anchoredPosition = new Vector2(120, -30);
        
        Image panelBg = fuelPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // Создаем слайдер топлива
        GameObject sliderObj = new GameObject("FuelSlider");
        sliderObj.transform.SetParent(fuelPanel.transform, false);
        
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.1f, 0.3f);
        sliderRect.anchorMax = new Vector2(0.9f, 0.7f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;
        
        fuelSlider = sliderObj.AddComponent<Slider>();
        fuelSlider.minValue = 0f;
        fuelSlider.maxValue = 1f;
        fuelSlider.value = 1f;
        
        // Создаем текст топлива
        GameObject textObj = new GameObject("FuelText");
        textObj.transform.SetParent(fuelPanel.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 0.3f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        fuelText = textObj.AddComponent<Text>();
        fuelText.text = "Топливо: 100%";
        fuelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        fuelText.fontSize = 12;
        fuelText.color = Color.white;
        fuelText.alignment = TextAnchor.MiddleCenter;
        
        // Скрываем UI по умолчанию
        fuelPanel.SetActive(false);
    }
    
    void Update()
    {
        // Проверяем выбор слота зажигалки
        CheckLighterSelection();
        
        // Обновляем состояние зажигалки в руке
        UpdateLighterInHand();
    }
    
    void CheckLighterSelection()
    {
        // Проверяем, выбран ли слот с зажигалкой
        bool wasSelected = isLighterSelected;
        isLighterSelected = IsLighterSlotSelected();
        
        if (isLighterSelected && !wasSelected)
        {
            // Зажигалка выбрана - показываем в руке
            ShowLighterInHand();
        }
        else if (!isLighterSelected && wasSelected)
        {
            // Зажигалка не выбрана - скрываем
            HideLighterInHand();
        }
    }
    
    bool IsLighterSlotSelected()
    {
        // Проверяем, выбран ли слот с зажигалкой
        // Это зависит от вашей системы выбора слотов
        // Предполагаем, что есть система выбора слотов
        
        // Здесь нужно интегрироваться с вашей системой выбора слотов
        // Например, если у вас есть HUDController:
        HUDController hudController = FindObjectOfType<HUDController>();
        if (hudController != null)
        {
            // Предполагаем, что у HUDController есть selectedSlot
            return hudController.selectedSlot == lighterSlotIndex;
        }
        
        return false;
    }
    
    void ShowLighterInHand()
    {
        if (lighterInHand != null)
        {
            lighterInHand.SetActive(true);
            
            if (lighterMechanics != null)
            {
                lighterMechanics.SetInHand(true);
            }
            
            // Показываем UI топлива
            if (fuelSlider != null)
            {
                fuelSlider.transform.parent.gameObject.SetActive(true);
            }
            
            Debug.Log("🔥 Зажигалка в руке!");
        }
    }
    
    void HideLighterInHand()
    {
        if (lighterInHand != null)
        {
            lighterInHand.SetActive(false);
            
            if (lighterMechanics != null)
            {
                lighterMechanics.SetInHand(false);
            }
            
            // Скрываем UI топлива
            if (fuelSlider != null)
            {
                fuelSlider.transform.parent.gameObject.SetActive(false);
            }
            
            Debug.Log("💨 Зажигалка убрана из руки!");
        }
    }
    
    void UpdateLighterInHand()
    {
        if (isLighterSelected && lighterMechanics != null)
        {
            // Обновляем UI топлива
            if (fuelSlider != null)
            {
                fuelSlider.value = lighterMechanics.GetFuelPercentage();
            }
            
            if (fuelText != null)
            {
                fuelText.text = $"Топливо: {lighterMechanics.GetFuelPercentage() * 100:F1}%";
            }
        }
    }
    
    public void RefillLighter()
    {
        if (lighterMechanics != null)
        {
            lighterMechanics.RefillFuel();
        }
    }
    
    public bool IsLighterLit()
    {
        return lighterMechanics != null && lighterMechanics.isLit;
    }
    
    public bool HasFuel()
    {
        return lighterMechanics != null && lighterMechanics.HasFuel();
    }
}

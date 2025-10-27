using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Этот скрипт автоматически создает правильную структуру UI для инвентаря
/// Добавьте его на пустой GameObject и нажмите кнопку "Setup Inventory UI" в инспекторе
/// </summary>
public class InventorySetupHelper : MonoBehaviour
{
    [Header("Setup")]
    public bool autoSetup = true;
    
    void Start()
    {
        if (autoSetup)
        {
            CreateInventoryUI();
            Destroy(this.gameObject); // Удаляем после настройки
        }
    }
    
    [ContextMenu("Setup Inventory UI")]
    void CreateInventoryUI()
    {
        // Удаляем старый Canvas если есть
        GameObject oldCanvas = GameObject.Find("InventoryCanvas");
        if (oldCanvas != null)
        {
            DestroyImmediate(oldCanvas);
        }
        
        // Создаем Canvas
        GameObject canvas = new GameObject("InventoryCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvas.AddComponent<GraphicRaycaster>();
        
        // Создаем EventSystem если его нет
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Создаем панель инвентаря
        GameObject inventoryPanel = new GameObject("InventoryPanel");
        inventoryPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = inventoryPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(800, 600);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = inventoryPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Добавляем компонент InventorySystem
        InventorySystem inventorySystem = inventoryPanel.AddComponent<InventorySystem>();
        inventorySystem.inventoryPanel = inventoryPanel;
        
        // Создаем заголовок
        GameObject title = new GameObject("Title");
        title.transform.SetParent(inventoryPanel.transform, false);
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(-40, 60);
        titleRect.anchoredPosition = new Vector2(0, -30);
        
        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "ИНВЕНТАРЬ";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // Создаем кнопку закрытия
        GameObject closeButton = new GameObject("CloseButton");
        closeButton.transform.SetParent(inventoryPanel.transform, false);
        RectTransform closeRect = closeButton.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(50, 50);
        closeRect.anchoredPosition = new Vector2(-25, -25);
        
        Image closeImage = closeButton.AddComponent<Image>();
        closeImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        Button closeBtn = closeButton.AddComponent<Button>();
        closeBtn.onClick.AddListener(() => inventorySystem.ToggleInventory());
        
        GameObject closeText = new GameObject("Text");
        closeText.transform.SetParent(closeButton.transform, false);
        RectTransform closeTextRect = closeText.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI closeTMP = closeText.AddComponent<TextMeshProUGUI>();
        closeTMP.text = "X";
        closeTMP.fontSize = 30;
        closeTMP.alignment = TextAlignmentOptions.Center;
        closeTMP.color = Color.white;
        
        // Создаем Grid для слотов
        GameObject grid = new GameObject("InventoryGrid");
        grid.transform.SetParent(inventoryPanel.transform, false);
        RectTransform gridRect = grid.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0, 0);
        gridRect.anchorMax = new Vector2(1, 1);
        gridRect.offsetMin = new Vector2(20, 20);
        gridRect.offsetMax = new Vector2(-20, -80);
        
        GridLayoutGroup gridLayout = grid.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(120, 120);
        gridLayout.spacing = new Vector2(10, 10);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 5;
        gridLayout.childAlignment = TextAnchor.UpperLeft;
        
        inventorySystem.inventoryGrid = grid.transform;
        
        // Создаем префаб слота
        CreateSlotPrefab(inventorySystem);
        
        // Скрываем панель по умолчанию
        inventoryPanel.SetActive(false);
        
        Debug.Log("✅ UI инвентаря создан успешно! Нажмите I для открытия инвентаря.");
    }
    
    void CreateSlotPrefab(InventorySystem inventorySystem)
    {
        // Создаем слот
        GameObject slot = new GameObject("ItemSlot");
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(120, 120);
        
        Image slotBg = slot.AddComponent<Image>();
        slotBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        InventorySlot slotScript = slot.AddComponent<InventorySlot>();
        
        // Иконка предмета
        GameObject icon = new GameObject("ItemIcon");
        icon.transform.SetParent(slot.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.3f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;
        iconImage.preserveAspect = true;
        slotScript.itemIcon = iconImage;
        
        // Название предмета
        GameObject nameObj = new GameObject("ItemName");
        nameObj.transform.SetParent(slot.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0.25f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 14;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        nameText.enableWordWrapping = false;
        slotScript.itemNameText = nameText;
        
        // Пустой слот индикатор
        GameObject emptySlot = new GameObject("EmptySlot");
        emptySlot.transform.SetParent(slot.transform, false);
        RectTransform emptyRect = emptySlot.AddComponent<RectTransform>();
        emptyRect.anchorMin = Vector2.zero;
        emptyRect.anchorMax = Vector2.one;
        emptyRect.offsetMin = Vector2.zero;
        emptyRect.offsetMax = Vector2.zero;
        
        Image emptyImage = emptySlot.AddComponent<Image>();
        emptyImage.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        slotScript.emptySlot = emptySlot;
        
        // Сохраняем как префаб
        if (!System.IO.Directory.Exists("Assets/Prefabs"))
        {
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
        }
        
        #if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(slot, "Assets/Prefabs/ItemSlot.prefab");
        #endif
        
        inventorySystem.inventorySlotPrefab = slot;
        
        Debug.Log("✅ Префаб слота создан: Assets/Prefabs/ItemSlot.prefab");
    }
}

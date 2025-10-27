using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Управляет началом игры и выдачей стартовых предметов
/// </summary>
public class GameStartupManager : MonoBehaviour
{
    [Header("Startup Settings")]
    public bool skipCutscene = false; // Для тестирования
    public float cutsceneDuration = 5f; // Длительность катсцены в секундах
    
    [Header("Starting Items")]
    public bool giveDiary = true;
    public bool giveLighter = true;
    public bool giveKey = true;
    
    [Header("Item Icons (optional)")]
    public Sprite diaryIcon;
    public Sprite lighterIcon;
    public Sprite keyIcon;
    
    [Header("UI References")]
    public GameObject cutsceneUI; // Панель катсцены (создается автоматически)
    
    private bool hasGivenItems = false;
    private AutoInventoryUI inventoryUI;
    
    void Start()
    {
        inventoryUI = FindObjectOfType<AutoInventoryUI>();
        
        if (inventoryUI == null)
        {
            Debug.LogError("AutoInventoryUI не найден! Создайте объект с компонентом AutoInventoryUI.");
            return;
        }
        
        if (skipCutscene)
        {
            StartCoroutine(GiveStartingItems());
        }
        else
        {
            StartCoroutine(PlayCutsceneAndGiveItems());
        }
    }
    
    IEnumerator PlayCutsceneAndGiveItems()
    {
        Debug.Log("🎬 Начинается катсцена...");
        
        // Создаем UI катсцены
        CreateCutsceneUI();
        
        // Показываем катсцену
        if (cutsceneUI != null)
        {
            cutsceneUI.SetActive(true);
        }
        
        // Ждем окончания катсцены
        yield return new WaitForSeconds(cutsceneDuration);
        
        // Скрываем катсцену
        if (cutsceneUI != null)
        {
            cutsceneUI.SetActive(false);
        }
        
        Debug.Log("🎬 Катсцена завершена!");
        
        // Выдаем предметы
        yield return StartCoroutine(GiveStartingItems());
    }
    
    IEnumerator GiveStartingItems()
    {
        if (hasGivenItems) yield break;
        
        Debug.Log("📦 Выдача стартовых предметов...");
        
        // Небольшая задержка для плавности
        yield return new WaitForSeconds(0.5f);
        
        if (giveDiary)
        {
            inventoryUI.AddItem(KeyItems.CreateDiary(diaryIcon));
            yield return new WaitForSeconds(0.3f);
        }
        
        if (giveLighter)
        {
            inventoryUI.AddItem(KeyItems.CreateLighter(lighterIcon));
            yield return new WaitForSeconds(0.3f);
        }
        
        if (giveKey)
        {
            inventoryUI.AddItem(KeyItems.CreateKey(keyIcon));
            yield return new WaitForSeconds(0.3f);
        }
        
        hasGivenItems = true;
        Debug.Log("✅ Все стартовые предметы выданы!");
    }
    
    void CreateCutsceneUI()
    {
        if (cutsceneUI != null) return;
        
        // Создаем Canvas для катсцены
        GameObject canvas = new GameObject("CutsceneCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasComponent.sortingOrder = 10; // Поверх всего
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // Создаем панель катсцены
        cutsceneUI = new GameObject("CutscenePanel");
        cutsceneUI.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = cutsceneUI.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImage = cutsceneUI.AddComponent<Image>();
        panelImage.color = Color.black;
        
        // Текст катсцены
        GameObject textObj = new GameObject("CutsceneText");
        textObj.transform.SetParent(cutsceneUI.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.4f);
        textRect.anchorMax = new Vector2(0.9f, 0.6f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMPro.TextMeshProUGUI cutsceneText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        cutsceneText.text = "Вы просыпаетесь в темной комнате...\n\nВокруг вас лежат странные предметы:\n• Старый дневник\n• Зажигалка\n• Ржавый ключ\n\nЧто-то не так с этим местом...";
        cutsceneText.fontSize = 24;
        cutsceneText.alignment = TMPro.TextAlignmentOptions.Center;
        cutsceneText.color = Color.white;
        
        // Индикатор загрузки
        GameObject loadingObj = new GameObject("LoadingText");
        loadingObj.transform.SetParent(cutsceneUI.transform, false);
        
        RectTransform loadingRect = loadingObj.AddComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(0.5f, 0.2f);
        loadingRect.anchorMax = new Vector2(0.5f, 0.2f);
        loadingRect.sizeDelta = new Vector2(400, 50);
        loadingRect.anchoredPosition = Vector2.zero;
        
        TMPro.TextMeshProUGUI loadingText = loadingObj.AddComponent<TMPro.TextMeshProUGUI>();
        loadingText.text = "Загрузка...";
        loadingText.fontSize = 18;
        loadingText.alignment = TMPro.TextAlignmentOptions.Center;
        loadingText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        
        // Анимация загрузки
        StartCoroutine(AnimateLoadingText(loadingText));
    }
    
    IEnumerator AnimateLoadingText(TMPro.TextMeshProUGUI loadingText)
    {
        string baseText = "Загрузка";
        int dots = 0;
        
        while (cutsceneUI != null && cutsceneUI.activeInHierarchy)
        {
            string dotsStr = new string('.', (dots % 4) + 1);
            loadingText.text = baseText + dotsStr;
            dots++;
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // Публичные методы для внешнего управления
    public void SkipCutscene()
    {
        if (cutsceneUI != null)
        {
            cutsceneUI.SetActive(false);
        }
        StartCoroutine(GiveStartingItems());
    }
    
    public void GiveItemManually(string itemName)
    {
        if (inventoryUI == null) return;
        
        switch (itemName.ToLower())
        {
            case "diary":
            case "дневник":
                inventoryUI.AddItem(KeyItems.CreateDiary(diaryIcon));
                break;
            case "lighter":
            case "зажигалка":
                inventoryUI.AddItem(KeyItems.CreateLighter(lighterIcon));
                break;
            case "key":
            case "ключ":
                inventoryUI.AddItem(KeyItems.CreateKey(keyIcon));
                break;
        }
    }
}
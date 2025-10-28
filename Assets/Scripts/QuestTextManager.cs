using UnityEngine;
using TMPro;

/// <summary>
/// Простой менеджер для управления текстом задач
/// Позволяет легко менять текст задач после определенных взаимодействий
/// </summary>
public class QuestTextManager : MonoBehaviour
{
    [Header("=== НАСТРОЙКИ ТЕКСТА ЗАДАЧ ===")]
    [SerializeField] private GameObject prologueTextObject; // Объект с текстом Prologue
    [SerializeField] private string newQuestText = "Новая задача: Найдите выход из этого места!"; // Новый текст задачи
    
    [Header("=== НАСТРОЙКИ АНИМАЦИИ ===")]
    [SerializeField] private float fadeOutDuration = 1f; // Длительность исчезновения старого текста
    [SerializeField] private float fadeInDuration = 1f; // Длительность появления нового текста
    [SerializeField] private float delayBetweenTexts = 0.5f; // Задержка между текстами
    
    private TextMeshProUGUI prologueTextComponent;
    private string originalText;
    private bool hasChangedText = false;
    
    void Start()
    {
        // Находим компонент TextMeshProUGUI на объекте Prologue
        if (prologueTextObject == null)
        {
            // Попробуем найти объект по имени
            GameObject prologueObj = GameObject.Find("Prologue");
            if (prologueObj != null)
            {
                prologueTextObject = prologueObj;
            }
        }
        
        if (prologueTextObject != null)
        {
            prologueTextComponent = prologueTextObject.GetComponent<TextMeshProUGUI>();
            if (prologueTextComponent != null)
            {
                originalText = prologueTextComponent.text;
                Debug.Log($"QuestTextManager: Найден текст Prologue: {originalText}");
            }
            else
            {
                Debug.LogError("QuestTextManager: Не найден компонент TextMeshProUGUI на объекте Prologue!");
            }
        }
        else
        {
            Debug.LogError("QuestTextManager: Не найден объект Prologue!");
        }
    }
    
    /// <summary>
    /// Публичный метод для смены текста задачи
    /// Вызывается из других скриптов (например, NPCController)
    /// </summary>
    public void ChangeQuestText()
    {
        if (hasChangedText)
        {
            Debug.Log("QuestTextManager: Текст уже был изменен!");
            return;
        }
        
        if (prologueTextComponent == null)
        {
            Debug.LogError("QuestTextManager: Компонент текста не найден!");
            return;
        }
        
        Debug.Log("QuestTextManager: Начинаем смену текста задачи...");
        StartCoroutine(ChangeTextCoroutine());
    }
    
    /// <summary>
    /// Корутина для плавной смены текста
    /// </summary>
    private System.Collections.IEnumerator ChangeTextCoroutine()
    {
        hasChangedText = true;
        
        // Этап 1: Плавно скрываем старый текст
        Debug.Log("QuestTextManager: Скрываем старый текст...");
        yield return StartCoroutine(FadeText(0f, fadeOutDuration));
        
        // Этап 2: Ждем немного
        yield return new WaitForSeconds(delayBetweenTexts);
        
        // Этап 3: Меняем текст
        Debug.Log($"QuestTextManager: Меняем текст на: {newQuestText}");
        prologueTextComponent.text = newQuestText;
        
        // Этап 4: Плавно показываем новый текст
        Debug.Log("QuestTextManager: Показываем новый текст...");
        yield return StartCoroutine(FadeText(1f, fadeInDuration));
        
        Debug.Log("QuestTextManager: Смена текста завершена!");
    }
    
    /// <summary>
    /// Корутина для плавного изменения прозрачности текста
    /// </summary>
    private System.Collections.IEnumerator FadeText(float targetAlpha, float duration)
    {
        Color startColor = prologueTextComponent.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            prologueTextComponent.color = Color.Lerp(startColor, targetColor, progress);
            
            yield return null;
        }
        
        prologueTextComponent.color = targetColor;
    }
    
    /// <summary>
    /// Метод для смены текста с кастомным сообщением
    /// </summary>
    public void ChangeQuestText(string customText)
    {
        if (hasChangedText)
        {
            Debug.Log("QuestTextManager: Текст уже был изменен!");
            return;
        }
        
        newQuestText = customText;
        ChangeQuestText();
    }
    
    /// <summary>
    /// Метод для сброса текста к оригинальному
    /// </summary>
    public void ResetQuestText()
    {
        if (prologueTextComponent != null && !string.IsNullOrEmpty(originalText))
        {
            prologueTextComponent.text = originalText;
            prologueTextComponent.color = new Color(prologueTextComponent.color.r, prologueTextComponent.color.g, prologueTextComponent.color.b, 1f);
            hasChangedText = false;
            Debug.Log("QuestTextManager: Текст сброшен к оригинальному!");
        }
    }
    
    /// <summary>
    /// Метод для проверки, был ли изменен текст
    /// </summary>
    public bool HasTextBeenChanged()
    {
        return hasChangedText;
    }
    
    /// <summary>
    /// Метод для установки нового текста задачи (без смены)
    /// </summary>
    public void SetNewQuestText(string text)
    {
        newQuestText = text;
    }
    
    // Методы для настройки в инспекторе
    public void SetPrologueTextObject(GameObject obj)
    {
        prologueTextObject = obj;
        if (obj != null)
        {
            prologueTextComponent = obj.GetComponent<TextMeshProUGUI>();
        }
    }
    
    public void SetNewQuestTextFromInspector(string text)
    {
        newQuestText = text;
    }
}

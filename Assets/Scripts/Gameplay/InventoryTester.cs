using UnityEngine;

/// <summary>
/// Скрипт для тестирования системы инвентаря
/// Добавьте его на любой GameObject для быстрого добавления предметов
/// </summary>
public class InventoryTester : MonoBehaviour
{
    [Header("Test Items")]
    public Sprite diaryIcon;
    public Sprite lighterIcon;
    public Sprite keyIcon;
    
    [Header("Test Keys")]
    public KeyCode addDiaryKey = KeyCode.Alpha1;
    public KeyCode addLighterKey = KeyCode.Alpha2;
    public KeyCode addKeyKey = KeyCode.Alpha3;
    
    void Update()
    {
        if (Input.GetKeyDown(addDiaryKey))
        {
            AddDiary();
        }
        
        if (Input.GetKeyDown(addLighterKey))
        {
            AddLighter();
        }
        
        if (Input.GetKeyDown(addKeyKey))
        {
            AddKey();
        }
    }
    
    void AddDiary()
    {
        if (InventorySystem.Instance != null)
        {
            InventoryItem diary = KeyItems.CreateDiary(diaryIcon);
            InventorySystem.Instance.AddItem(diary);
            Debug.Log("Добавлен дневник (нажата клавиша 1)");
        }
    }
    
    void AddLighter()
    {
        if (InventorySystem.Instance != null)
        {
            InventoryItem lighter = KeyItems.CreateLighter(lighterIcon);
            InventorySystem.Instance.AddItem(lighter);
            Debug.Log("Добавлена зажигалка (нажата клавиша 2)");
        }
    }
    
    void AddKey()
    {
        if (InventorySystem.Instance != null)
        {
            InventoryItem key = KeyItems.CreateKey(keyIcon);
            InventorySystem.Instance.AddItem(key);
            Debug.Log("Добавлен ключ (нажата клавиша 3)");
        }
    }
}
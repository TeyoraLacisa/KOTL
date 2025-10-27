using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("HUD Slots")]
    public Image[] slotBackgrounds;
    public Image[] slotIcons;
    
    [Header("Colors")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f);
    
    public int selectedSlot = 0;
    
    void Start()
    {
        // Автоматически найти все слоты
        FindSlots();
        UpdateSelection();
        Debug.Log("✅ HUD Controller готов!");
    }
    
    void FindSlots()
    {
        // Найти все слоты по имени
        slotBackgrounds = new Image[3];
        slotIcons = new Image[3];
        
        for (int i = 0; i < 3; i++)
        {
            GameObject slot = GameObject.Find($"Slot{i + 1}");
            if (slot != null)
            {
                slotBackgrounds[i] = slot.GetComponent<Image>();
                Transform iconTransform = slot.transform.Find("Icon");
                if (iconTransform != null)
                {
                    slotIcons[i] = iconTransform.GetComponent<Image>();
                }
                Debug.Log($"Найден слот {i + 1}");
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
        
        // Добавление цветных предметов клавишами Q, W, E
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddItem(0, Color.red);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddItem(1, Color.green);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddItem(2, Color.blue);
        }
    }
    
    void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < 3)
        {
            selectedSlot = slotIndex;
            UpdateSelection();
            Debug.Log($"Выбран слот {slotIndex + 1}");
        }
    }
    
    void UpdateSelection()
    {
        for (int i = 0; i < 3; i++)
        {
            if (slotBackgrounds[i] != null)
            {
                if (i == selectedSlot)
                {
                    slotBackgrounds[i].color = selectedColor;
                }
                else
                {
                    slotBackgrounds[i].color = normalColor;
                }
            }
        }
    }
    
    void AddItem(int slotIndex, Color color)
    {
        if (slotIcons[slotIndex] != null)
        {
            slotIcons[slotIndex].color = color;
            Debug.Log($"Добавлен предмет в слот {slotIndex + 1}");
        }
    }
}

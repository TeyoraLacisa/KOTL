using UnityEngine;
using UnityEngine.UI;

public class TestHUD : MonoBehaviour
{
    void Start()
    {
        CreateSimpleHUD();
    }
    
    void CreateSimpleHUD()
    {
        // Создаем Canvas
        GameObject canvas = new GameObject("TestHUD_Canvas");
        Canvas canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasComp.sortingOrder = 10;
        
        // Создаем панель
        GameObject panel = new GameObject("HUD_Panel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0);
        panelRect.anchorMax = new Vector2(0.5f, 0);
        panelRect.sizeDelta = new Vector2(300, 100);
        panelRect.anchoredPosition = new Vector2(0, -150);
        
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // Создаем 3 слота
        for (int i = 0; i < 3; i++)
        {
            CreateSlot(panel, i);
        }
        
        Debug.Log("✅ TestHUD создан!");
    }
    
    void CreateSlot(GameObject parent, int index)
    {
        GameObject slot = new GameObject($"Slot_{index}");
        slot.transform.SetParent(parent.transform, false);
        
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0, 0.5f);
        slotRect.anchorMax = new Vector2(0, 0.5f);
        slotRect.sizeDelta = new Vector2(80, 80);
        slotRect.anchoredPosition = new Vector2(20 + index * 90, 0);
        
        Image slotBg = slot.AddComponent<Image>();
        slotBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        Debug.Log($"Создан слот {index}");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Пробел нажат! HUD работает!");
        }
    }
}

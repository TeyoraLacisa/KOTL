using UnityEngine;

/// <summary>
/// Создает иконки для предметов инвентаря
/// Добавьте этот скрипт на любой GameObject для создания иконок
/// </summary>
public class ItemIconCreator : MonoBehaviour
{
    [Header("Icon Settings")]
    public int iconSize = 64;
    
    [Header("Generated Icons")]
    public Sprite diaryIcon;
    public Sprite lighterIcon;
    public Sprite keyIcon;
    
    void Start()
    {
        CreateIcons();
    }
    
    void CreateIcons()
    {
        // Создаем иконки
        diaryIcon = CreateColoredIcon(new Color(0.6f, 0.4f, 0.2f), "DiaryIcon");
        lighterIcon = CreateColoredIcon(Color.red, "LighterIcon");
        keyIcon = CreateColoredIcon(Color.yellow, "KeyIcon");
        
        Debug.Log("✅ Иконки предметов созданы!");
    }
    
    Sprite CreateColoredIcon(Color color, string name)
    {
        Texture2D texture = new Texture2D(iconSize, iconSize);
        
        // Заполняем цветом
        Color[] pixels = new Color[iconSize * iconSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Создаем спрайт
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, iconSize, iconSize), new Vector2(0.5f, 0.5f));
        sprite.name = name;
        
        return sprite;
    }
}

using UnityEngine;

/// <summary>
/// Генерирует простые иконки для предметов инвентаря
/// </summary>
public class ItemIconGenerator : MonoBehaviour
{
    [Header("Icon Settings")]
    public int iconSize = 64;
    
    public static Sprite CreateDiaryIcon()
    {
        return CreateColoredIcon(new Color(0.6f, 0.4f, 0.2f), "DiaryIcon");
    }
    
    public static Sprite CreateLighterIcon()
    {
        return CreateColoredIcon(Color.red, "LighterIcon");
    }
    
    public static Sprite CreateKeyIcon()
    {
        return CreateColoredIcon(Color.yellow, "KeyIcon");
    }
    
    public static Sprite CreateColoredIcon(Color color, string name)
    {
        // Создаем текстуру
        Texture2D texture = new Texture2D(64, 64);
        
        // Заполняем цветом
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Создаем спрайт
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        sprite.name = name;
        
        return sprite;
    }
    
    public static Sprite CreateTextIcon(string text, Color backgroundColor, Color textColor)
    {
        // Создаем текстуру
        Texture2D texture = new Texture2D(64, 64);
        
        // Заполняем фон
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Простая реализация текста (заглушка)
        // В реальном проекте лучше использовать TextMeshPro для генерации иконок
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Создаем спрайт
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        sprite.name = text + "Icon";
        
        return sprite;
    }
}
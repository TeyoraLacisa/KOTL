using UnityEngine;

/// <summary>
/// Класс для хранения предустановленных ключевых предметов
/// </summary>
public static class KeyItems
{
    // Названия ключевых предметов
    public const string DIARY = "Дневник";
    public const string LIGHTER = "Зажигалка";
    public const string KEY = "Ключ";
    
    // Описания предметов
    public static class Descriptions
    {
        public const string DIARY_DESC = "Старый потрепанный дневник. Страницы пожелтели от времени. Может содержать важную информацию.";
        public const string LIGHTER_DESC = "Металлическая зажигалка. Поможет осветить темные уголки и отпугнуть тени.";
        public const string KEY_DESC = "Ржавый ключ. Интересно, какую дверь он откроет?";
    }
    
    // Создание предустановленных предметов
    public static InventoryItem CreateDiary(Sprite icon = null)
    {
        if (icon == null)
            icon = ItemIconGenerator.CreateDiaryIcon();
        return new InventoryItem(DIARY, icon, Descriptions.DIARY_DESC, true);
    }
    
    public static InventoryItem CreateLighter(Sprite icon = null)
    {
        if (icon == null)
            icon = ItemIconGenerator.CreateLighterIcon();
        return new InventoryItem(LIGHTER, icon, Descriptions.LIGHTER_DESC, true);
    }
    
    public static InventoryItem CreateKey(Sprite icon = null)
    {
        if (icon == null)
            icon = ItemIconGenerator.CreateKeyIcon();
        return new InventoryItem(KEY, icon, Descriptions.KEY_DESC, true);
    }
}
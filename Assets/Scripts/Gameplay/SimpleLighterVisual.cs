using UnityEngine;

/// <summary>
/// Создает визуальную модель зажигалки (упрощенная версия)
/// </summary>
public class SimpleLighterVisual : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color lighterColor = new Color(0.8f, 0.8f, 0.9f);
    public Color flameColor = new Color(1f, 0.5f, 0f);
    
    private GameObject lighterBody;
    private GameObject lighterCap;
    private GameObject lighterWheel;
    private GameObject flameVisual;
    
    void Start()
    {
        CreateLighterVisual();
    }
    
    void CreateLighterVisual()
    {
        // Создаем корпус зажигалки
        lighterBody = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lighterBody.name = "LighterBody";
        lighterBody.transform.SetParent(transform);
        lighterBody.transform.localPosition = Vector3.zero;
        lighterBody.transform.localScale = new Vector3(0.3f, 0.8f, 0.3f);
        
        // Настраиваем материал корпуса
        Renderer bodyRenderer = lighterBody.GetComponent<Renderer>();
        Material bodyMaterial = new Material(Shader.Find("Standard"));
        bodyMaterial.color = lighterColor;
        bodyRenderer.material = bodyMaterial;
        
        // Создаем крышку зажигалки
        lighterCap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lighterCap.name = "LighterCap";
        lighterCap.transform.SetParent(transform);
        lighterCap.transform.localPosition = new Vector3(0, 0.6f, 0);
        lighterCap.transform.localScale = new Vector3(0.25f, 0.2f, 0.25f);
        
        Renderer capRenderer = lighterCap.GetComponent<Renderer>();
        capRenderer.material = bodyMaterial;
        
        // Создаем колесо зажигалки
        lighterWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lighterWheel.name = "LighterWheel";
        lighterWheel.transform.SetParent(transform);
        lighterWheel.transform.localPosition = new Vector3(0, 0.4f, 0);
        lighterWheel.transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
        lighterWheel.transform.localRotation = Quaternion.Euler(90, 0, 0);
        
        Renderer wheelRenderer = lighterWheel.GetComponent<Renderer>();
        wheelRenderer.material = bodyMaterial;
        
        // Создаем визуальное пламя (используем цилиндр)
        flameVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        flameVisual.name = "FlameVisual";
        flameVisual.transform.SetParent(transform);
        flameVisual.transform.localPosition = new Vector3(0, 1.2f, 0);
        flameVisual.transform.localScale = new Vector3(0.15f, 0.3f, 0.15f);
        
        Renderer flameRenderer = flameVisual.GetComponent<Renderer>();
        Material flameMaterial = new Material(Shader.Find("Standard"));
        flameMaterial.color = flameColor;
        flameRenderer.material = flameMaterial;
        
        // Скрываем пламя по умолчанию
        flameVisual.SetActive(false);
        
        Debug.Log("✅ Простая визуальная модель зажигалки создана!");
    }
    
    public void ShowFlame(bool show)
    {
        if (flameVisual != null)
        {
            flameVisual.SetActive(show);
        }
    }
    
    public void SetFlameIntensity(float intensity)
    {
        if (flameVisual != null)
        {
            flameVisual.transform.localScale = new Vector3(0.15f, 0.3f * intensity, 0.15f);
        }
    }
}

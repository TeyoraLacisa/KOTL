using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Механика зажигалки с топливом, светом и частицами
/// </summary>
public class LighterMechanics : MonoBehaviour
{
    [Header("Lighter Settings")]
    public bool isLit = false;
    public float fuelAmount = 100f; // Максимальное топливо
    public float fuelConsumptionRate = 1f; // Расход топлива в секунду
    public float fuelRefillAmount = 25f; // Количество топлива при заправке
    
    [Header("Light Settings")]
    public Light pointLight;
    public float lightIntensity = 1.5f;
    public float lightRange = 3f;
    public Color lightColor = new Color(1f, 0.6f, 0.2f); // Тёплый цвет
    
    [Header("Particle Settings")]
    public ParticleSystem flameParticles;
    public ParticleSystem sparkParticles;
    
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip igniteSound;
    public AudioClip burnSound;
    public AudioClip extinguishSound;
    
    [Header("UI Settings")]
    public Slider fuelSlider;
    public Text fuelText;
    
    [Header("Amber Interaction")]
    public float amberDetectionRange = 5f;
    public LayerMask amberLayer = 1;
    
    [Header("Visual")]
    public SimpleLighterVisual lighterVisual;
    public FirstPersonLighterAnimation lighterAnimation;
    
    private float currentFuel;
    private bool isInHand = false;
    private Coroutine burnCoroutine;
    private AudioSource burnAudioSource;
    
    public static LighterMechanics Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentFuel = fuelAmount;
        SetupLighter();
        UpdateUI();
    }
    
    void SetupLighter()
    {
        // Настройка света
        if (pointLight == null)
        {
            pointLight = gameObject.AddComponent<Light>();
        }
        
        pointLight.type = LightType.Point;
        pointLight.intensity = 0f;
        pointLight.range = lightRange;
        pointLight.color = lightColor;
        pointLight.enabled = false;
        
        // Настройка частиц пламени
        if (flameParticles == null)
        {
            GameObject flameObj = new GameObject("FlameParticles");
            flameObj.transform.SetParent(transform);
            flameObj.transform.localPosition = Vector3.zero;
            flameParticles = flameObj.AddComponent<ParticleSystem>();
        }
        
        SetupFlameParticles();
        
        // Настройка частиц искр
        if (sparkParticles == null)
        {
            GameObject sparkObj = new GameObject("SparkParticles");
            sparkObj.transform.SetParent(transform);
            sparkObj.transform.localPosition = Vector3.zero;
            sparkParticles = sparkObj.AddComponent<ParticleSystem>();
        }
        
        SetupSparkParticles();
        
        // Настройка аудио
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f;
        
        // Создаем отдельный AudioSource для непрерывного звука горения
        burnAudioSource = gameObject.AddComponent<AudioSource>();
        burnAudioSource.playOnAwake = false;
        burnAudioSource.loop = true;
        burnAudioSource.volume = 0.3f;
        
        Debug.Log("✅ Зажигалка настроена!");
    }
    
    void SetupFlameParticles()
    {
        var main = flameParticles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 0.5f;
        main.startSize = 0.1f;
        main.startColor = new Color(1f, 0.5f, 0f);
        main.maxParticles = 50;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        
        var emission = flameParticles.emission;
        emission.rateOverTime = 0f;
        
        var shape = flameParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.01f;
        
        var velocityOverLifetime = flameParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(1f);
        
        var colorOverLifetime = flameParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        flameParticles.Stop();
    }
    
    void SetupSparkParticles()
    {
        var main = sparkParticles.main;
        main.startLifetime = 0.3f;
        main.startSpeed = 2f;
        main.startSize = 0.05f;
        main.startColor = Color.yellow;
        main.maxParticles = 20;
        
        var emission = sparkParticles.emission;
        emission.rateOverTime = 0f;
        
        var shape = sparkParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.01f;
        
        sparkParticles.Stop();
    }
    
    void Update()
    {
        // Управление зажигалкой
        if (isInHand)
        {
            if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши
            {
                if (isLit)
                {
                    Extinguish();
                }
                else
                {
                    Ignite();
                }
            }
        }
        
        // Проверка взаимодействия с янтарём
        CheckAmberInteraction();
        
        // Обновление UI
        UpdateUI();
    }
    
    public void Ignite()
    {
        if (currentFuel <= 0)
        {
            Debug.Log("❌ Нет топлива!");
            return;
        }
        
        isLit = true;
        
        // Включаем свет
        pointLight.enabled = true;
        pointLight.intensity = lightIntensity;
        
        // Запускаем частицы пламени
        flameParticles.Play();
        
        // Запускаем частицы искр
        sparkParticles.Play();
        
        // Показываем визуальное пламя
        if (lighterVisual != null)
        {
            lighterVisual.ShowFlame(true);
        }
        
        // Запускаем анимацию зажигания
        if (lighterAnimation != null)
        {
            lighterAnimation.PlayIgniteAnimation();
        }
        
        // Воспроизводим звук зажигания
        if (igniteSound != null)
        {
            audioSource.PlayOneShot(igniteSound);
        }
        
        // Запускаем непрерывный звук горения
        if (burnSound != null)
        {
            burnAudioSource.clip = burnSound;
            burnAudioSource.Play();
        }
        
        // Запускаем расход топлива
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }
        burnCoroutine = StartCoroutine(BurnFuel());
        
        Debug.Log("🔥 Зажигалка зажжена!");
    }
    
    public void Extinguish()
    {
        isLit = false;
        
        // Выключаем свет
        pointLight.enabled = false;
        
        // Останавливаем частицы
        flameParticles.Stop();
        sparkParticles.Stop();
        
        // Скрываем визуальное пламя
        if (lighterVisual != null)
        {
            lighterVisual.ShowFlame(false);
        }
        
        // Запускаем анимацию тушения
        if (lighterAnimation != null)
        {
            lighterAnimation.PlayExtinguishAnimation();
        }
        
        // Останавливаем звук горения
        burnAudioSource.Stop();
        
        // Воспроизводим звук тушения
        if (extinguishSound != null)
        {
            audioSource.PlayOneShot(extinguishSound);
        }
        
        // Останавливаем расход топлива
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }
        
        Debug.Log("💨 Зажигалка потушена!");
    }
    
    IEnumerator BurnFuel()
    {
        while (isLit && currentFuel > 0)
        {
            currentFuel -= fuelConsumptionRate * Time.deltaTime;
            currentFuel = Mathf.Max(0, currentFuel);
            
            // Если топливо закончилось, тушим зажигалку
            if (currentFuel <= 0)
            {
                Extinguish();
                Debug.Log("⛽ Топливо закончилось!");
            }
            
            yield return null;
        }
    }
    
    public void RefillFuel()
    {
        currentFuel = Mathf.Min(fuelAmount, currentFuel + fuelRefillAmount);
        Debug.Log($"⛽ Заправлено! Топливо: {currentFuel:F1}%");
    }
    
    void CheckAmberInteraction()
    {
        if (!isLit) return;
        
        // Ищем янтарь в радиусе
        Collider[] ambers = Physics.OverlapSphere(transform.position, amberDetectionRange, amberLayer);
        
        foreach (Collider amber in ambers)
        {
            AmberGlow amberGlow = amber.GetComponent<AmberGlow>();
            if (amberGlow != null)
            {
                amberGlow.StartGlow();
            }
        }
    }
    
    void UpdateUI()
    {
        if (fuelSlider != null)
        {
            fuelSlider.value = currentFuel / fuelAmount;
        }
        
        if (fuelText != null)
        {
            fuelText.text = $"Топливо: {currentFuel:F1}%";
        }
    }
    
    public void SetInHand(bool inHand)
    {
        isInHand = inHand;
        if (!inHand && isLit)
        {
            Extinguish();
        }
    }
    
    public bool HasFuel()
    {
        return currentFuel > 0;
    }
    
    public float GetFuelPercentage()
    {
        return currentFuel / fuelAmount;
    }
    
    void OnDrawGizmosSelected()
    {
        // Показываем радиус обнаружения янтаря
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, amberDetectionRange);
    }
}

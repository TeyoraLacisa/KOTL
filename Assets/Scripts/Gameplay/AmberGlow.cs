using UnityEngine;
using System.Collections;

/// <summary>
/// Механика свечения янтаря при приближении зажигалки
/// </summary>
public class AmberGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    public Light amberLight;
    public float maxGlowIntensity = 2f;
    public float glowFadeSpeed = 2f;
    public Color glowColor = new Color(1f, 0.8f, 0.3f);
    
    [Header("Particle Settings")]
    public ParticleSystem glowParticles;
    
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip glowSound;
    
    private float currentGlowIntensity = 0f;
    private bool isGlowing = false;
    private Coroutine glowCoroutine;
    
    void Start()
    {
        SetupAmber();
    }
    
    void SetupAmber()
    {
        // Настройка света янтаря
        if (amberLight == null)
        {
            amberLight = gameObject.AddComponent<Light>();
        }
        
        amberLight.type = LightType.Point;
        amberLight.intensity = 0f;
        amberLight.range = 4f;
        amberLight.color = glowColor;
        amberLight.enabled = false;
        
        // Настройка частиц свечения
        if (glowParticles == null)
        {
            GameObject particleObj = new GameObject("GlowParticles");
            particleObj.transform.SetParent(transform);
            particleObj.transform.localPosition = Vector3.zero;
            glowParticles = particleObj.AddComponent<ParticleSystem>();
        }
        
        SetupGlowParticles();
        
        // Настройка аудио
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
        
        Debug.Log("✅ Янтарь настроен!");
    }
    
    void SetupGlowParticles()
    {
        var main = glowParticles.main;
        main.startLifetime = 2f;
        main.startSpeed = 0.2f;
        main.startSize = 0.05f;
        main.startColor = glowColor;
        main.maxParticles = 30;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        
        var emission = glowParticles.emission;
        emission.rateOverTime = 0f;
        
        var shape = glowParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;
        
        var velocityOverLifetime = glowParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.5f);
        
        var colorOverLifetime = glowParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(glowColor, 0.0f), new GradientColorKey(Color.clear, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        glowParticles.Stop();
    }
    
    public void StartGlow()
    {
        if (isGlowing) return;
        
        isGlowing = true;
        
        // Включаем свет
        amberLight.enabled = true;
        
        // Запускаем частицы
        glowParticles.Play();
        
        // Воспроизводим звук
        if (glowSound != null)
        {
            audioSource.PlayOneShot(glowSound);
        }
        
        // Запускаем корутину свечения
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }
        glowCoroutine = StartCoroutine(GlowEffect());
        
        Debug.Log("✨ Янтарь начинает светиться!");
    }
    
    public void StopGlow()
    {
        if (!isGlowing) return;
        
        isGlowing = false;
        
        // Останавливаем корутину
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }
        
        // Запускаем затухание
        StartCoroutine(FadeOutGlow());
        
        Debug.Log("🌙 Янтарь перестает светиться!");
    }
    
    IEnumerator GlowEffect()
    {
        while (isGlowing)
        {
            // Плавное изменение интенсивности свечения
            currentGlowIntensity = Mathf.Lerp(currentGlowIntensity, maxGlowIntensity, glowFadeSpeed * Time.deltaTime);
            amberLight.intensity = currentGlowIntensity;
            
            yield return null;
        }
    }
    
    IEnumerator FadeOutGlow()
    {
        while (currentGlowIntensity > 0.1f)
        {
            currentGlowIntensity = Mathf.Lerp(currentGlowIntensity, 0f, glowFadeSpeed * Time.deltaTime);
            amberLight.intensity = currentGlowIntensity;
            
            yield return null;
        }
        
        // Выключаем свет и частицы
        amberLight.enabled = false;
        glowParticles.Stop();
        currentGlowIntensity = 0f;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Проверяем, если зажигалка входит в зону
        LighterMechanics lighter = other.GetComponent<LighterMechanics>();
        if (lighter != null && lighter.isLit)
        {
            StartGlow();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Проверяем, если зажигалка выходит из зоны
        LighterMechanics lighter = other.GetComponent<LighterMechanics>();
        if (lighter != null)
        {
            StopGlow();
        }
    }
}

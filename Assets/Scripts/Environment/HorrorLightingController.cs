using UnityEngine;
using UnityEngine.Rendering;

namespace HorrorGame.Environment
{
    public class HorrorLightingController : MonoBehaviour
    {
        [Header("Lighting References")]
        [SerializeField] private Light mainLight;
        [SerializeField] private Light[] pointLights;
        [SerializeField] private GameObject postProcessVolume; // Изменено на GameObject для совместимости
        
        [Header("Horror Settings")]
        [SerializeField] private bool enableFlickering = true;
        [SerializeField] private float flickerIntensity = 0.3f;
        [SerializeField] private float flickerSpeed = 2f;
        [SerializeField] private Color horrorAmbientColor = new Color(0.1f, 0.1f, 0.2f);
        [SerializeField] private Color horrorFogColor = new Color(0.05f, 0.05f, 0.1f);
        
        [Header("Dynamic Lighting")]
        [SerializeField] private bool enableDynamicShadows = true;
        [SerializeField] private float shadowIntensity = 0.8f;
        [SerializeField] private float shadowDistance = 50f;
        
        private float originalMainLightIntensity;
        private float[] originalPointLightIntensities;
        private bool isFlickering = false;
        
        void Start()
        {
            InitializeLighting();
            SetupHorrorAtmosphere();
        }
        
        void Update()
        {
            if (enableFlickering)
            {
                HandleFlickering();
            }
        }
        
        void InitializeLighting()
        {
            // Store original intensities
            if (mainLight != null)
            {
                originalMainLightIntensity = mainLight.intensity;
            }
            
            originalPointLightIntensities = new float[pointLights.Length];
            for (int i = 0; i < pointLights.Length; i++)
            {
                if (pointLights[i] != null)
                {
                    originalPointLightIntensities[i] = pointLights[i].intensity;
                }
            }
        }
        
        void SetupHorrorAtmosphere()
        {
            // Set ambient lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = horrorAmbientColor;
            RenderSettings.ambientEquatorColor = horrorAmbientColor * 0.5f;
            RenderSettings.ambientGroundColor = Color.black;
            
            // Set fog
            RenderSettings.fog = true;
            RenderSettings.fogColor = horrorFogColor;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.02f;
            
            // Configure main light for horror atmosphere
            if (mainLight != null)
            {
                mainLight.color = new Color(0.8f, 0.9f, 1f); // Cool blue tint
                mainLight.intensity = 0.3f;
                mainLight.shadows = LightShadows.Soft;
                mainLight.shadowStrength = shadowIntensity;
            }
            
            // Configure point lights
            foreach (Light pointLight in pointLights)
            {
                if (pointLight != null)
                {
                    pointLight.color = new Color(1f, 0.6f, 0.3f); // Warm orange
                    pointLight.intensity = 0.2f;
                    pointLight.shadows = LightShadows.Soft;
                    pointLight.shadowStrength = shadowIntensity;
                }
            }
        }
        
        void HandleFlickering()
        {
            if (mainLight != null)
            {
                float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0) * flickerIntensity;
                mainLight.intensity = originalMainLightIntensity + flicker;
            }
            
            // Flicker point lights randomly
            for (int i = 0; i < pointLights.Length; i++)
            {
                if (pointLights[i] != null && Random.value < 0.01f) // 1% chance per frame
                {
                    float flicker = Random.Range(-flickerIntensity, flickerIntensity);
                    pointLights[i].intensity = originalPointLightIntensities[i] + flicker;
                }
            }
        }
        
        public void SetFlickering(bool enabled)
        {
            enableFlickering = enabled;
            
            if (!enabled && mainLight != null)
            {
                mainLight.intensity = originalMainLightIntensity;
            }
        }
        
        public void SetHorrorIntensity(float intensity)
        {
            intensity = Mathf.Clamp01(intensity);
            
            // Adjust ambient lighting
            RenderSettings.ambientSkyColor = Color.Lerp(Color.white, horrorAmbientColor, intensity);
            
            // Adjust fog
            RenderSettings.fogDensity = Mathf.Lerp(0f, 0.02f, intensity);
            
            // Adjust main light
            if (mainLight != null)
            {
                mainLight.intensity = Mathf.Lerp(1f, 0.3f, intensity);
                mainLight.color = Color.Lerp(Color.white, new Color(0.8f, 0.9f, 1f), intensity);
            }
        }
        
        public void TriggerLightFlicker(float duration)
        {
            StartCoroutine(LightFlickerCoroutine(duration));
        }
        
        private System.Collections.IEnumerator LightFlickerCoroutine(float duration)
        {
            float elapsed = 0f;
            bool originalFlickering = enableFlickering;
            enableFlickering = true;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            enableFlickering = originalFlickering;
        }
    }
}
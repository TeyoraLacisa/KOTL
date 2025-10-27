using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HorrorGame.Testing
{
    public class HorrorGameTester : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool autoTestOnStart = false;
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private float testInterval = 1f;
        
        [Header("Test Results")]
        [SerializeField] private TextMeshProUGUI testResultsText;
        [SerializeField] private Image testStatusIndicator;
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color failureColor = Color.red;
        
        private int testsPassed = 0;
        private int totalTests = 0;
        private bool isTesting = false;
        
        void Start()
        {
            if (autoTestOnStart)
            {
                StartTesting();
            }
        }
        
        void Update()
        {
            if (showDebugInfo)
            {
                DisplayDebugInfo();
            }
            
            // Тестовые клавиши
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StartTesting();
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                TestMovement();
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                TestAudio();
            }
            
            if (Input.GetKeyDown(KeyCode.F4))
            {
                TestUI();
            }
            
            if (Input.GetKeyDown(KeyCode.F5))
            {
                TestLighting();
            }
        }
        
        public void StartTesting()
        {
            if (isTesting) return;
            
            isTesting = true;
            testsPassed = 0;
            totalTests = 0;
            
            Debug.Log("🧪 Начинаем тестирование Lighthouse Guard...");
            
            // Запускаем все тесты
            TestMovement();
            TestAudio();
            TestUI();
            TestLighting();
            TestIntegration();
            
            // Результаты
            float successRate = (float)testsPassed / totalTests * 100f;
            Debug.Log($"✅ Тестирование завершено! Успешно: {testsPassed}/{totalTests} ({successRate:F1}%)");
            
            if (testResultsText != null)
            {
                testResultsText.text = $"Тесты: {testsPassed}/{totalTests}\nУспех: {successRate:F1}%";
            }
            
            if (testStatusIndicator != null)
            {
                testStatusIndicator.color = successRate >= 80f ? successColor : failureColor;
            }
            
            isTesting = false;
        }
        
        void TestMovement()
        {
            totalTests++;
            Debug.Log("🎮 Тестируем движение...");
            
            // Проверяем FPS контроллер
            var fpsController = FindObjectOfType<HorrorGame.Player.FPSController>();
            if (fpsController != null)
            {
                Debug.Log("✅ FPS Controller найден");
                testsPassed++;
            }
            else
            {
                Debug.LogError("❌ FPS Controller не найден!");
            }
            
            // Проверяем Character Controller
            var characterController = FindObjectOfType<CharacterController>();
            if (characterController != null)
            {
                Debug.Log("✅ Character Controller найден");
                testsPassed++;
            }
            else
            {
                Debug.LogError("❌ Character Controller не найден!");
            }
            
            // Проверяем камеру
            var camera = FindObjectOfType<Camera>();
            if (camera != null)
            {
                Debug.Log("✅ Камера найдена");
                testsPassed++;
            }
            else
            {
                Debug.LogError("❌ Камера не найдена!");
            }
        }
        
        void TestAudio()
        {
            totalTests++;
            Debug.Log("🔊 Тестируем аудио систему...");
            
            // Проверяем Audio Manager
            var audioManager = FindObjectOfType<HorrorGame.Audio.HorrorAudioManager>();
            if (audioManager != null)
            {
                Debug.Log("✅ Horror Audio Manager найден");
                testsPassed++;
                
                // Тестируем воспроизведение звуков
                try
                {
                    audioManager.PlayFootstepSound();
                    Debug.Log("✅ Звуки воспроизводятся");
                    testsPassed++;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Ошибка воспроизведения звуков: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("❌ Horror Audio Manager не найден!");
            }
            
            // Проверяем Audio Sources
            var audioSources = FindObjectsOfType<AudioSource>();
            if (audioSources.Length > 0)
            {
                Debug.Log($"✅ Найдено {audioSources.Length} Audio Source(ов)");
                testsPassed++;
            }
            else
            {
                Debug.LogError("❌ Audio Sources не найдены!");
            }
        }
        
        void TestUI()
        {
            totalTests++;
            Debug.Log("🖥 Тестируем UI систему...");
            
            // Проверяем UI Manager
            var uiManager = FindObjectOfType<HorrorGame.UI.HorrorUIManager>();
            if (uiManager != null)
            {
                Debug.Log("✅ Horror UI Manager найден");
                testsPassed++;
                
                // Тестируем UI функции
                try
                {
                    uiManager.SetHealth(75f);
                    uiManager.SetSanity(50f);
                    uiManager.ShowBloodEffect(0.3f);
                    Debug.Log("✅ UI функции работают");
                    testsPassed++;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Ошибка UI функций: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("❌ Horror UI Manager не найден!");
            }
            
            // Проверяем Canvas
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Debug.Log("✅ Canvas найден");
                testsPassed++;
            }
            else
            {
                Debug.LogWarning("⚠️ Canvas не найден (может быть нормально)");
            }
        }
        
        void TestLighting()
        {
            totalTests++;
            Debug.Log("💡 Тестируем систему освещения...");
            
            // Проверяем Lighting Controller
            var lightingController = FindObjectOfType<HorrorGame.Environment.HorrorLightingController>();
            if (lightingController != null)
            {
                Debug.Log("✅ Horror Lighting Controller найден");
                testsPassed++;
                
                // Тестируем функции освещения
                try
                {
                    lightingController.SetHorrorIntensity(0.5f);
                    lightingController.SetFlickering(true);
                    Debug.Log("✅ Функции освещения работают");
                    testsPassed++;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Ошибка функций освещения: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("❌ Horror Lighting Controller не найден!");
            }
            
            // Проверяем источники света
            var lights = FindObjectsOfType<Light>();
            if (lights.Length > 0)
            {
                Debug.Log($"✅ Найдено {lights.Length} источник(ов) света");
                testsPassed++;
            }
            else
            {
                Debug.LogWarning("⚠️ Источники света не найдены");
            }
        }
        
        void TestIntegration()
        {
            totalTests++;
            Debug.Log("🔗 Тестируем интеграцию систем...");
            
            // Проверяем, что все менеджеры доступны
            bool allManagersFound = true;
            
            if (FindObjectOfType<HorrorGame.Player.FPSController>() == null)
            {
                Debug.LogError("❌ FPS Controller не найден для интеграции");
                allManagersFound = false;
            }
            
            if (FindObjectOfType<HorrorGame.Audio.HorrorAudioManager>() == null)
            {
                Debug.LogError("❌ Audio Manager не найден для интеграции");
                allManagersFound = false;
            }
            
            if (FindObjectOfType<HorrorGame.UI.HorrorUIManager>() == null)
            {
                Debug.LogError("❌ UI Manager не найден для интеграции");
                allManagersFound = false;
            }
            
            if (FindObjectOfType<HorrorGame.Environment.HorrorLightingController>() == null)
            {
                Debug.LogError("❌ Lighting Controller не найден для интеграции");
                allManagersFound = false;
            }
            
            if (allManagersFound)
            {
                Debug.Log("✅ Все системы интегрированы");
                testsPassed++;
            }
        }
        
        void DisplayDebugInfo()
        {
            if (testResultsText != null)
            {
                string debugInfo = $"FPS: {(1.0f / Time.deltaTime):F0}\n";
                debugInfo += $"Позиция: {transform.position}\n";
                debugInfo += $"Время: {Time.time:F1}s\n";
                debugInfo += $"Тесты: {testsPassed}/{totalTests}";
                
                testResultsText.text = debugInfo;
            }
        }
        
        // Публичные методы для UI кнопок
        public void RunAllTests()
        {
            StartTesting();
        }
        
        public void TestMovementOnly()
        {
            TestMovement();
        }
        
        public void TestAudioOnly()
        {
            TestAudio();
        }
        
        public void TestUIOnly()
        {
            TestUI();
        }
        
        public void TestLightingOnly()
        {
            TestLighting();
        }
        
        // Метод для сброса тестов
        public void ResetTests()
        {
            testsPassed = 0;
            totalTests = 0;
            
            if (testResultsText != null)
            {
                testResultsText.text = "Тесты сброшены";
            }
            
            if (testStatusIndicator != null)
            {
                testStatusIndicator.color = Color.white;
            }
            
            Debug.Log("🔄 Тесты сброшены");
        }
    }
}
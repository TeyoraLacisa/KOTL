using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HorrorGame.UI
{
    public class HorrorUIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject crosshair;
        
        [Header("Health UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color criticalColor = Color.red;
        
        [Header("Sanity UI")]
        [SerializeField] private Slider sanityBar;
        [SerializeField] private TextMeshProUGUI sanityText;
        [SerializeField] private Image sanityBarFill;
        [SerializeField] private Color saneColor = Color.blue;
        [SerializeField] private Color insaneColor = new Color(0.5f, 0f, 0.5f); // Purple color
        
        [Header("Interaction UI")]
        [SerializeField] private TextMeshProUGUI interactionText;
        [SerializeField] private GameObject interactionPrompt;
        
        [Header("Horror Effects")]
        [SerializeField] private Image bloodOverlay;
        [SerializeField] private Image staticOverlay;
        [SerializeField] private float staticIntensity = 0.1f;
        [SerializeField] private float bloodFadeSpeed = 2f;
        
        [Header("Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        
        private bool isPaused = false;
        private bool isMenuOpen = false;
        private float currentHealth = 100f;
        private float currentSanity = 100f;
        private float bloodAlpha = 0f;
        
        public static HorrorUIManager Instance { get; private set; }
        
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
            InitializeUI();
            SetupSettings();
        }
        
        void Update()
        {
            HandleInput();
            UpdateHealthUI();
            UpdateSanityUI();
            UpdateHorrorEffects();
        }
        
        void InitializeUI()
        {
            // Hide all panels initially
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            
            // Show crosshair
            if (crosshair != null) crosshair.SetActive(true);
            
            // Initialize health and sanity
            currentHealth = 100f;
            currentSanity = 100f;
        }
        
        void SetupSettings()
        {
            // Setup volume sliders
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }
            
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            }
            
            // Setup fullscreen toggle
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }
            
            // Setup quality dropdown
            if (qualityDropdown != null)
            {
                qualityDropdown.value = QualitySettings.GetQualityLevel();
                qualityDropdown.onValueChanged.AddListener(SetQuality);
            }
        }
        
        void HandleInput()
        {
            // Pause menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
        
        void UpdateHealthUI()
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / 100f;
            }
            
            if (healthText != null)
            {
                healthText.text = Mathf.RoundToInt(currentHealth).ToString();
            }
            
            if (healthBarFill != null)
            {
                healthBarFill.color = Color.Lerp(criticalColor, healthyColor, currentHealth / 100f);
            }
        }
        
        void UpdateSanityUI()
        {
            if (sanityBar != null)
            {
                sanityBar.value = currentSanity / 100f;
            }
            
            if (sanityText != null)
            {
                sanityText.text = Mathf.RoundToInt(currentSanity).ToString();
            }
            
            if (sanityBarFill != null)
            {
                sanityBarFill.color = Color.Lerp(insaneColor, saneColor, currentSanity / 100f);
            }
        }
        
        void UpdateHorrorEffects()
        {
            // Blood overlay effect
            if (bloodOverlay != null)
            {
                Color bloodColor = bloodOverlay.color;
                bloodColor.a = Mathf.Lerp(bloodColor.a, bloodAlpha, bloodFadeSpeed * Time.deltaTime);
                bloodOverlay.color = bloodColor;
            }
            
            // Static overlay effect
            if (staticOverlay != null && currentSanity < 30f)
            {
                staticOverlay.gameObject.SetActive(true);
                Color staticColor = staticOverlay.color;
                staticColor.a = staticIntensity * (1f - currentSanity / 30f);
                staticOverlay.color = staticColor;
            }
            else if (staticOverlay != null)
            {
                staticOverlay.gameObject.SetActive(false);
            }
        }
        
        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }
        
        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }
        
        public void ShowMainMenu()
        {
            isMenuOpen = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
        }
        
        public void HideMainMenu()
        {
            isMenuOpen = false;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
        }
        
        public void ShowSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }
        
        public void HideSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }
        
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        public void SetHealth(float health)
        {
            currentHealth = Mathf.Clamp(health, 0f, 100f);
        }
        
        public void SetSanity(float sanity)
        {
            currentSanity = Mathf.Clamp(sanity, 0f, 100f);
        }
        
        public void ShowBloodEffect(float intensity)
        {
            bloodAlpha = Mathf.Clamp01(intensity);
        }
        
        public void ShowInteractionPrompt(string text)
        {
            if (interactionText != null)
            {
                interactionText.text = text;
            }
            
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
        
        public void HideInteractionPrompt()
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
        
        public void SetMasterVolume(float volume)
        {
            PlayerPrefs.SetFloat("MasterVolume", volume);
            // Apply to audio manager if available
            if (HorrorGame.Audio.HorrorAudioManager.Instance != null)
            {
                HorrorGame.Audio.HorrorAudioManager.Instance.SetMasterVolume(volume);
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
            if (HorrorGame.Audio.HorrorAudioManager.Instance != null)
            {
                HorrorGame.Audio.HorrorAudioManager.Instance.SetMusicVolume(volume);
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            PlayerPrefs.SetFloat("SFXVolume", volume);
            if (HorrorGame.Audio.HorrorAudioManager.Instance != null)
            {
                HorrorGame.Audio.HorrorAudioManager.Instance.SetSFXVolume(volume);
            }
        }
        
        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }
        
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
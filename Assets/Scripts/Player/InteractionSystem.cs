using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HorrorGame.Gameplay;

namespace HorrorGame.Player
{
    /// <summary>
    /// Система взаимодействия с объектами в хоррор-игре
    /// Использует Raycast для обнаружения объектов с тегом "Interactable"
    /// Показывает UI подсказку "Нажми E" при наведении
    /// </summary>
    public class InteractionSystem : MonoBehaviour
    {
        [Header("=== НАСТРОЙКИ ВЗАИМОДЕЙСТВИЯ ===")]
        [SerializeField] private float interactionRange = 3f; // Дистанция взаимодействия
        [SerializeField] private LayerMask interactionLayerMask = -1; // Слои для взаимодействия
        [SerializeField] private KeyCode interactionKey = KeyCode.E; // Клавиша взаимодействия
        
        [Header("=== UI ЭЛЕМЕНТЫ ===")]
        [SerializeField] private GameObject interactionUI; // UI панель с подсказкой
        [SerializeField] private Text interactionText; // Текст подсказки
        [SerializeField] private string interactionPrompt = "Нажми E"; // Текст подсказки
        [SerializeField] private bool autoCreateUI = true; // Автоматически создавать UI
        
        [Header("=== ССЫЛКИ ===")]
        [SerializeField] private Camera playerCamera; // Камера игрока
        [SerializeField] private Transform playerTransform; // Трансформ игрока
        
        [Header("=== СОБЫТИЯ ===")]
        [SerializeField] private UnityEvent OnInteractionStart; // Событие начала взаимодействия
        [SerializeField] private UnityEvent OnInteractionEnd; // Событие окончания взаимодействия
        
        // Приватные переменные
        private GameObject currentInteractable; // Текущий объект для взаимодействия
        private bool isInteracting = false; // Флаг взаимодействия
        private bool canInteract = true; // Флаг возможности взаимодействия
        
        void Start()
        {
            // Получаем ссылки на компоненты
            GetReferences();
            
            // Настраиваем UI
            SetupUI();
            
            Debug.Log("InteractionSystem: Система взаимодействия инициализирована!");
        }
        
        void Update()
        {
            // Проверяем возможность взаимодействия
            if (!canInteract) return;
            
            // Проверяем нажатие клавиши взаимодействия
            CheckInteractionInput();
            
            // Обновляем UI подсказки
            UpdateInteractionUI();
        }
        
        /// <summary>
        /// Получает ссылки на необходимые компоненты
        /// </summary>
        void GetReferences()
        {
            // Получаем камеру игрока
            if (playerCamera == null)
                playerCamera = GetComponentInChildren<Camera>();
            
            // Получаем трансформ игрока
            if (playerTransform == null)
                playerTransform = transform;
            
            // Если камера не найдена, ищем в дочерних объектах
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
                if (playerCamera != null)
                {
                    Debug.Log("InteractionSystem: Камера найдена автоматически");
                }
            }
        }
        
        /// <summary>
        /// Настраивает UI элементы
        /// </summary>
        void SetupUI()
        {
            // Если UI не назначен и включено автосоздание
            if (interactionUI == null && autoCreateUI)
            {
                CreateSimpleUI();
            }
            
            // Скрываем UI по умолчанию
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
        
        /// <summary>
        /// Создает простой UI для подсказки
        /// </summary>
        void CreateSimpleUI()
        {
            // Создаем Canvas
            GameObject canvasGO = new GameObject("InteractionCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            // Добавляем CanvasScaler для адаптивности
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Добавляем GraphicRaycaster
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Создаем панель для подсказки
            GameObject panelGO = new GameObject("InteractionPanel");
            panelGO.transform.SetParent(canvasGO.transform, false);
            
            RectTransform panelRect = panelGO.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.1f);
            panelRect.anchorMax = new Vector2(0.5f, 0.1f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(300, 60);
            
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            
            // Создаем текст подсказки
            GameObject textGO = new GameObject("InteractionText");
            textGO.transform.SetParent(panelGO.transform, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            interactionText = textGO.AddComponent<Text>();
            interactionText.text = interactionPrompt;
            interactionText.fontSize = 20;
            interactionText.color = Color.white;
            interactionText.alignment = TextAnchor.MiddleCenter;
            interactionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            interactionUI = panelGO;
            
            Debug.Log("InteractionSystem: UI создан автоматически!");
        }
        
        /// <summary>
        /// Проверяет нажатие клавиши взаимодействия
        /// </summary>
        void CheckInteractionInput()
        {
            if (Input.GetKeyDown(interactionKey))
            {
                if (currentInteractable != null && !isInteracting)
                {
                    StartInteraction();
                }
            }
        }
        
        /// <summary>
        /// Обновляет UI подсказки взаимодействия
        /// </summary>
        void UpdateInteractionUI()
        {
            // Выполняем Raycast для поиска объектов взаимодействия
            GameObject newInteractable = FindInteractableObject();
            
            // Если найден новый объект для взаимодействия
            if (newInteractable != null && newInteractable != currentInteractable)
            {
                currentInteractable = newInteractable;
                ShowInteractionUI();
            }
            // Если объект исчез из зоны взаимодействия
            else if (newInteractable == null && currentInteractable != null)
            {
                currentInteractable = null;
                HideInteractionUI();
            }
        }
        
        /// <summary>
        /// Находит объект для взаимодействия с помощью Raycast
        /// </summary>
        /// <returns>Объект для взаимодействия или null</returns>
        GameObject FindInteractableObject()
        {
            if (playerCamera == null) return null;
            
            // Создаем луч от камеры вперед
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            
            // Выполняем Raycast
            if (Physics.Raycast(ray, out hit, interactionRange, interactionLayerMask))
            {
                // Проверяем, имеет ли объект тег "Interactable"
                if (hit.collider.CompareTag("Interactable"))
                {
                    return hit.collider.gameObject;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Показывает UI подсказки взаимодействия
        /// </summary>
        void ShowInteractionUI()
        {
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
                
                // Обновляем текст подсказки
                if (interactionText != null)
                {
                    interactionText.text = interactionPrompt;
                }
            }
        }
        
        /// <summary>
        /// Скрывает UI подсказки взаимодействия
        /// </summary>
        void HideInteractionUI()
        {
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
        
        /// <summary>
        /// Начинает взаимодействие с объектом
        /// </summary>
        void StartInteraction()
        {
            if (currentInteractable == null) return;
            
            isInteracting = true;
            
            // Вызываем событие начала взаимодействия
            OnInteractionStart?.Invoke();
            
            // Вызываем метод Interact() у объекта
            IInteractable interactable = currentInteractable.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
            else
            {
                // Если у объекта нет интерфейса IInteractable, вызываем событие
                currentInteractable.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
            }
            
            Debug.Log($"InteractionSystem: Взаимодействие с {currentInteractable.name} начато");
        }
        
        /// <summary>
        /// Завершает взаимодействие с объектом
        /// </summary>
        public void EndInteraction()
        {
            if (!isInteracting) return;
            
            isInteracting = false;
            
            // Вызываем событие окончания взаимодействия
            OnInteractionEnd?.Invoke();
            
            Debug.Log("InteractionSystem: Взаимодействие завершено");
        }
        
        /// <summary>
        /// Включает/выключает возможность взаимодействия
        /// </summary>
        /// <param name="enabled">Включить взаимодействие</param>
        public void SetInteractionEnabled(bool enabled)
        {
            canInteract = enabled;
            
            if (!enabled)
            {
                HideInteractionUI();
                currentInteractable = null;
            }
            
            Debug.Log($"InteractionSystem: Взаимодействие {(enabled ? "включено" : "выключено")}");
        }
        
        /// <summary>
        /// Устанавливает дистанцию взаимодействия
        /// </summary>
        /// <param name="range">Новая дистанция</param>
        public void SetInteractionRange(float range)
        {
            interactionRange = Mathf.Max(0.1f, range);
        }
        
        /// <summary>
        /// Устанавливает текст подсказки
        /// </summary>
        /// <param name="text">Новый текст подсказки</param>
        public void SetInteractionPrompt(string text)
        {
            interactionPrompt = text;
            
            if (interactionText != null)
            {
                interactionText.text = interactionPrompt;
            }
        }
        
        // Методы для получения информации
        public bool IsInteracting() => isInteracting;
        public bool CanInteract() => canInteract;
        public GameObject GetCurrentInteractable() => currentInteractable;
        public float GetInteractionRange() => interactionRange;
        
        // Отладочная информация
        void OnGUI()
        {
            if (!canInteract) return;
            
            GUILayout.BeginArea(new Rect(10, 200, 300, 100));
            GUILayout.Label("=== Interaction System Debug ===");
            GUILayout.Label($"Дистанция: {interactionRange:F1}");
            GUILayout.Label($"Объект: {(currentInteractable != null ? currentInteractable.name : "Нет")}");
            GUILayout.Label($"Взаимодействие: {(isInteracting ? "ДА" : "НЕТ")}");
            GUILayout.EndArea();
        }
        
        // Визуализация луча в Scene View
        void OnDrawGizmos()
        {
            if (playerCamera == null) return;
            
            // Рисуем луч взаимодействия
            Gizmos.color = Color.green;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange);
            
            // Рисуем сферу в конце луча
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerCamera.transform.position + playerCamera.transform.forward * interactionRange, 0.1f);
        }
    }
}

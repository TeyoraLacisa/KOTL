using UnityEngine;

namespace HorrorGame.Gameplay
{
    /// <summary>
    /// Пример интерактивного объекта для хоррор-игры
    /// Показывает, как реализовать интерфейс IInteractable
    /// </summary>
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [Header("=== НАСТРОЙКИ ОБЪЕКТА ===")]
        [SerializeField] private string objectName = "Интерактивный объект"; // Название объекта
        [SerializeField] private string interactionMessage = "Вы взаимодействуете с объектом!"; // Сообщение при взаимодействии
        [SerializeField] private bool canInteractMultipleTimes = true; // Можно ли взаимодействовать несколько раз
        [SerializeField] private float interactionCooldown = 1f; // Кулдаун между взаимодействиями
        
        [Header("=== ЭФФЕКТЫ ===")]
        [SerializeField] private AudioClip interactionSound; // Звук взаимодействия
        [SerializeField] private ParticleSystem interactionEffect; // Эффект взаимодействия
        [SerializeField] private bool destroyAfterInteraction = false; // Уничтожить после взаимодействия
        
        [Header("=== ССЫЛКИ ===")]
        [SerializeField] private AudioSource audioSource; // Источник звука
        [SerializeField] private Renderer objectRenderer; // Рендерер объекта
        
        // Приватные переменные
        private bool hasInteracted = false; // Флаг взаимодействия
        private float lastInteractionTime = 0f; // Время последнего взаимодействия
        private Color originalColor; // Исходный цвет объекта
        
        void Start()
        {
            // Получаем ссылки на компоненты
            GetReferences();
            
            // Сохраняем исходный цвет объекта
            if (objectRenderer != null)
            {
                originalColor = objectRenderer.material.color;
            }
            
            Debug.Log($"InteractableObject: {objectName} инициализирован");
        }
        
        /// <summary>
        /// Получает ссылки на необходимые компоненты
        /// </summary>
        void GetReferences()
        {
            // Получаем AudioSource
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            
            // Получаем Renderer
            if (objectRenderer == null)
                objectRenderer = GetComponent<Renderer>();
            
            // Если AudioSource не найден, создаем его
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // 3D звук
            }
        }
        
        /// <summary>
        /// Основной метод взаимодействия
        /// Вызывается системой взаимодействия при нажатии E
        /// </summary>
        public void Interact()
        {
            // Проверяем кулдаун
            if (Time.time - lastInteractionTime < interactionCooldown)
            {
                Debug.Log($"{objectName}: Слишком рано для повторного взаимодействия!");
                return;
            }
            
            // Проверяем, можно ли взаимодействовать
            if (!canInteractMultipleTimes && hasInteracted)
            {
                Debug.Log($"{objectName}: Уже взаимодействовали с этим объектом!");
                return;
            }
            
            // Выполняем взаимодействие
            PerformInteraction();
            
            // Обновляем состояние
            hasInteracted = true;
            lastInteractionTime = Time.time;
            
            Debug.Log($"{objectName}: Взаимодействие выполнено");
        }
        
        /// <summary>
        /// Выполняет логику взаимодействия
        /// </summary>
        void PerformInteraction()
        {
            // Показываем сообщение
            ShowInteractionMessage();
            
            // Воспроизводим звук
            PlayInteractionSound();
            
            // Запускаем эффект
            PlayInteractionEffect();
            
            // Изменяем внешний вид объекта
            ChangeObjectAppearance();
            
            // Выполняем специфичную логику
            OnInteractionPerformed();
        }
        
        /// <summary>
        /// Показывает сообщение о взаимодействии
        /// </summary>
        void ShowInteractionMessage()
        {
            if (!string.IsNullOrEmpty(interactionMessage))
            {
                Debug.Log($"[{objectName}] {interactionMessage}");
                
                // Здесь можно добавить UI для показа сообщения
                // Например, через UIManager или EventSystem
            }
        }
        
        /// <summary>
        /// Воспроизводит звук взаимодействия
        /// </summary>
        void PlayInteractionSound()
        {
            if (audioSource != null && interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound);
            }
        }
        
        /// <summary>
        /// Запускает эффект взаимодействия
        /// </summary>
        void PlayInteractionEffect()
        {
            if (interactionEffect != null)
            {
                interactionEffect.Play();
            }
        }
        
        /// <summary>
        /// Изменяет внешний вид объекта
        /// </summary>
        void ChangeObjectAppearance()
        {
            if (objectRenderer != null)
            {
                // Временно меняем цвет на красный
                objectRenderer.material.color = Color.red;
                
                // Возвращаем исходный цвет через 0.5 секунды
                Invoke(nameof(ResetObjectColor), 0.5f);
            }
        }
        
        /// <summary>
        /// Возвращает исходный цвет объекта
        /// </summary>
        void ResetObjectColor()
        {
            if (objectRenderer != null)
            {
                objectRenderer.material.color = originalColor;
            }
        }
        
        /// <summary>
        /// Выполняет специфичную логику взаимодействия
        /// Переопределите этот метод в наследниках
        /// </summary>
        protected virtual void OnInteractionPerformed()
        {
            // Базовая логика - ничего не делаем
            // В наследниках можно добавить специфичную логику
        }
        
        /// <summary>
        /// Уничтожает объект после взаимодействия
        /// </summary>
        void DestroyAfterInteraction()
        {
            if (destroyAfterInteraction)
            {
                Debug.Log($"{objectName}: Объект уничтожен после взаимодействия");
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Устанавливает название объекта
        /// </summary>
        /// <param name="name">Новое название</param>
        public void SetObjectName(string name)
        {
            objectName = name;
        }
        
        /// <summary>
        /// Устанавливает сообщение взаимодействия
        /// </summary>
        /// <param name="message">Новое сообщение</param>
        public void SetInteractionMessage(string message)
        {
            interactionMessage = message;
        }
        
        /// <summary>
        /// Устанавливает возможность многократного взаимодействия
        /// </summary>
        /// <param name="canInteract">Можно ли взаимодействовать несколько раз</param>
        public void SetCanInteractMultipleTimes(bool canInteract)
        {
            canInteractMultipleTimes = canInteract;
        }
        
        /// <summary>
        /// Устанавливает кулдаун взаимодействия
        /// </summary>
        /// <param name="cooldown">Новый кулдаун в секундах</param>
        public void SetInteractionCooldown(float cooldown)
        {
            interactionCooldown = Mathf.Max(0f, cooldown);
        }
        
        /// <summary>
        /// Сбрасывает состояние взаимодействия
        /// </summary>
        public void ResetInteractionState()
        {
            hasInteracted = false;
            lastInteractionTime = 0f;
            Debug.Log($"{objectName}: Состояние взаимодействия сброшено");
        }
        
        // Методы для получения информации
        public string GetObjectName() => objectName;
        public bool HasInteracted() => hasInteracted;
        public bool CanInteractNow() => Time.time - lastInteractionTime >= interactionCooldown;
        public float GetCooldownTimeLeft() => Mathf.Max(0f, interactionCooldown - (Time.time - lastInteractionTime));
        
        // Отладочная информация
        void OnGUI()
        {
            if (Camera.main == null) return;
            
            // Показываем информацию об объекте, если он в поле зрения
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (screenPos.z > 0)
            {
                GUILayout.BeginArea(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 50, 100, 100));
                GUILayout.Label(objectName);
                GUILayout.Label($"Взаимодействовали: {(hasInteracted ? "ДА" : "НЕТ")}");
                if (!CanInteractNow())
                {
                    GUILayout.Label($"Кулдаун: {GetCooldownTimeLeft():F1}с");
                }
                GUILayout.EndArea();
            }
        }
    }
}

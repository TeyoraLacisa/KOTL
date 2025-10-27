using UnityEngine;

namespace HorrorGame.Player
{
    /// <summary>
    /// Красивое покачивание камеры с защитой от читов
    /// Работает только при движении по земле
    /// </summary>
    public class HeadBob : MonoBehaviour
    {
        [Header("=== НАСТРОЙКИ ПОКАЧИВАНИЯ ===")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private float walkSpeed = 2.2f;
        [SerializeField] private float runSpeed = 3.5f;
        [SerializeField] private float walkVerticalAmount = 0.04f;
        [SerializeField] private float runVerticalAmount = 0.08f;
        [SerializeField] private float walkHorizontalAmount = 0.02f;
        [SerializeField] private float runHorizontalAmount = 0.04f;
        [SerializeField] private float smoothing = 12f;
        
        [Header("=== ЗАЩИТА ОТ ЧИТОВ ===")]
        [SerializeField] private float minSpeedThreshold = 0.1f;
        [SerializeField] private float maxSpeedThreshold = 20f;
        [SerializeField] private bool requireGrounded = true;
        [SerializeField] private float maxBobAmount = 0.2f;
        
        [Header("=== ОГРАНИЧЕНИЕ УГЛОВ ===")]
        [SerializeField] private bool enableAngleLimits = true;
        [SerializeField] private float maxLookUpAngle = 80f;
        [SerializeField] private float maxLookDownAngle = -80f;
        [SerializeField] private float angleSmoothing = 10f;
        
        [Header("=== УПРАВЛЕНИЕ ===")]
        [SerializeField] private bool enableInputControl = true;
        [SerializeField] private bool isInputBlocked = false;
        [SerializeField] private float inputBlockDuration = 0f;
        [SerializeField] private bool showInputDebug = true;
        
        [Header("=== ССЫЛКИ ===")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private HorrorGame.Player.FPSController fpsController;
        
        // Приватные переменные
        private Vector3 originalCameraPosition;
        private Vector3 targetPosition;
        private float timer = 0f;
        private bool isMoving = false;
        private bool isRunning = false;
        private bool isGrounded = false;
        private float currentSpeed = 0f;
        
        // Защита от читов
        private float lastValidSpeed = 0f;
        private Vector3 lastValidPosition;
        private float cheatDetectionTimer = 0f;
        
        // Ограничение углов
        private float currentVerticalAngle = 0f;
        private float targetVerticalAngle = 0f;
        
        // Управление
        private float inputBlockTimer = 0f;
        private bool wasInputBlocked = false;
        
        void Start()
        {
            // Получаем ссылки на компоненты
            GetReferences();
            
            // Сохраняем оригинальную позицию
            if (cameraTransform != null)
            {
                originalCameraPosition = cameraTransform.localPosition;
                targetPosition = originalCameraPosition;
                lastValidPosition = originalCameraPosition;
            }
            
            Debug.Log("HeadBob: Инициализирован с защитой от читов!");
        }
        
        void Update()
        {
            if (!enableHeadBob || cameraTransform == null) return;
            
            // Проверяем блокировку ввода
            CheckInputBlock();
            
            // Проверяем движение и безопасность
            CheckMovement();
            CheckGrounded();
            ValidateSpeed();
            
            // Применяем покачивание только если всё безопасно
            if (IsSafeToBob())
            {
                ApplyHeadBob();
            }
            else
            {
                ReturnToOriginalPosition();
            }
            
            // Ограничиваем вертикальный угол
            if (enableAngleLimits)
            {
                LimitVerticalAngle();
            }
            
            // Плавно применяем изменения
            ApplySmoothMovement();
        }
        
        void GetReferences()
        {
            if (cameraTransform == null)
                cameraTransform = GetComponentInChildren<Camera>()?.transform;
                
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
                
            if (fpsController == null)
                fpsController = GetComponent<HorrorGame.Player.FPSController>();
        }
        
        void CheckInputBlock()
        {
            if (!enableInputControl) return;
            
            // Если ввод заблокирован, уменьшаем таймер
            if (isInputBlocked)
            {
                inputBlockTimer -= Time.deltaTime;
                
                // Если время истекло, разблокируем ввод
                if (inputBlockTimer <= 0f)
                {
                    isInputBlocked = false;
                    inputBlockTimer = 0f;
                    
                    if (showInputDebug)
                    {
                        Debug.Log("HeadBob: Ввод разблокирован!");
                    }
                }
            }
            
            // Сохраняем состояние для отслеживания изменений
            wasInputBlocked = isInputBlocked;
        }
        
        void CheckMovement()
        {
            if (characterController == null) return;
            
            // Если ввод заблокирован, не проверяем движение
            if (isInputBlocked)
            {
                isMoving = false;
                isRunning = false;
                currentSpeed = 0f;
                return;
            }
            
            // Получаем скорость движения
            Vector3 velocity = characterController.velocity;
            velocity.y = 0; // Игнорируем вертикальную скорость
            
            currentSpeed = velocity.magnitude;
            isMoving = currentSpeed > minSpeedThreshold;
            
            // Проверяем бег
            if (fpsController != null)
            {
                isRunning = fpsController.IsRunning();
            }
        }
        
        void CheckGrounded()
        {
            if (characterController == null) return;
            
            isGrounded = characterController.isGrounded;
        }
        
        void ValidateSpeed()
        {
            // Защита от слишком высокой скорости (читы)
            if (currentSpeed > maxSpeedThreshold)
            {
                Debug.LogWarning($"HeadBob: Обнаружена подозрительно высокая скорость: {currentSpeed:F2}. Блокируем покачивание.");
                currentSpeed = lastValidSpeed;
                isMoving = false;
            }
            
            // Сохраняем последнюю валидную скорость
            if (currentSpeed <= maxSpeedThreshold)
            {
                lastValidSpeed = currentSpeed;
            }
        }
        
        bool IsSafeToBob()
        {
            // Проверяем все условия безопасности
            if (!isMoving) return false;
            if (requireGrounded && !isGrounded) return false;
            if (currentSpeed > maxSpeedThreshold) return false;
            if (cameraTransform == null) return false;
            
            return true;
        }
        
        void ApplyHeadBob()
        {
            // Выбираем параметры в зависимости от бега
            float bobSpeed = isRunning ? runSpeed : walkSpeed;
            float verticalAmount = isRunning ? runVerticalAmount : walkVerticalAmount;
            float horizontalAmount = isRunning ? runHorizontalAmount : walkHorizontalAmount;
            
            // Ограничиваем максимальную амплитуду (защита от читов)
            verticalAmount = Mathf.Clamp(verticalAmount, 0f, maxBobAmount);
            horizontalAmount = Mathf.Clamp(horizontalAmount, 0f, maxBobAmount);
            
            // Увеличиваем таймер
            timer += Time.deltaTime * bobSpeed;
            
            // Создаем красивое покачивание
            float verticalBob = Mathf.Sin(timer) * verticalAmount;
            float horizontalBob = Mathf.Sin(timer * 0.5f) * horizontalAmount;
            
            // Применяем к целевой позиции
            targetPosition = originalCameraPosition + new Vector3(horizontalBob, verticalBob, 0);
            
            // Сохраняем валидную позицию
            lastValidPosition = targetPosition;
        }
        
        void ReturnToOriginalPosition()
        {
            // Плавно возвращаем камеру в исходное положение
            targetPosition = originalCameraPosition;
            
            // Сбрасываем таймер
            timer = 0f;
        }
        
        void ApplySmoothMovement()
        {
            // Плавно перемещаем камеру к целевой позиции
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPosition,
                Time.deltaTime * smoothing
            );
        }
        
        void LimitVerticalAngle()
        {
            if (cameraTransform == null) return;
            
            // Получаем текущий вертикальный угол
            float currentAngle = cameraTransform.localEulerAngles.x;
            
            // Конвертируем в диапазон -180 до 180
            if (currentAngle > 180f)
                currentAngle -= 360f;
            
            // Ограничиваем угол
            targetVerticalAngle = Mathf.Clamp(currentAngle, maxLookDownAngle, maxLookUpAngle);
            
            // Плавно применяем ограничение
            currentVerticalAngle = Mathf.Lerp(currentVerticalAngle, targetVerticalAngle, Time.deltaTime * angleSmoothing);
            
            // Применяем к камере
            Vector3 currentRotation = cameraTransform.localEulerAngles;
            currentRotation.x = currentVerticalAngle;
            cameraTransform.localEulerAngles = currentRotation;
        }
        
        // Публичные методы для настройки
        public void SetHeadBobEnabled(bool enabled)
        {
            enableHeadBob = enabled;
        }
        
        public void SetBobAmounts(float walkVertical, float runVertical, float walkHorizontal, float runHorizontal)
        {
            walkVerticalAmount = Mathf.Clamp(walkVertical, 0f, maxBobAmount);
            runVerticalAmount = Mathf.Clamp(runVertical, 0f, maxBobAmount);
            walkHorizontalAmount = Mathf.Clamp(walkHorizontal, 0f, maxBobAmount);
            runHorizontalAmount = Mathf.Clamp(runHorizontal, 0f, maxBobAmount);
        }
        
        public void SetBobSpeeds(float walk, float run)
        {
            walkSpeed = Mathf.Clamp(walk, 0.1f, 10f);
            runSpeed = Mathf.Clamp(run, 0.1f, 10f);
        }
        
        public void SetSmoothing(float smooth)
        {
            smoothing = Mathf.Clamp(smooth, 1f, 50f);
        }
        
        public void SetCheatProtection(bool requireGround, float maxSpeed, float maxBob)
        {
            requireGrounded = requireGround;
            maxSpeedThreshold = Mathf.Clamp(maxSpeed, 5f, 100f);
            maxBobAmount = Mathf.Clamp(maxBob, 0.01f, 1f);
        }
        
        public void SetAngleLimits(bool enabled, float maxUp, float maxDown, float smooth)
        {
            enableAngleLimits = enabled;
            maxLookUpAngle = Mathf.Clamp(maxUp, 0f, 90f);
            maxLookDownAngle = Mathf.Clamp(maxDown, -90f, 0f);
            angleSmoothing = Mathf.Clamp(smooth, 1f, 50f);
        }
        
        // Методы для управления вводом
        public void BlockInput(float duration = 0f)
        {
            if (!enableInputControl) return;
            
            isInputBlocked = true;
            inputBlockTimer = duration > 0f ? duration : inputBlockDuration;
            
            if (showInputDebug)
            {
                Debug.Log($"HeadBob: Ввод заблокирован на {inputBlockTimer:F1} секунд");
            }
        }
        
        public void UnblockInput()
        {
            if (!enableInputControl) return;
            
            isInputBlocked = false;
            inputBlockTimer = 0f;
            
            if (showInputDebug)
            {
                Debug.Log("HeadBob: Ввод разблокирован вручную");
            }
        }
        
        public void SetInputControl(bool enabled)
        {
            enableInputControl = enabled;
            
            if (!enabled)
            {
                isInputBlocked = false;
                inputBlockTimer = 0f;
            }
        }
        
        public void SetInputBlockDuration(float duration)
        {
            inputBlockDuration = Mathf.Clamp(duration, 0f, 300f); // Максимум 5 минут
        }
        
        public void SetInputDebug(bool enabled)
        {
            showInputDebug = enabled;
        }
        
        // Методы для получения информации
        public bool IsMoving() => isMoving;
        public bool IsRunning() => isRunning;
        public bool IsGrounded() => isGrounded;
        public float GetCurrentSpeed() => currentSpeed;
        public Vector3 GetOriginalPosition() => originalCameraPosition;
        
        // Методы для получения информации о блокировке
        public bool IsInputBlocked() => isInputBlocked;
        public float GetInputBlockTimeLeft() => inputBlockTimer;
        public bool IsInputControlEnabled() => enableInputControl;
        
        // Отладочная информация
        void OnGUI()
        {
            if (!enableHeadBob) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 220));
            GUILayout.Label("=== HeadBob Debug ===");
            GUILayout.Label($"Движение: {(isMoving ? "ДА" : "НЕТ")}");
            GUILayout.Label($"Бег: {(isRunning ? "ДА" : "НЕТ")}");
            GUILayout.Label($"На земле: {(isGrounded ? "ДА" : "НЕТ")}");
            GUILayout.Label($"Скорость: {currentSpeed:F2}");
            GUILayout.Label($"Безопасно: {(IsSafeToBob() ? "ДА" : "НЕТ")}");
            
            if (cameraTransform != null)
            {
                Vector3 pos = cameraTransform.localPosition;
                Vector3 rot = cameraTransform.localEulerAngles;
                GUILayout.Label($"Камера Y: {pos.y:F3}");
                GUILayout.Label($"Камера X: {pos.x:F3}");
                GUILayout.Label($"Угол X: {rot.x:F1}°");
                GUILayout.Label($"Ограничения: {(enableAngleLimits ? "ВКЛ" : "ВЫКЛ")}");
            }
            
            // Информация о блокировке ввода
            if (enableInputControl)
            {
                GUILayout.Label("=== УПРАВЛЕНИЕ ===");
                GUILayout.Label($"Ввод заблокирован: {(isInputBlocked ? "ДА" : "НЕТ")}");
                if (isInputBlocked)
                {
                    GUILayout.Label($"Осталось: {inputBlockTimer:F1}с");
                }
                GUILayout.Label($"Управление: {(enableInputControl ? "ВКЛ" : "ВЫКЛ")}");
            }
            
            GUILayout.EndArea();
        }
    }
}

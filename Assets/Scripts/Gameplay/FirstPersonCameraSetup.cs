using UnityEngine;

/// <summary>
/// Настройка камеры от первого лица для зажигалки
/// </summary>
public class FirstPersonCameraSetup : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public Vector3 cameraOffset = new Vector3(0, 0, 0);
    
    [Header("Lighter Position")]
    public Vector3 lighterOffset = new Vector3(0.3f, -0.2f, 0.5f);
    
    void Start()
    {
        SetupFirstPersonCamera();
    }
    
    void SetupFirstPersonCamera()
    {
        // Находим главную камеру если не назначена
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<Camera>();
        }
        
        if (playerCamera != null)
        {
            // Настраиваем камеру для отображения зажигалки
            playerCamera.fieldOfView = 75f; // Угол обзора для от первого лица
            
            Debug.Log("✅ Камера от первого лица настроена!");
        }
        else
        {
            Debug.LogWarning("❌ Камера не найдена!");
        }
    }
    
    public Vector3 GetLighterWorldPosition()
    {
        if (playerCamera != null)
        {
            // Вычисляем мировую позицию зажигалки относительно камеры
            Vector3 worldPosition = playerCamera.transform.position + 
                                   playerCamera.transform.right * lighterOffset.x +
                                   playerCamera.transform.up * lighterOffset.y +
                                   playerCamera.transform.forward * lighterOffset.z;
            
            return worldPosition;
        }
        
        return transform.position;
    }
    
    public Quaternion GetLighterWorldRotation()
    {
        if (playerCamera != null)
        {
            // Зажигалка поворачивается вместе с камерой
            return playerCamera.transform.rotation;
        }
        
        return transform.rotation;
    }
    
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            // Показываем позицию зажигалки в Scene View
            Gizmos.color = Color.yellow;
            Vector3 lighterPos = GetLighterWorldPosition();
            Gizmos.DrawWireSphere(lighterPos, 0.1f);
            
            // Показываем направление камеры
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 2f);
        }
    }
}

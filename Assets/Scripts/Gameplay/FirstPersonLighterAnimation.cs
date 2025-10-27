using UnityEngine;
using System.Collections;

/// <summary>
/// Анимация зажигалки от первого лица
/// </summary>
public class FirstPersonLighterAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float idleBobSpeed = 2f;
    public float idleBobAmount = 0.02f;
    public float igniteAnimationDuration = 0.5f;
    public float extinguishAnimationDuration = 0.3f;
    
    [Header("Position Settings")]
    public Vector3 idlePosition = new Vector3(0.3f, -0.2f, 0.5f);
    public Vector3 ignitePosition = new Vector3(0.25f, -0.15f, 0.6f);
    public Vector3 extinguishPosition = new Vector3(0.35f, -0.25f, 0.4f);
    
    [Header("Rotation Settings")]
    public Vector3 idleRotation = new Vector3(0, 0, 0);
    public Vector3 igniteRotation = new Vector3(-10, 0, 0);
    public Vector3 extinguishRotation = new Vector3(10, 0, 0);
    
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private bool isAnimating = false;
    private Coroutine currentAnimation;
    
    void Start()
    {
        // Сохраняем исходную позицию
        originalPosition = transform.localPosition;
        originalRotation = transform.localEulerAngles;
        
        // Устанавливаем позицию покоя
        transform.localPosition = idlePosition;
        transform.localEulerAngles = idleRotation;
        
        Debug.Log("✅ Анимация зажигалки от первого лица готова!");
    }
    
    void Update()
    {
        if (!isAnimating)
        {
            // Анимация покачивания в покое
            IdleBob();
        }
    }
    
    void IdleBob()
    {
        float bobOffset = Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmount;
        Vector3 bobPosition = idlePosition + Vector3.up * bobOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, bobPosition, Time.deltaTime * 5f);
    }
    
    public void PlayIgniteAnimation()
    {
        if (isAnimating) return;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateIgnite());
    }
    
    public void PlayExtinguishAnimation()
    {
        if (isAnimating) return;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateExtinguish());
    }
    
    IEnumerator AnimateIgnite()
    {
        isAnimating = true;
        
        Vector3 startPos = transform.localPosition;
        Vector3 startRot = transform.localEulerAngles;
        
        float elapsed = 0f;
        
        while (elapsed < igniteAnimationDuration)
        {
            float t = elapsed / igniteAnimationDuration;
            
            // Плавная анимация к позиции зажигания
            transform.localPosition = Vector3.Lerp(startPos, ignitePosition, t);
            transform.localEulerAngles = Vector3.Lerp(startRot, igniteRotation, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Небольшая пауза в позиции зажигания
        yield return new WaitForSeconds(0.1f);
        
        // Возврат к позиции покоя
        elapsed = 0f;
        startPos = transform.localPosition;
        startRot = transform.localEulerAngles;
        
        while (elapsed < igniteAnimationDuration * 0.5f)
        {
            float t = elapsed / (igniteAnimationDuration * 0.5f);
            
            transform.localPosition = Vector3.Lerp(startPos, idlePosition, t);
            transform.localEulerAngles = Vector3.Lerp(startRot, idleRotation, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isAnimating = false;
    }
    
    IEnumerator AnimateExtinguish()
    {
        isAnimating = true;
        
        Vector3 startPos = transform.localPosition;
        Vector3 startRot = transform.localEulerAngles;
        
        float elapsed = 0f;
        
        while (elapsed < extinguishAnimationDuration)
        {
            float t = elapsed / extinguishAnimationDuration;
            
            // Анимация тушения (встряхивание)
            Vector3 shakeOffset = new Vector3(
                Random.Range(-0.01f, 0.01f),
                Random.Range(-0.01f, 0.01f),
                Random.Range(-0.01f, 0.01f)
            );
            
            transform.localPosition = Vector3.Lerp(startPos, extinguishPosition, t) + shakeOffset;
            transform.localEulerAngles = Vector3.Lerp(startRot, extinguishRotation, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Возврат к позиции покоя
        elapsed = 0f;
        startPos = transform.localPosition;
        startRot = transform.localEulerAngles;
        
        while (elapsed < extinguishAnimationDuration * 0.5f)
        {
            float t = elapsed / (extinguishAnimationDuration * 0.5f);
            
            transform.localPosition = Vector3.Lerp(startPos, idlePosition, t);
            transform.localEulerAngles = Vector3.Lerp(startRot, idleRotation, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isAnimating = false;
    }
    
    public void SetIdlePosition(Vector3 position)
    {
        idlePosition = position;
    }
    
    public void SetIdleRotation(Vector3 rotation)
    {
        idleRotation = rotation;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Boat : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool shouldMove = true;
    
    [Header("Scene Load Settings")]
    [SerializeField] private string sceneToLoad = "NextScene";
    [SerializeField] private float loadAfterDistance = 50f;
    [SerializeField] private bool autoLoadScene = true;
    
    private Vector3 startPosition;
    private float distanceTraveled = 0f;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Move the boat forward
        if (shouldMove)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }
        
        // Calculate distance traveled
        distanceTraveled = Vector3.Distance(startPosition, transform.position);
        
        // Load scene after reaching specified distance
        if (autoLoadScene && distanceTraveled >= loadAfterDistance)
        {
            LoadTargetScene();
        }
    }
    
    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    // For debugging distance in Inspector
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName;
    public Sprite itemIcon;
    public string description;
    public bool isKeyItem = true;
    
    [Header("Pickup Settings")]
    public float pickupRange = 2f;
    public KeyCode pickupKey = KeyCode.E;
    
    [Header("Visual Feedback")]
    public GameObject pickupPrompt;
    public string pickupText = "Нажмите E чтобы подобрать";
    
    private bool isPlayerNearby = false;
    private GameObject player;
    
    void Start()
    {
        if (pickupPrompt != null)
        {
            pickupPrompt.SetActive(false);
        }
    }
    
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            isPlayerNearby = true;
            
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(true);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
            
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(false);
            }
        }
    }
    
    void TryPickup()
    {
        if (InventorySystem.Instance != null)
        {
            InventoryItem newItem = new InventoryItem(itemName, itemIcon, description, isKeyItem);
            
            if (InventorySystem.Instance.AddItem(newItem))
            {
                Debug.Log($"Подобран предмет: {itemName}");
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("InventorySystem не найден!");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
using UnityEngine;

public class CustomObjectCollector : ObjectCollector
{
    [Header("Custom Events")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private AudioClip customSound;
    
    protected override void OnFuelCollected()
    {
        base.OnFuelCollected();
        // Custom behavior when fuel is collected
        Debug.Log("Custom fuel collection behavior!");
    }
    
    protected override void OnTrigger1Activated()
    {
        base.OnTrigger1Activated();
        // Custom behavior for trigger 1
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
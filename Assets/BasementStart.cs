using UnityEngine;

public class BasementStart : MonoBehaviour
{
    public GameObject playerGameObject;
    public GameObject objectToDeactivate;
    public GameObject objectToActivate;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.gameObject == playerGameObject)
        {
            // Deactivate and activate objects
            if (objectToDeactivate != null)
                objectToDeactivate.SetActive(false);
            if (objectToActivate != null)
                objectToActivate.SetActive(true);

            // Unlock Teleport 3
            HorrorGame.Player.FPSController fpsController = playerGameObject.GetComponent<HorrorGame.Player.FPSController>();
            if (fpsController != null)
            {
                fpsController.UnlockTeleport3();
            }

            hasTriggered = true;
        }
    }
}

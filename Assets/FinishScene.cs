using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScene : MonoBehaviour
{
    public GameObject playerGameObject;
    public GameObject objectToDeactivate;
    public GameObject objectToActivate;
    [SerializeField] private float sceneLoadDelay = 1f; // Optional delay before loading scene
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.gameObject == playerGameObject)
        {
            hasTriggered = true;
            
            // Deactivate and activate objects
            if (objectToDeactivate != null)
                objectToDeactivate.SetActive(false);
            if (objectToActivate != null)
                objectToActivate.SetActive(true);

            // Unlock Teleport 3 if needed
            HorrorGame.Player.FPSController fpsController = playerGameObject.GetComponent<HorrorGame.Player.FPSController>();
            if (fpsController != null)
            {
                // If you need to call a method on FPSController before scene change
                // fpsController.SomeMethod();
            }

            // Load the new scene (this automatically unloads the current scene)
            Invoke("LoadGame1Scene", sceneLoadDelay);
        }
    }

    private void LoadGame1Scene()
    {
        SceneManager.LoadScene("Game 1");
    }

    // For testing - you can call this method from other scripts or events
    public void ManualTrigger()
    {
        if (!hasTriggered)
        {
            OnTriggerEnter(playerGameObject.GetComponent<Collider>());
        }
    }
}
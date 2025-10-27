using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    
    void Start()
    {
        // Get the Button component and add listener automatically
        Button button = GetComponent<Button>();
        button.onClick.AddListener(LoadScene);
    }
    
    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("No scene name specified in ButtonSceneLoader!");
        }
    }
}
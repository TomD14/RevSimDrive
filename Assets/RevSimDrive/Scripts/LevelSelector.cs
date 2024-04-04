using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    // Method to be called when a level button is clicked
    public void SelectLevel(string levelName)
    {
        // Load the selected level scene
        SceneManager.LoadScene(levelName);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Resets the scene when a given key is pressed.
/// </summary>
public class ResetScene : MonoBehaviour {
    public KeyCode Key = KeyCode.R;

    private void Update() {
        if (Input.GetKeyUp(Key))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
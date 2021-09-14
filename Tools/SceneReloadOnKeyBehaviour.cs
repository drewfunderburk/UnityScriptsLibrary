using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Allows reloading the current scene with a keypress
/// </summary>
public class SceneReloadOnKeyBehaviour : MonoBehaviour
{
    [SerializeField] private KeyCode _key;

    private void Update()
    {
        if (Input.GetKeyDown(_key))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
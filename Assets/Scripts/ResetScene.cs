using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    private void FixedUpdate() {
        if (Input.GetKey("r")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
    
public class MainMenuControl : MonoBehaviour
{
    public string sceneToLoad = "Prototype";
    public void OnMovement(InputValue value) {
        Vector2 move = value.Get<Vector2>();

        SceneManager.LoadScene(sceneToLoad);
    }
}

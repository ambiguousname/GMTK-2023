using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
    
public class MainMenuControl : MonoBehaviour
{
    public AudioSource toPlay;
    public string sceneToLoad = "Prototype";
    public void OnMovement(InputValue value) {
        Vector2 move = value.Get<Vector2>();
        if (move.x != 0 || move.y != 0) {
            toPlay.pitch = Random.Range(0.8f, 1.2f);
            toPlay.Play();
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

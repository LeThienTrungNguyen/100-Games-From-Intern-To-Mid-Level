using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trap : MonoBehaviour
{
    void Awake()
    {
        transform.AddComponent<BoxCollider2D>();
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset lại scene hiện tại
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}

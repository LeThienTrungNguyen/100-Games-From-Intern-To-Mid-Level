using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Weight : MonoBehaviour
{
    public string scene;
    public string currentScene;
    void Update()
    {
        if(transform.position.y < -10f)
        {
            if (currentScene.Length > 0) SceneManager.LoadScene(currentScene);
            else Application.Quit();
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (scene.Length > 0) SceneManager.LoadScene(scene);
            else Application.Quit();
        }
    }
    public float distanceFromChainEnd = 0.2f;
    public void ConnectRopeEnd(Rigidbody2D endRB)
    {
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = endRB;
        joint.anchor = Vector2.zero;
        joint.connectedAnchor = new Vector2(0, -distanceFromChainEnd);
        GetComponent<Rigidbody2D>().mass = 100f;
    }
}
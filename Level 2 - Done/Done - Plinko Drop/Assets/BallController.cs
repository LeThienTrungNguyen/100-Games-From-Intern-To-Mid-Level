using UnityEngine;

public class BallController : MonoBehaviour
{
    void Awake()
    {
        // ignore collisions between balls
        bool b = Random.value >= 0.5f;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Ball"),b);
        GetComponent<Rigidbody2D>().gravityScale = Random.Range(1f, 11f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

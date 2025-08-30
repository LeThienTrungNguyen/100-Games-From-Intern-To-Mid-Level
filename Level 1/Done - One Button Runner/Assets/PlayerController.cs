using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float gameOverFallenHeight;
    void Awake()
    {
        instance = this;
    }
    void FixedUpdate()
    {
        if (transform.position.y < gameOverFallenHeight)
        {
            GameController.instance.GameOver();
        }
    }
    public void MoveHorizontal(Transform target, float time = 2f)
    {
        // Thêm kiểm tra null cho target để tránh lỗi nếu playerLandPos bị null
        if (target != null)
        {
            transform.DOMoveX(target.position.x, time).SetEase(Ease.Linear)
            .OnStart(()=>
            {
                GetComponent<Rigidbody2D>().gravityScale = 0;
            })
            .OnComplete(() =>
            {
                GameController.instance.IgnoreLayerCollisionPlayerLadder(false);
                GameController.instance.canInteract = true;
                GameController.instance.CreateGround();
                GetComponent<Rigidbody2D>().gravityScale = 1;
            })
            ;
        }
        else
        {
            Debug.LogError("MoveHorizontal : Target Transform is null!");
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
        GroundController groundController;

        if (collision.transform.root.TryGetComponent<GroundController>(out groundController))
        {
            if (groundController.isLanded) { return; }
            MoveHorizontal(groundController.PlayerStandPosition, 0.3f);
            groundController.isLanded = true;
            GameController.instance.SetNewStartGround(groundController);
            GameController.instance.AddScore();
        }
    }
    
}

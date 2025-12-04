using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;

    public void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        controller.input = new Vector3(h, 0f, v).normalized;
    }

    public void Move()
    {
        if (controller == null || controller.rb == null) return;

        Vector3 vel = controller.input * controller.moveSpeed;
        controller.rb.velocity = new Vector3(vel.x, controller.rb.velocity.y, vel.z);

        if (controller.input.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(controller.input);
            controller.transform.rotation = Quaternion.Slerp(
                controller.transform.rotation, target, Time.deltaTime * 10f);
        }
    }
}

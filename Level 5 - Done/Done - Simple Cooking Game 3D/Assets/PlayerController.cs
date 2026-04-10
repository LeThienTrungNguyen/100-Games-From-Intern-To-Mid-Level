using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteract))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Raycast")]
    public float checkDistance = 2f;
    public LayerMask counterLayer;

    [Header("Highlight")]
    public Material counterSelectedMaterial;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector3 input;

    private PlayerMovement movement;
    private PlayerInteract interact;
    public bool isPicking = false;
    public Transform PickingObject = null;
    public Transform handPosition;
    public LayerMask cutableLayer;
    public LayerMask cookableLayer;
    public LayerMask cuttedLayer;
    public LayerMask deliveriableLayer;

    public bool isBringingPlate;
    public List<LayerMask> deliveryObjLst;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        movement = GetComponent<PlayerMovement>();
        interact = GetComponent<PlayerInteract>();

        // Gán tham chiếu để 2 class kia có thể truy cập
        movement.controller = this;
        interact.controller = this;
    }

    void Update()
    {
        movement.HandleInput();
        interact.CheckCounterForwardAndHandleHighlight();
        //interact.HandleInteract();
    }

    void FixedUpdate()
    {
        movement.Move();
    }
}

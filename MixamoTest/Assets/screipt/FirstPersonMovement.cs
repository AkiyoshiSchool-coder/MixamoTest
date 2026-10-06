using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public InputActionAsset inputActions;
    private InputAction runAction;
    private InputAction moveAction;
    [SerializeField] private Transform MyCamera;
    [SerializeField] private Vector3 cPos;
    public StudioEventEmitter steps;
    private float stepTimer = 0;
    [SerializeField] private float stepCD;
    private bool playerGrounded;
    [SerializeField] private GroundCheck groundcode;

    Rigidbody rigidbody;
    Rigidbody box;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        MyCamera = Camera.main.transform;
        cPos = new Vector3(transform.eulerAngles.x,MyCamera.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        runAction = InputSystem.actions.FindAction("Run");
    }

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,MyCamera.transform.eulerAngles.y, transform.eulerAngles.z);
        cPos = new Vector3(transform.eulerAngles.x,MyCamera.transform.eulerAngles.y, transform.eulerAngles.z);
        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity = moveAction.ReadValue<Vector2>() * targetMovingSpeed;

        // Apply movement.
        rigidbody.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.linearVelocity.y, targetVelocity.y);
        if(box != null) //Se o Push script da box um valor ela se move com ele
        {
            box.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.linearVelocity.y, targetVelocity.y);
        }
    }

    void Update()
    {
        stepTimer += Time.deltaTime;
        playerGrounded = groundcode.groundedcheck();
        if(moveAction.IsPressed() && stepTimer > stepCD && playerGrounded)
        {
            stepTimer = 0;
            steps.Play();
        }
    }

    public void PushingObject(Rigidbody rigidbody) //chamado pelo Push Script
    {
        box = rigidbody;
    }
    public void StopPushing()  //chamado pelo Push Script
    {
        box = null;
    }

    public void Stop(bool stoporNot)
    {
        if(stoporNot)
        {
        speed = 0;
        runSpeed = 0;
        }
        else
        {
            speed = 5;
            runSpeed = 9;
        }
    }
}
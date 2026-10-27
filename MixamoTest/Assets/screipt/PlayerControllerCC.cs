
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : MonoBehaviour
{
    //Controle
    [SerializeField] private CharacterController characterController;   
    [SerializeField] private float WalkSpeed = 5.5f;
    [SerializeField] private float RunningSpeed = 9f;
    [SerializeField] private float JumpForce = 8f;
    [SerializeField] private float Gravity = 20f;
    [SerializeField] private float LookSensitivity = 0.2f;
    [SerializeField] private float LookAngleLimit = 90f; 

    [SerializeField] private float AnimPlayTransition;

    //Ações
    private InputAction MoveAction;
    private InputAction RunAction;
    private InputAction JumpAction;
    private InputAction LookAction;

    //Valores atuais
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private float currentVerticalSpeed;
    [SerializeField] private float LookAngle;

    [SerializeField] Animator Animator;
    UnityEngine.Vector2 AnimBlendVect;
    UnityEngine.Vector2 AnimVel;
    [SerializeField] private float AnimSmooth;

    [SerializeField] private UnityEngine.Quaternion CurrentRotaion;
    [SerializeField ] private float RotationSpeed;

    //Câmera
    [SerializeField] private Camera MainCamera;
    void Start()
    {
        //Declarando ações
        MoveAction = InputSystem.actions.FindAction("Move");
        RunAction = InputSystem.actions.FindAction("Sprint");
        JumpAction = InputSystem.actions.FindAction("Jump");
        LookAction = InputSystem.actions.FindAction("Look");
        
        //visibilidade do cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Definindo Velocidade
        currentMoveSpeed = WalkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector2 Movement = MoveAction.ReadValue<UnityEngine.Vector2>();
        //running
        currentMoveSpeed = RunAction.IsPressed() ? RunningSpeed : WalkSpeed;
        HandleMovement(Movement);

        //Aplica Câmera
        CurrentRotaion = Quaternion.Euler(0f, MainCamera.transform.eulerAngles.y,0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, CurrentRotaion,RotationSpeed*Time.deltaTime);
        Animacao();
    }
    private void HandleMovement(UnityEngine.Vector2 Movement)
    {
        //Movimentação com direção ajustada pela rotação
        UnityEngine.Vector3 moveDirection = (transform.forward * Movement.y + transform.right * Movement.x) * currentMoveSpeed;

        if(characterController.isGrounded) // Checa se player no chão
        {
            if(JumpAction.WasPressedThisFrame())
            {

                currentVerticalSpeed = JumpForce;
            }
            else
            {
                currentVerticalSpeed = -2f; //Mantém contanto com o chão
            }
        }
        else
        {
            currentVerticalSpeed -= Gravity*Time.deltaTime;
        }

        moveDirection.y = currentVerticalSpeed;

        //Aplica a move
        characterController.Move(moveDirection*Time.deltaTime); 
    }

    
    private void Animacao()
    {
        UnityEngine.Vector2 Input = MoveAction.ReadValue<UnityEngine.Vector2>();
        AnimBlendVect = UnityEngine.Vector2.SmoothDamp(AnimBlendVect, Input, ref AnimVel, AnimSmooth);
        Animator.SetFloat("MoveX", AnimBlendVect.x);
        Animator.SetFloat("MoveZ", AnimBlendVect.y);
    }
}

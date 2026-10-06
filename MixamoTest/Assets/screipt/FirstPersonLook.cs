using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Unity.Cinemachine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    private float sensitivity = 2f;
    public float baseSensivity, mobileSensivity;
    public float smoothing = 1.5f;

    [SerializeField] private CinemachineCamera mcam;
    [SerializeField] private CinemachineInputAxisController mcamControl;

    [SerializeField] private float normalVision = 60;

    Vector2 velocity;
    Vector2 frameVelocity;

    private InputAction lookAction;

    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            mcamControl.Controllers[0].Input.Gain = mobileSensivity; 
            mcamControl.Controllers[1].Input.Gain = -mobileSensivity; 
        }
        else
        {
            mcamControl.Controllers[0].Input.Gain = baseSensivity; 
            mcamControl.Controllers[1].Input.Gain = -baseSensivity; 
        }
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;
        lookAction = InputSystem.actions.FindAction("Look");

        sensitivity = baseSensivity;
    }

    void Update()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);
    }

    public void Freeze(bool freeze)
    {
        if(freeze)
        {
            mcamControl.Controllers[0].Enabled = false; // https://discussions.unity.com/t/919323
            mcamControl.Controllers[1].Enabled = false;
        }
        else
        {
            mcamControl.Controllers[0].Enabled = true;
            mcamControl.Controllers[1].Enabled = true;
        }
    }
}

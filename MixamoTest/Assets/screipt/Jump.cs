using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Jump : MonoBehaviour
{
    Rigidbody rigidbody;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    bool groundCheck;
    private InputAction jumpAction;
    Rigidbody box;
    bool chao;
    [SerializeField] private float tempo;
    [SerializeField] Animator Animator;
    [SerializeField] private float AnimationPlayerTransition = 0.15f;

    void Awake()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        // Get rigidbody.
        rigidbody = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        // Jump when the Jump button is pressed and we are on the ground.
        if (jumpAction.triggered && groundCheck)
        {
            Animator.CrossFade("Jumping",AnimationPlayerTransition);
            rigidbody.AddForce(Vector3.up * 100 * jumpStrength); 
            
            if (box != null)  //Se o Push script da box um valor ela se move com ele
            {
                box.constraints &= ~RigidbodyConstraints.FreezePositionY;
                box.AddForce(Vector3.up * 100 * jumpStrength); 
                StartCoroutine(TempoParaVoltar());
            }
            
            Jumped?.Invoke();
        }
    }

    IEnumerator TempoParaVoltar()
    {
        yield return new WaitForSeconds(tempo);
        if (box != null)  //Se o Push script da box um valor ela se move com ele
        {
            box.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }

    public void PushingObject(Rigidbody rigidbody) //chamado pelo Push Script
    {
        box = rigidbody;
    }

    public void StopPushing() //chamado pelo Push Script
    {
        if(box != null)
        {
            box = null;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("ground"))
        {
            groundCheck = true;
        }
    }private void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("ground"))
        {
            groundCheck = false;
        }
    }
}

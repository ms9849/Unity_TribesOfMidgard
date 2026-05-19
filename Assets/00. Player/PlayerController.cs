using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Transform PlayerTransform = null;
    Animator PlayerAnimator = null;

    public float PlayerSpeed;
    public Vector2 MoveInput { get; private set; }

    public bool isAttackKeyPressed { get; set; } = false;
    public bool isInteractKeyPressed { get; set; }  = false;
    public bool isSprintKeyPressed { get; set; } = false;
    public Interaction CurrentInteractionObject { get; set; } = null;

    void Awake()
    {
        PlayerTransform = transform;
        PlayerAnimator = GetComponent<Animator>();
        PlayerSpeed = 10.0f;
    }
    void Start()
    {
    }

    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            isAttackKeyPressed = true;
        else
            isAttackKeyPressed = false;
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isInteractKeyPressed = true;
        }

        if (null == CurrentInteractionObject)
        {
            isInteractKeyPressed = false;
            return;
        }

        Debug.Log("인터랙션~");

    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            isSprintKeyPressed = true;
        else if (context.phase == InputActionPhase.Canceled) 
            isSprintKeyPressed = false;
    }
}

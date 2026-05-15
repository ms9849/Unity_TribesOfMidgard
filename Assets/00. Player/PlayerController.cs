using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Transform PlayerTransform = null;
    Animator PlayerAnimator = null;

    public float PlayerSpeed;
    public Vector2 MoveInput { get; private set; }

    public bool isInteracable { get; set; }
    public bool isAttackKeyPressed { get; private set; }
    public bool isInteractKeyPressed { get; private set; }
    public bool isSprintKeyPressed { get; private set; }
    public Interaction CurrentInteractionObject { get; set; }

    void Start()
    {
        PlayerTransform = transform;
        PlayerAnimator = GetComponent<Animator>();
        isInteracable = true;
        PlayerSpeed = 25.0f;
    }

    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        Debug.Log(MoveInput);
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
            isInteractKeyPressed = true;
        else
            isInteractKeyPressed = false;

        if (false == isInteracable)
            isInteractKeyPressed = false;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            isSprintKeyPressed = true;
        else if (context.phase == InputActionPhase.Canceled) 
            isSprintKeyPressed = false;
    }
}

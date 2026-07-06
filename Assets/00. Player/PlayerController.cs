using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Transform PlayerTransform = null;
    Animator PlayerAnimator = null;
    Inventory PlayerInventory = null;

    [SerializeField] private InventoryUI PlayerInventoryUI;

    public float PlayerSpeed;
    public Vector2 MoveInput { get; private set; }
    public bool isAttackKeyPressed { get; set; } = false;
    public bool isInteractKeyPressed { get; set; }  = false;
    public bool isSprintKeyPressed { get; set; } = false;
    public Interaction CurrentInteractionObject { get; set; } = null;
    public PLAYER_WEAPON CurrentWeapon { get; private set; } = PLAYER_WEAPON.NAKED;
    public bool isRootMotionEnabled { get; set; } = false;

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

    public void OnAnimatorMove()
    {
        if (!isRootMotionEnabled)
            return;

        transform.position += PlayerAnimator.deltaPosition;
        transform.rotation *= PlayerAnimator.deltaRotation;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            isAttackKeyPressed = true;
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

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            PlayerInventoryUI.ToggleActive();
    }
}

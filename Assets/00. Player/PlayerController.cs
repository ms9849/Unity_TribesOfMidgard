using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Transform PlayerTransform = null;
    Animator PlayerAnimator = null;
    Inventory PlayerInventory = null;

    [SerializeField] private InventoryUI PlayerInventoryUI;

    // 부위(EQUIP_TYPE)별로 현재 장착중인 장비를 관리합니다.
    private Dictionary<EQUIP_TYPE, ItemSO> EquippedItems = new Dictionary<EQUIP_TYPE, ItemSO>();

    public float PlayerSpeed;
    public float RotationSpeed;
    public Vector2 MoveInput { get; private set; }
    public bool isAttackKeyPressed { get; set; } = false;
    public bool isInteractKeyPressed { get; set; }  = false;
    public bool isSprintKeyPressed { get; set; } = false;
    public Interaction CurrentInteractionObject { get; set; } = null;
    public bool isRootMotionEnabled { get; set; } = false;

    void Awake()
    {
        PlayerTransform = transform;
        PlayerAnimator = GetComponent<Animator>();
        PlayerSpeed = 10.0f;
        RotationSpeed = 10.0f;
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
        if (context.phase == InputActionPhase.Started && !PlayerInventoryUI.IsActive)
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

    // 해당 부위에 현재 장착중인 아이템을 반환합니다. 없으면 null.
    public ItemSO GetEquippedItem(EQUIP_TYPE type)
    {
        EquippedItems.TryGetValue(type, out ItemSO item);
        return item;
    }

    // item.EquipType 부위에 아이템을 장착하고, 기존에 장착되어 있던 아이템(없으면 null)을 반환합니다.
    public ItemSO EquipItem(ItemSO item)
    {
        EquippedItems.TryGetValue(item.EquipType, out ItemSO previousItem);
        EquippedItems[item.EquipType] = item;

        return previousItem;
    }

    // item.EquipType 부위에 현재 장착중인 아이템이 item과 같을 때만 장착을 해제합니다.
    public void UnequipItem(ItemSO item)
    {
        if (EquippedItems.TryGetValue(item.EquipType, out ItemSO equippedItem) && equippedItem == item)
            EquippedItems.Remove(item.EquipType);
    }
}

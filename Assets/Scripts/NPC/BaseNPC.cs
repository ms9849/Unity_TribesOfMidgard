using UnityEngine;
using UnityEngine.InputSystem;

// 모든 NPC가 상속받는 기반 클래스. 플레이어와의 콜라이더 충돌로 NPCInteractionUI를 띄우고 상호작용 입력을 처리합니다.
public abstract class BaseNPC : MonoBehaviour
{
    [SerializeField] private string NPCName;

    private PlayerInput InteractingPlayer;
    protected Inventory InteractingInventory;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        InteractingPlayer = other.GetComponent<PlayerInput>();
        InteractingInventory = other.GetComponent<Inventory>();
        NPCInteractionUI.Instance.Show(transform, NPCName);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        InteractingPlayer = null;
        InteractingInventory = null;
        NPCInteractionUI.Instance.Hide();
    }

    protected virtual void Update()
    {
        if (InteractingPlayer != null && InteractingPlayer.actions["Player/Interact"].WasPressedThisFrame())
        {
            Interact();
        }
    }

    protected virtual void Interact()
    {
    }
}

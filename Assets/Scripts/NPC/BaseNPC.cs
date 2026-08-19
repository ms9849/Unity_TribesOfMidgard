using UnityEngine;
using UnityEngine.InputSystem;

// 모든 NPC가 상속받는 기반 클래스. 플레이어와의 콜라이더 충돌로 NPCInteractionUI를 띄우고 상호작용 입력을 처리합니다.
public abstract class BaseNPC : MonoBehaviour
{
    [SerializeField] private string NPCName;

    private PlayerInput InteractingPlayer;
    protected Inventory InteractingInventory;
    bool isInteractbale = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        InteractingPlayer = player.GetComponent<PlayerInput>();
        InteractingInventory = player.GetComponent<Inventory>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        NPCInteractionUI.Instance.Show(transform, NPCName);
        isInteractbale = true;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        NPCInteractionUI.Instance.Hide();
        isInteractbale = false;
    }

    protected virtual void Update()
    {
        if (true == isInteractbale && InteractingPlayer.actions["Player/Interact"].WasPressedThisFrame())
        {
            Interact();
        }
    }

    protected virtual void Interact()
    {
    }
}

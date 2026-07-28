using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator playerAnimator { get; private set; }
    public PlayerController playerController { get; private set; }
    public Inventory playerInventory { get; private set; }
    public Rigidbody playerRigidbody { get; private set; }
    public Health playerHealth { get; private set; }
    PlayerStateMachine playerFSM;

    /*TEST CODE */
    [Header("Test SO")]
    public ItemSO TestWood;

    /*TEST CODE */
    [Header("WEAPON SO")]
    public ItemSO TestWeapon;

    [Header("Chest SO")]
    public List<ItemSO> TestArmors;


    [Header("시작 지급 아이템")]
    public ItemSO StartingAxe;

    /* ***** */
    void Awake()
    {
        playerInventory = GetComponent<Inventory>();
        playerController = GetComponent<PlayerController>();
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerHealth = GetComponent<Health>();
        playerFSM = new PlayerStateMachine(this);
    }

    void Start()
    {
        Debug.Log($"[플레이어 초기화] 인스턴스 이름: {gameObject.name} | 컨트롤러 주소: {playerController.GetHashCode()}");

        GrantStartingAxe();

        playerHealth.OnDeath += HandleDeath;
    }

    // 게임 시작 시 도끼 1개를 지급하고 바로 장착시킵니다.
    void GrantStartingAxe()
    {
        if (StartingAxe == null)
            return;

        playerInventory.AddItem(StartingAxe, 1);

        for (int i = 0; i < playerInventory.Slots.Count; i++)
        {
            if (!playerInventory.Slots[i].IsEmpty() && playerInventory.Slots[i].Item == StartingAxe)
            {
                playerInventory.EquipItemAt(i);
                break;
            }
        }
    }

void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        playerFSM.ChangeState(StateID.Dead);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerInventory.AddItem(TestWood, 1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            playerInventory.AddItem(TestWeapon, 1);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach(ItemSO item in TestArmors)
            {
                playerInventory.AddItem(item, 1);   
            }
        }

        playerFSM.Update();
    }

    void FixedUpdate()
    {
        playerFSM.FixedUpdate();
    }
}

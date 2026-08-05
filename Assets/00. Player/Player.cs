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
    public GameObject LastAttacker { get; private set; }
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
    public ItemSO StartingPickaxe;

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

        GrantStartingItem(StartingAxe);
        GrantStartingItem(StartingPickaxe);

        playerHealth.OnDeath += HandleDeath;
        playerHealth.OnDamaged += HandleDamaged;
    }

    // 게임 시작 시 아이템 1개를 지급하고 바로 장착시킵니다.
    void GrantStartingItem(ItemSO item)
    {
        if (item == null)
            return;

        playerInventory.AddItem(item, 1);

        for (int i = 0; i < playerInventory.Slots.Count; i++)
        {
            if (!playerInventory.Slots[i].IsEmpty() && playerInventory.Slots[i].Item == item)
            {
                playerInventory.EquipItemAt(i);
                break;
            }
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandleDeath;
            playerHealth.OnDamaged -= HandleDamaged;
        }
    }

    void HandleDeath()
    {
        playerFSM.ChangeState(StateID.Dead);
    }

    void HandleDamaged(float amount, GameObject attacker)
    {
        if (playerHealth.IsAlive)
        {
            LastAttacker = attacker;
            playerFSM.ChangeState(StateID.Hit);
        }
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

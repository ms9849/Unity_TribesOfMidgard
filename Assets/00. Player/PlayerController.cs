using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Transform PlayerTransform = null;
    Animator PlayerAnimator = null;
    Inventory PlayerInventory = null;
    Health PlayerHealth = null;
    Rigidbody PlayerRigidbody = null;

    [SerializeField] private InventoryUI PlayerInventoryUI;
    // 무기/도끼 장착 시 실제 메쉬가 붙을 손 본(Transform). Inspector에서 직접 지정합니다.
    [SerializeField] private Transform WeaponSocket;
    // 갑옷(CHEST/PANTS) 장착 시 본을 재바인딩할 기준이 되는 Player 리그의 루트. Inspector에서 직접 지정합니다.
    [SerializeField] private Transform SkeletonRoot;

    // 검격 시 플레이어 정면에 스폰할 슬래시 이펙트 프리팹. Inspector에서 직접 지정합니다.
    [SerializeField] private GameObject SlashEffectPrefab;
    // 콤보 타(1/2/3타)별 스폰 위치/회전/수명. Inspector에서 각 타별로 다르게 조정합니다.
    [SerializeField] private SlashEffectSettings SlashEffectFirst;
    [SerializeField] private SlashEffectSettings SlashEffectSecond;
    [SerializeField] private SlashEffectSettings SlashEffectThird;

    // 나무 채집 시작 시 재생할, 씬에 배치된 연기 이펙트. Inspector에서 직접 지정합니다.
    [SerializeField] private ParticleSystem CollectSmokeEffect;

    // 채집 대상 위치로 스모크 이펙트를 옮겨 재생합니다. PlayerCollectState가 나무 채집 시작 시 호출합니다.
    public void PlayCollectSmokeEffect(Vector3 position)
    {
        if (CollectSmokeEffect == null)
            return;

        CollectSmokeEffect.transform.position = position;
        CollectSmokeEffect.Play();
    }

    // 부위(EQUIP_TYPE)별로 현재 장착중인 장비를 관리합니다.
    private Dictionary<EQUIP_TYPE, ItemSO> EquippedItems = new Dictionary<EQUIP_TYPE, ItemSO>();
    // 부위(EQUIP_TYPE)별로 실제로 생성되어 붙어있는 장비 메쉬 인스턴스를 관리합니다.
    private Dictionary<EQUIP_TYPE, GameObject> EquippedVisuals = new Dictionary<EQUIP_TYPE, GameObject>();
    // 본 이름 -> SkeletonRoot 하위 실제 본 Transform 캐시. 갑옷 본 재바인딩에 사용됩니다.
    private Dictionary<string, Transform> SkeletonBoneMap;
    // 손에는 한 번에 하나의 장비만 보여줍니다(예: 평소엔 무기, 나무 채집 중엔 도끼).
    private EQUIP_TYPE ActiveHandVisual = EQUIP_TYPE.WEAPON;

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
        PlayerHealth = GetComponent<Health>();
        PlayerRigidbody = GetComponent<Rigidbody>();
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

        PlayerRigidbody.MovePosition(PlayerRigidbody.position + PlayerAnimator.deltaPosition);
        PlayerRigidbody.MoveRotation(PlayerRigidbody.rotation * PlayerAnimator.deltaRotation);
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

    // 마우스 스크린 좌표를 플레이어 발밑 높이의 수평면에 투영해 월드 좌표를 구합니다.
    // 공격 방향 판정에 사용됩니다(지형 콜라이더 유무와 무관하게 항상 값이 나옵니다).
    public bool TryGetMouseWorldPoint(out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;

        Camera mainCamera = Camera.main;
        if (mainCamera == null || Mouse.current == null)
            return false;

        Ray mouseRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, PlayerTransform.position);

        if (groundPlane.Raycast(mouseRay, out float distance))
        {
            worldPoint = mouseRay.GetPoint(distance);
            return true;
        }

        return false;
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


    // ESC 입력 시 현재 활성화된 모든 UI(인벤토리, 제작 UI)를 비활성화합니다.
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        if (InventoryUI.Instance != null && InventoryUI.Instance.IsActive)
            InventoryUI.Instance.SetActive(false);

        if (CreateItemUI.Instance != null && CreateItemUI.Instance.IsActive)
            CreateItemUI.Instance.SetActive(false);
    }


    // 해당 부위에 현재 장착중인 아이템을 반환합니다. 없으면 null.
    public ItemSO GetEquippedItem(EQUIP_TYPE type)
    {
        EquippedItems.TryGetValue(type, out ItemSO item);
        return item;
    }

    // 무기가 장착되어 있는지 여부. 공격 상태 진입 가능 여부 판단에 사용됩니다.
    public bool IsWeaponEquipped()
    {
        return GetEquippedItem(EQUIP_TYPE.WEAPON) != null;
    }

    // 도끼가 장착되어 있는지 여부. 채집 상태 진입 가능 여부 판단에 사용됩니다.
    public bool IsAxeEquipped()
    {
        return GetEquippedItem(EQUIP_TYPE.AXE) != null;
    }

    // 손에 보여줄 장비를 하나만 지정합니다. 나머지 장비의 메쉬는 비활성화됩니다.
    public void SetActiveHandVisual(EQUIP_TYPE type)
    {
        ActiveHandVisual = type;

        foreach (KeyValuePair<EQUIP_TYPE, GameObject> Pair in EquippedVisuals)
        {
            // 손에 드는 장비(WEAPON/AXE)끼리만 서로 토글하고, 갑옷 등 다른 부위는 건드리지 않습니다.
            if (Pair.Key != EQUIP_TYPE.WEAPON && Pair.Key != EQUIP_TYPE.AXE)
                continue;

            if (Pair.Value != null)
                Pair.Value.SetActive(Pair.Key == ActiveHandVisual);
        }
    }

    // item.EquipType 부위에 아이템을 장착하고, 기존에 장착되어 있던 아이템(없으면 null)을 반환합니다.
public ItemSO EquipItem(ItemSO item)
    {
        EquippedItems.TryGetValue(item.EquipType, out ItemSO previousItem);
        EquippedItems[item.EquipType] = item;

        UpdateEquipVisual(item);
        RecalculateStats();

        if (item.EquipType == EQUIP_TYPE.WEAPON)
            SetupWeaponHitbox(item);

        return previousItem;
    }

// 무기 장착 시 비주얼에 붙은 WeaponHitbox에 공격자/데미지를 세팅합니다.
    private void SetupWeaponHitbox(ItemSO item)
    {
        WeaponHitbox Hitbox = GetEquippedWeaponHitbox();
        if (Hitbox != null)
            Hitbox.SetOwnerAndDamage(gameObject, item.WeaponDamage);
    }

    // 현재 장착중인 무기의 WeaponHitbox를 반환합니다. 없으면 null. PlayerSwordAttackState가 Arm/Disarm 호출 시 사용합니다.
    public WeaponHitbox GetEquippedWeaponHitbox()
    {
        if (EquippedVisuals.TryGetValue(EQUIP_TYPE.WEAPON, out GameObject visual) && visual != null)
            return visual.GetComponentInChildren<WeaponHitbox>();

        return null;
    }

    // 콤보 타(combo)에 해당하는 슬래시 이펙트 설정을 반환합니다. 없으면 null.
    private SlashEffectSettings GetSlashEffectSettings(ATTACK_COMBO combo)
    {
        return combo switch
        {
            ATTACK_COMBO.FIRST => SlashEffectFirst,
            ATTACK_COMBO.SECOND => SlashEffectSecond,
            ATTACK_COMBO.THIRD => SlashEffectThird,
            _ => null
        };
    }

    // 해당 콤보 타의 애니메이션이 이 진행도(normalizedTime)를 넘어야 슬래시 이펙트가 스폰됩니다.
    // PlayerSwordAttackState가 Update()에서 매 프레임 체크할 때 사용합니다.
    public float GetSlashEffectSpawnNormalizedTime(ATTACK_COMBO combo)
    {
        return GetSlashEffectSettings(combo)?.SpawnNormalizedTime ?? 0f;
    }

    // 플레이어 정면 고정 위치에 슬래시 이펙트를 스폰합니다. PlayerSwordAttackState가 콤보 타(combo)별로 호출합니다.
    public void PlaySlashEffect(ATTACK_COMBO combo)
    {
        if (SlashEffectPrefab == null)
            return;

        SlashEffectSettings Settings = GetSlashEffectSettings(combo);

        if (Settings == null)
            return;

        Vector3 SpawnPosition = PlayerTransform.position + PlayerTransform.forward * Settings.Distance + Vector3.up * Settings.Height;
        Quaternion Rotation = PlayerTransform.rotation * Quaternion.Euler(Settings.RotationOffset);
        GameObject Effect = Instantiate(SlashEffectPrefab, SpawnPosition, Rotation);
        Destroy(Effect, Settings.Lifetime);
    }


    // item.EquipType 부위에 현재 장착중인 아이템이 item과 같을 때만 장착을 해제합니다.
    public void UnequipItem(ItemSO item)
    {
        if (EquippedItems.TryGetValue(item.EquipType, out ItemSO equippedItem) && equippedItem == item)
        {
            EquippedItems.Remove(item.EquipType);
            ClearEquipVisual(item.EquipType);
            RecalculateStats();
        }
    }

    // 현재 장착중인 모든 장비의 스텟 보너스를 합산해 Health에 반영합니다.
    private void RecalculateStats()
    {
        if (PlayerHealth == null)
            return;

        float TotalDefense = 0f;
        float TotalMaxHpBonus = 0f;

        foreach (ItemSO Item in EquippedItems.Values)
        {
            TotalDefense += Item.DefenseBonus;
            TotalMaxHpBonus += Item.MaxHpBonus;
        }

        PlayerHealth.SetDefense(TotalDefense);
        PlayerHealth.SetBonusMaxHp(TotalMaxHpBonus);
    }

    // 해당 부위에 붙어있던 기존 메쉬를 제거하고, item.EquipModel이 있으면 새로 생성해 붙입니다.
    private void UpdateEquipVisual(ItemSO item)
    {
        ClearEquipVisual(item.EquipType);

        if (item.EquipModel == null)
            return;

        GameObject Visual;

        if (IsSkinnedEquipType(item.EquipType))
        {
            Visual = AttachSkinnedEquip(item.EquipModel);

            if (Visual == null)
                return;
        }
        else
        {
            Transform Socket = GetEquipSocket(item.EquipType);

            if (Socket == null)
                return;

            Visual = Instantiate(item.EquipModel, Socket);

            // 손 본은 캐릭터 리그의 임포트 스케일이 누적되어 로컬 스케일이 1이 아닙니다(예: 100배).
            // 장비 메쉬가 본래 크기로 보이도록 소켓의 누적 스케일을 상쇄하고, 소켓 원점에 맞춥니다.
            Vector3 SocketScale = Socket.lossyScale;
            Visual.transform.localScale = new Vector3(1f / SocketScale.x, 1f / SocketScale.y, 1f / SocketScale.z);
            Visual.transform.localPosition = Vector3.zero;
            Visual.transform.localRotation = Quaternion.identity;
            Visual.SetActive(item.EquipType == ActiveHandVisual);
        }

        EquippedVisuals[item.EquipType] = Visual;
    }

    // CHEST/PANTS처럼 Player와 같은 스켈레톤을 공유하는 스킨 메쉬 장비인지 여부.
    private bool IsSkinnedEquipType(EQUIP_TYPE type)
    {
        return 
            type == EQUIP_TYPE.CHEST || 
            type == EQUIP_TYPE.PANTS || 
            type == EQUIP_TYPE.GLOVE ||
            type == EQUIP_TYPE.HELMET ||
            type == EQUIP_TYPE.SHOES;
    }

    // 갑옷 프리팹을 인스턴스화한 뒤, 프리팹이 들고 있는 자체 스켈레톤 대신 Player의 실제 스켈레톤 본으로
    // SkinnedMeshRenderer의 bones/rootBone을 이름 기준으로 재바인딩합니다. 나머지(복제 스켈레톤 등)는 폐기합니다.
    private GameObject AttachSkinnedEquip(GameObject prefab)
    {
        if (SkeletonRoot == null)
            return null;

        GameObject Instance = Instantiate(prefab);
        SkinnedMeshRenderer SourceRenderer = Instance.GetComponentInChildren<SkinnedMeshRenderer>();

        if (SourceRenderer == null)
        {
            Destroy(Instance);
            return null;
        }

        Dictionary<string, Transform> BoneMap = GetSkeletonBoneMap();
        Transform[] OldBones = SourceRenderer.bones;
        Transform[] NewBones = new Transform[OldBones.Length];

        for (int i = 0; i < OldBones.Length; i++)
        {
            if (OldBones[i] != null && BoneMap.TryGetValue(OldBones[i].name, out Transform Match))
                NewBones[i] = Match;
        }

        SourceRenderer.bones = NewBones;

        if (SourceRenderer.rootBone != null && BoneMap.TryGetValue(SourceRenderer.rootBone.name, out Transform RootMatch))
            SourceRenderer.rootBone = RootMatch;

        SourceRenderer.transform.SetParent(transform, true);
        Destroy(Instance);

        return SourceRenderer.gameObject;
    }

    // 본 이름 -> Transform 매핑을 최초 1회만 만들어 캐시합니다.
    private Dictionary<string, Transform> GetSkeletonBoneMap()
    {
        if (SkeletonBoneMap == null)
        {
            SkeletonBoneMap = new Dictionary<string, Transform>();
            CollectBones(SkeletonRoot, SkeletonBoneMap);
        }

        return SkeletonBoneMap;
    }

    private void CollectBones(Transform node, Dictionary<string, Transform> map)
    {
        if (!map.ContainsKey(node.name))
            map[node.name] = node;

        foreach (Transform Child in node)
            CollectBones(Child, map);
    }

    // 해당 부위에 생성되어 있던 메쉬 인스턴스를 제거합니다.
    private void ClearEquipVisual(EQUIP_TYPE type)
    {
        if (EquippedVisuals.TryGetValue(type, out GameObject visual) && visual != null)
            Destroy(visual);

        EquippedVisuals.Remove(type);
    }

    // 부위별로 장비 메쉬가 붙을 소켓(본)을 반환합니다.
    private Transform GetEquipSocket(EQUIP_TYPE type)
    {
        switch (type)
        {
            case EQUIP_TYPE.WEAPON:
            case EQUIP_TYPE.AXE:
                return WeaponSocket;
            default:
                return null;
        }
    }
}

// 콤보 타 하나에 대한 슬래시 이펙트 스폰 위치/회전/수명 설정.
[System.Serializable]
public class SlashEffectSettings
{
    public float Distance = 1.0f;
    public float Height = 0.0f;
    public Vector3 RotationOffset;
    public float Lifetime = 2.0f;
    // 애니메이션이 이 진행도(0~1)를 넘어야 이펙트가 스폰됩니다.
    public float SpawnNormalizedTime = 0.0f;
}

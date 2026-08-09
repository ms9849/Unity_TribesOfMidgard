using UnityEngine;

[RequireComponent(typeof(MonsterController))]
[RequireComponent(typeof(Health))]
public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject hpUIPrefab;
    [SerializeField] private Vector3 hpUIOffset = new Vector3(0f, 5.5f, 0f);
    [SerializeField] private string monsterName;
    public MonsterController monsterController { get; private set; }
    public Health monsterHealth { get; private set; }
    public IMonsterAttack monsterAttack { get; private set; }
    public IMonsterProjectile monsterProjectile { get; private set; } = null;
    public Animator monsterAnimator { get; private set; }
    MonsterStateMachine monsterFSM;

    void Awake()
    {
        monsterController = GetComponent<MonsterController>();
        monsterHealth = GetComponent<Health>();
        monsterAttack = GetComponent<IMonsterAttack>();
        monsterProjectile = GetComponent<IMonsterProjectile>();
        monsterAnimator = GetComponentInChildren<Animator>();
        monsterFSM = new MonsterStateMachine(this);
    }

    void Start()
    {
        monsterHealth.OnDeath += HandleDeath;
        SpawnHPUI();
    }

    // 몬스터 머리 위에 체력바 UI를 생성하고 자신의 Health를 타겟으로 연결합니다.
    void SpawnHPUI()
    {
        if (hpUIPrefab == null)
            return;

        GameObject HPUIInstance = Instantiate(hpUIPrefab, transform);
        HPUI HPUIComponent = HPUIInstance.GetComponent<HPUI>();

        if (HPUIComponent != null)
        {
            HPUIComponent.TransformOffset = hpUIOffset;
            HPUIComponent.SetTarget(gameObject);
        }
    }

    void OnDestroy()
    {
        if (monsterHealth != null)
            monsterHealth.OnDeath -= HandleDeath;

            
        QuestManager.OnEnemyKilled?.Invoke(monsterName);
    }

    void HandleDeath()
    {
        monsterFSM.ChangeState(MonsterStateID.Dead);
    }

    void Update()
    {
        monsterFSM.Update();
    }

    void FixedUpdate()
    {
        monsterFSM.FixedUpdate();
    }
}

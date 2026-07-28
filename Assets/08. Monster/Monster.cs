using UnityEngine;

[RequireComponent(typeof(MonsterController))]
[RequireComponent(typeof(Health))]
public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject hpUIPrefab;

    public MonsterController monsterController { get; private set; }
    public Health monsterHealth { get; private set; }
    public IMonsterAttack monsterAttack { get; private set; }
    public Animator monsterAnimator { get; private set; }
    MonsterStateMachine monsterFSM;

    void Awake()
    {
        monsterController = GetComponent<MonsterController>();
        monsterHealth = GetComponent<Health>();
        monsterAttack = GetComponent<IMonsterAttack>();
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
        HPUIInstance.GetComponent<HPUI>()?.SetTarget(gameObject);
    }

    void OnDestroy()
    {
        if (monsterHealth != null)
            monsterHealth.OnDeath -= HandleDeath;
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

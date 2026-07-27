# 전투 시스템 구현 메모 (플레이어 ↔ 몬스터)

> 이 문서는 실제 구현 전에 정리한 설계 메모입니다. 코드는 없고, 어떤 파일에 어떤 메서드를 추가/수정해야 하는지와 그 이유만 정리했습니다.

## 0. 현재 상태 요약 (조사 결과)

* `Health` / `IDamageable`(`Assets/Scripts/Combat/`)은 이미 플레이어·몬스터 양쪽에서 재사용 가능한 형태로 잘 분리되어 있음. **그대로 사용.**
* 몬스터 → 플레이어: `MonsterMeleeAttack.Attack()`이 사거리 체크 후 즉시 `TakeDamage` 호출 (애니메이션 타이밍과 무관). **건드리지 않음.**
* 플레이어 → 몬스터: 현재 전혀 없음. `PlayerSwordAttackState`는 콤보 애니메이션 전환만 함.
* `PlayerIdleState`에서 `IsWeaponEquipped()`가 true여야 Attack 상태 진입 가능 → **Attack 상태 안에서는 WEAPON이 항상 장착되어 있음이 보장됨.**
* 플레이어 사망 처리 없음: `Player.cs`는 `Health`를 참조하지 않고, `OnDeath` 구독도 없음. `StateID`에 `Dead`도 없음.
* `ItemSO`에는 데미지 관련 필드가 없음 (`DefenseBonus`, `MaxHpBonus`만 존재).

## 1. 결정된 방향

| 항목 | 결정 |
|---|---|
| 공격 판정 방식 | 무기에 트리거 콜라이더 상시 부착 + `OnTriggerEnter` |
| 데미지 수치 출처 | `ItemSO`(무기)에 데미지 필드 추가 |
| 몬스터 히트 리액션 | 지금은 넣지 않음 (추후 과제) |
| 플레이어 사망 처리 | Player FSM에 `Dead` 상태 추가 |

---

## 2. 파일별 변경 목록

### 2-1. `Assets/Scripts/Item/ItemSO.cs` (수정)
* `public float WeaponDamage;` 필드 추가 (`DefenseBonus`/`MaxHpBonus` 옆에 "공격 스텟" 헤더로).
* `EquipType == EQUIP_TYPE.WEAPON`인 아이템에서만 의미 있는 값. 도끼(AXE)는 지금 범위에서는 데미지 없음(공격 자체가 안 됨 — Attack 상태는 WEAPON 장착 시에만 진입).

### 2-2. 신규 컴포넌트: `WeaponHitbox` (예: `Assets/Scripts/Combat/WeaponHitbox.cs`)
무기 프리팹(`ItemSO.EquipModel`)의 칼날 부분에 자식 오브젝트로 붙는 트리거 콜라이더용 스크립트. `MonsterMeleeAttack`과 대칭되는 "무기 쪽 공격 판정" 역할.

정의할 것:
* `Collider`는 `isTrigger = true`, 기본 비활성 상태(또는 `IsArmed = false`)로 시작.
* `SetOwnerAndDamage(GameObject attacker, float damage)` — 누가 휘두르는 무기인지, 데미지가 얼마인지 세팅. 무기 장착 시 1회 호출.
* `Arm()` — 판정 시작. 내부적으로 "이번 스윙에서 이미 맞춘 대상" 기록용 컬렉션(`HashSet<IDamageable>` 등)을 초기화하고 `IsArmed = true`.
* `Disarm()` — 판정 종료. `IsArmed = false`.
* `OnTriggerEnter(Collider other)` — `IsArmed`가 아니면 무시. `other`(또는 부모)에서 `IDamageable` 탐색 → 이미 맞춘 대상이면 무시 → 아니면 기록 후 `TakeDamage(damage, attacker)` 호출.
* 콜라이더가 자기 자신(플레이어)에 안 맞도록 레이어/태그 필터 또는 attacker 비교 필요.

### 2-3. `PlayerController.cs` (수정)
* 무기 장착 시(`EquipItem` 내부, `EquipType == WEAPON`일 때) 방금 생성된 무기 비주얼에서 `WeaponHitbox`를 찾아 `SetOwnerAndDamage(gameObject, item.WeaponDamage)` 호출. (`RecalculateStats()`와 비슷한 타이밍에 처리하면 자연스러움.)
* `public WeaponHitbox GetEquippedWeaponHitbox()` 같은 조회용 메서드 추가 — `EquippedVisuals[EQUIP_TYPE.WEAPON]`에서 `GetComponentInChildren<WeaponHitbox>()`. `PlayerSwordAttackState`가 Arm/Disarm 호출할 때 사용.

### 2-4. `PlayerSwordAttackState.cs` (수정)
* `Enter()`: 무기 히트박스 참조를 가져와 `Arm()` 호출 (1콤보 시작).
* 콤보 전환 지점(`SwordAttack2`, `SwordAttack3`로 `Play()` 호출하는 곳) 각각에서 다시 `Arm()` 호출 — "스윙당 1회"를 콤보 히트마다 리셋하기 위함 (안 하면 2타/3타에서 이미 맞춘 몬스터를 못 맞춤).
* `Exit()`: `Disarm()` 호출 — 콤보가 끝나거나 취소될 때 판정 종료.

### 2-5. 플레이어 사망 처리
* `Assets/00. Player/States/PlayerState.cs`의 `StateID` enum에 `Dead` 추가 (`End` 바로 앞).
* 신규 `PlayerDeadState.cs` (`MonsterDeadState`와 대칭 구조):
  * `Enter()`: 이동/입력 정지(리지드바디 속도 0, 필요하면 `PlayerController`에 `IsDead` 플래그 추가해 입력 콜백 무시), 사망 애니메이션 재생.
  * `Update()`는 오버라이드하지 않음 → base가 no-op이라 자동으로 상태가 멈춰서 Dead에서 못 빠져나감 (`MonsterDeadState`와 동일한 트릭).
  * 게임오버 UI/리스폰 트리거는 여기서 호출 (지금 범위 밖이면 TODO 주석만 남겨도 됨).
* `PlayerStateMachine.CreateStates()` switch에 `case StateID.Dead: States[(int)State] = new PlayerDeadState(this); break;` 추가.
* `Player.cs`:
  * `Health` 캐싱 추가 (`Awake()`에서 `GetComponent<Health>()`).
  * `Start()`에서 `playerHealth.OnDeath += HandleDeath;` 구독.
  * `HandleDeath() { playerFSM.ChangeState(StateID.Dead); }` — `Monster.cs`의 `HandleDeath`와 완전히 동일한 패턴.
  * `OnDestroy()`에서 구독 해제.

---

## 3. 핵심 흐름 정리

**공격 판정 흐름**
1. 무기 장착 → `PlayerController`가 `WeaponHitbox`에 데미지/오너 세팅.
2. `Attack` 상태 진입/콤보 전환 시 → `WeaponHitbox.Arm()`.
3. 스윙 중 콜라이더가 몬스터 콜라이더와 겹침 → `OnTriggerEnter`에서 `IDamageable.TakeDamage()` 호출 → 몬스터 `Health`가 데미지 처리 (`OnDamaged`/`OnDeath` 이벤트는 기존 로직 그대로 작동).
4. 콤보 전환/상태 종료 시 → `Disarm()`.

**플레이어 사망 흐름**
1. 몬스터 공격 → 플레이어 `Health.TakeDamage()` → HP 0 → `OnDeath` 발생.
2. `Player.HandleDeath()` → FSM `Dead` 전이.
3. `PlayerDeadState.Enter()`가 입력 차단 + 사망 연출, 이후 상태 고착.

---

## 4. 구현 순서 제안
1. `ItemSO.WeaponDamage` 필드 추가 → 기존 무기 SO 에셋에 값 채우기.
2. `WeaponHitbox` 작성 → 무기 프리팹에 콜라이더+스크립트 부착, Inspector에서 칼날 크기/위치 조정.
3. `PlayerController`에 장착 시 세팅 로직 + 조회 메서드 추가.
4. `PlayerSwordAttackState`에 Arm/Disarm 연결 → Play 모드에서 몬스터가 실제로 맞는지 확인 (Console/`Health.OnDamaged` 로그로 검증).
5. `StateID.Dead` + `PlayerDeadState` + `Player.cs` 구독 추가 → 몬스터 공격으로 플레이어 HP를 0까지 깎아 사망 전이 확인.

## 5. 미결/추가 고려사항 (지금 범위 밖, 필요 시 별도 논의)
* 몬스터 히트 리액션(스태거) — 이번엔 제외하기로 함.
* 넉백, 히트스탑, 사운드/이펙트 훅(최근 추가된 사운드매니저와 연동) — 미정.
* 도끼(AXE)로도 공격 가능하게 할지 여부 — 현재는 WEAPON만 Attack 진입 가능.
* 플레이어 사망 후 리스폰/게임오버 UI 흐름 — `PlayerDeadState.Enter()`가 훅 지점이 될 뿐, 실제 UI/리스폰 로직은 별도 설계 필요.

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

---

## 3. 핵심 흐름 정리

**공격 판정 흐름**
1. 무기 장착 → `PlayerController`가 `WeaponHitbox`에 데미지/오너 세팅.
2. `Attack` 상태 진입/콤보 전환 시 → `WeaponHitbox.Arm()`.
3. 스윙 중 콜라이더가 몬스터 콜라이더와 겹침 → `OnTriggerEnter`에서 `IDamageable.TakeDamage()` 호출 → 몬스터 `Health`가 데미지 처리 (`OnDamaged`/`OnDeath` 이벤트는 기존 로직 그대로 작동).
4. 콤보 전환/상태 종료 시 → `Disarm()`.

---

## 4. 구현 순서 제안
1. `ItemSO.WeaponDamage` 필드 추가 → 기존 무기 SO 에셋에 값 채우기.
2. `WeaponHitbox` 작성 → 무기 프리팹에 콜라이더+스크립트 부착, Inspector에서 칼날 크기/위치 조정.
3. `PlayerController`에 장착 시 세팅 로직 + 조회 메서드 추가.
4. `PlayerSwordAttackState`에 Arm/Disarm 연결 → Play 모드에서 몬스터가 실제로 맞는지 확인 (Console/`Health.OnDamaged` 로그로 검증).
5. `StateID.Dead` + `PlayerDeadState` + `Player.cs` 구독 추가 → 몬스터 공격으로 플레이어 HP를 0까지 깎아 사망 전이 확인.

---

## 5. 몬스터 공격 타입 다양화 (예: FireGiant)

### 5-0. 조사 결과
* `IMonsterAttack`(`Assets/08. Monster/Attacks/IMonsterAttack.cs`)은 `Attack(Transform target)` 한 메서드뿐인 인터페이스. `Monster.cs`가 `GetComponent<IMonsterAttack>()`로 루트 오브젝트에서 다형적으로 가져와 사용 → **몬스터 프리팹마다 다른 구현체를 붙이기만 하면 되는 구조. FSM/`Monster.cs` 수정 불필요.**
* 기존 `MonsterMeleeAttack`은 `Attack()` 호출 즉시(애니메이션 타이밍 무관) `TakeDamage`를 호출함. FireGiant처럼 "타격 시점에 맞춰 판정"하려는 공격에는 이 패턴을 그대로 못 씀 → **새 컴포넌트로 분리, 기존 `MonsterMeleeAttack`은 건드리지 않음.**
* `MonsterMeleeAttack`은 루트 오브젝트에 붙고 `animator = GetComponentInChildren<Animator>()`로 자식 모델의 Animator를 참조함. **Animation Event는 Animator가 붙은 그 자식 GameObject를 대상으로 SendMessage하므로, 루트의 공격 컴포넌트로 직접 이벤트를 못 보냄 → 자식 쪽에 작은 릴레이 컴포넌트 필요.**
* FireGiant는 현재 모델/애니메이터 컨트롤러 에셋만 존재, 전투 스크립트/프리팹 세팅 없음 (새로 구성 필요).
* 판정 방식은 즉발형(Animation Event 시점에 `OverlapSphere`/`OverlapBox` 1회 판정)으로 결정 — 지속형 콜라이더(화염 장판 등)는 지금 범위 밖.

### 5-1. 신규 파일: `Assets/08. Monster/Attacks/MonsterAoeAttack.cs`
* `MonoBehaviour, IMonsterAttack, IAnimationHitReceiver` (아래 5-3 인터페이스 구현).
* 필드: `damage`, `attackCooldown`, `hitRadius`, `hitPoint`(Transform, 비어있으면 `transform` 사용), `targetLayer`(LayerMask, Player만 지정).
* `Attack(Transform target)`: 쿨다운 체크 → `lastAttackTime` 갱신 → `animator.SetTrigger("Attack")`만 하고 **데미지는 주지 않음** (`MonsterMeleeAttack`과의 핵심 차이).
* `OnAttackHit()`: `Physics.OverlapSphere(hitPoint.position, hitRadius, targetLayer)` 결과를 순회하며 `IDamageable`을 찾아 `TakeDamage(damage, gameObject)` 호출. 같은 대상이 콜라이더 여러 개로 중복 히트되지 않게 순회 중 `HashSet<IDamageable>`로 중복 제거.

### 5-2. 신규 파일: `Assets/08. Monster/Attacks/IAnimationHitReceiver.cs`
* `void OnAttackHit();` 하나만 선언. Animation Event 릴레이가 부모 쪽 공격 컴포넌트를 타입에 의존하지 않고 호출하기 위한 최소 인터페이스.

### 5-3. 신규 파일: `Assets/08. Monster/Attacks/MonsterAttackAnimationRelay.cs`
* Animator가 붙은 자식 모델 오브젝트에 부착.
* `OnAttackHit()` (Animation Event가 호출할 이름과 동일) → `GetComponentInParent<IAnimationHitReceiver>()?.OnAttackHit()`로 전달만 함.

### 5-4. 에셋 세팅 (Editor 작업, 구현 단계에서 진행)
* FireGiant 프리팹 루트에 `MonsterController`, `Health`, `MonsterAoeAttack` 부착 (`MonsterMeleeAttack` 대신).
* 자식 모델(Animator 소유) 오브젝트에 `MonsterAttackAnimationRelay` 부착.
* `FireGiant_Animator.controller`의 Attack 클립에서 타격 프레임에 Animation Event 추가, 함수명 `OnAttackHit` 지정.
* `hitPoint`는 타격 지점(손/무기 근처) 트랜스폼으로 지정, `hitRadius`/`damage`/`targetLayer` Inspector에서 조정.

### 5-5. 확인 방법
* Play 모드에서 FireGiant 공격 애니메이션 재생 중 타격 프레임에서만 플레이어가 맞는지 확인 (애니메이션 시작 즉시가 아니라).
* `Health.OnDamaged` 로그 또는 Console로 정확히 프레임당 1회만 데미지가 들어가는지 검증 (중복 히트 없음).

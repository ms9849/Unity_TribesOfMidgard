# Player System

## Player FSM (`Assets/00. Player/`)
* **`PlayerStateMachine : StateMachine`**: Owns a fixed-size array of `PlayerState` indexed by `StateID` enum (`Idle, Walk, Run, Attack, WheelWind, Collect, End`), built in `CreateStates()`. `ChangeState` handles `Exit()`/`Enter()`.
* **`PlayerState : State`**: Per-state base (`Assets/00. Player/States/PlayerState.cs`). Holds refs to `Player` and `PlayerStateMachine`. Concrete states check transitions in `Update()` by reading input flags from `PlayerController`.
* **`PlayerSwordAttackState`**: Implements a 3-hit combo using an `ATTACK_COMBO` enum and animation-normalized-time checks against Animator states (`SwordAttack1/2/3`). Does *not* spawn separate FSM states per combo hit.
* **Root Motion**: Opt-in per state via `PlayerController.isRootMotionEnabled` (e.g., true during attack) and consumed in `PlayerController.OnAnimatorMove()`.

## Player Component Composition
* **`Player.cs`**: Orchestrator MonoBehaviour. Caches components (`Animator`, `PlayerController`, `Inventory`) in `Awake()`, owns the `PlayerStateMachine`, and drives it from `Update()`.
    * *Note:* Contains test code gated under `TestWood` (pressing `J` adds an item). Marked with `/*TEST CODE*/`. Treat as scaffolding only.
* **`PlayerController.cs`**: Input/state surface. Receives Input System callbacks (`OnMove`, `OnAttack`, etc.) and exposes them as public properties/flags for FSM states to poll and clear.
* **Input Assets**: `Assets/00. Player/PlayerInputAction.inputactions` (Active) | `Assets/InputSystem_Actions.inputactions` (Unity default, largely unused).
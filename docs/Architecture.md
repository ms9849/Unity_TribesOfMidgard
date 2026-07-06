# Core Architecture

## Generic FSM Base (`Assets/Scripts/States/`)
* **`State`**: Abstract base class (`Enter`, `Exit`, `Update`).
* **`StateMachine`**: Abstract base class (`Update`).
* Minimal, domain-agnostic bases. All concrete state machines derive from these.

## Assembly Structure
* No `.asmdef` files exist in this project.
* All runtime scripts compile into the single default `Assembly-CSharp` assembly.
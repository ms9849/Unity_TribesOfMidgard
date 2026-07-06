# Project Overview
A Unity URP action/survival-crafting game (Tribes of Midgard-inspired), built with Unity **6000.3.14f1**. Uses the new Input System, Cinemachine, Timeline, Polybrush, and Splines. All building, running, and testing happens through the Unity Editor or via UnityMCP tools.

## Language Convention
* Commit messages and in-code comments are written in **Korean**. Follow this convention when writing them for this repository.
* General communication with the user can be in Korean, but code structure should follow standard naming conventions.

---

## AI Behavioral Guidelines (Tradeoff: Bias toward caution over speed)

### 1. Think Before Coding
* **Don't assume. Don't hide confusion. Surface tradeoffs.**
* Before implementing: State your assumptions explicitly. If uncertain, ask.
* If multiple interpretations exist, present them - don't pick silently.
* If a simpler approach exists, say so. Push back when warranted.
* If something is unclear, stop. Name what's confusing. Ask.

### 2. Simplicity First
* **Minimum code that solves the problem. Nothing speculative.**
* No features beyond what was asked. No abstractions for single-use code.
* No "flexibility" or "configurability" that wasn't requested.
* If you write 200 lines and it could be 50, rewrite it. 
* Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

### 3. Surgical Changes
* **Touch only what you must. Clean up only your own mess.**
* When editing existing code or Unity components:
  * Don't "improve" adjacent code, comments, or formatting.
  * Don't refactor things that aren't broken. Match existing style.
  * If you notice unrelated dead code, mention it - don't delete it.
* When your changes create orphans:
  * Remove imports/variables/functions that YOUR changes made unused.
  * Don't remove pre-existing dead code unless asked.
* *The test:* Every changed line/YAML modification should trace directly to the user's request.

### 4. Goal-Driven Execution
* **Define success criteria. Loop until verified.**
* *Note: Since no test assembly is set up, use Unity Editor, Play Mode, or Console Logs for verification.*
* Transform tasks into verifiable goals:
  * "Add validation" → "Write debug logs for invalid inputs, then verify via Editor"
  * "Fix the bug" → "Define how to reproduce it in Play Mode, then confirm the fix"
* For multi-step tasks, state a brief plan before executing:
  1. [Step] → verify: [Unity Console/Editor check]
  2. [Step] → verify: [Unity Console/Editor check]

---

## Working with this Project

* **UnityMCP Tools:** This project uses **UnityMCP** (`com.coplaydev.unity-mcp`). Prefer these MCP tools over hand-editing `.unity`/`.prefab` YAML files. Check `mcpforunity://custom-tools` for project-specific commands.
* **Compilation:** After creating/editing a script, use `read_console` or poll `editor_state.isCompiling` to confirm compilation succeeded before referencing new types (requires a domain reload).
* **Testing:** No test assembly is set up. Rely on Editor state and Play Mode testing.

---

## Documentation Index
Read the following files as needed for specific systemic contexts:
* **[Core Architecture](docs/Architecture.md):** Generic FSM base and assembly structure.
* **[Player System](docs/Player.md):** Player FSM, states, and component composition.
* **[Inventory & UI](docs/Inventory_UI.md):** Item definitions, slot logic, and custom grid UI.
* **[Interaction System](docs/Interaction.md):** World interaction triggers and logic.
* **[Scriptable Objects](docs/ScriptableObjects.md):** Data-driven design patterns for items/interactions.
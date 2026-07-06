# Interaction System

**Path:** `Assets/Scripts/Interaction/`

* **`Interaction` (MonoBehaviour)**: Placed on trigger colliders. 
    * Reads its assigned `InteractionSO`.
    * Drives a floating world-space UI canvas (name/type/sprite).
    * Sets and clears `PlayerController.CurrentInteractionObject` during `OnTriggerEnter`/`OnTriggerExit` with objects tagged `"Player"`.
* **FSM Integration**: Player states read `CurrentInteractionObject.InteractionData.InteractionType` to decide whether to transition into the `Collect` state.
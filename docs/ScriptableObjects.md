# Scriptable Objects (Data-Driven Design)

Both gameplay data types follow the same `[CreateAssetMenu(... menuName = "Scriptable Objects/...")]` pattern to expose designer-facing fields. 

**Rule:** Always follow this SO pattern for new item/interactable types rather than hardcoding data in components.

## Implementations
* **`InteractionSO`**: Data definition for an interactable. Contains name, sprite, and `INTERACTION_TYPE` (`WOOD`, `STONE`, `NPC`).
* **`ItemSO`**: Item data definition. Contains sprite, name, and `IsGatherable` boolean.
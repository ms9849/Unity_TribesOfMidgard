# Item & Inventory System

## Inventory Logic (`Assets/Scripts/Item/`)
* **`Slot`**: Plain serializable class holding one `ItemSO` and a count. Exposes an `OnSlotUpdated` `Action` fired from `SetItem()`—the sole change-notification mechanism for UI.
* **`Inventory`**: MonoBehaviour owning a fixed-size `List<Slot>` (`MaxSlots = 25`). Handles linear-scan first-fit stacking in `AddItem`.

## UI Implementation
* **`InventoryUI`**: Instantiates one `SlotUI` prefab per inventory slot. Lays them out manually using `anchoredPosition` math (`Columns`, `CellSize`, `Spacing`, top-left anchored). 
* **`SlotUI`**: Each slot subscribes to its own `Slot.OnSlotUpdated` to refresh its `Image` independently. There is no central inventory-changed event.
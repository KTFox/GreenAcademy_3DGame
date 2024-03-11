using RPG.Inventories;
using UnityEngine;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]
        private InventorySlotUI inventorySlotPrefab;

        private Inventory playerInventory;

        private void Awake()
        {
            playerInventory = Inventory.PlayerInventory;
        }

        private void OnEnable()
        {
            playerInventory.OnInventoryUpdated += Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerInventory.SlotSize; i++)
            {
                var inventorySlot = Instantiate(inventorySlotPrefab, transform);
                inventorySlot.SetUp(playerInventory, i);
            }
        }
    }
}

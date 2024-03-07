using UnityEngine;
using RPG.Inventories;
using RPG.Utility.UI;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// Handles spawning pickups when _item dropped into the world.
    /// Must be placed on the roof canvas where items can be dragged. 
    /// Will be called if dropped over empty space. 
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItemSO>
    {
        #region IDragDestination implements
        public void AddItems(InventoryItemSO item, int number)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<ItemDropper>().DropItem(item, number);
        }

        public int GetMaxAcceptable(InventoryItemSO item)
        {
            return int.MaxValue;
        }
        #endregion
    }
}
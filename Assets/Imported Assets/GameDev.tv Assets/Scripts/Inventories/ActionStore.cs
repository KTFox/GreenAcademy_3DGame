﻿using System;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// Provides the storage for an action bar. The bar has a finite quantity of
    /// slots that can be filled and actions in the slots can be "used".
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class ActionStore : MonoBehaviour, ISaveable
    {
        // STATE
        Dictionary<int, DockedItemSlot> dockedItems = new Dictionary<int, DockedItemSlot>();
        private class DockedItemSlot 
        {
            public ActionItem item;
            public int number;
        }

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action storeUpdated;

        /// <summary>
        /// Get the action at the given _slotIndex.
        /// </summary>
        public ActionItem GetAction(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].item;
            }
            return null;
        }

        /// <summary>
        /// Get the quantity of items left at the given _slotIndex.
        /// </summary>
        /// <returns>
        /// Will return 0 if no _inventoryItem is in the _slotIndex or the _inventoryItem has
        /// been fully consumed.
        /// </returns>
        public int GetNumber(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].number;
            }
            return 0;
        }

        /// <summary>
        /// Add an _inventoryItem to the given _slotIndex.
        /// </summary>
        /// <param name="item">What _inventoryItem should be added.</param>
        /// <param name="index">Where should the _inventoryItem be added.</param>
        /// <param name="number">How many items to add.</param>
        public void AddAction(InventoryItem item, int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {  
                if (object.ReferenceEquals(item, dockedItems[index].item))
                {
                    dockedItems[index].number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.item = item as ActionItem;
                slot.number = number;
                dockedItems[index] = slot;
            }
            if (storeUpdated != null)
            {
                storeUpdated();
            }
        }

        /// <summary>
        /// UseActionItem the _inventoryItem at the given slot. If the _inventoryItem is consumable one
        /// instance will be destroyed until the _inventoryItem is removed completely.
        /// </summary>
        /// <param name="user">The character that wants to use this action.</param>
        /// <returns>False if the action could not be executed.</returns>
        public bool Use(int index, GameObject user)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].item.Use(user);
                if (dockedItems[index].item.isConsumable())
                {
                    RemoveItems(index, 1);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a given quantity of items from the given slot.
        /// </summary>
        public void RemoveItems(int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].number -= number;
                if (dockedItems[index].number <= 0)
                {
                    dockedItems.Remove(index);
                }
                if (storeUpdated != null)
                {
                    storeUpdated();
                }
            }
            
        }

        /// <summary>
        /// What is the maximum quantity of items allowed in this slot.
        /// 
        /// This takes into account whether the slot already contains an _inventoryItem
        /// and whether it is the same type. Will only accept multiple if the
        /// _inventoryItem is consumable.
        /// </summary>
        /// <returns>Will return int.MaxValue when there is not effective bound.</returns>
        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;

            if (dockedItems.ContainsKey(index) && !object.ReferenceEquals(item, dockedItems[index].item))
            {
                return 0;
            }
            if (actionItem.isConsumable())
            {
                return int.MaxValue;
            }
            if (dockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }

        /// PRIVATE

        [System.Serializable]
        private struct DockedItemRecord
        {
            public string itemID;
            public int number;
        }

        object ISaveable.CaptureState()
        {
            var state = new Dictionary<int, DockedItemRecord>();
            foreach (var pair in dockedItems)
            {
                var record = new DockedItemRecord();
                record.itemID = pair.Value.item.GetItemID();
                record.number = pair.Value.number;
                state[pair.Key] = record;
            }
            return state;
        }

        void ISaveable.RestoreState(object state)
        {
            var stateDict = (Dictionary<int, DockedItemRecord>)state;
            foreach (var pair in stateDict)
            {
                AddAction(InventoryItem.GetFromID(pair.Value.itemID), pair.Key, pair.Value.number);
            }
        }
    }
}
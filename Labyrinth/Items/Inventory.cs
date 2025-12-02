using System.Diagnostics.CodeAnalysis;

namespace Labyrinth.Items
{
    /// <summary>
    /// Inventory of collectable items for rooms and players.
    /// </summary>
    /// <param name="item">Optional initial item in the inventory.</param>
    public abstract class Inventory(ICollectable? item = null)
    {
        /// <summary>
        /// True if the inventory has items, false otherwise.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_items))]
        public bool HasItems => _items.Count > 0;

        /// <summary>
        /// Gets the types of the items in the inventory.
        /// </summary>
        public IEnumerable<Type> ItemTypes => _items.Select(i => i.GetType());

        /// <summary>
        /// Places an item in the inventory, removing it from another one.
        /// </summary>
        /// <param name="from">The inventory from which the item is taken. The item is removed from this inventory.</param>
        /// <param name="nth">The index of the item to take from the source inventory (0-based).</param>
        /// <exception cref="InvalidOperationException">Thrown if the source inventory has no items (check with <see cref="HasItems"/>).</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if nth is out of range.</exception>
        public void MoveItemFrom(Inventory from, int nth = 0)
        {
            if (!from.HasItems)
            {
                throw new InvalidOperationException("No item to take from the source inventory");
            }
            if (nth < 0 || nth >= from._items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(nth), "Index out of range");
            }
            var item = from._items[nth];
            from._items.RemoveAt(nth);
            _items.Add(item);
        }

        /// <summary>
        /// Swaps items between inventories
        /// </summary>
        /// <param name="from">The inventory to swap items with</param>
        public void SwapItems(Inventory from)
        {
            var tmp = _items;
            _items = from._items;
            from._items = tmp;
        }

        protected List<ICollectable> _items = item != null ? [item] : [];
    }
}

using Labyrinth.Items;
using Labyrinth.Tiles;

namespace Labyrinth.Build
{
    /// <summary>
    /// Manage the creation of doors and key rooms ensuring each door has a corresponding key room.
    /// </summary>
    public sealed class Keymaster : IDisposable
    {
        /// <summary>
        /// Ensure all created doors have a corresponding key room and vice versa.
        /// </summary>
        /// <exception cref="InvalidOperationException">Some keys are missing or are not placed.</exception>
        public void Dispose()
        {
            if (_unplacedKeys.Count > 0 || _emptyKeyRooms.Count > 0)
            {
                throw new InvalidOperationException("Unmatched key/door creation");
            }
        }

        /// <summary>
        /// Create a new door and place its key in a previously created empty key room (if any).
        /// </summary>
        /// <returns>Created door</returns>
        public Door NewDoor()
        {
            var door = new Door();
            var tempInventory = new MyInventory();
            door.LockAndTakeKey(tempInventory);
            _unplacedKeys.Add(tempInventory);
            PlaceKeys();
            return door;
        }

        /// <summary>
        /// Create a new room with key and place the key if a door was previously created.
        /// </summary>
        /// <returns>Created key room</returns>
        public Room NewKeyRoom()
        {
            var room = new Room();
            _emptyKeyRooms.Add(room);
            PlaceKeys();
            return room;
        }

        private void PlaceKeys()
        {
            while (_unplacedKeys.Count > 0 && _emptyKeyRooms.Count > 0)
            {
                var keyInventory = _unplacedKeys[0];
                var keyRoom = _emptyKeyRooms[0];
                
                keyRoom.Pass().MoveItemFrom(keyInventory);
                
                _unplacedKeys.RemoveAt(0);
                _emptyKeyRooms.RemoveAt(0);
            }
        }

        private readonly List<MyInventory> _unplacedKeys = new();
        private readonly List<Room> _emptyKeyRooms = new();
    }
}
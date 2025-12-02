using Labyrinth.Build;
using Labyrinth.Items;
using Labyrinth.Tiles;

namespace LabyrinthTest.Build;

[TestFixture(Description = "Keymaster unit test class")]
public class KeyMasterTest
{
    #region Dispose Tests
    
    [Test]
    public void Dispose_WithNoDoorsOrKeys_DoesNotThrow()
    {
        var keymaster = new Keymaster();

        Assert.DoesNotThrow(() => keymaster.Dispose());
    }

    [Test]
    public void Dispose_WithMatchedDoorAndKey_DoesNotThrow()
    {
        var keymaster = new Keymaster();
        keymaster.NewDoor();
        keymaster.NewKeyRoom();

        Assert.DoesNotThrow(() => keymaster.Dispose());
    }

    [Test]
    public void Dispose_WithUnplacedDoor_ThrowsInvalidOperationException()
    {
        var keymaster = new Keymaster();
        keymaster.NewDoor();

        var ex = Assert.Throws<InvalidOperationException>(() => keymaster.Dispose());
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    [Test]
    public void Dispose_WithUnplacedKeyRoom_ThrowsInvalidOperationException()
    {
        var keymaster = new Keymaster();
        keymaster.NewKeyRoom();

        var ex = Assert.Throws<InvalidOperationException>(() => keymaster.Dispose());
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    [Test]
    public void Dispose_WithMultipleUnmatchedDoorsAndKeys_ThrowsInvalidOperationException()
    {
        var keymaster = new Keymaster();
        keymaster.NewDoor();
        keymaster.NewDoor();
        keymaster.NewKeyRoom();

        var ex = Assert.Throws<InvalidOperationException>(() => keymaster.Dispose());
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    #endregion

    #region NewDoor Tests

    [Test]
    public void NewDoor_CreatesLockedDoor()
    {
        var keymaster = new Keymaster();

        var door = keymaster.NewDoor();

        Assert.That(door, Is.Not.Null);
        Assert.That(door.IsLocked, Is.True);
        Assert.That(door.IsTraversable, Is.False);
    }

    [Test]
    public void NewDoor_WithExistingKeyRoom_PlacesKeyInRoom()
    {
        var keymaster = new Keymaster();
        var keyRoom = keymaster.NewKeyRoom();

        var door = keymaster.NewDoor();

        using var all = Assert.EnterMultipleScope();
        Assert.That(door.IsLocked, Is.True);
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
    }

    [Test]
    public void NewDoor_Multiple_CreatesMultipleLockedDoors()
    {
        var keymaster = new Keymaster();

        var door1 = keymaster.NewDoor();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();

        using var all = Assert.EnterMultipleScope();
        Assert.That(door1.IsLocked, Is.True);
        Assert.That(door2.IsLocked, Is.True);
        Assert.That(door3.IsLocked, Is.True);
    }

    #endregion

    #region NewKeyRoom Tests

    [Test]
    public void NewKeyRoom_CreatesEmptyRoom()
    {
        var keymaster = new Keymaster();

        var room = keymaster.NewKeyRoom();

        Assert.That(room, Is.Not.Null);
        Assert.That(room.Pass().HasItems, Is.False);
    }

    [Test]
    public void NewKeyRoom_WithExistingDoor_ReceivesKey()
    {
        var keymaster = new Keymaster();
        var door = keymaster.NewDoor();

        var keyRoom = keymaster.NewKeyRoom();

        using var all = Assert.EnterMultipleScope();
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
        Assert.That(door.IsLocked, Is.True);
    }

    [Test]
    public void NewKeyRoom_Multiple_CreatesMultipleRooms()
    {
        var keymaster = new Keymaster();

        var room1 = keymaster.NewKeyRoom();
        var room2 = keymaster.NewKeyRoom();
        var room3 = keymaster.NewKeyRoom();

        using var all = Assert.EnterMultipleScope();
        Assert.That(room1, Is.Not.Null);
        Assert.That(room2, Is.Not.Null);
        Assert.That(room3, Is.Not.Null);
    }

    #endregion

    #region Integration Tests - Door/Key Matching

    [Test]
    public void NewDoorThenNewKeyRoom_KeyIsPlacedInRoom()
    {
        var keymaster = new Keymaster();

        var door = keymaster.NewDoor();
        var keyRoom = keymaster.NewKeyRoom();

        using var all = Assert.EnterMultipleScope();
        Assert.That(door.IsLocked, Is.True);
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
    }

    [Test]
    public void NewKeyRoomThenNewDoor_KeyIsPlacedInRoom()
    {
        var keymaster = new Keymaster();

        var keyRoom = keymaster.NewKeyRoom();
        var door = keymaster.NewDoor();

        using var all = Assert.EnterMultipleScope();
        Assert.That(door.IsLocked, Is.True);
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
    }

    [Test]
    public void MultipleDoorsBeforeKeys_AllKeysArePlacedCorrectly()
    {
        var keymaster = new Keymaster();

        var door1 = keymaster.NewDoor();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();
        var keyRoom1 = keymaster.NewKeyRoom();
        var keyRoom2 = keymaster.NewKeyRoom();
        var keyRoom3 = keymaster.NewKeyRoom();

        using var all = Assert.EnterMultipleScope();
        Assert.That(door1.IsLocked, Is.True);
        Assert.That(door2.IsLocked, Is.True);
        Assert.That(door3.IsLocked, Is.True);
        Assert.That(keyRoom1.Pass().HasItems, Is.True);
        Assert.That(keyRoom2.Pass().HasItems, Is.True);
        Assert.That(keyRoom3.Pass().HasItems, Is.True);
    }

    [Test]
    public void MultipleKeysBeforeDoors_AllKeysArePlacedCorrectly()
    {
        var keymaster = new Keymaster();

        var keyRoom1 = keymaster.NewKeyRoom();
        var keyRoom2 = keymaster.NewKeyRoom();
        var keyRoom3 = keymaster.NewKeyRoom();
        var door1 = keymaster.NewDoor();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();

        using var all = Assert.EnterMultipleScope();
        Assert.That(door1.IsLocked, Is.True);
        Assert.That(door2.IsLocked, Is.True);
        Assert.That(door3.IsLocked, Is.True);
        Assert.That(keyRoom1.Pass().HasItems, Is.True);
        Assert.That(keyRoom2.Pass().HasItems, Is.True);
        Assert.That(keyRoom3.Pass().HasItems, Is.True);
    }

    [Test]
    public void InterleavedDoorsAndKeys_AllKeysArePlacedCorrectly()
    {
        var keymaster = new Keymaster();

        var door1 = keymaster.NewDoor();
        var keyRoom1 = keymaster.NewKeyRoom();
        var keyRoom2 = keymaster.NewKeyRoom();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();
        var keyRoom3 = keymaster.NewKeyRoom();

        using var all = Assert.EnterMultipleScope();
        Assert.That(door1.IsLocked, Is.True);
        Assert.That(door2.IsLocked, Is.True);
        Assert.That(door3.IsLocked, Is.True);
        Assert.That(keyRoom1.Pass().HasItems, Is.True);
        Assert.That(keyRoom2.Pass().HasItems, Is.True);
        Assert.That(keyRoom3.Pass().HasItems, Is.True);
    }

    [Test]
    public void KeyCanOpenCorrespondingDoor()
    {
        var keymaster = new Keymaster();
        var door = keymaster.NewDoor();
        var keyRoom = keymaster.NewKeyRoom();
        var playerInventory = new MyInventory();

        playerInventory.MoveItemFrom(keyRoom.Pass());
        var canOpen = door.Open(playerInventory);

        using var all = Assert.EnterMultipleScope();
        Assert.That(canOpen, Is.True);
        Assert.That(door.IsOpened, Is.True);
        Assert.That(door.IsTraversable, Is.True);
    }

    [Test]
    public void FiveDoorsAndFiveKeys_AllMatch()
    {
        var keymaster = new Keymaster();
        var doors = new Door[5];
        var keyRooms = new Room[5];

        for (int i = 0; i < 5; i++)
        {
            doors[i] = keymaster.NewDoor();
        }
        for (int i = 0; i < 5; i++)
        {
            keyRooms[i] = keymaster.NewKeyRoom();
        }

        using var all = Assert.EnterMultipleScope();
        for (int i = 0; i < 5; i++)
        {
            Assert.That(doors[i].IsLocked, Is.True, $"Door {i} should be locked");
            Assert.That(keyRooms[i].Pass().HasItems, Is.True, $"KeyRoom {i} should have a key");
        }
        Assert.DoesNotThrow(() => keymaster.Dispose());
    }

    #endregion

    #region Using Statement Tests

    [Test]
    public void UsingStatement_WithBalancedDoorsAndKeys_DisposesSuccessfully()
    {
        Assert.DoesNotThrow(() =>
        {
            using var keymaster = new Keymaster();
            keymaster.NewDoor();
            keymaster.NewKeyRoom();
        });
    }

    [Test]
    public void UsingStatement_WithUnmatchedDoor_ThrowsOnDispose()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var keymaster = new Keymaster();
            keymaster.NewDoor();
        });
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    #endregion
}
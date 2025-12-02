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
        // Arrange
        var keymaster = new Keymaster();

        // Act & Assert
        Assert.DoesNotThrow(() => keymaster.Dispose());
    }

    [Test]
    public void Dispose_WithMatchedDoorAndKey_DoesNotThrow()
    {
        // Arrange
        var keymaster = new Keymaster();
        keymaster.NewDoor();
        keymaster.NewKeyRoom();

        // Act & Assert
        Assert.DoesNotThrow(() => keymaster.Dispose());
    }

    [Test]
    public void Dispose_WithUnplacedDoor_ThrowsInvalidOperationException()
    {
        // Arrange
        var keymaster = new Keymaster();
        keymaster.NewDoor();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => keymaster.Dispose());
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    [Test]
    public void Dispose_WithUnplacedKeyRoom_ThrowsInvalidOperationException()
    {
        // Arrange
        var keymaster = new Keymaster();
        keymaster.NewKeyRoom();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => keymaster.Dispose());
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    [Test]
    public void Dispose_WithMultipleUnmatchedDoorsAndKeys_ThrowsInvalidOperationException()
    {
        // Arrange
        var keymaster = new Keymaster();
        keymaster.NewDoor();
        keymaster.NewDoor();
        keymaster.NewKeyRoom();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => keymaster.Dispose());
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    #endregion

    #region NewDoor Tests

    [Test]
    public void NewDoor_CreatesLockedDoor()
    {
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var door = keymaster.NewDoor();

        // Assert
        Assert.That(door, Is.Not.Null);
        Assert.That(door.IsLocked, Is.True);
        Assert.That(door.IsTraversable, Is.False);
    }

    [Test]
    public void NewDoor_WithExistingKeyRoom_PlacesKeyInRoom()
    {
        // Arrange
        var keymaster = new Keymaster();
        var keyRoom = keymaster.NewKeyRoom();

        // Act
        var door = keymaster.NewDoor();

        // Assert
        using var all = Assert.EnterMultipleScope();
        Assert.That(door.IsLocked, Is.True);
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
    }

    [Test]
    public void NewDoor_Multiple_CreatesMultipleLockedDoors()
    {
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var door1 = keymaster.NewDoor();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();

        // Assert
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
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var room = keymaster.NewKeyRoom();

        // Assert
        Assert.That(room, Is.Not.Null);
        Assert.That(room.Pass().HasItems, Is.False);
    }

    [Test]
    public void NewKeyRoom_WithExistingDoor_ReceivesKey()
    {
        // Arrange
        var keymaster = new Keymaster();
        var door = keymaster.NewDoor();

        // Act
        var keyRoom = keymaster.NewKeyRoom();

        // Assert
        using var all = Assert.EnterMultipleScope();
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
        Assert.That(door.IsLocked, Is.True);
    }

    [Test]
    public void NewKeyRoom_Multiple_CreatesMultipleRooms()
    {
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var room1 = keymaster.NewKeyRoom();
        var room2 = keymaster.NewKeyRoom();
        var room3 = keymaster.NewKeyRoom();

        // Assert
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
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var door = keymaster.NewDoor();
        var keyRoom = keymaster.NewKeyRoom();

        // Assert
        using var all = Assert.EnterMultipleScope();
        Assert.That(door.IsLocked, Is.True);
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
    }

    [Test]
    public void NewKeyRoomThenNewDoor_KeyIsPlacedInRoom()
    {
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var keyRoom = keymaster.NewKeyRoom();
        var door = keymaster.NewDoor();

        // Assert
        using var all = Assert.EnterMultipleScope();
        Assert.That(door.IsLocked, Is.True);
        Assert.That(keyRoom.Pass().HasItems, Is.True);
        Assert.That(keyRoom.Pass().ItemTypes.First(), Is.EqualTo(typeof(Key)));
    }

    [Test]
    public void MultipleDoorsBeforeKeys_AllKeysArePlacedCorrectly()
    {
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var door1 = keymaster.NewDoor();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();
        var keyRoom1 = keymaster.NewKeyRoom();
        var keyRoom2 = keymaster.NewKeyRoom();
        var keyRoom3 = keymaster.NewKeyRoom();

        // Assert
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
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var keyRoom1 = keymaster.NewKeyRoom();
        var keyRoom2 = keymaster.NewKeyRoom();
        var keyRoom3 = keymaster.NewKeyRoom();
        var door1 = keymaster.NewDoor();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();

        // Assert
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
        // Arrange
        var keymaster = new Keymaster();

        // Act
        var door1 = keymaster.NewDoor();
        var keyRoom1 = keymaster.NewKeyRoom();
        var keyRoom2 = keymaster.NewKeyRoom();
        var door2 = keymaster.NewDoor();
        var door3 = keymaster.NewDoor();
        var keyRoom3 = keymaster.NewKeyRoom();

        // Assert
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
        // Arrange
        var keymaster = new Keymaster();
        var door = keymaster.NewDoor();
        var keyRoom = keymaster.NewKeyRoom();
        var playerInventory = new MyInventory();

        // Act
        playerInventory.MoveItemFrom(keyRoom.Pass());
        var canOpen = door.Open(playerInventory);

        // Assert
        using var all = Assert.EnterMultipleScope();
        Assert.That(canOpen, Is.True);
        Assert.That(door.IsOpened, Is.True);
        Assert.That(door.IsTraversable, Is.True);
    }

    [Test]
    public void FiveDoorsAndFiveKeys_AllMatch()
    {
        // Arrange
        var keymaster = new Keymaster();
        var doors = new Door[5];
        var keyRooms = new Room[5];

        // Act
        for (int i = 0; i < 5; i++)
        {
            doors[i] = keymaster.NewDoor();
        }
        for (int i = 0; i < 5; i++)
        {
            keyRooms[i] = keymaster.NewKeyRoom();
        }

        // Assert
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
        // Arrange & Act & Assert
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
        // Arrange & Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var keymaster = new Keymaster();
            keymaster.NewDoor();
        });
        Assert.That(ex?.Message, Is.EqualTo("Unmatched key/door creation"));
    }

    #endregion
}
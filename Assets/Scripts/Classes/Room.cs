using System.Collections.Generic;
using UnityEngine;

public class Room {
  /**** SERIALIZED VARS ****/

  /**** PUBLIC VARS ****/
  public RoomSO data;
  public bool isActive = false;
  public HashSet<Vector2Int> roomCellList = new HashSet<Vector2Int>();
  public Vector2Int startCell = -Vector2Int.one;
  public Vector2Int endCell = -Vector2Int.one;
  public RoomController roomController;

  /**** PRIVATE VARS ****/
  public List<ObjectController> placedObjects { get; private set; } = new List<ObjectController>();

  public Room(RoomSO _data, bool _isActive, RoomController controller) {
    data = _data;
    isActive = _isActive;
    roomController = controller;
  }

  public void AddPlacedObject(ObjectController newObject) {
    placedObjects.Add(newObject);
  }

  /**** PRIVATE ****/
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingManager : MonoBehaviour {
  private static BuildingManager _instance;

  public static BuildingManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<BuildingManager>();
        if (_instance == null) {
          GameObject go = new GameObject("BuildingManager");
          _instance = go.AddComponent<BuildingManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/
  [Header("Bulding Settings")]
  [SerializeField] private RoomController roomPrefab;

  [Header("DEBUG SETTINGS")]
  [SerializeField] private bool showState = false;
  [SerializeField] private GameObject buildingStateContainer;
  [SerializeField] private TMP_Text targetBuildingText;
  [SerializeField] private TMP_Text minTargetSizeText;
  [SerializeField] private TMP_Text requiredEntrancesText;
  [SerializeField] private TMP_Text maxEntrancesText;

  /**** PUBLIC VARS ****/
  public static RoomSO targetBuilding = null;
  public static Room currentRoom = null;
  public static RoomController currentRoomController = null;

  /**** PRIVATE VARS ****/
  // ! TODO: Remove cause I don't remember this
  public static int totalObjectsInRooms = 0;
  public static List<Room> placedRoomsList { get; private set; } = new List<Room>();

  /**** UNITY HOOKS ****/
  private void OnEnable() {
    // On State changed
    GameModeManager.OnStateChanged += OnGameModeStateChanged;
  }

  private void OnDisable() {
    // On State changed
    GameModeManager.OnStateChanged -= OnGameModeStateChanged;
  }

  private void Awake() { }
  private void Start() { }

  private void Update() {
    if (!showState || GameModeManager.IsPlacingRoomObjects) {
      buildingStateContainer.SetActive(false);
      return;
    }

    if (targetBuilding != null) {
      buildingStateContainer.SetActive(true);
      targetBuildingText.text = targetBuilding.roomName;
      minTargetSizeText.text = $"{targetBuilding.minSize.x}x{targetBuilding.minSize.y}";
      requiredEntrancesText.text = targetBuilding.requiredEntrances.ToString();
      maxEntrancesText.text = targetBuilding.maxEntrances.ToString();
    }
  }

  /**** PRIVATE ****/
  private void OnGameModeStateChanged(EGameMode state) {
    switch (state) {
      case EGameMode.IDLE:
        targetBuilding = null;
        currentRoom = null;
        break;

      case EGameMode.BUILDING_MODE__SELECTING_AREA:
        currentRoom = new Room(targetBuilding, false, currentRoomController);
        break;

      case EGameMode.BUILDING_MODE__PLACING_OBJECTS:
        currentRoomController = Instantiate(roomPrefab);
        if (!placedRoomsList.Contains(currentRoom))
          placedRoomsList.Add(currentRoom);

        Vector2Int roomSize2D = currentRoom.endCell - currentRoom.startCell;
        Vector3 newPosition = new Vector3(currentRoom.endCell.x - roomSize2D.x / 2, 0, currentRoom.endCell.y - roomSize2D.y / 2);

        currentRoomController.transform.localScale = new Vector3(roomSize2D.x, 2, roomSize2D.y);
        currentRoomController.transform.position = newPosition;
        currentRoom.roomController = currentRoomController;
        break;

      case EGameMode.BUILDING_MODE__ROOM_FINISHED:
        // Reset Data
        targetBuilding = null;
        currentRoom = null;
        currentRoomController = null;
        break;

      default:
        break;
    }
  }

  /**** PUBLIC ****/
  public static void SetCurrentRoomCells(HashSet<Vector2Int> roomCells, Vector2Int startCell, Vector2Int endCell) {
    if (currentRoom == null) return;

    currentRoom.roomCellList = new HashSet<Vector2Int>(roomCells);
    currentRoom.startCell = startCell;
    currentRoom.endCell = endCell;

    // Mark the inner padding to block objects placement
    for (int i = startCell.x; i <= endCell.x; ++i) {
      GridManager.BlockCellForObjects(i, startCell.y);
      GridManager.BlockCellForObjects(i, endCell.y);
    }

    for (int i = startCell.y; i <= endCell.y; ++i) {
      GridManager.BlockCellForObjects(startCell.x, i);
      GridManager.BlockCellForObjects(endCell.x, i);
    }
  }

  public static ObjectSO GetObjectToPlace() {
    if (GameModeManager.wasRoomFinished || currentRoom == null) return null;

    int objectsPlacedCount = currentRoom.placedObjects.Count;
    List<ObjectSO> objectsToPlace = currentRoom.data.requiredObjectsTypeList;

    if (objectsToPlace.Count == objectsPlacedCount) return null;
    return objectsToPlace[objectsPlacedCount];
  }

  public static void AddPlacedObjectToCurrentRoom(ObjectController placedObject) {
    currentRoom.AddPlacedObject(placedObject);
    placedObject.transform.SetParent(currentRoomController.transform);

    if (currentRoom.data.requiredObjectsTypeList.Count == currentRoom.placedObjects.Count) {
      GameModeManager.ChangeState(EGameMode.BUILDING_MODE__ROOM_FINISHED);
    }

    ++totalObjectsInRooms;
  }

  public static void AddPlacedObjectToRoom(ObjectController placedObject, Room targetRoom = null) {
    if (targetRoom == null) return;

    targetRoom.AddPlacedObject(placedObject);
    placedObject.transform.SetParent(targetRoom.roomController.transform);
    ++totalObjectsInRooms;
  }

  public static int GetCurrentRoomObjectsCost() {
    int cost = 0;

    foreach (ObjectSO objToPlace in targetBuilding.requiredObjectsTypeList) {
      cost += objToPlace.cost;
    }

    return cost;
  }

  public static int GetCurrentRoomTileCost() {
    return targetBuilding.costPerTile;
  }

  public static Room GetRoomAtCell(Vector2Int cellPosition) {
    foreach (Room room in placedRoomsList) {
      if (room.roomCellList.Contains(cellPosition)) {
        return room;
      }
    }
    return null;
  }

  public static Room GetRoomAtCell(int x, int z) {
    Vector2Int cellPosition = new Vector2Int(x, z);

    foreach (Room room in placedRoomsList) {
      if (room.roomCellList.Contains(cellPosition)) {
        return room;
      }
    }
    return null;
  }

  /**** COROUTINES ****/
}

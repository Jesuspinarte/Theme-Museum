using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class PlacingSystem : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [Header("Placement Settings")]
  [SerializeField] private LayerMask cellLayerMask;

  [Header("Input Action References")]
  [SerializeField] private InputActionReference mousePlaceAction;
  [SerializeField] private InputActionReference mouseCancelAction;

  [Header("Debug Settings")]
  [SerializeField] private bool showState = false;
  [SerializeField] private GameObject placementStateContainer;
  [SerializeField] private TMP_Text currentPlacingObjectText;

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/
  private bool _canBePlaced = true;
  private Room _currentRoom = null;
  private RaycastHit _hit;
  private Collider[] _hitColliders = null;
  private Transform _placementCheck = null;
  private ObjectSO _currentObjectData = null;
  private ObjectController _currentObjectToPlace = null;

  /**** UNITY HOOKS ****/
  private void Awake() { }

  private void OnEnable() {
    GameModeManager.OnStateChanged += OnGameModeStateChanged;

    if (mousePlaceAction.action != null) {
      mousePlaceAction.action.started += PlaceCurrentObject;
    }

    if (mouseCancelAction.action != null) {
      mouseCancelAction.action.started += CancelObjectPlacement;
    }
  }

  private void OnDisable() {
    GameModeManager.OnStateChanged -= OnGameModeStateChanged;

    if (mousePlaceAction.action != null) {
      mousePlaceAction.action.started -= PlaceCurrentObject;
    }

    if (mouseCancelAction.action != null) {
      mouseCancelAction.action.started -= CancelObjectPlacement;
    }
  }

  private void Start() { }

  private void Update() {
    UpdateDebugData();
    PlaceObjectsOnBuildingMode();
    PlaceObjectsOnFurnitureMode();
  }

  /**** PRIVATE ****/
  private void PlaceCurrentObject(CallbackContext ctx) {
    if (
      !_canBePlaced ||
      (!GameModeManager.IsPlacingRoomObjects && !GameModeManager.IsPlacingFurniture) ||
      _currentObjectData == null
    ) return;

    if (_currentObjectToPlace.transform.position == -Vector3.one) return;

    _currentObjectToPlace.UpdateObjectAssetByType(EObjectAsset.FINAL);

    if (GameModeManager.IsPlacingRoomObjects)
      BuildingManager.AddPlacedObjectToCurrentRoom(_currentObjectToPlace);
    else
      BuildingManager.AddPlacedObjectToRoom(_currentObjectToPlace, _currentRoom);

    ResetCells(true);

    // Reset local variables
    _canBePlaced = true;
    _hitColliders = null;
    _placementCheck = null;
    _currentObjectData = null;
    _currentObjectToPlace = null;

    GetNextObjectToPlaceFromRoom();
    SoundManager.Instance.PlaySfxSound(EFxSoundType.PLACEMENT);
  }

  private void CancelObjectPlacement(CallbackContext ctx) {
    if (!GameModeManager.IsPlacingFurniture) return;

    ResetCells(false);
    Destroy(_currentObjectToPlace.gameObject);

    // Reset local variables
    _canBePlaced = true;
    _hitColliders = null;
    _placementCheck = null;
    _currentObjectData = null;
    _currentObjectToPlace = null;

    FurnitureManager.targetObject = null;
    GameModeManager.ChangeState(EGameMode.IDLE);
  }

  private void UpdateDebugData() {
    if (
      !showState ||
      _currentObjectData == null ||
      (!GameModeManager.IsPlacingRoomObjects && !GameModeManager.IsPlacingFurniture) ||
      GameModeManager.IsOnIdleMode
    ) {
      placementStateContainer.SetActive(false);
      return;
    }

    if (_currentObjectData != null) {
      placementStateContainer.SetActive(true);
      currentPlacingObjectText.text = _currentObjectData.name;
    }
  }

  private void GetNextObjectToPlaceFromRoom() {
    if (GameModeManager.IsPlacingFurniture) {
      GameModeManager.ChangeState(EGameMode.FURNITURE_MODE__FINISH_PLACEMENT);
      _currentObjectData = null;
      return;
    }

    _currentObjectData = BuildingManager.GetObjectToPlace();

    if (_currentObjectData == null) return;

    _currentObjectToPlace = Instantiate(_currentObjectData.prefab);
    _currentRoom = BuildingManager.currentRoom;
  }

  private void GetFurnitureObjectToPlace() {
    _currentObjectData = FurnitureManager.targetObject;

    if (_currentObjectData == null) return;

    _currentObjectToPlace = Instantiate(_currentObjectData.prefab);
    _currentRoom = BuildingManager.currentRoom;
  }

  private void OnGameModeStateChanged(EGameMode state) {
    switch (state) {
      case EGameMode.IDLE:
        _currentObjectData = null;
        break;

      case EGameMode.BUILDING_MODE__PLACING_OBJECTS:
        GetNextObjectToPlaceFromRoom();
        break;

      case EGameMode.FURNITURE_MODE__PLACING_OBJECT:
        GetFurnitureObjectToPlace();
        break;

      default:
        break;
    }
  }

  private void ResetCells(bool hasObject = false) {
    if (_hitColliders == null) return;

    for (int i = 0; i < _hitColliders.Length; i++) {
      Vector2Int cellPosition = new Vector2Int((int)_hitColliders[i].transform.position.x, (int)_hitColliders[i].transform.position.z);

      if (GridManager.Instance.IsCellWithinBounds(cellPosition))
        GridManager.PaintCell(cellPosition, EMat.ORIGINAL, false);

      if (hasObject)
        GridManager.BlockCellForObjects(cellPosition);
    }

    _hitColliders = null;
  }

  private bool ValidateObjectRules(ObjectSO objData, Room targetRoom) {
    if (objData == null) return false;

    switch (objData.placementRule) {
      case EObjectPlacementRule.ANYWHERE:
        return true;

      case EObjectPlacementRule.HALLWAY_ONLY:
        return targetRoom == null;

      case EObjectPlacementRule.SPECIFIC_ROOM_ONLY:
        if (targetRoom == null) return false;
        return targetRoom.data.roomType == objData.requiredRoomType;

      default:
        return false;
    }
  }

  private void CheckPlacementOnBuildingMode() {
    if (_hitColliders == null || _currentRoom == null) return;

    _canBePlaced = true;
    bool cellHasObject;

    for (int i = 0; i < _hitColliders.Length; i++) {
      Vector2Int cellPosition = new Vector2Int((int)_hitColliders[i].transform.position.x, (int)_hitColliders[i].transform.position.z);
      cellHasObject = false;

      if (!GridManager.Instance.IsCellWithinBounds(cellPosition)) {
        _canBePlaced = false;
        break;
      }

      if (!_currentRoom.roomCellList.Contains(cellPosition) || GridManager.CellHasObject(cellPosition)) {
        _canBePlaced = false;
        cellHasObject = true;
      }

      if (cellHasObject)
        GridManager.PaintCell(cellPosition, EMat.ERROR, false);
      else
        GridManager.PaintCell(cellPosition, EMat.SELECTED, false);
    }

    if (!_canBePlaced)
      _currentObjectToPlace.UpdateObjectAssetByType(EObjectAsset.ERROR);
    else
      _currentObjectToPlace.UpdateObjectAssetByType(EObjectAsset.PREVIEW);
  }

  private void CheckPlacementOnFurnitureMode() {
    if (_hitColliders == null) return;

    _canBePlaced = true;
    _currentRoom = null;
    bool cellHasObject;

    if (!EconomyManager.CanAffordAmount(_currentObjectData.cost)) {
      _canBePlaced = false;
    }

    for (int i = 0; i < _hitColliders.Length; i++) {
      Vector2Int cellPosition = new Vector2Int((int)_hitColliders[i].transform.position.x, (int)_hitColliders[i].transform.position.z);
      cellHasObject = false;

      if (!GridManager.Instance.IsCellWithinBounds(cellPosition)) {
        _canBePlaced = false;
        break;
      }

      if (GridManager.CellHasObject(cellPosition)) {
        _canBePlaced = false;
        cellHasObject = true;
      }

      Room roomAtCell = BuildingManager.GetRoomAtCell(cellPosition);
      _currentRoom = roomAtCell;

      if (!ValidateObjectRules(_currentObjectData, roomAtCell)) {
        _canBePlaced = false;
      }

      if (roomAtCell == null && GridManager.IsCellOccupied(cellPosition)) {
        _canBePlaced = false;
      }

      if (cellHasObject || !_canBePlaced) GridManager.PaintCell(cellPosition, EMat.ERROR, false);
      else GridManager.PaintCell(cellPosition, EMat.SELECTED, false);
    }

    if (!_canBePlaced) _currentObjectToPlace.UpdateObjectAssetByType(EObjectAsset.ERROR);
    else _currentObjectToPlace.UpdateObjectAssetByType(EObjectAsset.PREVIEW);
  }

  private void PlaceObjectsOnBuildingMode() {
    if (!GameModeManager.IsPlacingRoomObjects) return;

    Vector3 positionToPlace;
    ResetCells();

    _hitColliders = null;

    if (_currentObjectToPlace == null) return;

    if (GameManager.GetCurrentRaycast(cellLayerMask, out _hit)) {
      positionToPlace = _hit.collider.transform.position;
      positionToPlace.y = 0;
    } else
      positionToPlace = -Vector3.one;

    _currentObjectToPlace.transform.position = positionToPlace;

    _placementCheck = _currentObjectToPlace.placementCheck;

    _hitColliders = Physics.OverlapBox(_placementCheck.position, _placementCheck.localScale / 2, Quaternion.identity, cellLayerMask);
    CheckPlacementOnBuildingMode();
  }

  private void PlaceObjectsOnFurnitureMode() {
    if (!GameModeManager.IsPlacingFurniture) return;

    Vector3 positionToPlace;
    ResetCells();

    if (_currentObjectToPlace == null) return;

    if (GameManager.GetCurrentRaycast(cellLayerMask, out _hit)) {
      positionToPlace = _hit.collider.transform.position;
      positionToPlace.y = 0;
    } else
      positionToPlace = -Vector3.one;

    _currentObjectToPlace.transform.position = positionToPlace;
    _placementCheck = _currentObjectToPlace.placementCheck;
    _hitColliders = Physics.OverlapBox(_placementCheck.position, _placementCheck.localScale / 2, Quaternion.identity, cellLayerMask);

    CheckPlacementOnFurnitureMode();
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

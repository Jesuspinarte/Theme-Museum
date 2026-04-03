using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class BuildingSystem : MonoBehaviour {
  private const float WALL_OFFSET = .5f;

  /**** SERIALIZED VARS ****/
  [Header("Input Action References")]
  [SerializeField] private InputActionReference mouseStartDragAction;
  [SerializeField] private InputActionReference mouseCancelDragAction;

  [Header("References")]
  [SerializeField] private LayerMask cellLayerMask;
  [SerializeField] private LayerMask wallLayerMask;
  [SerializeField] private WallController wallPrefab;

  [Header("Containers")]
  [SerializeField] private Transform wallsContainer;

  /**** PRIVATE VARS ****/
  private RaycastHit _hit;

  private int _entriesCounter = 0;

  private bool _canBuildRoom = true;
  private bool _canBeAfforded = true;
  private bool _hasMinimunSize = true;

  private Vector2Int _endCell = -Vector2Int.one;
  private Vector2Int _startCell = -Vector2Int.one;

  private Vector2Int _endSelectedCell = -Vector2Int.one;
  private Vector2Int _startSelectedCell = -Vector2Int.one;

  private WallController _hoveredWall = null;
  private HashSet<WallController> _previewWallSet = new HashSet<WallController>();
  private HashSet<Vector2Int> _currentCellSelection = new HashSet<Vector2Int>();

  private bool IsSelectionValid => _startSelectedCell != -Vector2Int.one && _endSelectedCell != -Vector2Int.one;

  /**** UNITY HOOKS ****/
  private void OnEnable() {
    if (mouseStartDragAction.action != null) {
      // Drag action
      mouseStartDragAction.action.started += OnGridDragStarted;
      mouseStartDragAction.action.canceled += OnGridDragFinished;
      // Remove wall
      mouseStartDragAction.action.performed += OnRemovePlaceholderWall;
    }

    if (mouseCancelDragAction.action != null) {
      // Drag cancel
      mouseCancelDragAction.action.performed += OnGridDragCancelled;
    }

    GameModeManager.OnStateChanged += OnGameModeStateChanged;
  }

  private void OnDisable() {
    if (mouseStartDragAction.action != null) {
      // Drag action
      mouseStartDragAction.action.started -= OnGridDragStarted;
      mouseStartDragAction.action.canceled -= OnGridDragFinished;
      // Remove wall
      mouseStartDragAction.action.performed -= OnRemovePlaceholderWall;
    }

    // Drag cancel
    if (mouseCancelDragAction.action != null) {
      mouseCancelDragAction.action.performed -= OnGridDragCancelled;
    }

    GameModeManager.OnStateChanged -= OnGameModeStateChanged;
  }

  private void Awake() {
    _startCell = GridManager.Instance.GridSize + Vector2Int.one;
  }

  private void Start() { }

  private void Update() {
    OnCellHovered();
    OnDrag();
    HoverEntries();
  }

  /*********************** ACTION HANDLERS ***********************/
  private void OnGridDragStarted(CallbackContext ctx) {
    if (!GameModeManager.IsSelectingCell) return;

    ResetSelection(EGameMode.BUILDING_MODE__SELECTING_AREA);

    _startCell = GetHoveredCell();
    _endCell = _startCell;
    PaintFloorSelection(EMat.HOVER);
  }

  private void OnGridDragFinished(CallbackContext ctx) {
    if (!GameModeManager.IsSelectingArea) return;

    if (!IsSelectionValid) {
      GameModeManager.ChangeState(EGameMode.BUILDING_MODE__SELECTING_CELL);
      return;
    }

    PaintFloorSelection(EMat.SELECTED);
    DrawPlaceholderWalls();

    if (_canBuildRoom)
      GameModeManager.ChangeState(EGameMode.BUILDING_MODE__PLACING_ENTRANCE);
    else
      GameModeManager.ChangeState(EGameMode.BUILDING_MODE__CANT_BUILD);
  }

  private void OnGridDragCancelled(CallbackContext ctx) {
    if (!GameModeManager.IsOnBuildingMode) return;
    if (GameModeManager.IsPlacingRoomObjects) return;

    EconomyManager.currentBuildingCost = BuildingManager.GetCurrentRoomObjectsCost();
    EconomyManager.Instance.UpdateBuildingCostText();

    ResetSelection();
  }

  private void OnRemovePlaceholderWall(CallbackContext ctx) {
    if (!GameModeManager.CanPlaceEntries) return;
    if (_hoveredWall == null || _hoveredWall._hasError || !_canBuildRoom) return;

    _previewWallSet.Remove(_hoveredWall);
    Destroy(_hoveredWall.gameObject);
    _hoveredWall = null;
    ++_entriesCounter;

    SoundManager.Instance.PlaySfxSound(EFxSoundType.DRAG);

    if (_entriesCounter < BuildingManager.targetBuilding.requiredEntrances) return;

    if (_entriesCounter >= BuildingManager.targetBuilding.maxEntrances) {
      GameModeManager.ChangeState(EGameMode.BUILDING_MODE__PLACING_OBJECTS);
    }
    else {
      GameModeManager.ChangeState(EGameMode.BUILDING_MODE__READY_FOR_OBJECTS);
    }
  }
  /*************************************************************/

  /**** PRIVATE ****/
  private void OnGameModeStateChanged(EGameMode state) {
    switch (state) {
      case EGameMode.IDLE:
        ResetSelection();
        break;

      case EGameMode.BUILDING_MODE__SELECTING_CELL:
        if (BuildingManager.targetBuilding != null) {
          EconomyManager.currentBuildingCost = BuildingManager.GetCurrentRoomObjectsCost();
          EconomyManager.Instance.UpdateBuildingCostText();
        }

        RemovePlacedWalls();
        break;

      case EGameMode.BUILDING_MODE__PLACING_OBJECTS:
        SoundManager.Instance.PlaySfxSound(EFxSoundType.PLACEMENT);
        PreviewRoom();
        break;

      case EGameMode.BUILDING_MODE__ROOM_FINISHED:
        SoundManager.Instance.PlaySfxSound(EFxSoundType.PLACEMENT);
        CommitSelection();
        break;

      default:
        break;
    }
  }

  private Vector2Int GetHoveredCell() {
    if (!GameModeManager.IsSelectingArea && !GameModeManager.IsSelectingCell) return -Vector2Int.one;

    if (GameManager.GetCurrentRaycast(cellLayerMask, out _hit))
      return _hit.transform.GetComponent<CellController>().cellPosition;

    return -Vector2Int.one;
  }

  private void HoverEntries() {
    if (!GameModeManager.CanPlaceEntries) return;

    if (_hoveredWall != null)
      _hoveredWall.UpdateMaterial(EMat.SELECTED);
    _hoveredWall = null;

    if (GameManager.IsMouseOverUI) return;
    if (!GameManager.GetCurrentRaycast(wallLayerMask, out _hit)) return;

    WallController newHoveredWall = _hit.transform.GetComponent<WallController>();

    if (_previewWallSet.Contains(newHoveredWall)) {
      _hoveredWall = newHoveredWall;
      _hoveredWall.UpdateMaterial(EMat.HOVER);
    }
  }

  private void SetSelectedCells() {
    _startSelectedCell = new Vector2Int(
      Mathf.Min(_startCell.x, _endCell.x),
      Mathf.Min(_startCell.y, _endCell.y)
    );
    _endSelectedCell = new Vector2Int(
      Mathf.Max(_startCell.x, _endCell.x),
      Mathf.Max(_startCell.y, _endCell.y)
    );
  }

  private void PaintCell(int x, int z, EMat matType) {
    bool isOccupied = false;
    EMat targetMat = matType;

    if (GridManager.IsCellOccupied(x, z) && matType != EMat.ORIGINAL) {
      targetMat = EMat.ERROR;
      _canBuildRoom = false;
    }

    if (!_hasMinimunSize && matType != EMat.ORIGINAL) targetMat = EMat.ERROR;
    if (!_canBeAfforded && matType != EMat.ORIGINAL) targetMat = EMat.ERROR;

    // ! TODO: Check this later later to see if here is the place to mark the cells as occupied
    // if (GameModeManager.CurrentState == EGameMode.BUILDING_MODE__PLACING_OBJECTS)
    if (GameModeManager.CurrentState == EGameMode.BUILDING_MODE__ROOM_FINISHED)
      isOccupied = true;

    GridManager.PaintCell(x, z, targetMat, isOccupied);
    _currentCellSelection.Add(new Vector2Int(x, z));
  }

  private void CalculateCost() {
    if (!GameModeManager.IsSelectingArea) return;

    int buildingCost = BuildingManager.GetCurrentRoomObjectsCost();
    int buildingWidth = _endSelectedCell.x - _startSelectedCell.x;
    int buildingLength = _endSelectedCell.y - _startSelectedCell.y;

    int areaCost = buildingWidth * buildingLength * BuildingManager.GetCurrentRoomTileCost();

    buildingCost += areaCost;
    EconomyManager.currentBuildingCost = buildingCost;
    EconomyManager.Instance.UpdateBuildingCostText();
  }

  private void PaintFloorSelection(EMat matType) {
    SetSelectedCells();
    _canBuildRoom = true;
    _canBeAfforded = true;
    _currentCellSelection.Clear();

    if (!IsSelectionValid) return;
    if (matType != EMat.ORIGINAL) CheckMinimumBuildingSize();

    if (GameModeManager.CurrentState == EGameMode.BUILDING_MODE__PLACING_OBJECTS)
      AddMarginToRoom();

    CalculateCost();

    if (!EconomyManager.CanAfforBuildingCost()) {
      _canBuildRoom = false;
      _canBeAfforded = false;
    }

    for (int i = _startSelectedCell.x; i <= _endSelectedCell.x; ++i)
      for (int j = _startSelectedCell.y; j <= _endSelectedCell.y; ++j)
        PaintCell(i, j, matType);
  }

  private void AddPlaceholderWall(Vector3 position, Vector3 rotation = default, bool isOccupied = false) {
    if (position.x < 0f || position.z < 0f) return;

    WallController newWall;

    newWall = Instantiate(wallPrefab, position, Quaternion.identity, wallsContainer);
    newWall.UpdateMaterial(EMat.SELECTED);
    newWall.transform.Rotate(rotation);
    newWall.SetHasError(!_hasMinimunSize || !_canBeAfforded || isOccupied);
    _previewWallSet.Add(newWall);
  }

  // Preview room walls
  private void DrawPlaceholderWalls() {
    if (!IsSelectionValid) return;

    SoundManager.Instance.PlaySfxSound(EFxSoundType.CELL_HOVERING);

    // Bottom and Top walls
    for (int i = _startSelectedCell.x; i <= _endSelectedCell.x; ++i) {
      AddPlaceholderWall(
        new Vector3(i, WALL_OFFSET, _endSelectedCell.y + WALL_OFFSET),
        default,
        GridManager.cellGrid[i][_endSelectedCell.y].isOccupied
      );
      AddPlaceholderWall(
        new Vector3(i, WALL_OFFSET, _startSelectedCell.y - WALL_OFFSET),
        default,
        GridManager.cellGrid[i][_startSelectedCell.y].isOccupied
      );
    }

    // Right and Left walls
    for (int j = _startSelectedCell.y; j <= _endSelectedCell.y; ++j) {
      Vector3 wallRotation = new Vector3(0f, 90f, 0f);

      AddPlaceholderWall(
        new Vector3(_endSelectedCell.x + WALL_OFFSET, WALL_OFFSET, j),
        wallRotation,
        GridManager.cellGrid[_endSelectedCell.x][j].isOccupied
      );
      AddPlaceholderWall(
        new Vector3(_startSelectedCell.x - WALL_OFFSET, WALL_OFFSET, j),
        wallRotation,
        GridManager.cellGrid[_startSelectedCell.x][j].isOccupied
      );
    }
  }

  private void CheckMinimumBuildingSize() {
    _hasMinimunSize = true;

    if (GameModeManager.IsSelectingArea) {
      int minSizeWidth = BuildingManager.targetBuilding.minSize.x;
      int minSizeLength = BuildingManager.targetBuilding.minSize.y;

      if ((_endSelectedCell.x - _startSelectedCell.x) < minSizeWidth - 1) _hasMinimunSize = false;
      if ((_endSelectedCell.y - _startSelectedCell.y) < minSizeLength - 1) _hasMinimunSize = false;
    }
  }

  private void OnDrag() {
    if (!GameModeManager.IsSelectingArea) return;

    Vector2Int newEndCell = GetHoveredCell();
    if (newEndCell == _endCell) return;

    SoundManager.Instance.PlaySfxSound(EFxSoundType.DRAG);

    PaintFloorSelection(EMat.ORIGINAL);
    _endCell = newEndCell;
    PaintFloorSelection(EMat.HOVER);
  }

  private void OnCellHovered() {
    if (!GameModeManager.IsSelectingCell) return;

    Vector2Int hoveredCell = GetHoveredCell();
    if (hoveredCell == _startCell) return;

    SoundManager.Instance.PlaySfxSound(EFxSoundType.CELL_HOVERING);

    PaintFloorSelection(EMat.ORIGINAL);
    _startCell = hoveredCell;
    _endCell = _startCell;
    PaintFloorSelection(EMat.HOVER);
  }

  private void AddMarginToRoom() {
    _startSelectedCell.x -= _startSelectedCell.x == 0 ? 0 : 1;
    _startSelectedCell.y -= _startSelectedCell.y == 0 ? 0 : 1;

    _endSelectedCell.x += _endSelectedCell.x == GridManager.Instance.GridSize.x ? 0 : 1;
    _endSelectedCell.y += _endSelectedCell.y == GridManager.Instance.GridSize.y ? 0 : 1;
  }

  private void PreviewRoom() {
    foreach (WallController wc in _previewWallSet)
      wc.UpdateMaterial(EMat.ORIGINAL);

    PaintFloorSelection(EMat.ORIGINAL);
    // ! TODO: If cancelled restart those cells
    // ? THIS IS TO BLOCK THE INNER PADDING AND THE OUTER PADDING BECAUSE I DON'T WANNA COMPLICATE MYSLEF
    BuildingManager.SetCurrentRoomCells(_currentCellSelection, _startSelectedCell + Vector2Int.one, _endSelectedCell - Vector2Int.one);
    BuildingManager.SetCurrentRoomCells(_currentCellSelection, _startSelectedCell, _endSelectedCell);
  }

  private void CommitSelection() {
    _previewWallSet.Clear();
    // ! TODO: If cancelled restart those cells
    BuildingManager.SetCurrentRoomCells(_currentCellSelection, _startSelectedCell, _endSelectedCell);
    PaintFloorSelection(EMat.ORIGINAL);
    ResetBuildingProperties();
  }

  private void RemovePlacedWalls() {
    foreach (WallController wc in _previewWallSet)
      if (wc != null) Destroy(wc.gameObject);

    _previewWallSet.Clear();
  }

  private void ResetBuildingProperties(EGameMode nextState = EGameMode.IDLE) {
    _endCell = -Vector2Int.one;
    _startCell = -Vector2Int.one;
    _endSelectedCell = -Vector2Int.one;
    _startSelectedCell = -Vector2Int.one;

    _hoveredWall = null;
    _canBuildRoom = true;
    _canBeAfforded = true;
    _hasMinimunSize = true;
    _entriesCounter = 0;

    _currentCellSelection.Clear();

    if (GameModeManager.IsOnIdleMode) return;
    if (GameModeManager.wasRoomFinished) return;

    if (GameModeManager.IsOnBuildingMode && !GameModeManager.IsSelectingCell)
      nextState = EGameMode.BUILDING_MODE__SELECTING_CELL;

    GameModeManager.ChangeState(nextState);
  }

  private void ResetSelection(EGameMode nextState = EGameMode.IDLE) {
    RemovePlacedWalls();
    PaintFloorSelection(EMat.ORIGINAL);
    ResetBuildingProperties(nextState);
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

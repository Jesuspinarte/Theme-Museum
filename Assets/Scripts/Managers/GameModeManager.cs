using System;
using TMPro;
using UnityEngine;

public class GameModeManager : MonoBehaviour {
  private static GameModeManager _instance;

  public static GameModeManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<GameModeManager>();
        if (_instance == null) {
          GameObject go = new GameObject("GameModeManager");
          _instance = go.AddComponent<GameModeManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/
  [Header("Debug Settings")]
  [SerializeField] private bool showState = false;
  [SerializeField] private TMP_Text stateText;
  [SerializeField] private GameObject stateTextContainer;

  /**** PUBLIC VARS ****/
  public static EGameMode CurrentState { get; private set; } = EGameMode.IDLE;
  public static event Action<EGameMode> OnStateChanged;

  public static bool IsOnIdleMode => CurrentState == EGameMode.IDLE;
  public static bool IsGameOver => CurrentState == EGameMode.GAME_OVER;
  public static bool IsSelectingArea => CurrentState == EGameMode.BUILDING_MODE__SELECTING_AREA;
  public static bool IsSelectingCell => CurrentState == EGameMode.BUILDING_MODE__SELECTING_CELL;
  public static bool IsPlacingRoomObjects => CurrentState == EGameMode.BUILDING_MODE__PLACING_OBJECTS;
  public static bool CanPlaceEntries => CurrentState == EGameMode.BUILDING_MODE__PLACING_ENTRANCE || CurrentState == EGameMode.BUILDING_MODE__READY_FOR_OBJECTS;
  public static bool wasRoomFinished => CurrentState == EGameMode.BUILDING_MODE__ROOM_FINISHED;
  public static bool IsPlacingFurniture => CurrentState == EGameMode.FURNITURE_MODE__PLACING_OBJECT;

  /// <summary>
  /// Checks if the Current State is on any of the BuildingMode Steps
  /// </summary>
  public static bool IsOnBuildingMode =>
      CurrentState == EGameMode.BUILDING_MODE__SELECTING_CELL ||
      CurrentState == EGameMode.BUILDING_MODE__SELECTING_AREA ||
      CurrentState == EGameMode.BUILDING_MODE__PLACING_ENTRANCE ||
      CurrentState == EGameMode.BUILDING_MODE__READY_FOR_OBJECTS ||
      CurrentState == EGameMode.BUILDING_MODE__PLACING_OBJECTS ||
      CurrentState == EGameMode.BUILDING_MODE__READY_TO_FINISH_ROOM ||
      CurrentState == EGameMode.BUILDING_MODE__CANT_BUILD;

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void Awake() { }

  private void Start() {
    OnStateChanged?.Invoke(CurrentState);
    // PrintDebugState();
  }

  private void Update() {
    if (showState) {
      stateTextContainer.SetActive(true);
      stateText.text = CurrentState.ToString();
    }
    else {
      stateTextContainer.SetActive(false);
    }
  }

  /**** PRIVATE ****/
  private static void EnterState(EGameMode state) {
    switch (state) {
      case EGameMode.IDLE:
        break;

      case EGameMode.BUILDING_MODE__SELECTING_CELL:
        break;

      case EGameMode.BUILDING_MODE__SELECTING_AREA:
        break;

      case EGameMode.BUILDING_MODE__PLACING_ENTRANCE:
        break;

      case EGameMode.BUILDING_MODE__READY_FOR_OBJECTS:
        break;

      case EGameMode.BUILDING_MODE__PLACING_OBJECTS:
        break;

      case EGameMode.BUILDING_MODE__READY_TO_FINISH_ROOM:
        break;

      case EGameMode.HIRING_STAFF_MODE:
        break;

      default:
        break;
    }
  }

  private static void ExitState(EGameMode state) { }

  /**** PUBLIC ****/
  public static void ChangeState(EGameMode newState) {
    if (CurrentState == newState) return;

    ExitState(CurrentState);
    CurrentState = newState;
    EnterState(CurrentState);

    OnStateChanged?.Invoke(CurrentState);
  }

  /**** COROUTINES ****/

  /**** HELPERS ****/
}

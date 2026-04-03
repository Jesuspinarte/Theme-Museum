using UnityEngine;
using UnityEngine.UI;

public class RoomButtonController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [field: SerializeField] public RoomSO targetRoomData { get; private set; }

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/
  private Button _button;

  /**** UNITY HOOKS ****/
  private void Awake() {
    _button = GetComponent<Button>();
  }

  private void OnEnable() {
    _button.onClick.AddListener(OnRoomButtonClicked);
  }

  private void OnDisable() {
    _button.onClick.RemoveListener(OnRoomButtonClicked);
  }

  private void Start() { }
  private void Update() { }

  /**** PRIVATE ****/
  private void OnRoomButtonClicked() {
    BuildingManager.targetBuilding = targetRoomData;

    EconomyManager.currentBuildingCost = BuildingManager.GetCurrentRoomObjectsCost();
    EconomyManager.Instance.UpdateBuildingCostText();

    GameModeManager.ChangeState(EGameMode.BUILDING_MODE__SELECTING_CELL);
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

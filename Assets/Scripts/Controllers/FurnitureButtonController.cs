using UnityEngine;
using UnityEngine.UI;

public class FurnitureButtonController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [field: SerializeField] public ObjectSO targetFurnitureData { get; private set; }

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
    FurnitureManager.targetObject = targetFurnitureData;

    EconomyManager.currentBuildingCost = targetFurnitureData.cost;
    EconomyManager.Instance.UpdateBuildingCostText();

    GameModeManager.ChangeState(EGameMode.FURNITURE_MODE__PLACING_OBJECT);
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

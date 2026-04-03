using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [Header("Main Menu Buttons")]
  [SerializeField] private Button buildButton;
  [SerializeField] private Button staffButton;
  [SerializeField] private Button furnitureButton;

  [Header("Sub Menus")]
  [SerializeField] private GameObject staffMenu;
  [SerializeField] private GameObject buildingMenu;
  [SerializeField] private GameObject furnitureMenu;

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void OnEnable() {
    // Main Menu
    buildButton.onClick.AddListener(OnBuildButtonClicked);
    staffButton.onClick.AddListener(OnStaffButtonClicked);
    furnitureButton.onClick.AddListener(OnFurnitureButtonClicked);

    // On State changed
    GameModeManager.OnStateChanged += OnGameModeStateChanged;
  }

  private void OnDisable() {
    // Main Menu
    buildButton.onClick.RemoveListener(OnBuildButtonClicked);
    staffButton.onClick.RemoveListener(OnStaffButtonClicked);
    furnitureButton.onClick.RemoveListener(OnFurnitureButtonClicked);

    // On State changed
    GameModeManager.OnStateChanged -= OnGameModeStateChanged;
  }

  private void Awake() { }
  private void Start() { }
  private void Update() { }

  /**** PRIVATE ****/
  private void OnGameModeStateChanged(EGameMode state) {
    switch (state) {
      case EGameMode.IDLE:
        buildingMenu.SetActive(false);
        furnitureMenu.SetActive(false);
        break;

      case EGameMode.MENU__BUILDING:
        buildingMenu.SetActive(true);
        furnitureMenu.SetActive(false);
        break;

      case EGameMode.MENU__FURNITURE:
        buildingMenu.SetActive(false);
        furnitureMenu.SetActive(true);
        break;

      default:
        buildingMenu.SetActive(false);
        furnitureMenu.SetActive(false);
        break;
    }
  }

  private void OnBuildButtonClicked() {
    if (GameModeManager.IsOnIdleMode)
      GameModeManager.ChangeState(EGameMode.MENU__BUILDING);
    else
      GameModeManager.ChangeState(EGameMode.IDLE);
  }

  private void OnStaffButtonClicked() {
    // GameModeManager.ChangeState(EGameMode.BUILDING_MODE);
  }

  private void OnFurnitureButtonClicked() {
    if (GameModeManager.IsOnIdleMode)
      GameModeManager.ChangeState(EGameMode.MENU__FURNITURE);
    else
      GameModeManager.ChangeState(EGameMode.IDLE);
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

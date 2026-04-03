using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipHandler : MonoBehaviour {
  private static TooltipHandler _instance;

  public static TooltipHandler Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<TooltipHandler>();
        if (_instance == null) {
          GameObject go = new GameObject("TooltipHandler");
          _instance = go.AddComponent<TooltipHandler>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/
  [Header("Tooltip Reference")]
  [SerializeField] private TMP_Text toolTipText;
  [SerializeField] private GameObject tooltipContainer;

  [Header("Tooltip Info")]
  [SerializeField] private TMP_Text toolTipInfoTitleText;
  [SerializeField] private TMP_Text toolTipInfoDescriptionText;
  [SerializeField] private GameObject tooltipInfoContainer;
  [SerializeField] private LayerMask objectLayer;

  [Header("Buttons References")]
  [SerializeField] private Button acceptButton;
  [SerializeField] private Button cancelButton;
  // [SerializeField] private TMP_Text acceptButtonText;
  // [SerializeField] private TMP_Text cancelButtonText;
  [SerializeField] private GameObject buttonsContainer;

  [Header("TOOLTIP TEXTS")]
  [Header("Building Mode")]
  [SerializeField, TextArea(1, 3)] private string cantBuildTooltipText;
  [SerializeField, TextArea(1, 3)] private string selectingAreaTooltipText;
  [SerializeField, TextArea(1, 3)] private string createRoomTooltipText;
  [SerializeField, TextArea(1, 3)] private string removeWallsTooltipText;
  [SerializeField, TextArea(1, 3)] private string palceObjectsTooltipText;

  /**** PRIVATE VARS ****/
  private EGameMode _acceptButtonState = EGameMode.IDLE;
  private EGameMode _cancelButtonState = EGameMode.IDLE;
  private RaycastHit _hit;

  /**** UNITY HOOKS ****/
  private void OnEnable() {
    acceptButton.onClick.AddListener(OnAcceptClicked);
    cancelButton.onClick.AddListener(OnCancelClicked);

    GameModeManager.OnStateChanged += OnGameModeChanged;
  }

  private void OnDisable() {
    acceptButton.onClick.RemoveListener(OnAcceptClicked);
    cancelButton.onClick.RemoveListener(OnCancelClicked);

    GameModeManager.OnStateChanged -= OnGameModeChanged;
  }

  private void Awake() { }
  private void Start() { }

  private void Update() {
    HandleTooltipRaycast();
  }

  private void HandleTooltipRaycast() {
    Ray ray = Camera.main.ScreenPointToRay(GameManager.MousePosition);

    if (Physics.Raycast(ray, out _hit, Mathf.Infinity, objectLayer)) {
      ObjectTooltipController objController = _hit.transform.GetComponent<ObjectTooltipController>();

      if (objController == null)
        HideInfoTooltip();
      else
        ShowInfoTooltip(objController.objectData.objectName, objController.objectData.description);
    }
    else {
      HideInfoTooltip();
    }
  }

  /**** PRIVATE ****/
  private void OnAcceptClicked() {
    GameModeManager.ChangeState(_acceptButtonState);
  }

  private void OnCancelClicked() {
    GameModeManager.ChangeState(_cancelButtonState);
  }

  private void OnGameModeChanged(EGameMode state) {
    switch (state) {
      case EGameMode.IDLE:
        HideTooltip();
        break;

      case EGameMode.BUILDING_MODE__SELECTING_CELL:
        HideButtons();
        ShowTooltipText(selectingAreaTooltipText);
        break;

      case EGameMode.BUILDING_MODE__SELECTING_AREA:
        break;

      case EGameMode.BUILDING_MODE__PLACING_ENTRANCE:
        HideButtons();
        ShowTooltipText(removeWallsTooltipText);
        break;

      case EGameMode.BUILDING_MODE__READY_FOR_OBJECTS:
        ShowTooltipText(createRoomTooltipText);
        ShowButtons();

        _acceptButtonState = EGameMode.BUILDING_MODE__PLACING_OBJECTS;
        _cancelButtonState = EGameMode.BUILDING_MODE__SELECTING_CELL;
        break;

      case EGameMode.BUILDING_MODE__PLACING_OBJECTS:
        ShowTooltipText(palceObjectsTooltipText);
        HideButtons();
        break;

      case EGameMode.BUILDING_MODE__READY_TO_FINISH_ROOM:
        break;

      case EGameMode.BUILDING_MODE__CANT_BUILD:
        HideButtons();
        ShowTooltipText(cantBuildTooltipText);
        break;

      case EGameMode.BUILDING_MODE__ROOM_FINISHED:
        HideTooltip();
        break;

      default:
        break;
    }
  }

  /**** PUBLIC ****/
  private void ShowTooltipText(string text = default) {
    tooltipContainer.SetActive(true);
    toolTipText.text = text;
  }

  private void HideTooltip() {
    HideButtons();
    tooltipContainer.SetActive(false);
  }

  private void HideButtons() {
    buttonsContainer.SetActive(false);

    _acceptButtonState = EGameMode.IDLE;
    _cancelButtonState = EGameMode.IDLE;
  }

  private void ShowButtons() {
    buttonsContainer.SetActive(true);
  }

  public void ShowInfoTooltip(string title, string description) {
    toolTipInfoTitleText.text = title;
    toolTipInfoDescriptionText.text = description;
    tooltipInfoContainer.SetActive(true);
  }

  public void HideInfoTooltip() {
    tooltipInfoContainer.SetActive(false);
  }

  /**** COROUTINES ****/
}

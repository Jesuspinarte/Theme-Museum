using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class GameManager : MonoBehaviour {
  private static GameManager _instance;

  public static GameManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<GameManager>();
        if (_instance == null) {
          GameObject go = new GameObject("GameManager");
          _instance = go.AddComponent<GameManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/
  [Header("Input Action References")]
  [SerializeField] private InputActionReference mouseMovementAction;

  [Header("Game Settings")]
  [SerializeField] private GameObject endScreen;

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/
  private static Camera _camera;
  public static Vector2 MousePosition = Vector2.zero;
  public static bool IsMouseOverUI => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

  /**** UNITY HOOKS ****/
  private void Awake() {
    _camera = Camera.main;
  }

  private void OnEnable() {
    GameModeManager.OnStateChanged += OnGameModeStateChanged;

    if (mouseMovementAction.action != null)
      mouseMovementAction.action.performed += OnMouseMove;
  }

  private void OnDisable() {
    GameModeManager.OnStateChanged -= OnGameModeStateChanged;

    // Mouse position
    if (mouseMovementAction.action != null)
      mouseMovementAction.action.performed -= OnMouseMove;
  }

  private void Start() { }
  private void Update() { }

  /**** PRIVATE ****/
  private void OnGameModeStateChanged(EGameMode state) {
    switch (state) {
      case EGameMode.GAME_OVER:
        endScreen.SetActive(true);
        SoundManager.Instance.PlaySfxSound(EFxSoundType.GAME_OVER);
        break;

      default:
        break;
    }
  }

  private void OnMouseMove(CallbackContext ctx) {
    MousePosition = ctx.ReadValue<Vector2>();
  }

  /**** PUBLIC ****/
  public static bool GetCurrentRaycast(LayerMask layerMask, out RaycastHit hitInfo) {
    Ray rayOrigin = _camera.ScreenPointToRay(MousePosition);
    return Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, layerMask);
  }

  /**** COROUTINES ****/
}

using UnityEngine;

public class CameraController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [Header("Pan Settings")]
  public float panSpeed = 20f;
  public float panBorderThickness = 15f;

  [Header("Map Limits")]
  public Vector2Int panLimitTop = new Vector2Int(50, 50);
  public Vector2Int panLimitBottom = new Vector2Int(-20, -20);

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void Awake() { }
  private void OnEnable() { }
  private void OnDisable() { }
  private void Start() { }

  private void Update() {
    Vector3 pos = transform.position;
    Vector2 mousePos = GameManager.MousePosition; ;

    Vector3 flatForward = transform.forward;
    flatForward.y = 0;
    flatForward.Normalize();

    Vector3 flatRight = transform.right;
    flatRight.y = 0;
    flatRight.Normalize();

    if (mousePos.y >= Screen.height - panBorderThickness) {
      pos += flatForward * panSpeed * Time.deltaTime;
    }
    else if (mousePos.y <= panBorderThickness) {
      pos -= flatForward * panSpeed * Time.deltaTime;
    }

    if (mousePos.x >= Screen.width - panBorderThickness) {
      pos += flatRight * panSpeed * Time.deltaTime;
    }
    else if (mousePos.x <= panBorderThickness) {
      pos -= flatRight * panSpeed * Time.deltaTime;
    }

    pos.x = Mathf.Clamp(pos.x, panLimitBottom.x, panLimitTop.x);
    pos.z = Mathf.Clamp(pos.z, panLimitBottom.y, panLimitTop.y);

    transform.position = pos;
  }

  /**** PRIVATE ****/

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

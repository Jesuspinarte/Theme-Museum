using UnityEngine;

public class FurnitureManager : MonoBehaviour {
  private static FurnitureManager _instance;

  public static FurnitureManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<FurnitureManager>();
        if (_instance == null) {
          GameObject go = new GameObject("FurnitureManager");
          _instance = go.AddComponent<FurnitureManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/

  /**** PUBLIC VARS ****/
  public static ObjectSO targetObject = null;

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void Awake() { }
  private void OnEnable() { }
  private void OnDisable() { }
  private void Start() { }
  private void Update() { }

  /**** PRIVATE ****/

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

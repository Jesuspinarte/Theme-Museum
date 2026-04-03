using UnityEngine;

public class ObjectTooltipController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [Header("Object Settings")]
  [field: SerializeField] public ObjectSO objectData { get; private set; }

  /**** PUBLIC VARS ****/

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

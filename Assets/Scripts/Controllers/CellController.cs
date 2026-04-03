using UnityEngine;

public class CellController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [Header("Mat Options")]
  [SerializeField] private Material errorMat;
  [SerializeField] private Material hoveredMat;
  [SerializeField] private Material selectedMat;

  [Header("Cell References")]
  [SerializeField] private GameObject plane;

  /**** PUBLIC VARS ****/
  public Vector2Int cellPosition { get; private set; }

  /**** PRIVATE VARS ****/
  private Material _originalMat;
  private MeshRenderer _planeMeshRenderer;

  /**** UNITY HOOKS ****/
  private void Awake() {
    _planeMeshRenderer = plane.GetComponent<MeshRenderer>();
    _originalMat = _planeMeshRenderer.sharedMaterial;
  }

  private void Start() {
    cellPosition = new Vector2Int((int)plane.transform.position.x, (int)plane.transform.position.z);
  }

  /**** PRIVATE ****/

  /**** PUBLIC ****/
  public void UpdateMaterial(EMat matType) {
    switch (matType) {
      case EMat.ERROR:
        _planeMeshRenderer.sharedMaterial = errorMat;
        break;
      case EMat.HOVER:
        _planeMeshRenderer.sharedMaterial = hoveredMat;
        break;
      case EMat.ORIGINAL:
        _planeMeshRenderer.sharedMaterial = _originalMat;
        break;
      case EMat.SELECTED:
        _planeMeshRenderer.sharedMaterial = selectedMat;
        break;
      default:
        _planeMeshRenderer.sharedMaterial = _originalMat;
        break;
    }
  }

  /**** COROUTINES ****/
}
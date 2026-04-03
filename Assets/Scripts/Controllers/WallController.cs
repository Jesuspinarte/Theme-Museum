using UnityEngine;
using UnityEngine.AI;

public class WallController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [SerializeField] private GameObject wallParticles;

  [Header("Mat Options")]
  [SerializeField] private Material errorMat;
  [SerializeField] private Material hoveredMat;
  [SerializeField] private Material selectedMat;

  /**** PRIVATE VARS ****/
  private Material _originalMat;
  private MeshRenderer _meshRenderer;
  private NavMeshObstacle _navMeshObstacle;

  /**** PUBLIC VARS ****/
  public bool _hasError { get; private set; } = false;

  /**** UNITY HOOKS ****/
  private void Awake() {
    _meshRenderer = GetComponent<MeshRenderer>();
    _navMeshObstacle = GetComponent<NavMeshObstacle>();
    _originalMat = _meshRenderer.sharedMaterial;
  }

  private void Start() { }
  private void Update() { }

  /**** PRIVATE ****/

  /**** PUBLIC ****/
  public void UpdateMaterial(EMat matType) {
    if (_hasError) {
      _meshRenderer.sharedMaterial = errorMat;
      return;
    }

    switch (matType) {
      case EMat.ERROR:
        _meshRenderer.sharedMaterial = errorMat;
        break;
      case EMat.HOVER:
        _meshRenderer.sharedMaterial = hoveredMat;
        break;
      case EMat.ORIGINAL:
        _meshRenderer.sharedMaterial = _originalMat;
        _navMeshObstacle.enabled = true;
        Instantiate(wallParticles, transform.position, wallParticles.transform.rotation);
        break;
      case EMat.SELECTED:
        _meshRenderer.sharedMaterial = selectedMat;
        break;
      default:
        _meshRenderer.sharedMaterial = _originalMat;
        break;
    }
  }

  public void SetHasError(bool hasError) {
    _hasError = hasError;

    if (hasError) UpdateMaterial(EMat.ERROR);
  }

  /**** COROUTINES ****/
}

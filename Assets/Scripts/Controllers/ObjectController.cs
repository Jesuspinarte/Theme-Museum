using UnityEngine;

public class ObjectController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [Header("Placement Settings")]
  [field: SerializeField] public Transform placementCheck { get; private set; }
  [SerializeField] private GameObject finalObject;
  [SerializeField] private GameObject previewObject;
  [SerializeField] private GameObject errorPreviewObject;
  [SerializeField] private GameObject builtParticles;

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void Awake() {
    UpdateObjectAssetByType(EObjectAsset.PREVIEW);
  }

  private void OnEnable() { }
  private void OnDisable() { }
  private void Start() { }
  private void Update() { }

  /**** PRIVATE ****/

  /**** PUBLIC ****/
  public void UpdateObjectAssetByType(EObjectAsset assetType) {
    switch (assetType) {
      case EObjectAsset.ERROR:
        finalObject.SetActive(false);
        previewObject.SetActive(false);
        errorPreviewObject.SetActive(true);
        break;

      case EObjectAsset.PREVIEW:
        finalObject.SetActive(false);
        previewObject.SetActive(true);
        errorPreviewObject.SetActive(false);
        break;

      case EObjectAsset.FINAL:
        finalObject.SetActive(true);
        previewObject.SetActive(false);
        errorPreviewObject.SetActive(false);
        Instantiate(builtParticles, transform.position, builtParticles.transform.rotation);
        break;

      default:
        break;
    }
  }

  /**** COROUTINES ****/
}

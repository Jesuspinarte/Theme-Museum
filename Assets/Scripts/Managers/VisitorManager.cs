using System.Collections;
using UnityEngine;

public class VisitorManager : MonoBehaviour {
  private static VisitorManager _instance;

  public static VisitorManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<VisitorManager>();
        if (_instance == null) {
          GameObject go = new GameObject("VisitorManager");
          _instance = go.AddComponent<VisitorManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/
  [Header("Visitor Settings")]
  [SerializeField] private VisitorController visitorPrefab;
  [SerializeField] private Transform spawnPoint;
  [SerializeField] private Transform visitorsContainer;
  [SerializeField] private float firstSpawnAfterSeconds = 10f;

  [Header("Dynamic Spawning Settings")]
  [SerializeField] private float slowestSpawnRate = 8f;
  [SerializeField] private float fastestSpawnRate = 1.5f;
  [SerializeField] private float timeReductionPerObject = 0.4f;

  /**** PUBLIC VARS ****/
  public Transform SpawnPoint => spawnPoint;
  public float bonusPerObject = 0.025f;

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void Awake() { }
  private void OnEnable() { }
  private void OnDisable() { }

  private void Start() {
    StartCoroutine(SpawnVisitor(firstSpawnAfterSeconds));
  }

  private void Update() { }

  /**** PRIVATE ****/
  private float CalculateCurrentSpawnRate() {
    int totalObjects = BuildingManager.totalObjectsInRooms;
    float calculatedRate = slowestSpawnRate - (totalObjects * timeReductionPerObject);
    return Mathf.Clamp(calculatedRate, fastestSpawnRate, slowestSpawnRate);
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
  private IEnumerator SpawnVisitor(float seconds) {
    yield return new WaitForSeconds(seconds);

    while (!GameModeManager.IsGameOver) {
      Instantiate(visitorPrefab, spawnPoint.position, Quaternion.identity, visitorsContainer);
      float currentWaitTime = CalculateCurrentSpawnRate();
      yield return new WaitForSeconds(currentWaitTime);
    }
  }
}

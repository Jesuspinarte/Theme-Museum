using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VisitorController : MonoBehaviour {
  /**** SERIALIZED VARS ****/
  [SerializeField] private NavMeshAgent navAgent;
  [SerializeField] private Vector2Int donationRange;

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/
  private Transform _targetObject = null;
  private EVisitorState _visitorState = EVisitorState.IDLE;

  /**** UNITY HOOKS ****/
  private void Awake() {
    if (BuildingManager.placedRoomsList.Count == 0) {
      Destroy(gameObject);
      return;
    }

    SetTargetDestinationObject();
  }

  private void OnEnable() { }
  private void OnDisable() { }

  private void Start() {

  }

  private void Update() {
    checkState();
  }

  /**** PRIVATE ****/
  private void SetTargetDestinationObject() {
    int randIdx = Random.Range(0, BuildingManager.placedRoomsList.Count);
    Room targetRoom = BuildingManager.placedRoomsList[randIdx];

    if (targetRoom.placedObjects.Count == 0) {
      Destroy(gameObject);
      return;
    }

    randIdx = Random.Range(0, targetRoom.placedObjects.Count);
    _targetObject = targetRoom.placedObjects[randIdx].transform;

    navAgent.SetDestination(_targetObject.position);
    _visitorState = EVisitorState.WALKING_TO_OBJECT;
  }

  private void LeaveMuseum() {
    navAgent.SetDestination(VisitorManager.Instance.SpawnPoint.position);
    _visitorState = EVisitorState.LEAVING;
  }

  private bool HasReachedDestination() {
    if (navAgent.pathPending)
      return false;

    if (navAgent.remainingDistance <= navAgent.stoppingDistance) {
      if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f) {
        return true;
      }
    }

    return false;
  }

  private void checkState() {
    switch (_visitorState) {
      case EVisitorState.WALKING_TO_OBJECT:
        if (HasReachedDestination())
          StartCoroutine(AdmireObject());
        break;

      case EVisitorState.LEAVING:
        if (HasReachedDestination())
          Destroy(gameObject);
        break;

      default:
        break;
    }
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
  private IEnumerator AdmireObject() {
    float timeToAdmire = Random.Range(0f, 3f);
    _visitorState = EVisitorState.OBSERVING;

    yield return new WaitForSeconds(timeToAdmire);

    // Should continue looking for another object?
    int totalObjectsInMuseum = BuildingManager.totalObjectsInRooms;
    float continueProbability = 0.2f + (totalObjectsInMuseum * VisitorManager.Instance.bonusPerObject);
    continueProbability = Mathf.Clamp(continueProbability, 0.2f, 0.85f);
    bool shouldContinue = Random.value <= continueProbability;

    if (shouldContinue)
      SetTargetDestinationObject();
    else
      LeaveMuseum();

    EconomyManager.Instance.AddRandomDonation(donationRange);
  }
}

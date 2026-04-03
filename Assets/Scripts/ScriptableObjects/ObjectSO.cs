using UnityEngine;

[CreateAssetMenu(fileName = "NewObject", menuName = "Museum/Object Type")]
public class ObjectSO : ScriptableObject {
  [Header("Basic Info")]
  public string objectName;
  public EObject type;
  public int cost;
  [TextArea] public string description;

  [Header("Simulation Stats")]
  public int reputationOnBuilt;
  public float satisfactionBonus;

  [Header("Placement Rules")]
  public EObjectPlacementRule placementRule = EObjectPlacementRule.ANYWHERE;
  public ERoom requiredRoomType;

  [Header("Visuals")]
  public ObjectController prefab;
}

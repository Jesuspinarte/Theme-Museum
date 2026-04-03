using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomType", menuName = "Museum/Room Type")]
public class RoomSO : ScriptableObject {
  [Header("Basic Info")]
  public string roomName;
  public ERoom roomType;
  public int costPerTile;
  [TextArea] public string description;

  [Header("Requirements")]
  public Vector2Int minSize = new Vector2Int(5, 5);
  public int requiredEntrances = 2;
  public int maxEntrances = 8;
  public EStaff requiredStaff;
  [Header("Content")]
  // public List<EObject> allowedObjectsTypeList;
  public EObject allowedObjectsTypeList;
  public List<ObjectSO> requiredObjectsTypeList;

  [Header("Visuals")]
  public Material floorMaterial;
  public Material wallMaterial;
}
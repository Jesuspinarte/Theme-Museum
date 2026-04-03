using UnityEngine;

public class GridManager : MonoBehaviour {
  private static GridManager _instance;

  public static GridManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<GridManager>();
        if (_instance == null) {
          GameObject go = new GameObject("GridManager");
          _instance = go.AddComponent<GridManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED ****/
  [Header("Grid Settings")]
  [SerializeField] private GameObject cellPrefab = null;
  [SerializeField] private Vector2Int gridSize = Vector2Int.one;
  [SerializeField] private Vector2Int mainEntranceArea = new Vector2Int(5, 2);

  /**** PRIVATE VARS ****/
  public static Cell[][] cellGrid { get; private set; }
  private static GameObject _gridContainer;

  /**** PUBLIC VARS ****/
  public Vector2Int GridSize => gridSize;

  /**** UNITY HOOKS ****/
  private void Awake() {
    _gridContainer = new GameObject("GridContainer");
    _gridContainer.transform.position = Vector3.zero;
    _gridContainer.transform.SetParent(transform);

    cellGrid = new Cell[gridSize.x][];

    for (int i = 0; i < gridSize.x; ++i) {
      cellGrid[i] = new Cell[gridSize.y];

      for (int j = 0; j < gridSize.y; ++j) {
        bool isOccupied = false;
        bool hasObject = false;

        Vector3 cellPos = new Vector3(i, 0, j);
        GameObject go = Instantiate(cellPrefab, cellPos, Quaternion.identity, _gridContainer.transform);

        if (i <= mainEntranceArea.x && j <= mainEntranceArea.y) {
          isOccupied = true;
          hasObject = true;
        }

        // Initializes cell data
        cellGrid[i][j] = new Cell(
          go.GetComponentInChildren<CellController>(),
          isOccupied,
          hasObject
        );
      }
    }
  }

  private void Start() { }

  private void Update() { }

  /**** PRIVATE ****/

  /**** PUBLIC ****/
  public static bool IsCellOccupied(int x, int z) {
    return cellGrid[x][z].isOccupied;
  }

  public static bool IsCellOccupied(Vector2Int position) {
    return cellGrid[position.x][position.y].isOccupied;
  }

  public static bool CellHasObject(int x, int z) {
    return cellGrid[x][z].hasObject;
  }

  public static bool CellHasObject(Vector2Int position) {
    return cellGrid[position.x][position.y].hasObject;
  }

  public static bool IsCellWithinBounds(int x, int z) {
    if (x < 0 || z < 0) return false;
    if (x >= cellGrid.Length || z >= cellGrid[0].Length) return false;

    return true;
  }

  public bool IsCellWithinBounds(Vector2Int position) {
    if (position.x < 0 || position.y < 0) return false;
    if (position.x >= gridSize.x || position.y >= gridSize.y) return false;

    return true;
  }

  public static void PaintCell(Vector2Int position, EMat matType, bool isOcupied) {
    Cell cell = cellGrid[position.x][position.y];

    cell.cellController.UpdateMaterial(matType);
    cell.isOccupied = cell.isOccupied || isOcupied;
  }

  public static void PaintCell(int x, int z, EMat matType, bool isOcupied) {
    Cell cell = cellGrid[x][z];

    cell.cellController.UpdateMaterial(matType);
    cell.isOccupied = cell.isOccupied || isOcupied;
  }

  public static void BlockCellForObjects(int x, int z) {
    cellGrid[x][z].hasObject = true;
  }

  public static void BlockCellForObjects(Vector2Int cellPosition) {
    cellGrid[cellPosition.x][cellPosition.y].hasObject = true;
  }

  /**** COROUTINES ****/
}

public class Cell {
  public CellController cellController;
  public bool isOccupied;
  public bool hasObject;

  public Cell(CellController controller, bool occupied, bool containsObject) {
    cellController = controller;
    isOccupied = occupied;
    hasObject = containsObject;
  }
}
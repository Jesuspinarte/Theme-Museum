[System.Flags]
public enum EObject {
  NOTHING = 0,
  BONE = 1 << 0,
  ASIAN = 1 << 1,
  AFRICAN = 1 << 2,
  LABORATORY = 1 << 3,
  FURNITURE = 1 << 4,
  DECORATIVE = 1 << 5,
  EVERYTHING = ~0
}

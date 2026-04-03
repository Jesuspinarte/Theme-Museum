using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour {
  private static EconomyManager _instance;

  public static EconomyManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<EconomyManager>();
        if (_instance == null) {
          GameObject go = new GameObject("EconomyManager");
          _instance = go.AddComponent<EconomyManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/
  [Header("Economy Settings")]
  [SerializeField] private int initialMoney;
  [SerializeField] private int targetDonation = 100;

  [Header("UI Settings")]
  [SerializeField] private TMP_Text moneyText;
  [SerializeField] private GameObject costContainer;
  [SerializeField] private TMP_Text costText;
  [SerializeField] private TMP_Text donationText;

  /**** PUBLIC VARS ****/
  public static int totalDonations = 0;
  public static int currentMoney = 1000;
  public static int currentBuildingCost = 0;

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void Awake() {
    currentMoney = initialMoney;
    UpdateMoneyText();
    UpdateDonationText();
  }

  private void OnEnable() {
    GameModeManager.OnStateChanged += OnGameModeStateChanged;
  }

  private void OnDisable() {
    GameModeManager.OnStateChanged -= OnGameModeStateChanged;
  }

  private void Start() { }
  private void Update() { }

  /**** PRIVATE ****/
  private void OnGameModeStateChanged(EGameMode state) {
    switch (state) {
      case EGameMode.IDLE:
        currentBuildingCost = 0;
        costContainer.SetActive(false);
        UpdateBuildingCostText();
        break;

      case EGameMode.BUILDING_MODE__SELECTING_CELL:
      case EGameMode.FURNITURE_MODE__PLACING_OBJECT:
        costContainer.SetActive(true);
        break;

      case EGameMode.BUILDING_MODE__ROOM_FINISHED:
      case EGameMode.FURNITURE_MODE__FINISH_PLACEMENT:
        currentMoney -= currentBuildingCost;
        UpdateMoneyText();
        GameModeManager.ChangeState(EGameMode.IDLE);
        break;

      default:
        break;
    }
  }

  /**** PUBLIC ****/
  /// <summary>
  /// Checks if a given price can be afforded
  /// </summary>
  public static bool CanAffordAmount(int amount) {
    if (currentMoney - amount < 0) return false;
    return true;
  }

  /// <summary>
  /// Checks if the current building cost can be afforded
  /// </summary>
  public static bool CanAfforBuildingCost() {
    if (currentMoney - currentBuildingCost < 0) return false;
    return true;
  }

  public void UpdateMoneyText() {
    moneyText.text = currentMoney.ToString();
  }

  public void UpdateDonationText() {
    donationText.text = $"{totalDonations}/{targetDonation}";

    if (totalDonations == targetDonation)
      GameModeManager.ChangeState(EGameMode.GAME_OVER);
  }

  public void UpdateBuildingCostText() {
    costText.text = currentBuildingCost.ToString();
  }

  public void AddBudget(int amount) {
    currentMoney += amount;
    UpdateMoneyText();
  }

  public void AddDonator() {
    ++totalDonations;
    UpdateDonationText();
  }

  public void AddRandomDonation(Vector2Int donationRange) {
    int randDonation = Random.Range(donationRange.x, donationRange.y);
    SoundManager.Instance.PlaySfxSound(EFxSoundType.DONATION);
    AddBudget(randDonation);
    AddDonator();
  }

  /**** COROUTINES ****/
}

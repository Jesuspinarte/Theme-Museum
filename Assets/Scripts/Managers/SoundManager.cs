using UnityEngine;

public class SoundManager : MonoBehaviour {
  private static SoundManager _instance;

  public static SoundManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<SoundManager>();
        if (_instance == null) {
          GameObject go = new GameObject("SoundManager");
          _instance = go.AddComponent<SoundManager>();
        }
      }
      return _instance;
    }
  }

  /**** SERIALIZED VARS ****/
  [Header("Audio Sources")]
  public AudioSource sfxSource;
  public AudioSource dragSource;
  public AudioSource musicSource;
  public AudioSource ambienceSource;

  [Header("Sound Fxs")]
  public AudioClip dragSound;
  public AudioClip cellHoveringSound;
  public AudioClip placementSound;
  public AudioClip donationSound;
  public AudioClip gameOverSound;

  [Header("Background Sounds")]
  public AudioClip bacgroundMusic;
  public AudioClip ambienceSounds;

  /**** PUBLIC VARS ****/

  /**** PRIVATE VARS ****/

  /**** UNITY HOOKS ****/
  private void Awake() { }

  private void OnEnable() {
    GameModeManager.OnStateChanged += OnGameModeStateChanged;
  }

  private void OnDisable() {
    GameModeManager.OnStateChanged -= OnGameModeStateChanged;
  }

    private void OnGameModeStateChanged(EGameMode state) {
    switch (state) {
      case EGameMode.GAME_OVER:
        musicSource.Stop();
        ambienceSource.Stop();
        break;

      default:
        break;
    }
  }

  private void Start() {
    if (bacgroundMusic != null) {
      musicSource.clip = bacgroundMusic;
      musicSource.loop = true;
      musicSource.Play();
    }

    if (ambienceSource != null) {
      ambienceSource.clip = ambienceSounds;
      ambienceSource.loop = true;
      ambienceSource.Play();
    }
  }

  private void Update() { }

  /**** PRIVATE ****/
  /**
 * Plays the specified sound effect.
 */
  public void PlaySfxSound(EFxSoundType sfx) {
    if (sfxSource == null) return;

    switch (sfx) {
      case EFxSoundType.CELL_HOVERING:
        sfxSource.PlayOneShot(cellHoveringSound);
        break;
      case EFxSoundType.DONATION:
        sfxSource.PlayOneShot(donationSound);
        break;
      case EFxSoundType.DRAG:
        dragSource.clip = dragSound;
        dragSource.Play();
        break;
      case EFxSoundType.GAME_OVER:
        sfxSource.PlayOneShot(gameOverSound);
        break;
      case EFxSoundType.PLACEMENT:
        sfxSource.PlayOneShot(placementSound);
        break;
      default:
        return;
    }
  }

  /**** PUBLIC ****/

  /**** COROUTINES ****/
}

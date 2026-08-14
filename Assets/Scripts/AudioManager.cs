using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioClips")]
    [SerializeField] private AudioClip[] music;
    [SerializeField] private AudioClip playerShootSfx;
    [SerializeField] private AudioClip playerHurtSfx;
    [SerializeField] private AudioClip playerDiedSfx;
    [SerializeField] private AudioClip enemyShootSfx;
    [SerializeField] private AudioClip enemyPunchSfx;
    [SerializeField] private AudioClip enemyGranadeSfx;
    [SerializeField] private AudioClip enemyDiedSfx;
    [SerializeField] private AudioClip buttonClickedSfx;

    [Header("AudioSources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerShoot playerShoot;

    private int _lastMusicIndex = -1;
    
    public static AudioManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }


    private void Start()
    {
        playerMovement.onDie += OnPlayerDied;
        playerMovement.onDamage += OnPlayerHurt;
        playerShoot.OnPlayerShot += OnPlayerShoot;

        PlayRandomMusic();
    }

    private void Update()
    {
        if (!musicSource.isPlaying)
            PlayRandomMusic();
    }

    private void OnDestroy()
    {

        if (playerMovement == null || playerShoot == null) return;
        playerMovement.onDie -= OnPlayerDied;
        playerMovement.onDamage -= OnPlayerHurt;
        playerShoot.OnPlayerShot -= OnPlayerShoot;
    }

    /*
    public void RegisterEnemy(FsmManager enemy)
    {
        enemy.onEnemyShoot += OnEnemyShoot;
        enemy.onEnemyPunch += OnEnemyPunch;
        enemy.onEnemyThrewGranade += OnEnemyGranade;
        enemy.onDie += OnEnemyDied;
    }

    public void UnregisterEnemy(FsmManager enemy)
    {
        enemy.onEnemyShoot -= OnEnemyShoot;
        enemy.onEnemyPunch -= OnEnemyPunch;
        enemy.onEnemyThrewGranade -= OnEnemyGranade;
        enemy.onDie -= OnEnemyDied;
    }
 */
    private void OnPlayerShoot() => sfxSource.PlayOneShot(playerShootSfx);
    private void OnPlayerHurt(float current, float total) => sfxSource.PlayOneShot(playerHurtSfx);
    private void OnPlayerDied() => sfxSource.PlayOneShot(playerDiedSfx);
    private void OnEnemyShoot() => sfxSource.PlayOneShot(enemyShootSfx);
    private void OnEnemyPunch() => sfxSource.PlayOneShot(enemyPunchSfx);
    private void OnEnemyGranade() => sfxSource.PlayOneShot(enemyGranadeSfx);
    private void OnEnemyDied() => sfxSource.PlayOneShot(enemyDiedSfx);
    private void OnButtonClicked() => sfxSource.PlayOneShot(buttonClickedSfx);

    private void PlayRandomMusic()
    {
        if (music.Length == 0) return;

        int randomIndex;
        do { randomIndex = Random.Range(0, music.Length); }
        while (randomIndex == _lastMusicIndex && music.Length > 1);

        _lastMusicIndex = randomIndex;
        musicSource.clip = music[randomIndex];
        musicSource.Play();
    }
}
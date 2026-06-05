using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource hookLoopSource;

    [Header("Start Menu Music Playlist")]
    [SerializeField] private AudioClip[] startMenuMusicPlaylist;
    [Range(0f, 1f)]
    [SerializeField] private float startMenuMusicVolume = 0.4f;

    [Header("Gameplay Music Playlist")]
    [SerializeField] private AudioClip[] gameplayMusicPlaylist;
    [Range(0f, 1f)]
    [SerializeField] private float gameplayMusicVolume = 0.4f;

    [Header("Shop / Highscore Music")]
    [SerializeField] private AudioClip shopMusic;
    [Range(0f, 1f)]
    [SerializeField] private float shopMusicVolume = 0.4f;

    [Header("General SFX")]
    [SerializeField] private AudioClip buttonClickSound;
    [Range(0f, 1f)]
    [SerializeField] private float buttonClickVolume = 1f;

    [SerializeField] private AudioClip hookShootSound;
    [Range(0f, 1f)]
    [SerializeField] private float hookShootVolume = 1f;

    [SerializeField] private AudioClip itemDeliverSound;
    [Range(0f, 1f)]
    [SerializeField] private float itemDeliverVolume = 1f;

    [SerializeField] private AudioClip explosionSound;
    [Range(0f, 1f)]
    [SerializeField] private float explosionVolume = 1f;

    [SerializeField] private AudioClip purchaseSound;
    [Range(0f, 1f)]
    [SerializeField] private float purchaseVolume = 1f;

    [SerializeField] private AudioClip fewMomentsLaterSound;
    [Range(0f, 1f)]
    [SerializeField] private float fewMomentsLaterVolume = 1f;

    [SerializeField] private AudioClip gameOverSound;
    [Range(0f, 1f)]
    [SerializeField] private float gameOverVolume = 1f;

    [Header("Gerrit Rewards")]
    [SerializeField] private AudioClip gerritCoinsSound;
    [Range(0f, 1f)]
    [SerializeField] private float gerritCoinsVolume = 1f;

    [SerializeField] private AudioClip gerritPieSound;
    [Range(0f, 1f)]
    [SerializeField] private float gerritPieVolume = 1f;

    [SerializeField] private AudioClip gerritStrengthSound;
    [Range(0f, 1f)]
    [SerializeField] private float gerritStrengthVolume = 1f;

    private int currentStartMenuMusicIndex;
    private int currentGameplayMusicIndex;

    private bool isPlayingStartMenuPlaylist;
    private bool isPlayingGameplayPlaylist;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        hookLoopSource = gameObject.AddComponent<AudioSource>();
        hookLoopSource.playOnAwake = false;
        hookLoopSource.loop = true;
    }

    private void Update()
    {
        if (musicSource == null) return;

        if (isPlayingStartMenuPlaylist && !musicSource.isPlaying)
            PlayNextStartMenuSong();

        if (isPlayingGameplayPlaylist && !musicSource.isPlaying)
            PlayNextGameplaySong();
    }

    public void PlayStartMenuMusic()
    {
        if (startMenuMusicPlaylist == null || startMenuMusicPlaylist.Length == 0) return;

        StopHookShoot();

        isPlayingStartMenuPlaylist = true;
        isPlayingGameplayPlaylist = false;

        currentStartMenuMusicIndex = Random.Range(0, startMenuMusicPlaylist.Length);
        PlayCurrentStartMenuSong();
    }

    private void PlayCurrentStartMenuSong()
    {
        if (startMenuMusicPlaylist[currentStartMenuMusicIndex] == null) return;

        musicSource.clip = startMenuMusicPlaylist[currentStartMenuMusicIndex];
        musicSource.volume = startMenuMusicVolume;
        musicSource.loop = false;
        musicSource.Play();
    }

    private void PlayNextStartMenuSong()
    {
        currentStartMenuMusicIndex++;

        if (currentStartMenuMusicIndex >= startMenuMusicPlaylist.Length)
            currentStartMenuMusicIndex = 0;

        PlayCurrentStartMenuSong();
    }

    public void PlayGameplayMusic()
    {
        if (gameplayMusicPlaylist == null || gameplayMusicPlaylist.Length == 0) return;

        StopHookShoot();

        isPlayingStartMenuPlaylist = false;
        isPlayingGameplayPlaylist = true;

        currentGameplayMusicIndex = Random.Range(0, gameplayMusicPlaylist.Length);
        PlayCurrentGameplaySong();
    }

    private void PlayCurrentGameplaySong()
    {
        if (gameplayMusicPlaylist[currentGameplayMusicIndex] == null) return;

        musicSource.clip = gameplayMusicPlaylist[currentGameplayMusicIndex];
        musicSource.volume = gameplayMusicVolume;
        musicSource.loop = false;
        musicSource.Play();
    }

    private void PlayNextGameplaySong()
    {
        currentGameplayMusicIndex++;

        if (currentGameplayMusicIndex >= gameplayMusicPlaylist.Length)
            currentGameplayMusicIndex = 0;

        PlayCurrentGameplaySong();
    }

    public void PlayShopMusic()
    {
        StopHookShoot();

        isPlayingStartMenuPlaylist = false;
        isPlayingGameplayPlaylist = false;

        if (shopMusic == null) return;

        musicSource.clip = shopMusic;
        musicSource.volume = shopMusicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        isPlayingStartMenuPlaylist = false;
        isPlayingGameplayPlaylist = false;

        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayButtonClick() => PlaySFX(buttonClickSound, buttonClickVolume);
    public void PlayItemDeliver() => PlaySFX(itemDeliverSound, itemDeliverVolume);
    public void PlayExplosion() => PlaySFX(explosionSound, explosionVolume);
    public void PlayPurchase() => PlaySFX(purchaseSound, purchaseVolume);
    public void PlayFewMomentsLater() => PlaySFX(fewMomentsLaterSound, fewMomentsLaterVolume);
    public void PlayGameOver() => PlaySFX(gameOverSound, gameOverVolume);

    public void PlayGerritCoins() => PlaySFX(gerritCoinsSound, gerritCoinsVolume);
    public void PlayGerritPie() => PlaySFX(gerritPieSound, gerritPieVolume);
    public void PlayGerritStrength() => PlaySFX(gerritStrengthSound, gerritStrengthVolume);

    public void PlayCustomSFX(AudioClip clip) => PlaySFX(clip, 1f);

    public void PlayHookShoot()
    {
        if (hookShootSound == null || hookLoopSource == null) return;
        if (hookLoopSource.isPlaying) return;

        hookLoopSource.clip = hookShootSound;
        hookLoopSource.volume = hookShootVolume;
        hookLoopSource.loop = true;
        hookLoopSource.Play();
    }

    public void StopHookShoot()
    {
        if (hookLoopSource != null && hookLoopSource.isPlaying)
            hookLoopSource.Stop();
    }

    private void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayCustomSFX(AudioClip clip, float volume)
{
    if (clip == null || sfxSource == null) return;

    sfxSource.PlayOneShot(clip, volume);
}
}
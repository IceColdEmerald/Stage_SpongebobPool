using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource hookLoopSource;

    [Header("Start Menu Music Playlist")]
    [SerializeField] private AudioClip[] startMenuMusicPlaylist;

    [Header("Gameplay Music Playlist")]
    [SerializeField] private AudioClip[] gameplayMusicPlaylist;

    [Header("Other Music")]
    [SerializeField] private AudioClip shopMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip bubbleTransitionSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip hookShootSound;
    [SerializeField] private AudioClip itemDeliverSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip purchaseSound;
    [SerializeField] private AudioClip noMoneySound;
    [SerializeField] private AudioClip levelClearSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip fewMomentsLaterSound;

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
        musicSource.loop = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        hookLoopSource = gameObject.AddComponent<AudioSource>();
        hookLoopSource.playOnAwake = false;
        hookLoopSource.loop = true;
    }

    private void Update()
    {
        if (musicSource == null)
            return;

        if (isPlayingStartMenuPlaylist && !musicSource.isPlaying)
            PlayNextStartMenuSong();

        if (isPlayingGameplayPlaylist && !musicSource.isPlaying)
            PlayNextGameplaySong();
    }

    public void PlayStartMenuMusic()
    {
        if (startMenuMusicPlaylist == null || startMenuMusicPlaylist.Length == 0)
            return;

        StopHookShoot();

        isPlayingStartMenuPlaylist = true;
        isPlayingGameplayPlaylist = false;

        currentStartMenuMusicIndex = Random.Range(0, startMenuMusicPlaylist.Length);
        PlayCurrentStartMenuSong();
    }

    private void PlayCurrentStartMenuSong()
    {
        if (startMenuMusicPlaylist[currentStartMenuMusicIndex] == null)
            return;

        musicSource.clip = startMenuMusicPlaylist[currentStartMenuMusicIndex];
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
        if (gameplayMusicPlaylist == null || gameplayMusicPlaylist.Length == 0)
            return;

        StopHookShoot();

        isPlayingStartMenuPlaylist = false;
        isPlayingGameplayPlaylist = true;

        currentGameplayMusicIndex = Random.Range(0, gameplayMusicPlaylist.Length);
        PlayCurrentGameplaySong();
    }

    private void PlayCurrentGameplaySong()
    {
        if (gameplayMusicPlaylist[currentGameplayMusicIndex] == null)
            return;

        musicSource.clip = gameplayMusicPlaylist[currentGameplayMusicIndex];
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

        PlayMusic(shopMusic);
    }

    public void StopMusic()
    {
        isPlayingStartMenuPlaylist = false;
        isPlayingGameplayPlaylist = false;

        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayBubbleTransition() => PlaySFX(bubbleTransitionSound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayItemDeliver() => PlaySFX(itemDeliverSound);
    public void PlayExplosion() => PlaySFX(explosionSound);
    public void PlayPurchase() => PlaySFX(purchaseSound);
    public void PlayNoMoney() => PlaySFX(noMoneySound);
    public void PlayLevelClear() => PlaySFX(levelClearSound);
    public void PlayGameOver() => PlaySFX(gameOverSound);
    public void PlayFewMomentsLater() => PlaySFX(fewMomentsLaterSound);
    public void PlayCustomSFX(AudioClip clip) => PlaySFX(clip);

    public void PlayHookShoot()
    {
        if (hookShootSound == null || hookLoopSource == null)
            return;

        if (hookLoopSource.isPlaying)
            return;

        hookLoopSource.clip = hookShootSound;
        hookLoopSource.loop = true;
        hookLoopSource.Play();
    }

    public void StopHookShoot()
    {
        if (hookLoopSource != null && hookLoopSource.isPlaying)
            hookLoopSource.Stop();
    }

    private void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null || musicSource == null)
            return;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(sfxClip);
    }
}
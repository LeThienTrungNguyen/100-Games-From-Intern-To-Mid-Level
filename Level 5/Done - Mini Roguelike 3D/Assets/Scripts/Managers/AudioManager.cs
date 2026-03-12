using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM Settings")]
    public List<AudioClip> playlist;
    [Range(0, 1)] public float bgmVolume = 0.5f;
    private AudioSource bgmSource;
    private int currentTrackIndex = -1;
    private float bgmFadeMultiplier = 1f; // Trở lại mặc định là 1

    [Header("SFX Master Settings")]
    [Range(0, 20)] public float sfxMasterMultiplier = 1.0f; // Hệ số nhân âm lượng SFX (0 - 5 lần)

    [Header("Basic Mining Sounds")]
    public AudioClip miningSFX;
    public AudioClip breakSFX;
    public AudioClip debrisSFX;
    public AudioClip pickupSFX;

    [Header("Special Mining Sounds")]
    public AudioClip metalHitSFX;
    public AudioClip gemHitSFX;

    [Header("New Action Sounds")]
    public AudioClip footstepSFX;
    public AudioClip paperSlideSFX;
    public AudioClip notificationSFX;
    public AudioClip letterOpenSFX;
    public AudioClip letterCloseSFX;
    public AudioClip deliveryBoxArrivedSFX;
    public AudioClip openDeliveryBoxSFX;
    public AudioClip claimSFX;
    public AudioClip counterTickSFX;
    public AudioClip bombThrowSFX;
    public AudioClip bombExplosionSFX;
    public AudioClip medkitUsingSFX;
    public AudioClip medkitFinishSFX;
    public AudioClip eatUsingSFX;
    public AudioClip eatFinishSFX;
    public AudioClip drinkUsingSFX;
    public AudioClip drinkFinishSFX;

    private AudioSource miningSource;
    private AudioSource footstepSource;
    private AudioSource usageSource;
    private float nextDebrisPlayTime = 0f;

    [Header("Pool Settings")]
    public int poolSize = 15;
    private List<AudioSource> sfxPool = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Khởi tạo AudioSource cho đào quặng
        miningSource = gameObject.AddComponent<AudioSource>();
        miningSource.spatialBlend = 0.5f; 
        miningSource.minDistance = 5f;   
        miningSource.maxDistance = 25f;
        miningSource.rolloffMode = AudioRolloffMode.Linear;

        // Khởi tạo AudioSource cho bước chân
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.spatialBlend = 0f; // Nghe trực tiếp từ tai người chơi (2D)
        footstepSource.playOnAwake = false;
        footstepSource.loop = true; // Chế độ lặp lại liên tục

        // Khởi tạo AudioSource cho âm thanh đang sử dụng (Medkit, Food, Drink)
        usageSource = gameObject.AddComponent<AudioSource>();
        usageSource.spatialBlend = 0f;
        usageSource.playOnAwake = false;
        usageSource.loop = true;

        // Khởi tạo AudioSource cho nhạc nền
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = false;
        bgmSource.spatialBlend = 0f;
        bgmSource.volume = 0f; // Bắt đầu từ 0

        // Khởi tạo SFX Pool
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject sfxObj = new GameObject("PooledSFX_" + i);
            sfxObj.transform.SetParent(this.transform);
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Add(source);
        }
    }

    private AudioSource GetAvailableSource()
    {
        // Tìm AudioSource đang không phát
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying) return source;
        }

        // Nếu hết sạch, mở rộng pool thêm một chút
        GameObject newSfxObj = new GameObject("PooledSFX_Extra");
        newSfxObj.transform.SetParent(this.transform);
        AudioSource newSource = newSfxObj.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        sfxPool.Add(newSource);
        return newSource;
    }

    private void Start()
    {
        PlayNextTrack();
    }

    private void Update()
    {
        if (!bgmSource.isPlaying && playlist != null && playlist.Count > 0)
        {
            PlayNextTrack();
        }

        // Áp dụng hệ số nhân âm lượng để hỗ trợ Fade In từ IntroManager
        if (bgmSource != null)
        {
            float targetVol = bgmVolume * bgmFadeMultiplier;
            // Nếu không đang trong quá trình chuyển bài của Coroutine thì mới áp dụng trực tiếp
            bgmSource.volume = targetVol;
        }
    }

    public void SetBGMFadeMultiplier(float multiplier)
    {
        bgmFadeMultiplier = Mathf.Clamp01(multiplier);
    }

    private void PlayNextTrack()
    {
        if (playlist == null || playlist.Count == 0) return;
        int nextIndex = Random.Range(0, playlist.Count);
        if (playlist.Count > 1 && nextIndex == currentTrackIndex)
            nextIndex = (nextIndex + 1) % playlist.Count;

        currentTrackIndex = nextIndex;
        StartCoroutine(Co_FadeTrack(playlist[currentTrackIndex]));
    }

    private IEnumerator Co_FadeTrack(AudioClip newClip)
    {
        float fadeTime = 1.5f;
        if (bgmSource.isPlaying)
        {
            float startVol = bgmSource.volume;
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVol, 0, t / fadeTime);
                yield return null;
            }
        }
        bgmSource.clip = newClip;
        bgmSource.Play();
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, bgmVolume, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = bgmVolume;
    }

    public void PlayMiningSound(Vector3 position, VoxelChunk.BlockType type)
    {
        if (miningSource.isPlaying) return;

        AudioClip clipToPlay = miningSFX;
        if (type == VoxelChunk.BlockType.Iron || type == VoxelChunk.BlockType.Gold)
            clipToPlay = metalHitSFX;
        else if (type == VoxelChunk.BlockType.Diamond)
            clipToPlay = gemHitSFX;

        if (clipToPlay != null)
        {
            miningSource.transform.position = position;
            miningSource.clip = clipToPlay;
            // Áp dụng hệ số Master Multiplier
            miningSource.volume = Mathf.Clamp(0.6f * sfxMasterMultiplier, 0, 1.0f);
            miningSource.pitch = Random.Range(0.95f, 1.05f);
            miningSource.Play();
        }
    }

    public void StopMiningSound()
    {
        if (miningSource != null && miningSource.isPlaying) miningSource.Stop();
    }

    public void PlayDebrisSound(Vector3 position)
    {
        if (Time.time < nextDebrisPlayTime) return;
        if (debrisSFX != null)
        {
            PlayClip(debrisSFX, position, 0.4f);
            nextDebrisPlayTime = Time.time + debrisSFX.length;
        }
    }

    public void PlayBreakSound(Vector3 position) => PlayClip(breakSFX, position, 1.0f);
    public void PlayPickupSound(Vector3 position) => PlayClip(pickupSFX, position, 0.8f);

    // PHƯƠNG THỨC MỚI CHO CÁC HÀNH ĐỘNG
    public void PlayFootstepSound()
    {
        if (footstepSFX == null) return;
        
        // Nếu đã đang phát rồi thì không làm gì cả (để nó tự loop)
        if (footstepSource.isPlaying) return;

        footstepSource.clip = footstepSFX;
        footstepSource.volume = Mathf.Clamp(0.3f * sfxMasterMultiplier, 0, 1.0f);
        footstepSource.pitch = 1.0f; // Để pitch cố định cho âm thanh loop ổn định hơn, hoặc random nhẹ nếu clip ngắn
        footstepSource.Play();
    }

    public void StopFootstepSound()
    {
        if (footstepSource.isPlaying) footstepSource.Stop();
    }

    public void PlayPaperSlideSound(Vector3 position) => PlayClip(paperSlideSFX, position, 0.7f);
    public void PlayNotificationSound(Vector3 position) => PlayClip(notificationSFX, position, 0.6f);
    public void PlayLetterOpenSound(Vector3 position) => PlayClip(letterOpenSFX, position, 0.8f);
    public void PlayLetterCloseSound(Vector3 position) => PlayClip(letterCloseSFX, position, 0.8f);
    public void PlayDeliveryBoxArrivedSound(Vector3 position) => PlayClip(deliveryBoxArrivedSFX, position, 1.0f);
    public void PlayOpenDeliveryBoxSound(Vector3 position) => PlayClip(openDeliveryBoxSFX, position, 0.9f);
    public void PlayClaimSound(Vector3 position) => PlayClip(claimSFX, position, 0.8f);
    public void PlayCounterTickSound(Vector3 position) => PlayClip(counterTickSFX, position, 0.4f);
    public void PlayBombThrowSound(Vector3 position) => PlayClip(bombThrowSFX, position, 0.7f);
    public void PlayBombExplosionSound(Vector3 position) => PlayClip(bombExplosionSFX, position, 1.2f);

    public void PlayMedkitUsingSound() => PlayUsageSound(medkitUsingSFX, 0.6f);
    public void PlayEatUsingSound() => PlayUsageSound(eatUsingSFX, 0.6f);
    public void PlayDrinkUsingSound() => PlayUsageSound(drinkUsingSFX, 0.6f);
    public void StopUsageSound() { if (usageSource.isPlaying) usageSource.Stop(); }

    public void PlayMedkitFinishSound(Vector3 position) => PlayClip(medkitFinishSFX, position, 0.8f);
    public void PlayEatFinishSound(Vector3 position) => PlayClip(eatFinishSFX, position, 0.8f);
    public void PlayDrinkFinishSound(Vector3 position) => PlayClip(drinkFinishSFX, position, 0.8f);

    private void PlayUsageSound(AudioClip clip, float volume)
    {
        if (clip == null || usageSource.clip == clip && usageSource.isPlaying) return;
        usageSource.clip = clip;
        usageSource.volume = Mathf.Clamp(volume * sfxMasterMultiplier, 0, 1.0f);
        usageSource.Play();
    }

    private void PlayClip(AudioClip clip, Vector3 position, float baseVolume)
    {
        if (clip == null) return;
        
        AudioSource source = GetAvailableSource();
        if (source == null) return;

        source.transform.position = position;
        source.clip = clip;
        source.volume = Mathf.Clamp(baseVolume * sfxMasterMultiplier, 0, 5.0f);
        
        source.spatialBlend = 0.5f; 
        source.minDistance = 5f;
        source.maxDistance = 30f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.pitch = Random.Range(0.95f, 1.05f);
            
        source.Play();
    }
}
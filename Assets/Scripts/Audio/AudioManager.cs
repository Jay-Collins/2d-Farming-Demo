using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _audioSourceSFX;
    [SerializeField] private AudioSource _audioSourceMenuSFX;
    
    [Header("Sound Effects")] 
    [SerializeField] private AudioClip _digSFX;
    [SerializeField] private AudioClip _waterSFX;
    [SerializeField] private AudioClip _seedsSFX;
    [SerializeField] private AudioClip _pickupSFX;
    [SerializeField] private AudioClip _menuClickSFX;
    [SerializeField] private AudioClip _doorCloseSFX;

    public void PlayDigSFX()
    {
        _audioSourceSFX.clip = _digSFX;
        _audioSourceSFX.Play();
    }

    public void PlayWaterSFX()
    {
        _audioSourceSFX.clip = _waterSFX;
        _audioSourceSFX.Play();
    }

    public void PlaySeedsSFX()
    {
        _audioSourceSFX.clip = _seedsSFX;
        _audioSourceSFX.Play();
    }

    public void PlayPickupSFX()
    {
        _audioSourceSFX.clip = _pickupSFX;
        _audioSourceSFX.Play();
    }

    public void PlayMenuWhistleSFX()
    {
        _audioSourceMenuSFX.clip = _menuClickSFX;
        _audioSourceMenuSFX.Play();
    }

    public void PlayDoorCloseSFX()
    {
        _audioSourceSFX.clip = _doorCloseSFX;
        _audioSourceSFX.Play();
    }
}

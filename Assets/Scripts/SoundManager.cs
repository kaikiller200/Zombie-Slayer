using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    // empty mag sound
    public AudioSource EmptyMagSound;

    [Header("Audio Channels")]
    public AudioSource ShootingChannel;
    public AudioSource ReloadChannel;

    [Header("shot audio clips")]
    public AudioClip M1911Shot;
    public AudioClip M4_8Shot;

    [Header("reload audio clips")]
    public AudioClip M4_8Reload;
    public AudioClip M1911Reload;

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.M1911:
                ShootingChannel.PlayOneShot(M1911Shot);
                break;
            case WeaponModel.M4_8: 
                ShootingChannel.PlayOneShot(M4_8Shot);
                break;
        }
    }
    public void PlayReloadingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.M1911:
                ReloadChannel.PlayOneShot(M1911Reload);
                break;
            case WeaponModel.M4_8:
                ReloadChannel.PlayOneShot(M4_8Reload);
                break;
        }
    }

}

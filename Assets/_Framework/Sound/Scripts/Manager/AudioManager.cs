using Ironcow;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource effect;

    [SerializeField] private Dictionary<string, AudioClip> audioPool = new Dictionary<string, AudioClip>();


    [HideInInspector] public float bgmVolume = 0.5f;
    [HideInInspector] public float effectVolume = 0.5f;

    [HideInInspector] public bool isInit;
    public void Init()
    {
    }

    public async void PlayBgm(string key, bool isLoop = true)
    {
        if (!audioPool.ContainsKey(key))
            audioPool.Add(key, await ResourceManager.instance.LoadAsset<AudioClip>(key, eAddressableType.Audio));
        source.clip = audioPool[key];
        source.loop = isLoop;
        source.Play();
    }

    public async void PlayOneShot(string key)
    {
        if (!audioPool.ContainsKey(key))
            audioPool.Add(key, await ResourceManager.instance.LoadAsset<AudioClip>(key, eAddressableType.Audio));
        effect.PlayOneShot(audioPool[key]);
    }

    public void StopBgm()
    {
        source.Stop();
    }

    public void SetBgmVolume(float volume)
    {
        source.volume = volume;
        bgmVolume = volume;
    }

    public void SetEffectVolume(float volume)
    {
        effect.volume = volume;
        effectVolume = volume;
    }

    public void SelectedCardSound(CardType cardType)
    {
        switch(cardType)
        {
            case CardType.Bbang:
                PlayOneShot("Fire01");
                break;
            case CardType.DeathMatch:
                PlayOneShot("DeathMatch");
                break;
            case CardType.Absorb:
                PlayOneShot("Absorb");
                break;
            case CardType.Hallucination:
                PlayOneShot("Swish");
                break;
            case CardType.MaturedSavings:
            case CardType.WinLottery:
                PlayOneShot("Acquire");
                break;
            case CardType.ContainmentUnit:
                PlayOneShot("BombTarget");
                break;
            case CardType.SatelliteTarget:
                PlayOneShot("SatelliteTarget");
                break;
            case CardType.Bomb:
                PlayOneShot("BombTarget");
                break;
            case CardType.AutoRifle:
            case CardType.DesertEagle:
            case CardType.SniperGun:
            case CardType.HandGun:
            case CardType.LaserPointer:
            case CardType.AutoShield:
            case CardType.StealthSuit:
            case CardType.Radar:
                PlayOneShot("HandlingIn");
                break;
            case CardType.Vaccine:
            case CardType.Call119:
                PlayOneShot("Heal");
                break;
            case CardType.Shield:
                PlayOneShot("Shield");
                break;
            case CardType.BigBbang:
            case CardType.Guerrilla:
                PlayOneShot("MachineGun");
                break;
        }
    }

}
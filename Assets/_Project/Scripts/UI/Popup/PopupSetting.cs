using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;

public class PopupSetting : UIBase
{
    [SerializeField] private Slider bgm;
    [SerializeField] private Slider effect;
    public override void Opened(object[] param)
    {
        bgm.value = AudioManager.instance.bgmVolume;
        effect.value = AudioManager.instance.effectVolume;
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupSetting>();
        AudioManager.instance.PlayOneShot("Button");
    }

    public void OnChangeBgm(float value)
    {
        AudioManager.instance.SetBgmVolume(value);
        bgm.value = value;
    }

    public void OnChangeEffect(float value)
    {
        AudioManager.instance.SetEffectVolume(value);
        effect.value = value;
    }

    public void OnLogout()
    {
        DataManager.instance.OnLogout();
        HideDirect();
        AudioManager.instance.PlayOneShot("Button");
    }
}
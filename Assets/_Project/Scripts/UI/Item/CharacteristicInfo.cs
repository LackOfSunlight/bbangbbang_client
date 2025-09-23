using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CharacteristicInfo : MonoBehaviour
{
    public Image image;
    public TMP_Text name;
    public TMP_Text description;


    public void Setting(Sprite sprite, string name, string description)
    {
        image.sprite = sprite;
        this.name.text = name;
        this.description.text = description;
    }

}

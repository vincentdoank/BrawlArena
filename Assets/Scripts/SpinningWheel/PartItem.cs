using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WTI.SpinningWheel
{
    public class PartItem : MonoBehaviour
    {
        public string avatarId;
        public Image partImage;
        public Image iconImage;

        public void Init(string avatarId, Color partColor, float partSize, Sprite iconSprite)
        {
            this.avatarId = avatarId;
            partImage.color = partColor;
            partImage.fillAmount = partSize;
            iconImage.sprite = iconSprite;
        }
    }
}

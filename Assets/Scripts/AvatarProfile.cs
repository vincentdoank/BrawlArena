using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarProfile : MonoBehaviour
{
    public Image avatarImage;
    public TMP_Text scoreText;

    public void SetAvatar(Sprite sprite)
    {
        avatarImage.sprite = sprite;
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
}

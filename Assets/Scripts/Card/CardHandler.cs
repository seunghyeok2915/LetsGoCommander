using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    CardPower cardPower;

    public Image illust;
    public Text cardText;

    public void Init(CardPower cardPower)
    {
        this.cardPower = cardPower;

        illust.sprite = cardPower.illust;
        cardText.text = cardPower.cardName;
    }
}

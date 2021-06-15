using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "AfterSchool/CardGame/Card")]
public class Card : ScriptableObject
{
    public string id;
    public string tagString;

    public bool usable;
    public bool disposable;

    public CardPower power;

    public void Init(string id, string tagString, CardPower defultCP, bool dispose = false, bool usable = true)
    {
        this.id = id;
        this.tagString = tagString;
        this.disposable = dispose;

        power = defultCP;
    }

    public Card Clone(bool setDispose = false)
    {
        var card = CreateInstance<Card>();

        bool dispose = setDispose || this.disposable;
        card.Init(id, tagString, power, dispose);
        return card;
    }

    public void OnUse()
    {

    }

    public void OnDraw()
    {

    }

    public void OnDrop()
    {

    }

    public void OnTurnEnd()
    {

    }
}

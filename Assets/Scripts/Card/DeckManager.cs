using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public Deck initialDeck;
    private Deck playerDeck;

    public List<CardHandler> cardsInHand;

    public void Start()
    {
        // Initial Deck 에서 player Deck 으로 Clone
        playerDeck = initialDeck;

        playerDeck.Shuffle();
        Draw();
        playerDeck.Shuffle();
        Draw();
        playerDeck.Shuffle();
        Draw();
    }

    public void Draw()
    {
        InstantiateCardObject(playerDeck.Draw());
        // Draw 호출 되면 InstantiateCardObject 실행
    }

    public void InstantiateCardObject(Card cardData)
    {
        var cardObject = Instantiate(cardPrefab, this.transform).GetComponent<CardHandler>();
        cardsInHand.Add(cardObject);
        cardObject.Init(cardData.power);
        // cardsInHands에 넣고, CardHandler 에서 initialize 실행
    }
}
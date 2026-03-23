using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public TMP_Text finalMessage;
    public TMP_Text probMessage;
    [SerializeField]private TMP_Text puntos_player;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
        CalculateProbabilities();
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */

        for (int i = 0; i < 52; i++)
        {
            // Como J,K y Q (Cartas 11, 12 y 13) tienen valor 10, aplicamos el if para filtrarlos
            // Y así les asignamos el valor de 10
            int cardValue = i % 13;

            if (cardValue == 0)       // AS
                values[i] = 11;
            else if (cardValue >= 10) // J Q K
                values[i] = 10;
            else
                values[i] = cardValue + 1;
        }



    }
    public int[] Shuffle = new int[52];
    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        // llenar array
        for (int i = 0; i < 52; i++)
        {
            Shuffle[i] = i;
        }

        // mezclar, aplicando el algoritmo de Fisher-Yates
        // Como funciona es, que recorre el array intercambiando de lugar los valores de manera aleatoria
        for (int i = 0; i < 52; i++)
        {
            int r = Random.Range(i, 52);
            int temp = Shuffle[i];
            Shuffle[i] = Shuffle[r];
            Shuffle[r] = temp;
        }

    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            puntos_player.text = player.GetComponent<CardHand>().points.ToString();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "¡Blackjack! Has ganado";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
         else if (dealer.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "¡Blackjack! Ha ganado el dealer";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
    }

    private void CalculateProbabilities()
    {

        /*Calcular las probabilidades de:
        
        Probabilidad de que el jugador obtenga más de 21 si pide una carta
        */

        // Nos aseguramos de que ambos tienen al menos 2 cartas antes de calcular
        if (player.GetComponent<CardHand>().cards.Count < 2 || dealer.GetComponent<CardHand>().cards.Count < 2)
        {
            probMessage.text = "";
            return;
        }
        // Obtenemos las manos del jugador y el dealer para conocer sus puntos
        CardHand playerHand = player.GetComponent<CardHand>();
        CardHand dealerHand = dealer.GetComponent<CardHand>();

        // Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador

        int dealerHigherCount = 0; //contador de casos en los que el dealer superaría al jugador con su carta oculta

        
        List<int> unseenValues = new List<int>();//lista de las cartas que aún no han salido 
        for (int i = cardIndex; i < 52; i++)
        {
            unseenValues.Add(values[Shuffle[i]]); // Añadir cartas restantes en el mazo
        }
        unseenValues.Add(dealerHand.cards[0].GetComponent<CardModel>().value); // Añadir la carta oculta

        foreach (int unseenVal in unseenValues)
        {
            // Calculamos qué puntos tendría el dealer si esta carta fuera su oculta
            int dealerHypotheticalPoints = CalculateHypotheticalPoints(dealerHand.cards, unseenVal, true);

            // Si el dealer nos superaría (y no se pasa de 21)
            if (dealerHypotheticalPoints > playerHand.points && dealerHypotheticalPoints <= 21)
            {
                dealerHigherCount++;
            }
        }


        //Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta

        int player17to21Count = 0;
        int playerBustCount = 0;
        int remainingInDeck = 52 - cardIndex;

        for (int i = cardIndex; i < 52; i++)
            {
              int nextCardVal = values[Shuffle[i]];

            // Probabilidad de que el jugador obtenga más de 21 si pide una carta
            int playerHypotheticalPoints = CalculateHypotheticalPoints(playerHand.cards, nextCardVal, false);

              if (playerHypotheticalPoints >= 17 && playerHypotheticalPoints <= 21)
                  player17to21Count++;

              if (playerHypotheticalPoints > 21)
                  playerBustCount++;            
        }

  
        //  mostrar los resultados  EN texto
    
        float probDealerHigher = ((float)dealerHigherCount / unseenValues.Count) * 100f;
        float prob17to21 = remainingInDeck > 0 ? ((float)player17to21Count / remainingInDeck) * 100f : 0;
        float probBust = remainingInDeck > 0 ? ((float)playerBustCount / remainingInDeck) * 100f : 0;

        probMessage.text = $"Probabilidades:\n" +
                           $"Dealer mas puntos que Jugador: {probDealerHigher:F1}%\n" +
                           $"17 a 21 con el siguiente (Hit): {prob17to21:F1}%\n" +
                           $"Pasarse con el siguiente (Hit): {probBust:F1}%";
    }

    // --- MÉTODO AUXILIAR ---
    // Calcula los puntos si se añadiera una carta extra a la mano evaluada.
    private int CalculateHypotheticalPoints(List<GameObject> handCards, int newCardValue, bool isDealerEvaluation)
    {
        int val = 0;
        int aces = 0;

        // Si es el dealer, ignoramos su carta real en la pos 0 
        int startIndex = isDealerEvaluation ? 1 : 0;

        for (int i = startIndex; i < handCards.Count; i++)
        {
            int cardVal = handCards[i].GetComponent<CardModel>().value;
            if (cardVal == 11) aces++;
            else val += cardVal;
        }

        // Sumamos la carta que estamos "imaginando" que sale
        if (newCardValue == 11) aces++;
        else val += newCardValue;

        // Lógica para que los Ases valgan 11 o 1 sin pasarse
        for (int i = 0; i < aces; i++)
        {
            if (val + 11 <= 21) val += 11;
            else val += 1;
        }

        return val;
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        int index = Shuffle[cardIndex];
        dealer.GetComponent<CardHand>().Push(faces[index], values[index]);
        cardIndex++;
        CalculateProbabilities();
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        int index = Shuffle[cardIndex];
        player.GetComponent<CardHand>().Push(faces[index], values[index]);
        cardIndex++;
        CalculateProbabilities();
    }


    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (cardIndex == 4)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
        }
        

        //Repartimos carta al jugador
        PushPlayer();
        puntos_player.text = player.GetComponent<CardHand>().points.ToString();
        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Perdiste!!!";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (cardIndex == 4)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
        }

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        while (dealer.GetComponent<CardHand>().points <= 16) {
            PushDealer();
        }
        if(dealer.GetComponent<CardHand>().points > 16)
        {
            if (dealer.GetComponent<CardHand>().points > 21)
            {
                finalMessage.text = "Ganaste!!!";
                hitButton.interactable = false;
                stickButton.interactable = false;

            }else if (dealer.GetComponent<CardHand>().points < player.GetComponent<CardHand>().points) // |puntos| dealer < player
            {
                finalMessage.text = "Ganaste!!!";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
            else
            {
                finalMessage.text = "Perdiste!!!";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
        }


    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().ClearCards();
        dealer.GetComponent<CardHand>().ClearCards();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}

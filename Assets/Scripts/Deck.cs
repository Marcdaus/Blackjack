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
    [SerializeField] private TMP_Text puntos_player;


    // --- VARIABLES DE APUESTAS Y DINERO ---
    public TMP_Text moneyText;       // Texto para el dinero del jugador
    public TMP_Text betText;         // Texto para la apuesta actual
    public Button bet10Button;       // Botón para apostar 10€
    public Button bet100Button;      // Botón para apostar 100€
    public Button bet1000Button;     // Botón para apostar 1000€
    public Button bet10MinusButton;       // Botón para apostar -10€
    public Button bet100MinusButton;      // Botón para apostar -100€
    public Button bet1000MinusButton;     // Botón para apostar -1000€
    public Button dealButton;        // Botón para repartir las cartas
    public Button restartButton;        // Botón para reiniciar la banca


    public int banca = 1000;         // Dinero inicial
    public int apuestaActual = 0;    // Lo que el jugador ha apostado esta ronda
    private bool enFaseApuestas = true; // Controla si estamos en el momento de apostar

    public int[] values = new int[52];
    int cardIndex = 0;

    private void Awake()
    {
        InitCardValues();
    }

    private void Start()
    {
        ShuffleCards();

        // Al empezar, activamos la fase de apuestas y desactivamos los botones de juego
        enFaseApuestas = true;
        ActualizarTextosApuestas();

        hitButton.interactable = false;
        stickButton.interactable = false;
        playAgainButton.interactable = false;
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

    // --- MÉTODOS DE APUESTAS ---
    public void Apostar10() { RealizarApuesta(10); }
    public void Apostar100() { RealizarApuesta(100); }
    public void Apostar1000() { RealizarApuesta(1000); }
    public void ApostarMinus10() { DeshacerApuesta(10); }
    public void ApostarMinus100() { DeshacerApuesta(100); }
    public void ApostarMinus1000() { DeshacerApuesta(1000); }

    public void Restart()
    {
        // Reiniciamos todo a sus valores por defecto y actualizamos la interfaz
        banca = 1000;
        apuestaActual = 0;
        ActualizarTextosApuestas();
    }

    private void RealizarApuesta(int cantidad)
    {
        // Comprobamos que haya suficiente banca en la mesa para quitar
        if (banca >= cantidad && enFaseApuestas)
        {
            banca -= cantidad;          // Restamos el dinero a la banca
            apuestaActual += cantidad;  // Se lo sumamos a la mesa
            ActualizarTextosApuestas(); // Actualizamos la interfaz gráfica
        }
    }
    private void DeshacerApuesta(int cantidad)
    {
        // Comprobamos que haya suficiente apuesta en la mesa para quitar
        if (apuestaActual >= cantidad && enFaseApuestas)
        {
            banca += cantidad;           // Devolvemos el dinero a la banca
            apuestaActual -= cantidad;   // Se lo restamos a la mesa
            ActualizarTextosApuestas();  // Actualizamos la interfaz gráfica
        }
    }

    public void RepartirCartas()
    {
        // Empezamos la partida y desactivamos las apuestas
        enFaseApuestas = false;
        ActualizarTextosApuestas();

        hitButton.interactable = true;
        stickButton.interactable = true;
        StartGame();
    }

    private void ActualizarTextosApuestas()
    {
        if (moneyText != null) moneyText.text = $"{banca}€";
        if (betText != null) betText.text = $"{apuestaActual}€";
        // En fase de apuestas, activamos o desactivamos los botones según la cantidad de dinero que haya en la banca o en la apuesta actual
        if (enFaseApuestas)
        {
            if (bet10Button != null) bet10Button.interactable = (banca >= 10);
            if (bet100Button != null) bet100Button.interactable = (banca >= 100);
            if (bet1000Button != null) bet1000Button.interactable = (banca >= 1000);
            if (bet10MinusButton != null) bet10MinusButton.interactable = (apuestaActual >= 10);
            if (bet100MinusButton != null) bet100MinusButton.interactable = (apuestaActual >= 100);
            if (bet1000MinusButton != null) bet1000MinusButton.interactable = (apuestaActual >= 1000);
            if (dealButton != null) dealButton.interactable = (apuestaActual > 0);

            if (restartButton != null) restartButton.interactable = true;
        }
        else // Si no estamos en fase de apuestas, desactivamos todos los botones relacionados con las apuestas
        {
            if (bet10Button != null) bet10Button.interactable = false;
            if (bet100Button != null) bet100Button.interactable = false;
            if (bet1000Button != null) bet1000Button.interactable = false;
            if (bet10MinusButton != null) bet10MinusButton.interactable = false;
            if (bet100MinusButton != null) bet100MinusButton.interactable = false;
            if (bet1000MinusButton != null) bet1000MinusButton.interactable = false;
            if (dealButton != null) dealButton.interactable = false;

            if (restartButton != null) restartButton.interactable = false;
        }
    }

    // Método acabar la ronda y repartimos dinero o quitamos, también el mensaje final
    private void TerminarJuego(string resultado)
    {
        hitButton.interactable = false;
        stickButton.interactable = false;
        playAgainButton.interactable = true;

        //========GANAR Y PERDER=========
        if (resultado == "ganar")
        {
            finalMessage.text = "Ganaste!!!";
            banca += apuestaActual * 2;
        }
        else if (resultado == "perder")
        {
            finalMessage.text = "Perdiste!!!";
        }
        //========EMPATE=========
        else if (resultado == "empate")
        {
            finalMessage.text = "Empate!!!";
            banca += apuestaActual;
        }
        //========CASOS BLACKJACK=========
        else if (resultado == "blackjack_player")
        {
            finalMessage.text = "¡Blackjack! Has ganado";
            banca += apuestaActual * 2;
        }
        else if (resultado == "blackjack_dealer")
        {
            finalMessage.text = "¡Blackjack! Ha ganado el dealer";
        }

        apuestaActual = 0;
        ActualizarTextosApuestas();
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            puntos_player.text = player.GetComponent<CardHand>().points.ToString();

            
        }
        /*TODO:
         * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points == 21 && dealer.GetComponent<CardHand>().points == 21)
        {
            TerminarJuego("empate");
        }
        else if (player.GetComponent<CardHand>().points == 21)
        {
            TerminarJuego("blackjack_player");
        }
        else if (dealer.GetComponent<CardHand>().points == 21)
        {
            TerminarJuego("blackjack_dealer");
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
        unseenValues.Add(dealerHand.cards[0].GetComponent<CardModel>().value); // Añadir la carta oculta como una posibilidad dentro de el pozo de cartas posibles, para no excluirla sin querer

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
        // Con la funcion CalculateHypotheticalPoints, básicamente le sumamos todas las cartas restantes y vemos los casos en los que se queda entre 17 y 21 o se pasa de 21
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

        // Sumamos la carta que estamos imaginando que sale
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
            TerminarJuego("perder");
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
        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }

        if (dealer.GetComponent<CardHand>().points > 16)
        {
            int pDealer = dealer.GetComponent<CardHand>().points;
            int pPlayer = player.GetComponent<CardHand>().points;

            if (pDealer > 21)
            {
                TerminarJuego("ganar"); // Ganar

            }
            else if (pDealer < pPlayer)
            {
                TerminarJuego("ganar"); // Ganar
            }
            else if (pDealer == pPlayer) 
            {
                TerminarJuego("empate"); // Empate
            }
            else
            {
                TerminarJuego("perder"); // Perder
            }
        }
    }

    public void PlayAgain()
    {
        finalMessage.text = "";
        player.GetComponent<CardHand>().ClearCards();
        dealer.GetComponent<CardHand>().ClearCards();
        cardIndex = 0;
        ShuffleCards();

        // Reiniciamos fase de apuestas
        enFaseApuestas = true;
        ActualizarTextosApuestas();

        playAgainButton.interactable = false;
        probMessage.text = "";
        puntos_player.text = "0";
    }
}
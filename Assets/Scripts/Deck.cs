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
    public Text finalMessage;
    public Text probMessage;
    [SerializeField]private Text puntos_player;

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
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        int index = Shuffle[cardIndex];
        dealer.GetComponent<CardHand>().Push(faces[index], values[index]);
        cardIndex++;        
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

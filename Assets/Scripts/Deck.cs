using System.Collections.Generic;
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

    public int[] values = new int[52];
    
    int cardIndex = 0;

    public Sprite[] facesBarajadas ;
    System.Random objetoRandom = new System.Random();




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
        
            int[] valoresOrdenados = { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };

        for(int i = 0; i< values.Length; i++)
        {
            values[i] = valoresOrdenados[i];
        }

        
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        //copio la baraja de faces en un clon que despues barajaré
        facesBarajadas = new Sprite[52];

        for(int i = 0;i< 52 ; i++)
        {
            facesBarajadas[i] = faces[i];
        }

        
        //mezclo el clon de faces
        facesBarajadas = mezclarBarajaFaces(facesBarajadas);

       /*
        int valor = getValorSprite(facesBarajadas[0]);
        Debug.Log(valor);
        */
        


    }

    void StartGame()
    {

        //activamos todos menos el de play again
        hitButton.gameObject.SetActive(true);
        stickButton.gameObject.SetActive(true);

        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */

            if(player.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "BlackJack";
                desactivarBotones();
                
            }
            if(dealer.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "BlackJack";
                desactivarBotones();
            }
            

        }

        //Debug.Log("Puntos del jugador" + player.GetComponent<CardHand>().points );
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
        dealer.GetComponent<CardHand>().Push(facesBarajadas[cardIndex], getValorSprite(facesBarajadas[cardIndex]));
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(facesBarajadas[cardIndex], getValorSprite(facesBarajadas[cardIndex])/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
        
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        //Repartimos carta al jugador
        PushPlayer();

        //Debug.Log("Puntos del jugador" + player.GetComponent<CardHand>().points);
        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */

        if(player.GetComponent<CardHand>().points == 21)
        {
            Stand();
        }

        //si se pasa de 21 pierde el jugador
        if (player.GetComponent<CardHand>().points > 21)
        {
            desactivarBotones();
            foreach (GameObject go in dealer.GetComponent<CardHand>().cards)
            {
                go.GetComponent<CardModel>().ToggleFace(true);
            }
            finalMessage.text = "HAS PERDIDO";
        }

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        

        if (dealer.GetComponent<CardHand>().points <= 16)
        {
            //si el dealer tiene menos de 16 pide
            while(dealer.GetComponent<CardHand>().points <= 17)
            {
                //mientras el dealer tenga menos de 17 o 17 pide
                PushDealer();
            }
            
        }
        if(dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points && dealer.GetComponent<CardHand>().points <= 21)
        {
            //si el dealer gana (tiene mas puntos y no se pasa de 21)
            finalMessage.text = "HA GANADO EL DEALER";
            foreach (GameObject go in dealer.GetComponent<CardHand>().cards)
            {
                go.GetComponent<CardModel>().ToggleFace(true);
            }
            desactivarBotones();
        }
        else if(dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
        {
            //si el dealer y el jugador obtienen la misma puntuacion
            finalMessage.text = "EMPATE";
            foreach (GameObject go in dealer.GetComponent<CardHand>().cards)
            {
                go.GetComponent<CardModel>().ToggleFace(true);
            }
            desactivarBotones();
        }
        else
        {
            //si el jugador tiene mas puntos y no se pasa de 21

            if(player.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "HAS GANADO CON BLACKJACK";
            }
            else
            {
                finalMessage.text = "HAS GANADO";
            }
            
            foreach (GameObject go in dealer.GetComponent<CardHand>().cards)
            {
                go.GetComponent<CardModel>().ToggleFace(true);
            }
            desactivarBotones();
        }
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

    //Metodo de mezclar la baraja de sprites clonada

    public Sprite[] mezclarBarajaFaces<Sprite>(Sprite[] array)
    {
        
        //var random = _random;
        for (int i = array.Length; i > 1; i--)
        {
            // Pick random element to swap.
            int j = objetoRandom.Next(i); // 0 <= j <= i-1
                                    // Swap.
            Sprite auxiliar  = array[j];
            array[j] = array[i - 1];
            array[i - 1] = auxiliar;
        }
        return array;
    }

    /*
     *Metodo para obtener el valor de un sprite
     */
    public int getValorSprite(Sprite sprite)
    {
        
        int valor = -1;
        for (int i = 0; i < faces.Length ; i++)
        {
            if (faces[i] == sprite)
            {
                valor = values[i];

               
            }

        }
        return valor;
    }

    //metodo para desactivar los botones restantes cuando se acaba el juego
    public void desactivarBotones()
    {
        //desactivamos todos menos el de play again
        hitButton.gameObject.SetActive(false);
        stickButton.gameObject.SetActive(false);
    }
}

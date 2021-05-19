using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;

    //botones de la interfaz
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Button botonApuesta;

    //mensajes a mostrar
    public Text finalMessage;
    public Text probMessage;
    public Text probMessage2;
    public Text probMessage3;

    //array de valores
    public int[] values = new int[52];
    
    //posicion de las cartas en el juego
    int cardIndex = 0;

    //array clonada de sprites , la cual desordenaremos
    public Sprite[] facesBarajadas ;

    //objeto que nos permite acceder a los metodos random
    System.Random objetoRandom = new System.Random();

    //Sistema de apuestas
    public int banca ;
    public int apuestaActual ;
    public Text miBanca;
    public Text apuestaText;


    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        //mezclamos
        ShuffleCards();
        //indicamos al usuario que tiene que apostar
        tiempoDeApuesta();

        //sistema de apuestas 
        finalMessage.text = "Apuesta una cantidad";
        miBanca.text = "Mi banca: " + banca;
        banca = 1000;
        apuestaActual = 0;
        //sistema de probabilidades
         probMessage.text = "";
        probMessage2.text = "";
        probMessage3.text = "";
        
        //deshabilitamos los botones no necesarios
        desactivarBotones();
        playAgainButton.gameObject.SetActive(false);
        
    }
    private void Update()
    {
        //apostar con las flechas
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            sumarApuesta();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            restarApuesta();
        }
    }
    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        
        //array con los valores ordenados
        int[] valoresOrdenados = { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };

        //rellenamos el array de valores con los valores ordenados
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

    public void StartGame()
    {
        
        //activamos todos menos el de play again
        hitButton.gameObject.SetActive(true);
        stickButton.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(false);

        //repartimos cartas al dealer
        for(int i = 0; i < 2; i++)
        {
            PushDealer();
        }

        //repartimos cartas al jugador
        for (int i = 0; i < 2; i++)
        {
            
           
            PushPlayer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */

            if (player.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "Has Ganado con BlackJack";
                
                desactivarBotones();
                banca = banca + (apuestaActual * 2);
                miBanca.text = "Mi banca: " + banca.ToString();
                playAgainButton.gameObject.SetActive(true);



            }
            if(dealer.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "BlackJack del Dealer";
                miBanca.text = "Mi banca: " + banca.ToString();
                playAgainButton.gameObject.SetActive(true);
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

        //accedemos a los valores de la mano del jugador y de la carta descubierta del dealer
        int valorCartaDestapada = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        int valorManoJugador = player.GetComponent<CardHand>().points;

        //Averiguamos los ases que quedan en la baraja
        int asesRestantes = 0;
        for(int i = cardIndex; i < facesBarajadas.Length; i++)
        {
            if(getValorSprite(facesBarajadas[i]) == 11)
            {
                asesRestantes++;
            }
        }

        //PRIMERA PROBABILIDAD

        decimal casosProbablesManoDealerSuperior = 0;
        decimal primeraProbabilidad;

        //SEGUNDA PROBABILIDAD

        decimal casosFavorablesHitJugador = 0;
        decimal segundaProbabilidad;

        //TERCERA PROBABILIDAD
        decimal casosSePasaDeVeintiuno = 0;
        decimal terceraProbabilidad;

        //bucle que recorre las cartas restantes de la baraja que no estan en juego
        for (int i = cardIndex; i < facesBarajadas.Length; i++)
        {
            int valorCarta = getValorSprite(facesBarajadas[i]);
            //condicion primera probabilidad
            if(valorCarta + valorCartaDestapada > valorManoJugador && valorCarta+valorCartaDestapada <= 21)
            {
                casosProbablesManoDealerSuperior++;
            }
            //condicion segunda probabilidad
            if(valorCarta + valorManoJugador >= 17 && valorCarta + valorManoJugador <= 21)
            {
                
                casosFavorablesHitJugador++;
            }
            //condicion tercera probabilidad
            if(valorCarta + valorManoJugador > 21)
            {
                casosSePasaDeVeintiuno++;
            }

        }
        //calculamos las probabilidades 
        decimal cartasEnJuego = 52 - cardIndex;
        primeraProbabilidad = decimal.Round((casosProbablesManoDealerSuperior / cartasEnJuego)*100,2);
        segundaProbabilidad = decimal.Round((casosFavorablesHitJugador / cartasEnJuego) * 100, 2);
        terceraProbabilidad = decimal.Round((casosSePasaDeVeintiuno / cartasEnJuego) * 100, 2);


        //editamos los textos de las probabilidades
        if(primeraProbabilidad == 0)
        {
            if(asesRestantes > 0)
            {
                probMessage.text = "Probabilidad de mejor mano del dealer: " + asesRestantes*(1/52) + "%";
            }
        }
        else
        {
            if (primeraProbabilidad > 100)
            {
                probMessage.text = "Probabilidad de mejor mano del dealer: " + 100 + "%";
            }
            else
            {
                probMessage.text = "Probabilidad de mejor mano del dealer: " + primeraProbabilidad.ToString() + "%";
            }
        }
        
        if(segundaProbabilidad == 0)
        {
            if (asesRestantes > 0)
            {
                probMessage2.text = "Probabilidad de obtener entre un 17 y un 21 si pides una carta: " + asesRestantes * (1 / 52) + "%";
            }
        }
        else
        {
            probMessage2.text = "Probabilidad de obtener entre un 17 y un 21 si pides una carta: " + segundaProbabilidad.ToString() + "%";
        }
       

        

        if (terceraProbabilidad > 100)
        {
            probMessage3.text = "Probabilidad de pasarte de 21 si pides otra carta " + 100 + "%";
        }
        else
        {
            probMessage3.text = "Probabilidad de pasarte de 21 si pides otra carta: " + terceraProbabilidad.ToString() + "%";
        }






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
        if(cardIndex > 3)
        {
            CalculateProbabilities();
        }
       
        
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
            playAgainButton.gameObject.SetActive(true);
            apuestaActual = 0;
            
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
            miBanca.text = "Mi banca: " + banca.ToString();
            apuestaActual = 0;
            playAgainButton.gameObject.SetActive(true);
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
            banca = banca + apuestaActual;
            miBanca.text = "Mi banca: " + banca.ToString();
            playAgainButton.gameObject.SetActive(true);
            apuestaActual = 0;
        }
        else
        {
            //si el jugador tiene mas puntos y no se pasa de 21

            if(player.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "HAS GANADO CON BLACKJACK";
                banca = banca + (apuestaActual*2);
                miBanca.text = "Mi banca: " + banca.ToString();
                playAgainButton.gameObject.SetActive(true);
                apuestaActual = 0;
            }
            else
            {
                finalMessage.text = "HAS GANADO";
                banca = banca + (apuestaActual * 2);
                miBanca.text = "Mi banca: " + banca.ToString();
                playAgainButton.gameObject.SetActive(true);
                apuestaActual = 0;
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
        tiempoDeApuesta();
    }


    /**
     * 
     * 
     * METODOS AÑADIDOS
     * 
     */

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

    //METODOS PARA AUMENTAR O DISMINUUIR LA APUESTA
    public void sumarApuesta()
    {
        if(banca >= 10)
        {
            apuestaActual = apuestaActual + 10;
            banca = banca - 10;

            apuestaText.text = apuestaActual.ToString();
            miBanca.text = "Mi banca: " + banca;
            finalMessage.text = "";
        }


    }
    public void restarApuesta()
    {
        if (apuestaActual >= 10)
        {
            apuestaActual = apuestaActual - 10;
            banca = banca + 10;

            apuestaText.text = apuestaActual.ToString();
            miBanca.text = "Mi banca: " + banca;
            finalMessage.text = "";
        }
        
    }

    //ON CLICK DEL BOTON APUESTA
    public void onClickBotonApuesta()
    {
        if (apuestaActual >= 10 )
        {
            ShuffleCards();
            StartGame();
            apuestaText.text = "0";
            //finalMessage.text = "";
            botonApuesta.interactable = false;
        }
        
    }
    //ESTE METODO CONTROLA EL ESTADO DE LOS BOTONES CUANDO EL JUGADOR ESTA APOSTANDO
    public void tiempoDeApuesta()
    {
        hitButton.gameObject.SetActive(false);
        stickButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        botonApuesta.interactable = true;

    }
}

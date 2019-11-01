using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tipoTotem
{
    explotacion,
    gobierno,
    medios
}
public enum modoTotemExplotacion
{
    saqueador, //rojo
    reinversor, //azul
    competencia, //amarillo
    clasista //verde
}
public enum modoTotemGobierno
{
    proglobalizacion, //rojo
    nacionalista, //azul
    esclavista, //amarillo
    concesivo //verde
}
public enum modoTotemMedios
{
    autoritario, //rojo
    entretenimiento, //azul
    publicidad, //amarillo
    institucional //verde
}

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animation))]
public class totem : MonoBehaviour
{
    public tipoTotem tipoEsteTotem;
    public modoTotemExplotacion modoExplotacion;
    public modoTotemGobierno modoGobierno;
    public modoTotemMedios modoMedios;

    public GameObject puertaArriba, puertaAbajo, puertaDerecha, puertaIzquierda, ondas;

    SpriteRenderer im;
    AudioSource son;
    Animation ani;


    [SerializeField]
    AudioClip[] sonidos;
    AudioClip audio1, audio2, audio3, audio4;

    //Cuando entra en la zona de proyeccion y es captado transmite osea que "funciona", incide en la realidad
    bool transmitiendo = false;

    //Llevamos la cuenta de cuantas personas (y opcionalmente bichos) estan en contacto con una determinad apuerta para
    //Saber en que modo tendria que estar el totem
    [SerializeField]
    int puertaRoja, puertaAzul, puertaAmarilla, puertaVerde;

    //El porcentaje de carga que lleva cada color, de 0 a 100
    //Deberian vaciarse todos menos el color ganador en un cambio de color
    [SerializeField]
    int barritaRoja, barritaAzul, barritaAmarilla, barritaVerde;
    
    // Start is called before the first frame update
    void Start()
    {
        im = GetComponent<SpriteRenderer>();
        son = GetComponent<AudioSource>();
        ani = GetComponent<Animation>();

        audio1 = sonidos[0];
        audio2 = sonidos[1];
        audio3 = sonidos[2];
        audio4 = sonidos[3];
        }

    // Update is called once per frame
    void Update()
    {
        ManejarPuertas();
    }
    

    //Llamados desde ManejarPuertas() cuando una puerta hegemonica llena la barrita. Son tres sobrecargas segun el tipo de totem.
    void CambiarModo(modoTotemExplotacion nuevoModo)
    {
        modoExplotacion = nuevoModo;
        CambiarColorTotem();
    }
    void CambiarModo(modoTotemGobierno nuevoModo)
    {
        modoGobierno = nuevoModo;
        CambiarColorTotem();
    }
    void CambiarModo(modoTotemMedios nuevoModo)
    {
        modoMedios = nuevoModo;
        CambiarColorTotem();
    }

    //Llamado desde CambiarModo(), nunca se ejecuta directamente
    void CambiarColorTotem()
    {
        if(tipoEsteTotem == tipoTotem.explotacion)
        {
            switch (modoExplotacion)
            {
                case modoTotemExplotacion.saqueador:
                    CambiarColor("rojo");
                    CambiarOndas("rojas");
                    CambiarSonidos(audio1);
                    break;
                case modoTotemExplotacion.reinversor:
                    CambiarColor("azul");
                    CambiarOndas("azules");
                    CambiarSonidos(audio2);
                    break;
                case modoTotemExplotacion.competencia:
                    CambiarColor("amarillo");
                    CambiarOndas("amarillas");
                    CambiarSonidos(audio3);
                    break;
                case modoTotemExplotacion.clasista:
                    CambiarColor("verde");
                    CambiarOndas("verdes");
                    CambiarSonidos(audio4);
                    break;
                default:
                    break;
            }
        } else if(tipoEsteTotem == tipoTotem.gobierno)
        {
            switch (modoGobierno)
            {
                case modoTotemGobierno.proglobalizacion:
                    CambiarColor("rojo");
                    CambiarOndas("rojas");
                    CambiarSonidos(audio1);
                    break;
                case modoTotemGobierno.nacionalista:
                    CambiarColor("azul");
                    CambiarOndas("azules");
                    CambiarSonidos(audio2);
                    break;
                case modoTotemGobierno.esclavista:
                    CambiarColor("amarillo");
                    CambiarOndas("amarillas");
                    CambiarSonidos(audio3);
                    break;
                case modoTotemGobierno.concesivo:
                    CambiarColor("verde");
                    CambiarOndas("verdes");
                    CambiarSonidos(audio4);
                    break;
                default:
                    break;
            }
        }
        else if(tipoEsteTotem == tipoTotem.medios)
        {
            switch (modoMedios)
            {
                case modoTotemMedios.autoritario:
                    CambiarColor("rojo");
                    CambiarOndas("rojas");
                    CambiarSonidos(audio1);
                    break;
                case modoTotemMedios.entretenimiento:
                    CambiarColor("azul");
                    CambiarOndas("azules");
                    CambiarSonidos(audio2);
                    break;
                case modoTotemMedios.publicidad:
                    CambiarColor("amarillo");
                    CambiarOndas("amarillas");
                    CambiarSonidos(audio3);
                    break;
                case modoTotemMedios.institucional:
                    CambiarColor("verde");
                    CambiarOndas("verdes");
                    CambiarSonidos(audio4);
                    break;
                default:
                    break;
            }
        }
    }

    //Llamado desde CambiarColorTotem, nunca se ejecuta directamente
    void CambiarColor(string color)
    {
        if(color == "rojo")
        {
            im.color = new Color(255, 0, 0);
            barritaAmarilla = 0;
            barritaAzul = 0;
            barritaVerde = 0;
        } else if(color == "amarillo")
        {
            im.color = new Color(255, 255, 0);
            barritaAzul = 0;
            barritaRoja = 0;
            barritaVerde = 0;
        } else if (color == "verde")
        {
            im.color = new Color(0, 255, 0);
            barritaAmarilla = 0;
            barritaAzul = 0;
            barritaRoja = 0;
        } else if (color == "azul")
        {
            im.color = new Color(0, 0, 255);
            barritaAmarilla = 0;
            barritaRoja = 0;
            barritaVerde = 0;
        }
    }

    void EmpezarATransmitir()
    {
        transmitiendo = true;
        //Prender audio
        //Cambiar modo si es necesario
        //Emitir señales visuales de transmision
    }
    void TerminarDeTransmitir()
    {
        transmitiendo = false;
        //Apagar audio
        //Cambiar modo si es necesario
        //Dejar de emitir señales visuales de transmision
    }


    //Cuando una persona (o eventualmente un bicho) le da a una puerta, lo agregamos para que le de mas peso a esa puerta
    public void EntrarColision(tipoPuerta puerta)
    {
        switch (puerta)
        {
            case tipoPuerta.roja:
                puertaRoja++;
                break;
            case tipoPuerta.azul:
                puertaAzul++;
                break;
            case tipoPuerta.amarilla:
                puertaAmarilla++;
                break;
            case tipoPuerta.verde:
                puertaVerde++;
                break;
            default:
                break;
        }
    }

    //Cuando una persona (o eventualmente un bicho) se va de una puerta, lo restamos de esa puerta
    public void SalirColision(tipoPuerta puerta)
    {
        switch (puerta)
        {
            case tipoPuerta.roja:
                puertaRoja--;
                break;
            case tipoPuerta.azul:
                puertaAzul--;
                break;
            case tipoPuerta.amarilla:
                puertaAmarilla--;
                break;
            case tipoPuerta.verde:
                puertaVerde--;
                break;
            default:
                break;
        }
    }

    //Esta funcion se encarga de analizar cuanta gent ey eventualmente bichos estan en contacto con cada puerta y hacer subir la
    //Barrita correspondiente. En caso de que corresponda, tambien llama a CambiarModo, que despues llama a CambiarColorTotem
    void ManejarPuertas()
    {
        if (puertaRoja > puertaAmarilla && puertaRoja > puertaAzul && puertaRoja > puertaVerde)
        {
            if (barritaRoja < 100)
            {
                barritaRoja += puertaRoja;
            }
            else if (barritaRoja > 100)
            {
                barritaRoja = 100;
            }
            else if (barritaRoja == 100)
            {
                if (tipoEsteTotem == tipoTotem.explotacion)
                {
                    CambiarModo(modoTotemExplotacion.saqueador);
                }
                else if (tipoEsteTotem == tipoTotem.gobierno)
                {
                    CambiarModo(modoTotemGobierno.proglobalizacion);
                }
                else if (tipoEsteTotem == tipoTotem.medios)
                {
                    CambiarModo(modoTotemMedios.autoritario);
                }
            }
        }
        if (puertaAzul > puertaAmarilla && puertaAzul > puertaRoja && puertaAzul > puertaVerde)
        {
            if (barritaAzul < 100)
            {
                barritaAzul += puertaAzul;
            }
            else if (barritaAzul > 100)
            {
                barritaAzul = 100;
            }
            else if (barritaAzul == 100)
            {
                if (tipoEsteTotem == tipoTotem.explotacion)
                {
                    CambiarModo(modoTotemExplotacion.reinversor);
                }
                else if (tipoEsteTotem == tipoTotem.gobierno)
                {
                    CambiarModo(modoTotemGobierno.nacionalista);
                }
                else if (tipoEsteTotem == tipoTotem.medios)
                {
                    CambiarModo(modoTotemMedios.entretenimiento);
                }
            }
        }
        if (puertaAmarilla > puertaRoja && puertaAmarilla > puertaAzul && puertaAmarilla > puertaVerde)
        {
            if (barritaAmarilla < 100)
            {
                barritaAmarilla += puertaAmarilla;
            }
            else if (barritaAmarilla > 100)
            {
                barritaAmarilla = 100;
            }
            else if (barritaAmarilla == 100)
            {
                if (tipoEsteTotem == tipoTotem.explotacion)
                {
                    CambiarModo(modoTotemExplotacion.competencia);
                }
                else if (tipoEsteTotem == tipoTotem.gobierno)
                {
                    CambiarModo(modoTotemGobierno.esclavista);
                }
                else if (tipoEsteTotem == tipoTotem.medios)
                {
                    CambiarModo(modoTotemMedios.publicidad);
                }
            }
        }
        if (puertaVerde > puertaRoja && puertaVerde > puertaAzul && puertaVerde > puertaAmarilla)
        {
            if (barritaVerde < 100)
            {
                barritaVerde += puertaVerde;
            }
            else if (barritaVerde > 100)
            {
                barritaVerde = 100;
            }
            else if (barritaVerde == 100)
            {
                if (tipoEsteTotem == tipoTotem.explotacion)
                {
                    CambiarModo(modoTotemExplotacion.clasista);
                }
                else if (tipoEsteTotem == tipoTotem.gobierno)
                {
                    CambiarModo(modoTotemGobierno.concesivo);
                }
                else if (tipoEsteTotem == tipoTotem.medios)
                {
                    CambiarModo(modoTotemMedios.institucional);
                }
            }
        }

    }

    //Aca solo cambiamos de color las ondas, el play o pause debiera manejarlo el apagar o prender transmision
    void CambiarOndas(string color)
    {
        switch (color)
        {
            case "rojas":
                ondas.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0); 
                break;
            case "azules":
                ondas.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
                break;
            case "verdes":
                ondas.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
                break;
            case "amarillas":
                ondas.GetComponent<SpriteRenderer>().color = new Color(255, 255, 0);
                break;
            default:
                Debug.LogWarning("LLamando a empezar ondas sin color");
                break;
        }
    }

    //Aca solo cambiamos el clip de sonido del reproductor, el play o pause debiera manejarlo el apagar o prender transmision
    void CambiarSonidos(AudioClip sonido)
    {
        son.clip = sonido;
    }
}

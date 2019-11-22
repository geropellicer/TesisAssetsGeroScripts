using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TIPOTOTEM
{
    /// <summary> COLOR: AZUL </summary>
    PUBLICOESTATAL,
    /// <summary> COLOR: VERDE </summary>
    PUBLICOMILITAR,
    /// <summary> COLOR: AMARILLO </summary>
    PRIVADOCOMERCIAL,
    /// <summary> COLOR: MAGENTA </summary>
    PRIVADOENTRETENIMIENTO
}


[RequireComponent(typeof(Animation))]
public class totem : MonoBehaviour
{
    globalVariables gV;

    /// <summary> El tipo de totem que es este. En la escena debería haber uno de cada uno. 
    /// Se debe asignar desde la lectura por webcam del patron (predefinir que patron es que tipo) </summary>
    [SerializeField]
    private TIPOTOTEM tipoEsteTotem;
    /// <summary> Devovlemos publicamente para agentes externons cual de los 4 tipo de totems es este totem. </summary>
    public TIPOTOTEM ObtenerTipoTotem()
    {
        return tipoEsteTotem;
    }

    /// <summary> Gráfico de ondas para dar una idea de sobre que forma está incidiendo.
    /// TODO: ver si no se puede aplicar el GLITCH HOLOGRAMA </summary>
    public GameObject ondas;

    /// <summary> Referencia al SpriteRederer component </summary>
    SpriteRenderer im;

    /// <summary> Referencia al animation component</summary>    
    Animation ani;

    /// <summary>Si ah sido detectado por las camaras y está siendo trackeado en el espacio.</summary>    
    [SerializeField]
    bool trackeado = false;


    /// <summary>Cuando entra en la zona de proyeccion y es captado y está prendido
    /// transmite osea que "funciona", incide en la realidad</summary>    
    [SerializeField]
    bool transmitiendo = false;
    /// <summary> Obtenemos si está transmitiendo o no paraagentes externos Implica que está reconocido por el
    /// sistema de trackeo y que está prendido desde el boton por el usuario o por los bichos. </summary>
    public bool ObtenerEstaTransmitiendo()
    {
        return transmitiendo;
    }

    float minXin = 0;
    float maxXin = 1;
    float minYin = 0;
    float maxYin = 1;

    public enum WEBCAM
    {
        WEBCAM1,
        WEBCAM2
    }
    public WEBCAM webcamTrackeando;

    int valorPrendido = 0;
    public int ObtenerValorPrendido()
    {
        return valorPrendido;
    }
  
    // Start is called before the first frame update
    void Start()
    {

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();

        im = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animation>();

        ondas = transform.GetChild(0).gameObject;

        transmitiendo = false;
        trackeado = false;
        ondas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    /// <summary> Solo si esta en el espacio y detectado, con este metodo prendemos el totem
    /// para que comience a transmitir (incidir en la realidad) </summary>
    void EmpezarATransmitir()
    {
        if(trackeado)
        {
            transmitiendo = true;
            ondas.SetActive(true);
        }
    }

    /// <summary> Apagamos el totem sea porque salio del trackeo o porque un usuario o los bichos lo apagaron. </summary>
    void TerminarDeTransmitir()
    {
        transmitiendo = false;
        ondas.SetActive(false);
    }

    /// <summary> Cuando el OSC detecta la imagen de este totem lo ponemos como trackeado. 
    /// Esta funcion se llama desde el OSCAntenasController</summary>
    public void SetTrackeando(int cam)
    {
        trackeado = true;
        im.enabled = true;
        if(cam == 1)
        {
            webcamTrackeando = WEBCAM.WEBCAM1;
        } else if(cam == 2)
        {
            webcamTrackeando = WEBCAM.WEBCAM2;
        } else {
            Debug.LogWarning("ERROR: se paso el número de webcam ' " + cam + " ' cuando solo deberia ser 1 ó 2");
        }
        
        if(transmitiendo)
        {
            EmpezarATransmitir();
        }
    }

    /// <summary> Cuando lo perdemos por OSC lo destrackeamos y lo apagamos.
     /// Esta funcion se llama desde el OSCAntenasController</summary>
    public void UnsetTrackeando(int cam) 
    {
        trackeado = false;
        im.enabled = false;

        ondas.SetActive(false);
    }

    public void ActualizarTotemOsc(float x, float y, float rotZ)
    {
        if(webcamTrackeando == WEBCAM.WEBCAM1)
        {
            transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, gV.minX, gV.maxX), Utilidades.Map(y, minYin, maxYin, gV.minY1, gV.maxY1), -0.1f);
        } else if(webcamTrackeando == WEBCAM.WEBCAM2)
        {
            transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, gV.minX, gV.maxX), Utilidades.Map(y, minYin, maxYin, gV.minY2, gV.maxY2), -0.1f);
        }
        transform.rotation = Quaternion.Euler(0,0, Utilidades.Map(rotZ, 0, 6, 0, 360));

    }

    public void EncenderTotem()
    {
        EmpezarATransmitir();
    }
    public void ApagarTotem()
    {
        TerminarDeTransmitir();
    }


    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        // TODO: en el follower debe
        // Si es un sujeto que tiene intencion de apagar una antena y tiene seleccionada justo esta antena
        if(other.tag == "sujeto" && other.GetComponent<Follower>().IntencionDeApagarAntena() &&
            GameObject.ReferenceEquals(other.GetComponent<Follower>().TotemSeleccionado(), gameObject))
        {
            if(transmitiendo)
            {
                if(valorPrendido > 0)
                {
                    valorPrendido--;
                } else {
                    ApagarTotemDesdeUnity();
                }
            } 
        } else if(other.tag == "sujeto" && other.GetComponent<Follower>().IntencionDePrenderAntena() &&
            GameObject.ReferenceEquals(other.GetComponent<Follower>().TotemSeleccionado(), gameObject))
        {
            if(!transmitiendo)
            {
                if(valorPrendido < 100)
                {
                    valorPrendido++;
                } else {
                    PrenderTotemDesdeUnity();
                }
            } 
        }
    }

    void ApagarTotemDesdeUnity()
    {
        int numAntena = 0;
        if(tipoEsteTotem == TIPOTOTEM.PUBLICOMILITAR)
        {
            numAntena = 1;
        } else if(tipoEsteTotem == TIPOTOTEM.PRIVADOCOMERCIAL)
        {
            numAntena = 2;
        } else if(tipoEsteTotem == TIPOTOTEM.PUBLICOESTATAL)
        {
            numAntena = 3;
        } else if(tipoEsteTotem == TIPOTOTEM.PRIVADOENTRETENIMIENTO)
        {
            numAntena = 4;
        }

        gV.oscAntenasController.EnviarApagarAntenaDesdeUnity(numAntena);
    }

    void PrenderTotemDesdeUnity()
    {
        int numAntena = 0;
        if(tipoEsteTotem == TIPOTOTEM.PUBLICOMILITAR)
        {
            numAntena = 1;
        } else if(tipoEsteTotem == TIPOTOTEM.PRIVADOCOMERCIAL)
        {
            numAntena = 2;
        } else if(tipoEsteTotem == TIPOTOTEM.PUBLICOESTATAL)
        {
            numAntena = 3;
        } else if(tipoEsteTotem == TIPOTOTEM.PRIVADOENTRETENIMIENTO)
        {
            numAntena = 4;
        }

        gV.oscAntenasController.EnviarPrenderAntenaDesdeUnity(numAntena);
    }
}

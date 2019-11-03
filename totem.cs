﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TIPOTOTEM
{
    PUBLICOESTATAL,
    PUBLICOMILITAR,
    PRIVADOCOMERCIAL,
    PRIVADOENTRETENIMIENTO
}

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animation))]
public class totem : MonoBehaviour
{
    OSC osc;
    globalVariables gV;

    /// <summary> La IP del arudino con el que se va a comunicar por OSC
    /// Se debe asignar desde la lectura por webcam del patron (predefinir que patron es que tipo) </summary>
    public string ipArduino;

    /// <summary> El tipo de totem que es este. En la escena debería haber uno de cada uno. 
    /// Se debe asignar desde la lectura por webcam del patron (predefinir que patron es que tipo) </summary>
    public TIPOTOTEM tipoEsteTotem;

    /// <summary> Gráfico de ondas para dar una idea de sobre que forma está incidiendo.
    /// TODO: ver si no se puede aplicar el waveform del audio como distorsión </summary>
    public GameObject ondas;

    /// <summary> Referencia al SpriteRederer component </summary>
    SpriteRenderer im;
    
    /// <summary> Referencia a la fuente de sonido.!-- TOOD: deberiamos cambiar por el envio OSC al totem </summary>
    AudioSource son;

    /// <summary> Referencia al animation component</summary>    
    Animation ani;

    /// <summary>Si ah sido detectado por las camaras y está siendo trackeado en el espacio.</summary>    
    [SerializeField]
    bool trackeado = false;


    /// <summary>Cuando entra en la zona de proyeccion y es captado y está prendido
    /// transmite osea que "funciona", incide en la realidad</summary>    
    [SerializeField]
    bool transmitiendo = false;

    float minXin = 0;
    float maxXin = 1;
    float minYin = 0;
    float maxYin = 1;

    float minX, maxX, minY1, maxY1, minY2, maxY2;

    public enum WEBCAM
    {
        WEBCAM1,
        WEBCAM2
    }
    public WEBCAM webcamTrackeando;
  
    // Start is called before the first frame update
    void Start()
    {
        osc = GameObject.Find("OSC Manger").GetComponent<OSC>();

        // Según que totem sea nos suscribimos al Tag OSC correspondiente
        if(tipoEsteTotem == TIPOTOTEM.PRIVADOCOMERCIAL)
        {
            osc.SetAddressHandler("/Totem1/Entered/", EntroTotem);
            osc.SetAddressHandler("/Totem1/Leave/", SalioTotem);
            osc.SetAddressHandler("/Totem1/Update/", ActualizarTotemOsc);
            osc.SetAddressHandler("/Totem1/Encender/", EncenderTotem);
            osc.SetAddressHandler("/Totem1/Apagar/", ApagarTotem);
        } else if(tipoEsteTotem == TIPOTOTEM.PRIVADOENTRETENIMIENTO)
        {
            osc.SetAddressHandler("/Totem2/Entered/", EntroTotem);
            osc.SetAddressHandler("/Totem2/Leave/", SalioTotem);
            osc.SetAddressHandler("/Totem2/Update/", ActualizarTotemOsc);
            osc.SetAddressHandler("/Totem2/Encender/", EncenderTotem);
            osc.SetAddressHandler("/Totem2/Apagar/", ApagarTotem);
        } else if(tipoEsteTotem == TIPOTOTEM.PUBLICOESTATAL)
        {
            osc.SetAddressHandler("/Totem3/Entered/", EntroTotem);
            osc.SetAddressHandler("/Totem3/Leave/", SalioTotem);
            osc.SetAddressHandler("/Totem3/Update/", ActualizarTotemOsc);
            osc.SetAddressHandler("/Totem3/Encender/", EncenderTotem);
            osc.SetAddressHandler("/Totem3/Apagar/", ApagarTotem);
        } else if(tipoEsteTotem == TIPOTOTEM.PUBLICOMILITAR)
        {
            osc.SetAddressHandler("/Totem4/Entered/", EntroTotem);
            osc.SetAddressHandler("/Totem4/Leave/", SalioTotem);
            osc.SetAddressHandler("/Totem4/Update/", ActualizarTotemOsc);
            osc.SetAddressHandler("/Totem4/Encender/", EncenderTotem);
            osc.SetAddressHandler("/Totem4/Apagar/", ApagarTotem);
        }

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();

        im = GetComponent<SpriteRenderer>();
        son = GetComponent<AudioSource>();
        ani = GetComponent<Animation>();

        ondas = transform.GetChild(0).gameObject;

        ActualizarCalibrado();
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
            im.enabled = true;
            //Prender audio
            //Cambiar modo si es necesario
            //Emitir señales visuales de transmision
        }
    }

    /// <summary> Apagamos el totem sea porque salio del trackeo o porque un usuario o los bichos lo apagaron. </summary>
    void TerminarDeTransmitir()
    {
        transmitiendo = false;
        im.enabled = false;
        //Apagar audio
        //Cambiar modo si es necesario
        //Dejar de emitir señales visuales de transmision
    }

    /// <summary> Cuando el OSC detecta la imagen de este totem lo ponemos como trackeado </summary>
    void SetTrackeando()
    {
        trackeado = true;
    }

    /// <summary> Cuando lo perdemos por OSC lo destrackeamos y lo apagamos</summary>
    void UnsetTrackeando() 
    {
        trackeado = false;
        transmitiendo = false;
    }


    void EntroTotem(OscMessage m)
    {
        SetTrackeando();
        float numWebcam = m.GetInt(3);
        if(numWebcam == 1)
        {
            webcamTrackeando = WEBCAM.WEBCAM1;
        } else if(numWebcam == 2)
        {
            webcamTrackeando = WEBCAM.WEBCAM2;
        } else {
            Debug.LogError("ERROR: El numero de kinect pasado por OSC no es invalido. Deberia ser 1 o 2. Se recibio: '" + numWebcam + "'");
        }

    }

    void SalioTotem(OscMessage m)
    {
        UnsetTrackeando();
    }

    void ActualizarTotemOsc(OscMessage m)
    {
        float x = m.GetFloat(0);
        float y = m.GetFloat(1);
        if(webcamTrackeando == WEBCAM.WEBCAM1)
        {
            Debug.Log("X: " + x + " - " + minXin + " - " + maxXin + " - " + minX + " - " + maxX);
            Debug.Log("Y: " + y + " - " + minYin + " - " + maxYin + " - " + minY1 + " - " + maxY1);
            transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, minX, maxX), Utilidades.Map(y, minYin, maxYin, minY1, maxY1), -0.1f);
        } else if(webcamTrackeando == WEBCAM.WEBCAM2)
        {
            transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, minX, maxX), Utilidades.Map(y, minYin, maxYin, minY2, maxY2), -0.1f);
        }
    }

    void EncenderTotem(OscMessage m)
    {
        EmpezarATransmitir();
        // Encender la luz del boton
        // TODO: Forward port and IP
        OscMessage message = new OscMessage();

        message.address = "/EncenderTotem/Luz";
        message.values.Add(1);
        osc.Send(message);
        
        // Apagar el parlante
        OscMessage message2 = new OscMessage();

        message2.address = "/EncenderTotem/Sonido";
        message2.values.Add(1);
        osc.Send(message2);
    }
    void ApagarTotem(OscMessage m)
    {
        TerminarDeTransmitir();
        // Apagar la luz del boton
        // TODO: Forward port and IP
        OscMessage message = new OscMessage();

        message.address = "/ApagarTotem/Luz";
        message.values.Add(0);
        osc.Send(message);
        
        // Apagar el parlante
        OscMessage message2 = new OscMessage();

        message2.address = "/ApagarTotem/Sonido";
        message2.values.Add(0);
        osc.Send(message2);
    }
    /// <summary> Lo corremos al incio para obtener los bordes del mapa según cada cámara. </summary>
    public void ActualizarCalibrado()
    {
        minX = gV.minX;
        maxX = gV.maxX;
        minY1 = gV.minY1;
        maxY1 = gV.maxY1;
        minY2 = gV.minY2;
        maxY2 = gV.maxY2;
        Debug.Log("TOTEM " + gameObject.name + " actualizo su calibracion." + "minX: " + minX + " - maxX: " + maxX);
    }

}

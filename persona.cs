using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persona : MonoBehaviour
{
    /// <summary>Referencia a las variables blobales de la escena</summary>
    globalVariables gV;

    /// <summary> CUantos frames despues de que la kinect lo pierda vamos a esperar antes de eliminarlo </summary>
    [SerializeField]
    int margenSupervivencia;

    /// <summary> Cuantos frames llevamos esperados desde que lo perdimos con la kinect </summary>
    [SerializeField]
    int framesSupervivenciaTranscurridos;

    /// <summary>Para no ejecutar el update todo el tiempo, esta variable determina la proxima vez que se debe ejecutar 
    /// basado en el intervalo definido</summary>
    float nextActionTime;

    /// <summary>Cada cuanto se ejecutará el update</summary>
    float intervalo;

    /// <summary>Para seleccionar que kinect la spwneó y cual la está siguiendo</summary>
    public enum kin
    {
        kin1,
        kin2
    }

    /// <summary>Referencia a la kinect que spawneó esta persona</summary>
    public kin kinectAsignada;

    public OSC osc;
    [SerializeField]
    int id;

    [SerializeField]
    bool funcionando;

    public bool EstaFuncionando()
    {
        return funcionando;
    }


    //Para personas
    float minXin = 0;
    float maxXin = 1;
    float minYin = 0;
    float maxYin = 1;

    private void Start()
    {
        nextActionTime = 0f;
        intervalo = 2.5f;

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!funcionando)
        {
            framesSupervivenciaTranscurridos++;
            if (framesSupervivenciaTranscurridos >= margenSupervivencia)
            {
                if (kinectAsignada == kin.kin1)
                {
                    EliminarPersona();
                }
                else
                {
                    EliminarPersona2();
                }
            }
        }
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + intervalo;

        }
    }


    public void Prender()
    {
        funcionando = true;
    }

    void Apagar()
    {
        funcionando = false;
    }

    public void RecibirDatosPersonaKin1(OscMessage m)
    {
        if (m.GetFloat(0) == id)
        {
            float x = m.GetFloat(1);
            float y = m.GetFloat(2);
            transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, gV.minX, gV.maxX), Utilidades.Map(y, minYin, maxYin, gV.minY1, gV.maxY1), -0.1f);
        }
    }


    public void RecibirDatosPersonaKin2(OscMessage m)
    {
        if (m.GetFloat(0) == id)
        {
            float x = m.GetFloat(1);
            float y = m.GetFloat(2);
            transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, gV.minX, gV.maxX), Utilidades.Map(y, minYin, maxYin, gV.minY2, gV.maxY2), -0.1f);
        }
    }

    public void Configurar(OSC o, int id, int kinect)
    {
        osc = o;
        this.id = id;
        if (kinect == 1)
        {
            kinectAsignada = kin.kin1;
        }
        else if (kinect == 2)
        {
            kinectAsignada = kin.kin2;
        }
        else
        {
            Debug.Log("Se configuró una persona incorrectamente desde OscPersonasController. Numero de kinect mal especificado");
        }

        if (kinectAsignada == kin.kin1)
        {
            osc.SetAddressHandler("/KIN1/Updated/", RecibirDatosPersonaKin1);
            osc.SetAddressHandler("/KIN1/Leave/", Desconectar);
        }
        if (kinectAsignada == kin.kin2)
        {
            osc.SetAddressHandler("/KIN2/Updated/", RecibirDatosPersonaKin2);
            osc.SetAddressHandler("/KIN2/Leave/", Desconectar);
        }

        gV.AgregarPersona(gameObject);
    }

    void EliminarPersona()
    {
            //Destroy(GetComponent<SpriteRenderer>());
            gV.EliminarPersona(gameObject);
            Destroy(gameObject);
    }
    void EliminarPersona2()
    {
            //Destroy(GetComponent<SpriteRenderer>());
            gV.EliminarPersona(gameObject);
            Destroy(gameObject);
    }

    public void PersonaReencontrada(OSC o, int id, int kinect)
    {
        framesSupervivenciaTranscurridos = 0;
        Configurar(o, id, kinect);
        Prender();
    }

    public void Desconectar(OscMessage m)
    {
        if (m.GetInt(0) == id)
        {
            Apagar();
        }
    }

}

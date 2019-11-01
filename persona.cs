using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persona : MonoBehaviour
{
    /// <summary>Referencia a las variables blobales de la escena</summary>
    globalVariables gV;

    /// <summary>Para no ejecutar el update todo el tiempo, esta variable determina la proxima vez que se debe ejecutar 
    /// basado en el intervalo definido</summary>
    float nextActionTime;

    /// <summary>Cada cuanto se ejecutará el update</summary>
    float intervalo;

    /// <summary>Referencia al audio</summary>
    AudioSource audio;

    /// <summary>Para seleccionar que kinect la spwneó y cual la está siguiendo</summary>
    public enum kin
    {
        kin1,
        kin2
    }

    /// <summary>Referencia a la kinect que spawneó esta persona</summary>
    public kin kinectAsignada;

    /// <summary>(VIEJO) Sujetos a los que les extraía recursos.</summary>
    [SerializeField]
    List<GameObject> sujetosInfluenciados;

    [SerializeField]
    List<GameObject>almacenesInfluenciados;

    [SerializeField]
    float factorAgrandar = 1;

    public OSC osc;
    [SerializeField]
    int id;
    
   [SerializeField]
    bool funcionando;

    bool impar;

    //Para personas
    float minXin = 0;
    float maxXin = 1;
    float minYin = 0;
    float maxYin = 1;

    [SerializeField]
    float minX1, maxX1, minY1, maxY1, minX2, maxX2, minY2, maxY2;

    //Para blobs
    float minXinBlob = -150;
    float maxXinBlob = -100;
    float minXoutBlob = -100;
    float maxXoutBlob = -25;


    private void Start()
    {
        nextActionTime = 0f;
        intervalo = 2.5f;

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
        ActualizarCalibrado();

        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (funcionando)
        {
            // TODO: Sacar o ajustar
            //Agrandar(factorAgrandar);
        }
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + intervalo;
            foreach(GameObject sujeto in sujetosInfluenciados)
            {
                sujeto.GetComponent<vida>().Atacar();
            }
            if (impar)
            {
                audio.Play();
            }
            else
            {
                audio.Pause();
            }
            impar = !impar;

            foreach(GameObject almacen in almacenesInfluenciados)
            {
                almacen.GetComponent<AlmacenDeComida>().RestarRosquillas();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (funcionando)
        {
            if (!GameObject.Equals(collision.gameObject, gameObject))
            {
                if (collision.tag == "arbustoDeComida")
                {
                    collision.gameObject.GetComponent<ArbustoDeComida>().psem.enabled = true;
                }
                if (collision.tag == "almacenDeComida")
                {
                    collision.gameObject.GetComponent<AlmacenDeComida>().psem.enabled = true;
                    collision.gameObject.GetComponent<AlmacenDeComida>().RobarComida();
                    almacenesInfluenciados.Add(collision.gameObject);
                }
                if (collision.tag == "fabricaDeComida")
                {
                    collision.gameObject.GetComponent<FabricaDeComida>().psem.enabled = true;
                }
                if (collision.tag == "sujeto")
                {
                    sujetosInfluenciados.Add(collision.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!GameObject.Equals(collision.gameObject, gameObject))
        {
            if (collision.tag == "arbustoDeComida")
            {
                collision.gameObject.GetComponent<ArbustoDeComida>().psem.enabled = false;
            }
            if (collision.tag == "sujeto")
            {
                if (sujetosInfluenciados.Contains(collision.gameObject))
                {
                    sujetosInfluenciados.Remove(collision.gameObject);
                }
            }
        }
    }

    Vector3 ObtenerCoordenadas()
    {
        float y = transform.position.y;
        float z = -0.1f;
        float x = Utilidades.Map(transform.position.x, minXinBlob, maxXinBlob, minXoutBlob, maxXoutBlob, true);

        return new Vector3(x,y,z);
    }

    public void Prender()
    {
        funcionando = true;
    }

    public void RecibirDatosPersonaKin1(OscMessage m)
    {
            if (m.GetFloat(0) == id)
            {
                float x = m.GetFloat(1);
                float y = m.GetFloat(2);
                transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, minX1, maxX1), Utilidades.Map(y, minYin, maxYin, minY1, maxY1), -0.1f);
            }
    }
    public void RecibirDatosPersonaKin2(OscMessage m)
    {
        if (m.GetFloat(0) == id)
        {
            float x = m.GetFloat(1);
            float y = m.GetFloat(2);
            transform.localPosition = new Vector3(Utilidades.Map(x, minXin, maxXin, minX2, maxX2), Utilidades.Map(y, minYin, maxYin, minY2, maxY2), -0.1f);
        }
    }

    public void Configurar(OSC o, int id, int kinect)
    {
        osc = o;
        this.id = id;
        if(kinect == 1)
        {
            kinectAsignada = kin.kin1;
        } else if(kinect == 2)
        {
            kinectAsignada = kin.kin2;
        }
        else
        {
            Debug.Log("Se configuró una persona incorrectamente desde oscPersonasController. Numero de kinect mal especificado");
        }

        if (kinectAsignada == kin.kin1)
        {
            osc.SetAddressHandler("/KIN1/Updated/", RecibirDatosPersonaKin1);
            osc.SetAddressHandler("/KIN1/Leave/", EliminarPersona);
        }
        if (kinectAsignada == kin.kin2)
        {
            osc.SetAddressHandler("/KIN2/Updated/", RecibirDatosPersonaKin2);
            osc.SetAddressHandler("/KIN2/Leave/", EliminarPersona2);
        }

    }

    void EliminarPersona(OscMessage m)
    {
        if (m.GetFloat(0) == id)
        {
            //Destroy(GetComponent<SpriteRenderer>());
            gV.EliminarPersona(gameObject);
            Destroy(gameObject);
        }
    }
    void EliminarPersona2(OscMessage m)
    {
        if (m.GetFloat(0) == id)
        {
            //Destroy(GetComponent<SpriteRenderer>());
            gV.EliminarPersona(gameObject);
            Destroy(gameObject);
        }
    }
    void Agrandar(float factor)
    {
        transform.localScale = new Vector3(transform.localScale.x + factor, transform.localScale.y + factor, transform.localScale.z + factor);
    }


    public void ActualizarCalibrado()
    {
        minX1 = gV.minX1;
        maxX1 = gV.maxX1;
        minY1 = gV.minY1;
        maxY1 = gV.maxY1;
        minX2 = gV.minX2;
        maxX2 = gV.maxX2;
        minY2 = gV.minY2;
        maxY2 = gV.maxY2;
        Debug.Log("Persona " + gameObject.name + " actualizo su calibracion.");
    }

    public void Atacar()
    {
        Agrandar(-.1f);
        if(transform.localScale.x < 3)
        {
            gV.EliminarPersona(gameObject);
            Destroy(gameObject);
        }
    }
}

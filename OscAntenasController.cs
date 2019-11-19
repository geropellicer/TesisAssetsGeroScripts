using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Este archivo maneja la lógica de activar conn el entered nomas, pasandole la camara que corresponde. 
Para los update de posiciones y demas ir al totem.cs

*/


public class OscAntenasController : MonoBehaviour
{
    public GameObject totem1, totem2, totem3, totem4;
    OSC osc;
    globalVariables gV;


    // Start is called before the first frame update
    void Start()
    {
        osc = GetComponent<OSC>();
        osc.SetAddressHandler("/Cam1/Entered/", Cam1TotemEntered);
        osc.SetAddressHandler("/Cam2/Entered/", Cam2TotemEntered);

        osc.SetAddressHandler("/Cam1/Leave/", Cam1TotemLeave);
        osc.SetAddressHandler("/Cam2/Leave/", Cam2TotemLeave);

        osc.SetAddressHandler("/Cam1/Update/", Cam1TotemUpdate);
        osc.SetAddressHandler("/Cam2/Update/", Cam2TotemUpdate);

        osc.SetAddressHandler("/Antena1/Prendida/", PrenderAntena1);
        osc.SetAddressHandler("/Antena2/Prendida/", PrenderAntena2);
        osc.SetAddressHandler("/Antena3/Prendida/", PrenderAntena3);
        osc.SetAddressHandler("/Antena4/Prendida/", PrenderAntena4);

        osc.SetAddressHandler("/Antena1/Apagada/", ApagarAntena1);
        osc.SetAddressHandler("/Antena2/Apagada/", ApagarAntena1);
        osc.SetAddressHandler("/Antena3/Apagada/", ApagarAntena1);
        osc.SetAddressHandler("/Antena4/Apagada/", ApagarAntena1);

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Cam1TotemEntered(OscMessage m)
    {
        int idTotem = m.GetInt(0);
        if(idTotem == 1)
        {
            totem1.GetComponent<totem>().SetTrackeando(1);
            totem1.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 2)
        {
            totem2.GetComponent<totem>().SetTrackeando(1);
            totem2.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 3)
        {
            totem3.GetComponent<totem>().SetTrackeando(1);
            totem3.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 4)
        {
            totem4.GetComponent<totem>().SetTrackeando(1);
            totem4.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        }
    }
    void Cam2TotemEntered(OscMessage m)
    {
        int idTotem = m.GetInt(0);
        if(idTotem == 1)
        {
            totem1.GetComponent<totem>().SetTrackeando(2);
            totem1.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 2)
        {
            totem2.GetComponent<totem>().SetTrackeando(2);
            totem2.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 3)
        {
            totem3.GetComponent<totem>().SetTrackeando(2);
            totem3.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 4)
        {
            totem4.GetComponent<totem>().SetTrackeando(2);
            totem4.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        }
    }


    void Cam1TotemLeave(OscMessage m)
    {
        int idTotem = m.GetInt(0);
        if(idTotem == 1)
        {
            totem1.GetComponent<totem>().UnsetTrackeando(1);
        } else if(idTotem == 2)
        {
            totem2.GetComponent<totem>().UnsetTrackeando(1);
        } else if(idTotem == 3)
        {
            totem3.GetComponent<totem>().UnsetTrackeando(1);
        } else if(idTotem == 4)
        {
            totem4.GetComponent<totem>().UnsetTrackeando(1);
        }
    }
    void Cam2TotemLeave(OscMessage m)
    {
        int idTotem = m.GetInt(0);
        if(idTotem == 1)
        {
            totem1.GetComponent<totem>().UnsetTrackeando(2);
        } else if(idTotem == 2)
        {
            totem2.GetComponent<totem>().UnsetTrackeando(2);
        } else if(idTotem == 3)
        {
            totem3.GetComponent<totem>().UnsetTrackeando(2);
        } else if(idTotem == 4)
        {
            totem4.GetComponent<totem>().UnsetTrackeando(2);
        }
    }

    void Cam1TotemUpdate(OscMessage m)
    {
        int idTotem = m.GetInt(0);
        if(idTotem == 1)
        {
            totem1.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 2)
        {
            totem2.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 3)
        {
            totem3.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 4)
        {
            totem4.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        }
    }
    void Cam2TotemUpdate(OscMessage m)
    {
        int idTotem = m.GetInt(0);
        if(idTotem == 1)
        {
            totem1.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 2)
        {
            totem2.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 3)
        {
            totem3.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        } else if(idTotem == 4)
        {
            totem4.GetComponent<totem>().ActualizarTotemOsc(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
        }
    }



    void PrenderAntena1(OscMessage m)
    {
        totem1.GetComponent<totem>().EncenderTotem();
    }
    void PrenderAntena2(OscMessage m)
    {
        totem2.GetComponent<totem>().EncenderTotem();
    }
    void PrenderAntena3(OscMessage m)
    {
        totem3.GetComponent<totem>().EncenderTotem();
    }
    void PrenderAntena4(OscMessage m)
    {
        totem4.GetComponent<totem>().EncenderTotem();
    }

    void ApagarAntena1(OscMessage m)
    {
        totem1.GetComponent<totem>().ApagarTotem();
    }
    void ApagarAntena2(OscMessage m)
    {
        totem2.GetComponent<totem>().ApagarTotem();
    }
    void ApagarAntena3(OscMessage m)
    {
        totem3.GetComponent<totem>().ApagarTotem();
    }
    void ApagarAntena4(OscMessage m)
    {
        totem4.GetComponent<totem>().ApagarTotem();
    }


}

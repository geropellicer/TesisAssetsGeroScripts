using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proTest : MonoBehaviour
{
    public OSC osc;

    // Start is called before the first frame update
    void Start()
    {
        osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
        osc.SetAddressHandler("/Gero/Test", Recibir);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Recibir(OscMessage m)
    {
        Debug.Log("Recibido");
        Debug.Log(m.GetInt(0));
    }
}

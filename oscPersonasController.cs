using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscPersonasController : MonoBehaviour
{
    public GameObject prePersonPrefab;
    OSC osc;
    globalVariables gV;


    // Start is called before the first frame update
    void Start()
    {
        osc = GetComponent<OSC>();
        osc.SetAddressHandler("/KIN1/Entered/", CrearPersona1);
        osc.SetAddressHandler("/KIN2/Entered/", CrearPersona2);
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CrearPersona1(OscMessage m)
    {
        Debug.Log("Sucediendo 1");
        GameObject p = Instantiate(prePersonPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
        p.GetComponent<persona>().Configurar(osc, Mathf.RoundToInt(m.GetInt(0)), 1);
    }

    void CrearPersona2(OscMessage m)
    {
        Debug.Log("Sucediendo 2");
        GameObject p = Instantiate(prePersonPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
        p.GetComponent<persona>().Configurar(osc, Mathf.RoundToInt(m.GetInt(0)), 2);
    }
}

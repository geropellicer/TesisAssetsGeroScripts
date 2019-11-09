using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class depositoDeComida : MonoBehaviour
{
    /// <summary> La cantidad de comida que tiene disponible para cosechar este depósito </summary>
    [SerializeField]
    int unidadesRestantes;
    /// <summary> Devuelve para externos la cantidad de comida que tiene disponible para cosechar este depósito </summary>
    public int ObtenerUnidadesRestantes()
    {
        return unidadesRestantes;
    }

    [SerializeField]
    /// <summary> Prefab de la comida para instanciar en cada cosecha </summary>
    GameObject prefabComida; 

    [SerializeField]
    /// <summary> Prefab de este mismo deposito para instanciar otro cuando muramos </summary>
    GameObject prefabSelf; 

    [SerializeField]
    /// <summary> Referencia a las variables globales para conocer el piso </summary>
    globalVariables gV;

        

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starteando");
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        unidadesRestantes = Random.Range(10,20);
    }

    // Update is called once per frame
    void Update()
    {
        if(unidadesRestantes <= 0)
        {
            Morir();
        }
    }

    public GameObject Cosechar()
    {
        if(unidadesRestantes > 0)
        {
            unidadesRestantes --;
            return Instantiate(prefabComida, transform.position, Quaternion.identity);
        } else {
            Debug.LogWarning("Se cosechó un depósito al que no le queda comida");
            return null;
        } 
    }

    void Morir()
    {
        CrearNuevo();
        Destroy(gameObject);
    }

    void CrearNuevo()
    {
        Instantiate(prefabSelf, Utilidades.PuntoRandom(gV.piso), Quaternion.Euler(0,0,Random.Range(0f, 360f)));
    }
}

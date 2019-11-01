using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tipoPuerta
{
    roja,
    azul,
    amarilla,
    verde
}

public class puertaTotem : MonoBehaviour
{
    GameObject totem;


    public tipoPuerta estaPuerta;

    // Start is called before the first frame update
    void Start()
    {
        totem = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D objetoColisionado)
    {
        if(objetoColisionado.tag == "persona")
        {
            totem.GetComponent<totem>().EntrarColision(estaPuerta);
        }
    }

    private void OnTriggerExit2D(Collider2D objetoColisionado)
    {
        if (objetoColisionado.tag == "persona")
        {
            totem.GetComponent<totem>().SalirColision(estaPuerta);
        }
    }
}

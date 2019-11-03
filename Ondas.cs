using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ondas : MonoBehaviour
{
    /// <summary> El tipo de totem del totem de esta onda. </summary>
    TIPOTOTEM tipoTotemOnda;

    // Start is called before the first frame update
    void Start()
    {
        tipoTotemOnda = transform.GetComponentInParent<totem>().ObtenerTipoTotem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary> Cuando entra en contacto con un sujeto, le suma al nivel de afectacion que tiene por parte del discurso </summary>
    /// <summary> que emite esta antena en particular. </summary>
    void OnTriggerEnter2D(Collider2D other) {
        if(transform.GetComponentInParent<totem>().ObtenerEstaTransmitiendo())
        {
            if(other.gameObject.tag == "sujeto"){
                other.gameObject.GetComponent<Follower>().AfectarPorAntena(tipoTotemOnda);
            }
        }
    }

    /// <summary> Cuando entra en contacto con un sujeto, le suma al nivel de afectacion que tiene por parte del discurso </summary>
    /// <summary> que emite esta antena en particular. </summary>
    void OnTriggerStay2D(Collider2D other) {
        if(transform.GetComponentInParent<totem>().ObtenerEstaTransmitiendo())
        {
            if(other.gameObject.tag == "sujeto"){
                other.gameObject.GetComponent<Follower>().AfectarPorAntena(tipoTotemOnda);
            }
        }
    }
}

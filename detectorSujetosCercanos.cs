using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectorSujetosCercanos : MonoBehaviour
{

    [SerializeField]
    GameObject creador;

    [SerializeField]
    bool hayAlguien;

    [SerializeField]
    int vidaTotal;

    [SerializeField]
    int vidaActual;

    [SerializeField]
    Vector3 ultimaPosDetectada;

    public Vector3 ObtenerUltimaPosDetectada()
    {
        return ultimaPosDetectada;
    }

    // Update is called once per frame
    void Update()
    {
        if(vidaActual < vidaTotal)
        {
            vidaActual++;
        } else{
            Morir();
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        vidaActual = 0;
        vidaTotal = 60;
        hayAlguien = false;
        ultimaPosDetectada = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger detector");
        // Si es un sujeto lo que detectamos
        if(other.tag == "sujeto")
        {
            if(other.gameObject.GetComponent<Follower>().persona != null)
            {
                // Si es un sujeto que tiene una persona distinta a la persona que invocó este detector
                if(!GameObject.Equals(other.gameObject.GetComponent<Follower>().persona.gameObject, creador))
                {
                    hayAlguien = true;
                    ultimaPosDetectada = other.transform.position;
                }
            } else if(other.gameObject.GetComponent<Follower>().persona == null)
            {
                hayAlguien = true;
                ultimaPosDetectada = other.transform.position;
            }
            
        }
    }

    void Morir()
    {
        creador.GetComponent<Seguido>().SetHaySujetosAjenosEnRango(hayAlguien);
        gameObject.SetActive(false);
    }
}

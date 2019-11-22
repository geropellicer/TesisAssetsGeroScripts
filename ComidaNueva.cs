using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComidaNueva : MonoBehaviour
{
    [SerializeField]
    Transform persona;
    [SerializeField]
    Transform sujeto;

    [SerializeField]
    Transform spotSujeto;

    [SerializeField]
    float distanciaStopPersona = 10;
    [SerializeField]
    float distanciaStopSujeto = 4;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    bool configurada;

    [SerializeField]
    GameObject sujetoQueComio;

    [SerializeField]
    bool comida;
    /// <summary> Cuando la comida es de una persona, la preseleccionamos cuando un bicho tiene intencion de comersela, 
    /// para que no este disponible para otro bicho mientras el que la preselcciono llega a comerla </summary>
    [SerializeField]
    public bool preseleccionada;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(configurada && !comida)
        {
            if(persona != null){
                MoveToPersona();
            } else {
                MoveToSujeto();
            }
        } else if(!configurada){
            Debug.Log("Comida no configurada desde update");
        } else if(comida)
        {
            if(Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y), 
                new Vector2(sujetoQueComio.transform.position.x, sujetoQueComio.transform.position.y)) > 2)
            {
                transform.position = Vector2.MoveTowards(transform.position, sujetoQueComio.transform.position, 2f);
            } else {
                if(sujetoQueComio.GetComponent<Follower>())
                {
                    sujetoQueComio.GetComponent<Follower>().Alimentarse();
                } else if (sujetoQueComio.GetComponent<Seguido>())
                {
                    sujetoQueComio.GetComponent<Seguido>().Alimentarse();
                }
                Destroy(gameObject);
            }
        }
    }

    void MoveToPersona()
    {
        if(persona != null){
            if(Vector2.Distance(persona.position, transform.position) > distanciaStopPersona)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, persona.position, step);
            }
        }
    }
    void MoveToSujeto()
    {
        if(sujeto != null){
            if(Vector2.Distance(spotSujeto.position, transform.position) > distanciaStopSujeto)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, spotSujeto.position, step);
            }
        } else {
            Debug.LogWarning("ERROR: intentando ir a sujeto sin sujeto seteado");
        }
    }

    public void Comer(GameObject _sujetoQueComio)
    {
        if(configurada)
        {
            comida = true;
            sujetoQueComio = _sujetoQueComio;
        } else {
            Debug.Log("Comida no configurada desde Comer()");
        }
    }

    public void ComerPorPersona(GameObject _sujetoQueComio)
    {
        if(configurada)
        {
            comida = true;
            sujetoQueComio = _sujetoQueComio;
        } else {
            Debug.Log("Comida no configurada desde Comer()");
        }
    }

    /// <summary> Esta la llamamos cuando se crea</summary>
    public void Configurar(GameObject _persona, GameObject _sujeto, GameObject _spotSujeto)
    {
        if(_persona != null)
        {
            persona = _persona.transform;
        } else{
            persona = null;
        }
        
        if(_spotSujeto != null)
        {
            spotSujeto = _spotSujeto.transform;
        } else{
            spotSujeto = null;
        }
        
        sujeto = _sujeto.transform;
        configurada = true;

    }

    public void ConfigurarSoloPersona(GameObject _persona)
    {
        persona = _persona.transform;
    }
}

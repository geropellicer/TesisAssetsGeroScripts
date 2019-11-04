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
    float distanciaStopPersona = 10;
    [SerializeField]
    float distanciaStopSujeto = 4;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    bool configurada;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(configurada)
        {
            if(persona != null){
                MoveToPersona();
            } else {
                MoveToSujeto();
            }
        } else{
            Debug.Log("Comida no configurada desde update");
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
        Debug.Log(Vector2.Distance(sujeto.position, transform.position) );
        Debug.Log( speed * Time.deltaTime);
        if(sujeto != null){
            if(Vector2.Distance(sujeto.position, transform.position) > distanciaStopSujeto)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, sujeto.position, step);
            }
        } else {
            Debug.LogWarning("ERROR: intentando ir a sujeto sin sujeto seteado");
        }
    }

    public void Comer(GameObject sujetoQueComio)
    {
        if(configurada)
        {
            sujetoQueComio.GetComponent<Follower>().Alimentarse();
            Destroy(gameObject);
        } else {
            Debug.Log("Comida no configurada desde Comer()");
        }
    }

    /// <summary> Esta la llamamos cuando se crea</summary>
    public void Configurar(GameObject _persona, GameObject _sujeto)
    {
        if(_persona != null)
        {
            persona = _persona.transform;
        } else{
            persona = null;
        }
        sujeto = _sujeto.transform;
        configurada = true;
    }

    public void ConfigurarSoloPersona(GameObject _persona)
    {
        persona = _persona.transform;
    }
}

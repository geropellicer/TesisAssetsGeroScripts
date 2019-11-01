using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class personasGatillo : MonoBehaviour
{
    [SerializeField]
    GameObject prefabAdministradorPersona;
    [SerializeField]
    GameObject pisoActual;
    [SerializeField]
    GameObject pisoProximo;

    [SerializeField]
    List<GameObject> personasAdministrando;

    // Start is called before the first frame update
    void Start()
    {
        personasAdministrando = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "persona")
        {
            AgregarPersona(collision.gameObject);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "persona")
        {
            EliminarPersona(collision.gameObject);
        }
    }

    void AgregarPersona(GameObject p)
    {
        if (!personasAdministrando.Contains(p))
        {
            personasAdministrando.Add(p);
            p.GetComponent<persona>().Prender();
        }
        else
        {
            Debug.LogWarning("ERROR: se intento agrregar una persona a la lista de personas administradas que ya estaba");
        }
    }

    void EliminarPersona(GameObject p)
    {
        if (personasAdministrando.Contains(p))
        {
            //p.GetComponent<persona>().Apagar();
            personasAdministrando.Remove(p);
        }
        else
        {
            Debug.LogWarning("ERROR: se intento eliminar una persona a la lista de personas administradas que no estaba");
        }
    }

}

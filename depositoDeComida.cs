using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class depositoDeComida : MonoBehaviour
{
    /// <summary> La cantidad de comida que tiene disponible para cosechar este depósito </summary>
    [SerializeField]
    int unidadesRestantes;
    /// <summary> Devuelve para externos la cantidad de comida que tiene disponible para cosechar este depósito </summary>
    public int GetUnidadesRestantes()
    {
        return unidadesRestantes;
    }

    [SerializeField]
    /// <summary> Prefab de la comida para instanciar en cada cosecha </summary>
    GameObject prefabComida; 
        

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}

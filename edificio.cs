using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edificio : MonoBehaviour
{
    [SerializeField]
    protected Transform[] spots;

    protected globalVariables gV;
    protected spawner sP;

    protected float nextActionTime;
    protected float intervalo;

    [SerializeField]
    protected int comidaAlmacenada;
    [SerializeField]
    protected int comidaCosechada;
    [SerializeField]
    protected int comidaProcesada;


    [SerializeField]
    protected List<GameObject> trabajadoresAsignados;
    [SerializeField]
    protected int cantMaxTrabajadores;

    [SerializeField]
    protected List<GameObject> fabricasCercanas;
    [SerializeField]
    protected float distanciaMaxFabricasCercanas;

    protected float trabajoNecesarioBase;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetSpotTrabajo()
    {
        return spots[trabajadoresAsignados.Count-1].position;
    }

    public void EliminarTrabajador(GameObject trabajador)
    {
        if (trabajadoresAsignados.Contains(trabajador))
        {
            trabajadoresAsignados.Remove(trabajador);
        }
        else
        {
            Debug.LogWarning("Error: se intentó sacar de una lista de trabajadores a un trabajador que no está en la lista");
        }
    }

}

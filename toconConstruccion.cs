using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class toconConstruccion : MonoBehaviour
{
    [SerializeField]
    int cantTrabajadoresAsignados;
    [SerializeField]
    List<GameObject> trabajadoresAsignados;
    [SerializeField]
    List<GameObject> spots;
    [SerializeField]
    int cantidadSpots;

    public enum tocon
    {
        nullo,
        soviet,
        almacen,
        fabrica
    }
    [SerializeField]
    public tocon tipoTocon = tocon.nullo;

    [SerializeField]
    int vida;

    globalVariables gV;
    spawner sP;

    // Start is called before the first frame update
    void Start()
    {
        cantidadSpots = 5;
        spots = new List<GameObject>();
        foreach (Transform child in transform)
        {
            spots.Add(child.gameObject);
        }
        trabajadoresAsignados = new List<GameObject>();

        vida = 999;
    }

    private void Awake()
    {
        sP = GameObject.Find("GlobalManager").GetComponent<spawner>();
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tipoTocon != tocon.nullo)
        {
            if (vida <= 0)
            {
                

                if (tipoTocon == tocon.soviet)
                {
                    GameObject toconGO = Instantiate(sP.prefabSoviet, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(-10,10))));
                    gV.cantToconesSoviets--;
                    gV.cantSoviets++;

                    //Actualizamos el graph
                    var guo = new GraphUpdateObject(toconGO.GetComponent<Collider2D>().bounds);
                    guo.updatePhysics = true;
                    AstarPath.active.UpdateGraphs(guo);
                }
                if (tipoTocon == tocon.almacen)
                {
                    GameObject toconGO = Instantiate(sP.prefabAlmacen, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10))));
                    gV.cantToconesAlmacenes--;
                    gV.cantAlmacenes++;

                    //Actualizamos el graph
                    var guo = new GraphUpdateObject(toconGO.GetComponent<Collider2D>().bounds);
                    guo.updatePhysics = true;
                    AstarPath.active.UpdateGraphs(guo);

                }
                if (tipoTocon == tocon.fabrica)
                {
                    GameObject toconGO = Instantiate(sP.prefabFabrica, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10))));
                    gV.cantToconesFabricas--;
                    gV.cantFabricas++;

                    //Actualizamos el graph
                    var guo = new GraphUpdateObject(toconGO.GetComponent<Collider2D>().bounds);
                    guo.updatePhysics = true;
                    AstarPath.active.UpdateGraphs(guo);

                }

                EliminarTodos();
                Destroy(gameObject);
            }
        }
    }

    void AsignarTrabajador(GameObject sujeto)
    {
        if(cantidadSpots > cantTrabajadoresAsignados)
        {
            trabajadoresAsignados.Add(sujeto);
            cantTrabajadoresAsignados++;
            sujeto.GetComponent<vida>().AsignarSpotConstruccion(spots[cantTrabajadoresAsignados-1]);
        }
    } 

    public void Construir()
    {
        if(vida > 0)
        {
            vida--;
        } 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cantidadSpots >= cantTrabajadoresAsignados && vida > 20)
        {
            if (other.gameObject.GetComponent<vida>())
            {
                AsignarTrabajador(other.gameObject);
            }
        }
    }

    public void EliminarPorMuerte(GameObject sujeto)
    {
        if (trabajadoresAsignados.Contains(sujeto))
        {
            trabajadoresAsignados.Remove(sujeto);
            cantTrabajadoresAsignados--;
        }
        else
        {
            Debug.Log("ERROR: intentando eliminar por muerte a un trabajador que no esta en la lista de asignados del tocon.");
        }
    }

    void EliminarTodos()
    {
        for(int i= 0; i<trabajadoresAsignados.Count; i++)
        {
            if (trabajadoresAsignados[i] != null)
            {
                trabajadoresAsignados[i].GetComponent<vida>().EliminarDeConstruccion();
            }
        }
    }

    public float GetVida()
    {
        return vida;
    }
}

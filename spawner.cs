using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[ExecuteInEditMode]
public class spawner : MonoBehaviour
{
    public int cantidadASpawnear;
    public GameObject manager;
    globalVariables gV;
    public GameObject prefabComida;

    public GameObject prefabSoviet;
    public GameObject prefabFabrica;
    public GameObject prefabAlmacen;
    public GameObject prefabTocon;
    public GameObject prefabArbusto;

    public float velocidadDelTiempo;

    [SerializeField]
    int sovietActualConstruirFabrica = 1;
    [SerializeField]
    int sovietActualConstruirAlmacen;

    public enum tipo
    {
        arbusto,
        almacen,
        fabrica,
        soviet,
        sujeto,
        comida,
        tocon
    }



    // Start is called before the first frame update
    void Start()
    {
        gV = manager.GetComponent<globalVariables>();
        sovietActualConstruirFabrica = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnearBicho()
    {
        if (cantidadASpawnear > 0)
        {
            for (int i = 0; i < cantidadASpawnear; i++)
            {
                GameObject.Instantiate(gV.prefabBicho, Utilidades.PuntoRandom(gV.piso), Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("no hay ingresada una cantidad a spawnear mayor a 0");
        }
    }
    public void SpawnearComidaRandom()
    {
        if (cantidadASpawnear > 0)
        {
            for (int i = 0; i < cantidadASpawnear; i++)
            {
                GameObject ultimaComida = GameObject.Instantiate(prefabComida, Utilidades.PuntoRandom(gV.piso), Quaternion.identity);
                ultimaComida.transform.position = new Vector3(ultimaComida.transform.position.x, .33f, ultimaComida.transform.position.z);
            }
        }
        else
        {
            Debug.LogWarning("no hay ingresada una cantidad a spawnear mayor a 0");
        }
    }
    
    public GameObject SpawnearComida(Vector3 position)
    {
        return (GameObject)GameObject.Instantiate(prefabComida, position, Quaternion.identity);
    }
    

    public void SpawnearArbustos()
    {
        if (cantidadASpawnear > 0)
        {
            for (int i = 0; i < cantidadASpawnear; i++)
            {
                GameObject ultimoArbusto = GameObject.Instantiate(prefabArbusto, Utilidades.PuntoRandom(gV.piso, -1), Quaternion.Euler(new Vector3(0,0,Random.Range(-90,90))));
                //Actualizamos el graph
                var guo = new GraphUpdateObject(ultimoArbusto.GetComponent<Collider2D>().bounds);
                guo.updatePhysics = true;
                AstarPath.active.UpdateGraphs(guo);

            }
        }
        else
        {
            Debug.LogWarning("no hay ingresada una cantidad a spawnear mayor a 0");
        }
    }

    public void SetVelocidadDelTiempo()
    {
        Time.timeScale = velocidadDelTiempo;
    }



    
    public void BorrarDelTipo(tipo tipoABorrar)
    {
        switch (tipoABorrar)
        {
            case tipo.almacen:
                foreach(GameObject a in GameObject.FindGameObjectsWithTag("almacenDeComida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.arbusto:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("arbustoDeComida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.comida:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("comida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.fabrica:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("fabricaDeComida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.soviet:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("sovietComunidad"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.sujeto:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("sujeto"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.tocon:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("tocon"))
                {
                    DestroyImmediate(a);
                }
                break;
            default:
                break;
        }
    }

}

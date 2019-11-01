using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class comida : MonoBehaviour
{
    public globalVariables gV;

    //Porcion puede ir de 40 a 100
    [SerializeField]
    float porcion;
    //Valor nutricional puede ir de -1 a 1
    [SerializeField]
    float valorNutricional;
    //Gusto puede ir de -1 a 1
    [SerializeField]
    float gusto;

   public float GetValorNutricional()
    {
        return valorNutricional;
    }
    public float GetPorcion()
    {
        return porcion;
    }
    public float GetGusto()
    {
        return gusto;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        tag = "comida";

        porcion = Random.Range(40, 100);
        valorNutricional = Random.Range(-1, 1);
        gusto = Random.Range(3, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

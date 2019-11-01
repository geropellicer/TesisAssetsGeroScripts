using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inicio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //El siguiente codigo guarda las posiciones, rotaciones y tamaño de las camaras solo si la aplicacion se ejecuta por primera vez
        //Esto es para evitar que al abrir por primera vez la aplicacion, se carguen los valores del disco que van a estar en 0
        if (PlayerPrefs.GetInt("uso") != 1)
        {
            GameObject.Find("GuardarYReiniciar").GetComponent<guardarYReiniciar>().GuardarTodo();
            PlayerPrefs.SetInt("uso", 1);
            PlayerPrefs.Save();
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

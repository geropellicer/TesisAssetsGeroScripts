using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;

public class cameraMovement : MonoBehaviour
{
    public enum tipoControl
    {
        uno,
        dos
    }

    public tipoControl camaraSeleccionada;

    [SerializeField]
    float velocidad = 1;

    [SerializeField]
    float velocidadZoom = 1;

    [SerializeField]
    Transform camaraAsignada;
    string ejeXAsignado;
    string ejeYAsignado;
    string ejeZoomAsignado;
    string ejeRotAsignado;
    // Start is called before the first frame update
    void Start()
    {
        if(camaraSeleccionada == tipoControl.uno){
            camaraAsignada = GameObject.Find("Main Camera 1").transform;
            ejeXAsignado = "Horizontal1";
            ejeYAsignado = "Vertical1";
            ejeZoomAsignado = "Vertical3";
            //ejeRotAsignado = "Horizontal3";
        } else if(camaraSeleccionada == tipoControl.dos)
        {
            camaraAsignada = GameObject.Find("Main Camera 2").transform;
            ejeXAsignado = "Horizontal2";
            ejeYAsignado = "Vertical2";
            ejeZoomAsignado = "Vertical4";
            //ejeRotAsignado = "Horizontal4";
        }
    }

    // Update is called once per frame
    void Update()
    {
        camaraAsignada.Translate(CnInputManager.GetAxis(ejeXAsignado) * Time.deltaTime * velocidad, CnInputManager.GetAxis(ejeYAsignado) * Time.deltaTime * velocidad, 0);
        camaraAsignada.GetComponent<Camera>().orthographicSize += CnInputManager.GetAxis(ejeZoomAsignado) * Time.deltaTime * velocidadZoom * -1;
    }

    public void RotarIzq()
    {
        camaraAsignada.Rotate(0, 0, -90);
    }
    public void RotarDer()
    {
        camaraAsignada.Rotate(0, 0, 90);
    }
}

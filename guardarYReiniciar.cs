using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class guardarYReiniciar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject cuadroReiniciar = null;

    void Start()
    {
        CargarTodo();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GuardarCamara1()
    {
        PlayerPrefs.SetFloat("Cam1_posX", GameObject.Find("Main Camera 1").transform.position.x);
        PlayerPrefs.SetFloat("Cam1_posY", GameObject.Find("Main Camera 1").transform.position.y);
        PlayerPrefs.SetFloat("Cam1_posZ", GameObject.Find("Main Camera 1").transform.position.z);
        PlayerPrefs.SetFloat("Cam1_rotX", GameObject.Find("Main Camera 1").transform.rotation.x);
        PlayerPrefs.SetFloat("Cam1_rotY", GameObject.Find("Main Camera 1").transform.rotation.y);
        PlayerPrefs.SetFloat("Cam1_rotZ", GameObject.Find("Main Camera 1").transform.rotation.z);
        PlayerPrefs.SetFloat("Cam1_zoom", GameObject.Find("Main Camera 1").GetComponent<Camera>().orthographicSize);
        PlayerPrefs.Save();
    }

    public void GuardarCamara2()
    {
        PlayerPrefs.SetFloat("Cam2_posX", GameObject.Find("Main Camera 2").transform.position.x);
        PlayerPrefs.SetFloat("Cam2_posY", GameObject.Find("Main Camera 2").transform.position.y);
        PlayerPrefs.SetFloat("Cam2_posZ", GameObject.Find("Main Camera 2").transform.position.z);
        PlayerPrefs.SetFloat("Cam2_rotX", GameObject.Find("Main Camera 2").transform.rotation.x);
        PlayerPrefs.SetFloat("Cam2_rotY", GameObject.Find("Main Camera 2").transform.rotation.y);
        PlayerPrefs.SetFloat("Cam2_rotZ", GameObject.Find("Main Camera 2").transform.rotation.z);
        PlayerPrefs.SetFloat("Cam2_zoom", GameObject.Find("Main Camera 2").GetComponent<Camera>().orthographicSize);
        PlayerPrefs.Save();
    }

    public void GuardarTodo()
    {
        GuardarCamara1();
        GuardarCamara2();
        GameObject.Find("BotonSetear").GetComponent<UISetearCalibrado>().Guardar();
    }

    public void AbrirReiniciar()
    {
        cuadroReiniciar.SetActive(true);
    }

    public void CerrarReiniciar()
    {
        cuadroReiniciar.SetActive(false);
    }

    public void ConfirmarReiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CargarTodo()
    {
        //CargarCamara1();
        //CargarCamara2();
        //GameObject.Find("BotonSetear").GetComponent<UISetearCalibrado>().Cargar();
    }

    void CargarCamara1()
    {
        GameObject.Find("Main Camera 1").transform.position = new Vector3(PlayerPrefs.GetFloat("Cam1_posX"), PlayerPrefs.GetFloat("Cam1_posY"), PlayerPrefs.GetFloat("Cam1_posZ"));
        GameObject.Find("Main Camera 1").transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("Cam1_rotX"), PlayerPrefs.GetFloat("Cam1_rotY"), PlayerPrefs.GetFloat("Cam1_rotZ"));
        GameObject.Find("Main Camera 1").GetComponent<Camera>().orthographicSize = PlayerPrefs.GetFloat("Cam1_zoom");
    }
    void CargarCamara2()
    {
        GameObject.Find("Main Camera 2").transform.position = new Vector3(PlayerPrefs.GetFloat("Cam2_posX"), PlayerPrefs.GetFloat("Cam2_posY"), PlayerPrefs.GetFloat("Cam2_posZ"));
        GameObject.Find("Main Camera 2").transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("Cam2_rotX"), PlayerPrefs.GetFloat("Cam2_rotY"), PlayerPrefs.GetFloat("Cam2_rotZ"));
        GameObject.Find("Main Camera 2").GetComponent<Camera>().orthographicSize = PlayerPrefs.GetFloat("Cam2_zoom");
    }
}
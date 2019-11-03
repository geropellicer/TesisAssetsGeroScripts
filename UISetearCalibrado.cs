using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetearCalibrado : MonoBehaviour
{
    public GameObject input_minX;
    public GameObject input_maxX;
    public GameObject input_minY1;
    public GameObject input_maxY1;
    public GameObject input_minY2;
    public GameObject input_maxY2;

    public GameObject output_minX;
    public GameObject output_maxX;
    public GameObject output_minY1;
    public GameObject output_maxY1;
    public GameObject output_minY2;
    public GameObject output_maxY2;

    public float minX, maxX, minY1, maxY1, minY2, maxY2;
    public float[] valores = new float[8];

    globalVariables gV;

    // Start is called before the first frame update
    void Start()
    {
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
    }

    private void Awake()
    {
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MostrarResultados();
        }
    }

    public void Guardar()
    {
        ConfigurarVariable();
        SetearCalibrado(valores);
        MostrarResultados();
    }

    void ConfigurarVariable()
    {
        minX = PlayerPrefs.GetFloat("kin_minX"); 
        maxX = PlayerPrefs.GetFloat("kin_maxX");
        minY1 = PlayerPrefs.GetFloat("kin1_minY");
        maxY1 = PlayerPrefs.GetFloat("kin1_maxY");
        minY2 = PlayerPrefs.GetFloat("kin2_minY");
        maxY2 = PlayerPrefs.GetFloat("kin2_maxY");

        if (input_minX.GetComponent<InputField>().text != "") {
            minX = float.Parse(input_minX.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_maxX.GetComponent<InputField>().text != "") {
            maxX = float.Parse(input_maxX.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_minY1.GetComponent<InputField>().text != "") {
            minY1 = float.Parse(input_minY1.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_maxY1.GetComponent<InputField>().text != "") {
            maxY1 = float.Parse(input_maxY1.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_minY2.GetComponent<InputField>().text != "")
        {
            minY2 = float.Parse(input_minY2.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_maxY2.GetComponent<InputField>().text != "")
        {
            maxY2 = float.Parse(input_maxY2.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }

        valores[0] = minX;
        valores[1] = maxX;
        valores[2] = minY1;
        valores[3] = maxY1;
        valores[4] = minY2;
        valores[5] = maxY2;
    }

    public static void SetearCalibrado(float[] val)
    {
        PlayerPrefs.SetFloat("kin_minX", val[0]);
        PlayerPrefs.SetFloat("kin_maxX", val[1]);
        PlayerPrefs.SetFloat("kin1_minY", val[2]);
        PlayerPrefs.SetFloat("kin1_maxY", val[3]);
        PlayerPrefs.SetFloat("kin2_minY", val[4]);
        PlayerPrefs.SetFloat("kin2_maxY", val[5]);
        PlayerPrefs.Save();
    }

    void MostrarResultados()
    {
        output_minX.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin_minX").ToString();
        output_maxX.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin_maxX").ToString();
        output_minY1.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin1_minY").ToString();
        output_maxY1.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin1_maxY").ToString();
        output_minY2.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin2_minY").ToString();
        output_maxY2.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin2_minY").ToString();
    }

    public void Cargar()
    {
        //TODO: Cargar a las varibles globales y desde ahi asignar al manager de personas
        gV.minX = PlayerPrefs.GetFloat("kin_minX");
        gV.maxX = PlayerPrefs.GetFloat("kin_maxX");
        gV.minY1 = PlayerPrefs.GetFloat("kin1_minY");
        gV.maxY1 = PlayerPrefs.GetFloat("kin1_maxY");
        gV.minY2 = PlayerPrefs.GetFloat("kin2_minY");
        gV.maxY2 = PlayerPrefs.GetFloat("kin2_maxY");
    }

}

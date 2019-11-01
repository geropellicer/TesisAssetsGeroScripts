using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetearCalibrado : MonoBehaviour
{
    public GameObject input_minX1;
    public GameObject input_maxX1;
    public GameObject input_minY1;
    public GameObject input_maxY1;
    public GameObject input_minX2;
    public GameObject input_maxX2;
    public GameObject input_minY2;
    public GameObject input_maxY2;

    public GameObject output_minX1;
    public GameObject output_maxX1;
    public GameObject output_minY1;
    public GameObject output_maxY1;
    public GameObject output_minX2;
    public GameObject output_maxX2;
    public GameObject output_minY2;
    public GameObject output_maxY2;

    public float minX1, maxX1, minY1, maxY1, minX2, maxX2, minY2, maxY2;
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
        minX1 = PlayerPrefs.GetFloat("kin1_minX"); 
        maxX1 = PlayerPrefs.GetFloat("kin1_maxX");
        minY1 = PlayerPrefs.GetFloat("kin1_minY");
        maxY1 = PlayerPrefs.GetFloat("kin1_maxY");
        minX2 = PlayerPrefs.GetFloat("kin2_minX");
        maxX2 = PlayerPrefs.GetFloat("kin2_maxX");
        minY2 = PlayerPrefs.GetFloat("kin2_minY");
        maxY2 = PlayerPrefs.GetFloat("kin2_maxY");

        if (input_minX1.GetComponent<InputField>().text != "") {
            minX1 = float.Parse(input_minX1.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_maxX1.GetComponent<InputField>().text != "") {
            maxX1 = float.Parse(input_maxX1.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_minY1.GetComponent<InputField>().text != "") {
            minY1 = float.Parse(input_minY1.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_maxY1.GetComponent<InputField>().text != "") {
            maxY1 = float.Parse(input_maxY1.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_minX2.GetComponent<InputField>().text != "")
        {
            minX2 = float.Parse(input_minX2.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_maxX2.GetComponent<InputField>().text != "")
        {
            maxX2 = float.Parse(input_maxX2.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_minY2.GetComponent<InputField>().text != "")
        {
            minY2 = float.Parse(input_minY2.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }
        if (input_maxY2.GetComponent<InputField>().text != "")
        {
            maxY2 = float.Parse(input_maxY2.GetComponent<InputField>().text, System.Globalization.NumberStyles.Number);
        }

        valores[0] = minX1;
        valores[1] = maxX1;
        valores[2] = minY1;
        valores[3] = maxY1;
        valores[4] = minX2;
        valores[5] = maxX2;
        valores[6] = minY2;
        valores[7] = maxY2;
    }

    public static void SetearCalibrado(float[] val)
    {
        PlayerPrefs.SetFloat("kin1_minX", val[0]);
        PlayerPrefs.SetFloat("kin1_maxX", val[1]);
        PlayerPrefs.SetFloat("kin1_minY", val[2]);
        PlayerPrefs.SetFloat("kin1_maxY", val[3]);
        PlayerPrefs.SetFloat("kin2_minX", val[4]);
        PlayerPrefs.SetFloat("kin2_maxX", val[5]);
        PlayerPrefs.SetFloat("kin2_minY", val[6]);
        PlayerPrefs.SetFloat("kin2_maxY", val[7]);
        PlayerPrefs.Save();
    }

    void MostrarResultados()
    {
        output_minX1.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin1_minX").ToString();
        output_maxX1.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin1_maxX").ToString();
        output_minY1.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin1_minY").ToString();
        output_maxY1.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin1_maxY").ToString();
        output_minX2.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin2_minX").ToString();
        output_maxX2.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin2_maxX").ToString();
        output_minY2.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin2_minY").ToString();
        output_maxY2.GetComponent<Text>().text = PlayerPrefs.GetFloat("kin2_minY").ToString();
    }

    public void Cargar()
    {
        //TODO: Cargar a las varibles globales y desde ahi asignar al manager de personas
        gV.minX1 = PlayerPrefs.GetFloat("kin1_minX");
        gV.maxX1 = PlayerPrefs.GetFloat("kin1_maxX");
        gV.minY1 = PlayerPrefs.GetFloat("kin1_minY");
        gV.maxY1 = PlayerPrefs.GetFloat("kin1_maxY");
        gV.minX2 = PlayerPrefs.GetFloat("kin2_minX");
        gV.maxX2 = PlayerPrefs.GetFloat("kin2_maxX");
        gV.minY2 = PlayerPrefs.GetFloat("kin2_minY");
        gV.maxY2 = PlayerPrefs.GetFloat("kin2_maxY");
    }

}

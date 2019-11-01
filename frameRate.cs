using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class frameRate : MonoBehaviour
{

    public Text t;
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "{0} FPS";

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Text>();
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

    }

    // Update is called once per frame
    void Update()
    {
        if(t != null)
        {
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
                t.text = string.Format(display, m_CurrentFps);
            }
        }
        else
        {
            Debug.LogWarning("Error: no hay un componente de texto en este objeto");
        }
    }
}

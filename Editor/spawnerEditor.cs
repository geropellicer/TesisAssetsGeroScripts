using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(spawner))]
public class spawnerEditor : Editor
{
    public spawner.tipo tipoABorrar;

    public override void OnInspectorGUI()
    {
        spawner sp = (spawner)target;

       if (GUILayout.Button("Spawnear bichos"))
        {
            sp.SpawnearBicho();
        }
        if (GUILayout.Button("Spawnear arbustos"))
        {
            sp.SpawnearArbustos();
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Set tiempo"))
        {
            sp.SetVelocidadDelTiempo();
        }


        EditorGUILayout.EnumPopup(tipoABorrar);

        if (GUILayout.Button("Borrar"))
        {
            sp.BorrarDelTipo(tipoABorrar);
        }
    }

    [MenuItem("Gero/New Option %#a")]
    private static void NewMenuOption()
    {
        Debug.Log("Opcion funcionando");
    }
}
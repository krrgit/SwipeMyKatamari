using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnRandomObject))]
public class SpawnRandomObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpawnRandomObject spawnRandomObject = (SpawnRandomObject)target;

        if (GUILayout.Button("Spawn Random Object"))
        {
            spawnRandomObject.SpawnRandomObjectInEditMode();
        }
    }
}
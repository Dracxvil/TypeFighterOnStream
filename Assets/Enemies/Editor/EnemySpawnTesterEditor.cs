using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawnTester))]
public class EnemySpawnTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        EnemySpawnTester tester = (EnemySpawnTester)target;

        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Spawn Test Enemy", GUILayout.Height(35)))
        {
            tester.SpawnEnemy();
        }

        GUI.backgroundColor = Color.white;
    }
}
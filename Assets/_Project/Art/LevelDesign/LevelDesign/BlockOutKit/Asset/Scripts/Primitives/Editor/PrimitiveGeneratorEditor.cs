using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
    [CustomEditor(typeof(PrimitiveGenerator))]
    [CanEditMultipleObjects]
    public class PrimitiveGeneratorEditor : Editor
    {
        PrimitiveGenerator PG;

        SerializedProperty liveBuildMode;
        SerializedProperty meshCollider;
        SerializedProperty selectedOBJ;

        void OnEnable()
        {
            PG = FindObjectOfType<PrimitiveGenerator>();
            liveBuildMode = serializedObject.FindProperty("LiveBuild");
            meshCollider = serializedObject.FindProperty("MeshCollider");
            selectedOBJ = serializedObject.FindProperty("selectedOBJ");

            if (PG.gameObject.GetComponent<MeshFilter>() == null) PG.gameObject.AddComponent<MeshFilter>();
            if (PG.gameObject.GetComponent<MeshRenderer>() == null) PG.gameObject.AddComponent<MeshRenderer>();
        }

        public override void OnInspectorGUI()
        {
            PG.selectedOBJ = Selection.activeGameObject;
            PG.manualUpdate();
            // DrawDefaultInspector();

            // Label Style
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;

            GUILayout.Space(10);

            GUILayout.Label("Current Primitive - click to Change", labelStyle);
            if (GUILayout.Button(PG.currentPrimitiveScript))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Box"), false, PG.Box);
                menu.AddItem(new GUIContent("Wedge"), false, PG.Wedge);
                menu.AddItem(new GUIContent("Sphere"), false, PG.Sphere);
                menu.AddItem(new GUIContent("IcoSphere"), false, PG.IcoSphere);
                menu.AddItem(new GUIContent("Cylinder"), false, PG.Cylinder);
                menu.AddItem(new GUIContent("Cone"), false, PG.Cone);
                menu.AddItem(new GUIContent("Torus"), false, PG.Torus);
                menu.AddItem(new GUIContent("TorusKnot"), false, PG.TorusKnot);
                menu.AddItem(new GUIContent("Table"), false, PG.Table);
                menu.AddItem(new GUIContent("Chair"), false, PG.Chair);
                menu.AddItem(new GUIContent("Stairs"), false, PG.Stairs);
                menu.AddItem(new GUIContent("Remove Script"), false, PG.Remove);

                menu.ShowAsContext();
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(meshCollider);
            serializedObject.ApplyModifiedProperties();

            serializedObject.Update();
            EditorGUILayout.PropertyField(liveBuildMode);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Build Geometry"))
            {
                PG.BuildGeomtery = true;
            }

        }

    }
}
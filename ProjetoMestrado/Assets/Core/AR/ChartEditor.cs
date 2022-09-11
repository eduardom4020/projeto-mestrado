//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(Chart))]
//[CanEditMultipleObjects]
//public class ChartEditor : Editor
//{
//    SerializedProperty Title;

//    void OnEnable()
//    {
//        Title = serializedObject.FindProperty("Title");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        EditorGUILayout.PropertyField(Title);
//        serializedObject.ApplyModifiedProperties();
//    }
//}
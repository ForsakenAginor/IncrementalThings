#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Source.Scripts.Utility.UI.Editor
{
    [CustomEditor(typeof(MultiGraphicButton))]
    public class MultiGraphicButtonEditor : ButtonEditor
    {
        private SerializedProperty _additionalGraphicsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _additionalGraphicsProperty = serializedObject.FindProperty("_allGraphics");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Multi Graphic Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_additionalGraphicsProperty, new GUIContent("Additional Graphics"));

            serializedObject.ApplyModifiedProperties();


        }
    }
}
#endif
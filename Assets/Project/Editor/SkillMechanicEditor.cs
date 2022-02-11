using Swap.Components;
using Swap.Components.Models;
using UnityEditor;

namespace Swap.UnityEditor
{
    [CustomEditor(typeof(SkillMechanic))]
    public class SkillMechanicEditor : Editor
    {
        private readonly string[] WeightSkillParams = { "Signal" };

        public override void OnInspectorGUI()
        {
            SkillMechanic mechanic = (SkillMechanic)target;

            DrawDefaultInspector();
            EditorGUILayout.Space();

            switch (mechanic.SkillType)
            {
                case SkillType.DetectWeight:
                    DrawSkillSettings(WeightSkillParams);
                    break;
            }
        }

        private void DrawSkillSettings(string[] skillParams)
        {
            EditorGUILayout.LabelField("Skill Settings", EditorStyles.boldLabel);
            foreach (string param in skillParams)
            {
                SerializedProperty property = serializedObject.FindProperty($"Settings.{param}");
                EditorGUILayout.PropertyField(property);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
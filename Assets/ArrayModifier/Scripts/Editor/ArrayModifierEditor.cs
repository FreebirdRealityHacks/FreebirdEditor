using UnityEditor;
using GyroVectors;

[CustomEditor(typeof(ArrayModifier))]
[CanEditMultipleObjects]
public class ArrayModifierEditor : Editor
{
    SerializedProperty heirachyArrangement;
    SerializedProperty dropDown1;
    SerializedProperty hasVisbleObject;
    SerializedProperty ArrayObject;
    SerializedProperty fitType;
    SerializedProperty Count;
    SerializedProperty Lenght;
    SerializedProperty offsetType;
    SerializedProperty OffSetAlongAxis;
    SerializedProperty Radius;
    SerializedProperty ArcExtent;
    SerializedProperty radialOffsetAxis;
    SerializedProperty dropDown2;
    SerializedProperty rotationType;
    SerializedProperty Center;
    SerializedProperty CopyComponents;
    SerializedProperty Randomize;
    SerializedProperty RandomThreshold;
    void OnEnable()
    {
        heirachyArrangement = serializedObject.FindProperty("heirachyArrangement");
        dropDown1 = serializedObject.FindProperty("dropDown1");
        hasVisbleObject = serializedObject.FindProperty("hasVisbleObject");
        ArrayObject = serializedObject.FindProperty("ArrayObject");
        fitType = serializedObject.FindProperty("fitType");
        Count = serializedObject.FindProperty("Count");
        Lenght = serializedObject.FindProperty("Lenght");
        offsetType = serializedObject.FindProperty("offsetType");
        OffSetAlongAxis = serializedObject.FindProperty("OffSetAlongAxis");
        Radius = serializedObject.FindProperty("Radius");
        ArcExtent = serializedObject.FindProperty("ArcExtent");
        radialOffsetAxis = serializedObject.FindProperty("radialOffsetAxis");
        dropDown2 = serializedObject.FindProperty("dropDown2");
        rotationType = serializedObject.FindProperty("rotationType");
        Center = serializedObject.FindProperty("Center");
        CopyComponents = serializedObject.FindProperty("CopyComponents");
        Randomize = serializedObject.FindProperty("Randomize");
        RandomThreshold = serializedObject.FindProperty("RandomThreshold");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawProperties();
        serializedObject.ApplyModifiedProperties();
    }
    void DrawProperties()
    {
        //EditorGUILayout.PropertyField(heirachyArrangement);
        if (hasVisbleObject.boolValue == false)
            EditorGUILayout.PropertyField(ArrayObject);
        else
            dropDown1.boolValue = EditorGUILayout.Foldout(dropDown1.boolValue, "Object", false);
        if (dropDown1.boolValue)
            EditorGUILayout.PropertyField(ArrayObject);

        EditorGUILayout.PropertyField(fitType);
        if (fitType.intValue == 0)
            EditorGUILayout.PropertyField(Count);
        else{
            EditorGUILayout.PropertyField(Lenght); 
            EditorGUILayout.PrefixLabel("(in meters)");
        }

        EditorGUILayout.PropertyField(offsetType);
        if (offsetType.intValue != 2)
            EditorGUILayout.PropertyField(OffSetAlongAxis);
        else
        {
            EditorGUILayout.PropertyField(Radius);
            EditorGUILayout.PropertyField(ArcExtent);
            EditorGUILayout.PropertyField(radialOffsetAxis);
            dropDown2.boolValue = EditorGUILayout.Foldout(dropDown2.boolValue, "Advanced", false);
            if (dropDown2.boolValue)
            {
                EditorGUILayout.PropertyField(rotationType);
                EditorGUILayout.PropertyField(Center);
            }
        }
        EditorGUILayout.PropertyField(CopyComponents);
        EditorGUILayout.PropertyField(Randomize);
        if (Randomize.boolValue)
            EditorGUILayout.PropertyField(RandomThreshold);
    }
}

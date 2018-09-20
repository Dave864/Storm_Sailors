using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ship))]
public class ShipEditor : Editor
{
    // Properties to change
    SerializedProperty s_baseSpeed;
    SerializedProperty s_slowTime;
    SerializedProperty s_uTurnTime;

    private void OnEnable()
    {
        // Get current values of the properties
        s_baseSpeed = serializedObject.FindProperty("baseSpeed");
        s_slowTime = serializedObject.FindProperty("slowTime");
        s_uTurnTime = serializedObject.FindProperty("uTurnTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Set base speed, with minimum value set
        EditorGUI.BeginChangeCheck();
        int newBaseSpeed = EditorGUILayout.DelayedIntField("Base Ship Speed", s_baseSpeed.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            // Apply a minimum value to base speed
            s_baseSpeed.intValue = (newBaseSpeed < 0) ? 0 : newBaseSpeed;
        }

        // Set base deceleration, with minimum value set
        EditorGUI.BeginChangeCheck();
        float newSlowTime = EditorGUILayout.DelayedFloatField("Time to slow down (sec)", s_slowTime.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            // Apply minimum value to base deceleration
            s_slowTime.floatValue = (newSlowTime < 0) ? 0 : newSlowTime;
        }

        // Set u-turn time, with minimum value set
        EditorGUI.BeginChangeCheck();
        float newTurnTime = EditorGUILayout.DelayedFloatField("Turnabout time (sec)", s_uTurnTime.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            // Apply minimum value to u-turn time
            s_uTurnTime.floatValue = (newTurnTime < 0) ? 0 : newTurnTime;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

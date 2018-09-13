using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(GaleMode))]
public class GaleModeEditor : Editor
{
    // Properties to alter
    SerializedProperty s_cloudSpawnTime;
    SerializedProperty s_cloudDispelTime;
    SerializedProperty s_cloudTimerSlider;

	protected virtual void OnEnable()
    {
        s_cloudSpawnTime = serializedObject.FindProperty("cloudSpawnTime");
        s_cloudDispelTime = serializedObject.FindProperty("cloudDispelTime");
        s_cloudTimerSlider = serializedObject.FindProperty("cloudTimerSlider");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Adjust the time it takes to spawn a cloud
        EditorGUI.BeginChangeCheck();
        float spawnTime = EditorGUILayout.FloatField("Cloud Spawn Time (sec)", s_cloudSpawnTime.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            s_cloudSpawnTime.floatValue = (spawnTime < 0) ? 0 : spawnTime;
        }

        // Adjust the time it takes to dispel a cloud
        EditorGUI.BeginChangeCheck();
        float dispelTime = EditorGUILayout.FloatField("Cloud Dispel Time (sec)", s_cloudDispelTime.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            s_cloudDispelTime.floatValue = (dispelTime < 0) ? 0 : dispelTime;
        }

        // Set the reference to slider ui for the cloud timer
        EditorGUILayout.PropertyField(s_cloudTimerSlider);

        serializedObject.ApplyModifiedProperties();
    }
}

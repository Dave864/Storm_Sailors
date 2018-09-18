using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(GaleMode))]
public class GaleModeEditor : Editor
{
    // Properties to alter
    SerializedProperty s_cloudLevelOverload;
    SerializedProperty s_cloudGrabTime;
    SerializedProperty s_cloudSpawnTime;
    SerializedProperty s_cloudDispelTime;
    SerializedProperty s_cloudTimerSlider;
    SerializedProperty s_dispelAllMult;

	protected virtual void OnEnable()
    {
        s_cloudLevelOverload = serializedObject.FindProperty("cloudLevelOverload");
        s_cloudGrabTime = serializedObject.FindProperty("cloudGrabTime");
        s_cloudSpawnTime = serializedObject.FindProperty("cloudSpawnTime");
        s_cloudDispelTime = serializedObject.FindProperty("cloudDispelTime");
        s_cloudTimerSlider = serializedObject.FindProperty("cloudTimerSlider");
        s_dispelAllMult = serializedObject.FindProperty("dispelAllMult");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Adjust the max level a gale cloud can be at
        EditorGUI.BeginChangeCheck();
        int lvl = EditorGUILayout.DelayedIntField("Max Gale Cloud Level", s_cloudLevelOverload.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            lvl = (lvl < 0) ? 0 : lvl;
            s_cloudLevelOverload.intValue = lvl;
        }

        // Adjust the time threshold for confirming a grab action
        EditorGUI.BeginChangeCheck();
        float grabTime = EditorGUILayout.Slider("Grab Threshold (sec)", s_cloudGrabTime.floatValue, 0.01f, 0.3f);
        if (EditorGUI.EndChangeCheck())
        {
            s_cloudGrabTime.floatValue = grabTime;
        }

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

        // Curve drawer editor for dispel all time
        EditorGUILayout.PropertyField(s_dispelAllMult);

        serializedObject.ApplyModifiedProperties();
    }
}

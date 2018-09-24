using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StormMode))]
public class StormModeEditor : Editor
{
    // Properites to change
    SerializedProperty s_stormLaunchTime;
    SerializedProperty s_stormSpawnTime;
    SerializedProperty s_stormGatherTime;
    SerializedProperty s_stormChargeMult;
    SerializedProperty s_stormTimerSlider;
    SerializedProperty s_stormLevelSustainable;
    SerializedProperty s_stormLevelOverload;

    // Range values for storm level GUI
    private readonly int maxLevel = 10;
    private float minStormValue;
    private float maxStormValue;

    // Value of level to check
    int lvlToCheck;

    // Use this for initialization
    void OnEnable()
    {
        s_stormLaunchTime = serializedObject.FindProperty("stormLaunchTime");
        s_stormSpawnTime = serializedObject.FindProperty("stormSpawnTime");
        s_stormGatherTime = serializedObject.FindProperty("stormGatherTime");
        s_stormChargeMult = serializedObject.FindProperty("stormChargeMult");
        s_stormTimerSlider = serializedObject.FindProperty("stormTimerSlider");
        s_stormLevelSustainable = serializedObject.FindProperty("stormLevelSustainable");
        s_stormLevelOverload = serializedObject.FindProperty("stormLevelOverload");
        minStormValue = s_stormLevelSustainable.intValue;
        maxStormValue = s_stormLevelOverload.intValue;
        lvlToCheck = 0;
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Set launch threshold time
        EditorGUI.BeginChangeCheck();
        float launchTime = EditorGUILayout.Slider("Launch Threshold (sec)", s_stormLaunchTime.floatValue, 0.01f, 0.3f);
        if (EditorGUI.EndChangeCheck())
        {
            s_stormLaunchTime.floatValue = launchTime;
        }

        // Set gale cloud gather time
        EditorGUI.BeginChangeCheck();
        float gatherTime = EditorGUILayout.Slider("Gale Cloud Gather (sec)", s_stormGatherTime.floatValue, 0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            s_stormGatherTime.floatValue = gatherTime;
        }

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Charge Time Settings");

        // Set base charge time
        EditorGUI.BeginChangeCheck();
        float spawnTime = EditorGUILayout.DelayedFloatField("Initial Spawn Time (sec)", s_stormSpawnTime.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            spawnTime = (spawnTime < 0) ? 0 : spawnTime;
            s_stormSpawnTime.floatValue = spawnTime;
        }

        // Set multiplier curve for charge time
        EditorGUILayout.PropertyField(s_stormChargeMult);

        // Show the charge time for various levels
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Charge Time Viewer");
        lvlToCheck = EditorGUILayout.IntSlider("Level", lvlToCheck, 0, Mathf.RoundToInt(maxStormValue) + 1);
        float chargeTime = s_stormChargeMult.animationCurveValue.Evaluate(lvlToCheck) * spawnTime;
        EditorGUILayout.LabelField("Charge Time (sec): " + chargeTime);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Storm Front Level Settings");

        // Editor GUI for storm levels
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.MinMaxSlider("Storm Front Level Range", ref minStormValue, ref maxStormValue, 0f, maxLevel);

        // Display the level values
        EditorGUILayout.LabelField("Sustainable Storm Level: " + Mathf.RoundToInt(minStormValue));
        EditorGUILayout.LabelField("Overload Storm Level: " + Mathf.RoundToInt(maxStormValue));

        if (EditorGUI.EndChangeCheck())
        {
            s_stormLevelSustainable.intValue = Mathf.RoundToInt(minStormValue);
            s_stormLevelOverload.intValue = Mathf.RoundToInt(maxStormValue);
        }
        EditorGUILayout.EndVertical();

        // Get slider for storm timer
        EditorGUILayout.PropertyField(s_stormTimerSlider);

        serializedObject.ApplyModifiedProperties();
    }
}

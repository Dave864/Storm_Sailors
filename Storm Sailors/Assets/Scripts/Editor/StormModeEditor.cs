using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StormMode))]
public class StormModeEditor : Editor
{
    // Properites to change
    SerializedProperty s_stormLevelSustainable;
    SerializedProperty s_stormLevelOverload;

    // Range values for storm level GUI
    private readonly int maxLevel = 20;
    private float minStormValue;
    private float maxStormValue;

	// Use this for initialization
	void OnEnable()
    {
        s_stormLevelSustainable = serializedObject.FindProperty("stormLevelSustainable");
        s_stormLevelOverload = serializedObject.FindProperty("stormLevelOverload");
        minStormValue = s_stormLevelSustainable.intValue;
        maxStormValue = s_stormLevelOverload.intValue;
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

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
        serializedObject.ApplyModifiedProperties();
    }
}

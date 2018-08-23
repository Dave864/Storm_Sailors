using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum POSITIONS
{
    N = 1,
    S = 0,
    E = 5,
    W = 2,
    NE = 7,
    NW = 4,
    SE = 6,
    SW = 3
}

[CustomEditor(typeof(CompassCenter))]
public class CompassCenterEditor : Editor
{
    // Properties to change
    SerializedProperty s_radius;
    SerializedProperty s_rotRate;
    SerializedProperty s_strtPos;

    // Variables and constants for handling setting the start position
    private POSITIONS strtIndex;
    private readonly Vector2[] vectorPos = new Vector2[] {new Vector2(0, -1),   // S
                                                          new Vector2(0, 1),    // N
                                                          new Vector2(-1, 0),   // W
                                                          new Vector2(-1, -1),  // SW
                                                          new Vector2(-1, 1),   // NW
                                                          new Vector2(1, 0),    // E
                                                          new Vector2(1, -1),   // SE
                                                          new Vector2(1, 1)};   // NE

    protected virtual void OnEnable()
    {
        // Get the current properties of the values
        s_radius = serializedObject.FindProperty("compassRadius");
        s_rotRate = serializedObject.FindProperty("rotRate");
        s_strtPos = serializedObject.FindProperty("strtPos");

        // Calculate the index of s_strtPos in vectorPos
        switch (Mathf.RoundToInt(s_strtPos.vector2Value.x))
        {
            case 1:
                if (s_strtPos.vector2Value.y > 0) { strtIndex = POSITIONS.NE; }
                else if (s_strtPos.vector2Value.y < 0) { strtIndex = POSITIONS.SE; }
                else { strtIndex = POSITIONS.E; }
                break;
            case -1:
                if (s_strtPos.vector2Value.y > 0) { strtIndex = POSITIONS.NW; }
                else if (s_strtPos.vector2Value.y < 0) { strtIndex = POSITIONS.SW; }
                else { strtIndex = POSITIONS.W; }
                break;
            default:
                if (s_strtPos.vector2Value.y > 0) { strtIndex = POSITIONS.N; }
                else { strtIndex = POSITIONS.S; }
                break;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Set radius of compass, with minimum bound
        EditorGUI.BeginChangeCheck();
        float newRadius = EditorGUILayout.DelayedFloatField("Compass Radius", s_radius.floatValue);
        newRadius = (newRadius < 0) ? 0.0f : newRadius;
        if (EditorGUI.EndChangeCheck())
        {
            s_radius.floatValue = newRadius;
        }

        // Set rotation rate of compass center, with minimum bound
        EditorGUI.BeginChangeCheck();
        float newRotRate = EditorGUILayout.DelayedFloatField("Rotation Time (sec)", s_rotRate.floatValue);
        newRotRate = (newRotRate < 0) ? 0.0f : newRotRate;
        if (EditorGUI.EndChangeCheck())
        {
            s_rotRate.floatValue = newRotRate;
        }

        // Set up drop down menu for intial position
        EditorGUI.BeginChangeCheck();
        strtIndex = (POSITIONS)EditorGUILayout.EnumPopup("Wizard Start Position", strtIndex);
        if (EditorGUI.EndChangeCheck())
        {
            s_strtPos.vector2Value = vectorPos[(int)strtIndex];
        }

        serializedObject.ApplyModifiedProperties();
    }
}

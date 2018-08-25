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

    // Holds the positions that the wizard can be at on the cloud rail
    private Dictionary<Vector2, Quaternion> cardinalRot = new Dictionary<Vector2, Quaternion>();

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

        // Initialize the dictionary of cardinal position rotations
        cardinalRot.Add(new Vector2(0, 1), Quaternion.identity);     // position N
        cardinalRot.Add(new Vector2(1, 1), Quaternion.identity);     // position NE
        cardinalRot.Add(new Vector2(1, 0), Quaternion.identity);     // position E
        cardinalRot.Add(new Vector2(1, -1), Quaternion.identity);    // position SE
        cardinalRot.Add(new Vector2(0, -1), Quaternion.identity);    // position S
        cardinalRot.Add(new Vector2(-1, -1), Quaternion.identity);   // position SW
        cardinalRot.Add(new Vector2(-1, 0), Quaternion.identity);    // position W
        cardinalRot.Add(new Vector2(-1, 1), Quaternion.identity);    // position NW

        List<Vector2> cardRotIndex = new List<Vector2>(cardinalRot.Keys);
        Vector3 compCntr = ((CompassCenter)target).transform.position;

        for (int i = 0; i < cardRotIndex.Count; i++)
        {
            cardinalRot[cardRotIndex[i]] = Quaternion.AngleAxis(45f * i, Vector3.up);
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

    private void OnSceneGUI()
    {
        Transform compTransform = ((CompassCenter)target).transform;

        // Create handle to adust the compass radius
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        float newRadius = Handles.ScaleSlider
            (
                s_radius.floatValue, 
                compTransform.position, 
                -compTransform.up, 
                Quaternion.identity,
                HandleUtility.GetHandleSize(compTransform.position),
                0.1f
            );
        if(EditorGUI.EndChangeCheck())
        {
            s_radius.floatValue = (newRadius < 0) ? 0 : newRadius;
        }
        serializedObject.ApplyModifiedProperties();

        // Draw markers to provide context for the different variables
        if (Event.current.type == EventType.Repaint)
        {
            // Create a circle with radius of the compass
            Handles.CircleHandleCap
                (
                    0,
                    compTransform.position,
                    compTransform.rotation,
                    s_radius.floatValue,
                    EventType.repaint
                );

            // Create marker to denote the initial wizard starting position
            Vector3 compPos = compTransform.position;
            Vector3 markerPos = cardinalRot[s_strtPos.vector2Value] * new Vector3(compPos.x, compPos.y, compPos.z + s_radius.floatValue);
            //Handles.color = Color.yellow;
            Handles.CubeHandleCap(0, markerPos, Quaternion.identity, 0.4f, EventType.Repaint);
            Handles.Label(markerPos, "Wizard Start\nDip");
        }
    }
}

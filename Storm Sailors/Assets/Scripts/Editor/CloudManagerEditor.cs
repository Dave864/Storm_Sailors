using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CloudManager))]
public class CloudManagerEditor : Editor
{
    // Properties to change
    SerializedProperty s_maxCloudCnt;
    SerializedProperty s_dipValue;
    SerializedProperty s_thunderheadPrefab;

    // Reference for target object transform
    Transform targT;
    float compassRad;

    private void OnEnable()
    {
        // Get the transform of the target object
        targT = ((CloudManager)target).transform;
        compassRad = GameObject.Find("Compass Center").GetComponent<CompassCenter>().compassRadius;

        // Get the current values of the properties
        s_maxCloudCnt = serializedObject.FindProperty("maxCloudCnt");
        s_dipValue = serializedObject.FindProperty("dipVal");
        s_thunderheadPrefab = serializedObject.FindProperty("thunderheadPrefab");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Cloud Information");

        // Set the maximum cloud count
        EditorGUI.BeginChangeCheck();
        int newMax = EditorGUILayout.IntSlider("Cloud Spawn Limit", s_maxCloudCnt.intValue, 1, 8);
        if(EditorGUI.EndChangeCheck())
        {
            s_maxCloudCnt.intValue = newMax;
        }

        // Set the dip value of the thunderhead
        EditorGUI.BeginChangeCheck();
        float newDip = EditorGUILayout.FloatField("Y-offset below compass", s_dipValue.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            s_dipValue.floatValue = (newDip < 0) ? 0 : newDip;
        }

        EditorGUILayout.EndVertical();

        // Set the reference to the thunderhead prefab
        EditorGUILayout.PropertyField(s_thunderheadPrefab);

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        Vector3 onCompassPos = new Vector3(targT.position.x, targT.position.y, targT.position.z + compassRad);
        Vector3 dipCompassPos = new Vector3(targT.position.x, targT.position.y - s_dipValue.floatValue, targT.position.z + compassRad);
        float cubeSize = 0.25f;

        // Draw markers to provide context for different values
        if (Event.current.type == EventType.Repaint)
        {
            // Create marker to denote the radius of the compass
            Handles.CircleHandleCap
                (
                    0,
                    targT.position,
                    targT.rotation,
                    compassRad,
                    EventType.Repaint
                );

            // Create marker for point on compass at dip value of zero
            Handles.CubeHandleCap(0, onCompassPos, Quaternion.identity, cubeSize, EventType.Repaint);

            // Create marker for point on compass at dip value
            Handles.CubeHandleCap(1, dipCompassPos, Quaternion.identity, cubeSize, EventType.Repaint);
            Handles.Label(dipCompassPos, "Cloud Spawn\nDip");

            // Draw a line between the two points
            Handles.DrawLine(onCompassPos, dipCompassPos);
        }
    }
}

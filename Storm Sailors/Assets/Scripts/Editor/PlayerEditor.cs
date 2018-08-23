using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    // Properties to change
    SerializedProperty s_strtPos;

    protected virtual void OnEnable()
    {
        s_strtPos = serializedObject.FindProperty("startPos");
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();

        // Create a handle to adjust the start position of the player
        EditorGUI.BeginChangeCheck();
        Vector3 newStrtPos = Handles.PositionHandle(s_strtPos.vector3Value, Quaternion.identity);
        Handles.Label(new Vector3(newStrtPos.x, 0, newStrtPos.z), "Player Starting Position");
        if(EditorGUI.EndChangeCheck())
        {
            s_strtPos.vector3Value = new Vector3(newStrtPos.x, 0, newStrtPos.z);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

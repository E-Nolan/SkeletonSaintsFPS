using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI))]
public class FieldOfViewEditor : Editor
{
    // To show the cone of view of the Enemy in only the Editor
    private void OnSceneGUI()
    {
        // Drawing a circle around the player for testing if the angle math is correct
        EnemyAI fov = (EnemyAI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);

        // Get the angle from DirectionFromAngle() to get right and left angle from the Enemy's transform 
        Vector3 leftViewAngle = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.ViewAngle / 2);
        Vector3 rightViewAngle = DirectionFromAngle(fov.transform.eulerAngles.y, fov.ViewAngle / 2);

        // Draw lines showing the angles returned from DirectionFromAngle() to show the Enemy's ViewRadius
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + leftViewAngle * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + rightViewAngle * fov.ViewRadius);

        // If Enemy can see Player, draw a green line to the Player to visualize detection in Editor
        if (fov.CanSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.PlayerGameObject.transform.position);
        }
    }

    /// <summary>
    /// Returns the angle given a Transform's eulerAngles.y and the (ViewAngle / 2)
    /// </summary>
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        // Use Sin and Cos to return the x and z floats of the angle in radians
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

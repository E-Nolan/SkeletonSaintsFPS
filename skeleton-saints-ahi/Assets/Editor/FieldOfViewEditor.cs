using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyAI fov = (EnemyAI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);

        Vector3 leftViewAngle = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.ViewAngle / 2);
        Vector3 rightViewAngle = DirectionFromAngle(fov.transform.eulerAngles.y, fov.ViewAngle / 2);

        Handles.color = Color.yellow;

        Handles.DrawLine(fov.transform.position, fov.transform.position + leftViewAngle * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + rightViewAngle * fov.ViewRadius);

        if (fov.CanSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.PlayerGameObject.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

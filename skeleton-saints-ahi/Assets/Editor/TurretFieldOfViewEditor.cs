using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Turret))]
public class TurretFieldOfViewEditor : Editor
{
    // To show the cone of view of the Enemy in only the Editor
    private void OnSceneGUI()
    {
        Turret fov = (Turret)target;

        // Draw circle around player to display ViewRadius
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);

        // Draw circle around player to display SprintDetectRadius
        Handles.color = Color.blue;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.SprintDetectRadius);

        // Draw circle around player to display WalkDetectRadius
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.WalkDetectRadius);

        // Draw circle around player to display ShootDetectRadius
        Handles.color = Color.magenta;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ShootDetectRadius);

        // Get the angle from DirectionFromAngle() to get right and left angle from the Enemy's transform 
        // for DrawLine(s) below
        Vector3 leftViewAngle = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.ViewAngle / 2);
        Vector3 rightViewAngle = DirectionFromAngle(fov.transform.eulerAngles.y, fov.ViewAngle / 2);
        Vector3 leftShootAngle = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.FireAngle / 2);
        Vector3 rightShootAngle = DirectionFromAngle(fov.transform.eulerAngles.y, fov.FireAngle / 4);

        // Draw lines showing the angles returned from DirectionFromAngle() to show the Enemy's ViewAngle
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + leftViewAngle * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + rightViewAngle * fov.ViewRadius);

        // Draw lines showing the angles returned from DirectionFromAngle() to show the Enemy's ShootAngle
        Handles.color = Color.black;
        Handles.DrawLine(fov.transform.position, fov.transform.position + leftShootAngle * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + rightShootAngle * fov.ViewRadius);

        // If Enemy can see Player, draw a green line to the Player to visualize detection in Editor
        if (fov.CanDetectPlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
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

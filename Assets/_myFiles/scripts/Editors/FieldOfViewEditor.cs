using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.GetRadiusHostile());
        
        Vector3 viewAngleHostile1 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.GetAngle() / 2);
        Vector3 viewAngleHostile2 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.GetAngle() / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleHostile1 * fov.GetRadiusSuspicious());
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleHostile2 * fov.GetRadiusSuspicious());

        Handles.color = Color.blue;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.GetRadiusSuspicious());

        if (fov.GetCanSeePlayer())
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(fov.transform.position, GameManager.m_Instance.GetPlayer().transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float anglesInDegrees)
    {
        anglesInDegrees += eulerY;

        return new Vector3(Mathf.Sin(anglesInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(anglesInDegrees * Mathf.Deg2Rad));
    }
}

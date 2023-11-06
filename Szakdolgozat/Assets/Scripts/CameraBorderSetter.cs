using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorderSetter : MonoBehaviour
{
    public PolygonCollider2D cameraBorder;
    public CinemachineConfiner confiner;
    public void Start()
    {
        cameraBorder.transform.position = new Vector3(-0.16f, -0.16f, 0);

        Vector2[] borderPoints = new Vector2[4];
        borderPoints[0] = new Vector2(0, 0);
        borderPoints[1] = new Vector2(InputTextHandler.mapSize * 0.32f, 0);
        borderPoints[2] = new Vector2(InputTextHandler.mapSize * 0.32f, 40);
        borderPoints[3] = new Vector2(0, 40);

        cameraBorder.points = borderPoints;
        cameraBorder.SetPath(0, borderPoints);
        cameraBorder.transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        cameraBorder.isTrigger = true;
        confiner.m_BoundingShape2D = cameraBorder;
        confiner.m_ConfineScreenEdges = true;
    }
}

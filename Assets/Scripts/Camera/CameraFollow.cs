using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class CameraFollow : MonoBehaviour
{
    private Transform minPoint;
    private Transform maxPoint;
    private Transform targetPoint;
    [SerializeField] private float XMax;
    [SerializeField] private float YMax;
    [SerializeField] private float XMin;
    [SerializeField] private float YMin;
    [SerializeField] private float Z;
    private Vector3 TargetPosition;
    [SerializeField] private float cameraSpeed;

    void Start()
    {
        minPoint = GameObject.Find("CameraMin").transform;
        maxPoint = GameObject.Find("CameraMax").transform;
        targetPoint = GameObject.Find("Player").transform;
        cameraSpeed = GameManager.Instance.cameraSpeed;
        if(minPoint != null)
        {
            XMin = minPoint.position.x;
            YMin = minPoint.position.y;
            Z = minPoint.position.z;
        }
        else
        {
            Debug.LogError("minPoint is null");
        }
        if(maxPoint != null)
        {
            XMax = maxPoint.position.x;
            YMax = maxPoint.position.y;
        }
        else
        {
            Debug.LogError("maxPoint is null");
        }
    }

    private void FixedUpdate()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        TargetPosition = targetPoint.position;
        TargetPosition.z = Z;
        TargetPosition.x = TargetPosition.x > XMax ? XMax : TargetPosition.x;
        TargetPosition.x = TargetPosition.x < XMin ? XMin : TargetPosition.x;
        TargetPosition.y = TargetPosition.y > YMax ? YMax : TargetPosition.y;
        TargetPosition.y = TargetPosition.y < YMin ? YMin : TargetPosition.y;
        //if ((m_camera.position - TargetPosition).x > 0.1f) 
        transform.position = Vector3.Lerp(transform.position,TargetPosition,cameraSpeed);
    }
}

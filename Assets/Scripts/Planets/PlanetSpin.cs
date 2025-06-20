using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class PlanetSpin : MonoBehaviour
{
    [SerializeField] protected float spinSpeed;
    void Start()
    {
        spinSpeed = GameManager.Instance.spinSpeed;
    }
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * spinSpeed * Time.fixedDeltaTime);
    }
}

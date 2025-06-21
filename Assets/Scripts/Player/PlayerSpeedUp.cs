using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class PlayerSpeedUp : MonoBehaviour
{
    public void SpeedUp()
    {
        GameManager.Instance.moveSpeed *= 2;
        Invoke("ResetSpeed", 0.5f);
    }

    public void ResetSpeed()
    {
        GameManager.Instance.moveSpeed /= 2;
    }
}

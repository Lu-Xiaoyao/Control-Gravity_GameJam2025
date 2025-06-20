using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StarPlace : MonoBehaviour
{
    public bool puzzleSolved = false;
    public UnityEvent onPuzzleSolved;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.transform.parent == transform.parent && !puzzleSolved)
        {
            puzzleSolved = true;
            onPuzzleSolved.Invoke();
        }
    }
}

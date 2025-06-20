using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] private List<StarPlace> requiredStars = new List<StarPlace>(); //需要解决的谜题，在面板手动设置
    [SerializeField] protected internal bool doorOpen = false;
    [SerializeField] private int puzzleSolvedRequired = 0;
    [SerializeField] private int puzzleSolvedCount = 0;
    void Start()
    {
        puzzleSolvedRequired = requiredStars.Count;
        puzzleSolvedCount = 0;
        foreach(StarPlace star in requiredStars)
        {
            star.onPuzzleSolved.AddListener(AddPuzzleSolved);
        }
    }

    void AddPuzzleSolved()
    {
        puzzleSolvedCount++;
        if(puzzleSolvedCount >= puzzleSolvedRequired)
        {
            doorOpen = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && doorOpen)
        {
            Invoke("NextLevel", 1f);
        }
    }

    void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

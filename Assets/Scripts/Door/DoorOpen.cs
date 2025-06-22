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
        if(requiredStars.Count == 0)
        {
            doorOpen = true;
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
        }
    }

    void AddPuzzleSolved()
    {
        puzzleSolvedCount++;
        if(puzzleSolvedCount >= puzzleSolvedRequired)
        {
            doorOpen = true;
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && doorOpen)
        {
            Invoke("NextLevel", 1f);
        }
        else if(other.gameObject.CompareTag("Player") && !doorOpen)
        {
            Debug.Log("Door is closed");
        }
    }

    void NextLevel()
    {
        AllControl.GameManager.Instance.levelComplete++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;
using UnityEngine.SceneManagement;

public class LoginButton : MonoBehaviour
{
    [SerializeField] private AudioSource buttonClick;
    void Start()
    {
        buttonClick = GameObject.Find("Audio/ClickAudio").GetComponent<AudioSource>();
    }
    public void OnNewGame()
    {
        SceneManager.LoadScene("LevelChoose");
        GameManager.Instance.levelComplete = 0;
        GameManager.Instance.deathCount = 0;
        buttonClick.Play();
    }

    public void OnLoadGame()
    {
        SceneManager.LoadScene("LevelChoose");
        buttonClick.Play();
    }

    public void OnExit()
    {
        Application.Quit();
        buttonClick.Play();
    }

    public void OnBack()
    {
        SceneManager.LoadScene("Login");
        buttonClick.Play();
    }
}

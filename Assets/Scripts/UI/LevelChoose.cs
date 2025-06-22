using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelChoose : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    private Image image;
    private Button button;
    private int levelIndex = 0;

    void Start()
    {
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        levelIndex = int.Parse(levelText.text);
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        
        // 设置Button的禁用颜色为不透明的深色
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(106f/255f, 106f/255f, 106f/255f, 1f); // 不透明的深灰色
            button.colors = colors;
        }
    }

    void FixedUpdate()
    {
        bool isUnlocked = levelIndex <= GameManager.Instance.levelComplete + 1;
        
        // 设置按钮的交互性
        if (button != null)
        {
            button.interactable = isUnlocked;
        }
        
        // 如果需要自定义颜色，可以这样设置
        if (isUnlocked)
        {
            image.color = new Color(255f, 255f, 255f, 1f);
        }
        else
        {
            image.color = new Color(106f, 106f, 106f, 1f);
        }
    }

    public void OnClickLevel()
    {
        if(levelIndex <= GameManager.Instance.levelComplete + 1)
        {
            SceneManager.LoadScene("Level" + levelIndex);
        }
    }
}

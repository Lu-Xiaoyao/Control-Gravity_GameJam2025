using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFlicker : MonoBehaviour
{
    // ������Inspector��������˸�ٶȣ���ֵԽ��͸���ȱ仯Խ��
    public float flickerSpeed = 1.0f;

    // ������Inspector������͸���ȷ�Χ��min�����max��������0~1��
    public float minAlpha = 0.2f;
    public float maxAlpha = 1.0f;

    // �洢Image�����������͸����
    private UnityEngine.UI.Image starImage;

    // Ŀ��͸���ȣ�ÿ������仯��
    private float targetAlpha;

    void Start()
    {
        // ��ȡImage����������Sprite������SpriteRenderer��
        starImage = GetComponent<UnityEngine.UI.Image>();

        // ��ʼ�����͸����Ŀ��
        targetAlpha = Random.Range(minAlpha, maxAlpha);
    }

    void Update()
    {
        // 1. ƽ�����ɵ�Ŀ��͸���ȣ�����˸����Ȼ��
        Color currentColor = starImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * flickerSpeed);
        starImage.color = currentColor;

        // 2. �ӽ�Ŀ��ʱ�������Ŀ��͸���ȣ�ʵ�֡�������������
        if (Mathf.Abs(currentColor.a - targetAlpha) < 0.01f)
        {
            targetAlpha = Random.Range(minAlpha, maxAlpha);
        }
    }
}

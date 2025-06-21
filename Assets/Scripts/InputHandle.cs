using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入处理，负责处理所有键盘输入事件
/// 没有处理的鼠标点击输入事件：
/// 1.星球身上的PlanetGravityShift脚本切换重力模式
/// 2.解谜道具中的star身上的StarReset脚本重置star位置
/// 在这里接收的键盘输入事件：
/// 1.玩家身上的AreaShow脚本显示重力区域
/// 2.玩家身上的ResetPlayer脚本重置玩家位置
/// 3.主相机身上的CameraSize脚本放大相机
/// </summary>
public class InputHandle : MonoBehaviour
{
    public static InputHandle Instance;
    private InputActions inputActions;
    [SerializeField] private AreaShow areaShow;  // 懒得Find了，记得拖拽赋值
    [SerializeField] private ResetPlayer resetPlayer;
    [SerializeField] private CameraSize cameraSize;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        inputActions = new InputActions();
        inputActions.player.Enable();
    }

    void Update()
    {
        if(inputActions.player.ShowArea.triggered)
        {
            areaShow.OnAreaShow();
        }
        if(inputActions.player.ResetPosition.triggered)
        {
            resetPlayer.ResetPosition();
        }
        if(inputActions.player.CameraEnlarge.triggered)
        {
            cameraSize.CameraEnlarge();
        }
    }
}

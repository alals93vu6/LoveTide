using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDetected : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] public bool isYuka;
    void Update()
    {
        CalculateDistanceToMouse();
        YukaDetected();
    }

    private void CalculateDistanceToMouse()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mouseScreenPosition = Input.mousePosition;
        
        // 将鼠标位置转换为世界坐标
        mouseScreenPosition.z = Camera.main.nearClipPlane; // 设置 z 轴距离为摄像机的 nearClipPlane
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        
        // 计算当前物体位置和鼠标位置之间的距离
        distance = Vector3.Distance(transform.position, mouseWorldPosition);
    }

    private void YukaDetected()
    {
        if (distance <= 50)
        {
            isYuka = true;
        }
        else
        {
            isYuka = false;
        }
    }
}

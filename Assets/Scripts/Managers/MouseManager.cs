using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// [System.Serializable]  // 显示在 Inspector 窗口
// public class EventsVector3 : UnityEvent<Vector3> {}
public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;  // 生成单例模式

    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;  // 保存射线碰撞到物体的信息

    public event Action<Vector3> OnMouseClicked;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    // 设置鼠标贴图
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            // 切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        // GetMouseButtonDown 0 是鼠标左键
        if (Input.GetMouseButtonDown(1) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
        }
    }
}

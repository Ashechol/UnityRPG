using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// [System.Serializable]  // 显示在 Inspector 窗口
// public class EventsVector3 : UnityEvent<Vector3> {}
public class MouseManager : Singleton<MouseManager>  // 继承单例模式
{
    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;  // 保存射线碰撞到物体的信息

    public static event Action<Vector3> OnMouseClicked;
    public static event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        // DontDestroyOnLoad(this);
    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    // 设置鼠标贴图
    void SetCursorTexture()
    {
        // 2020版之前直接在update调用 camera.main 开销大
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
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
        }
    }
}

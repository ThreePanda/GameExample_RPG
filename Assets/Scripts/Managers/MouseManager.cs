using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 以下内容为---在Unity的Hierarchy中下挂Action事件，手动选择执行内容---的声明
 * using UnityEngine.Events;
 * [System.Serializable]
 * public class EventVector3 : UnityEvent<Vector3> {}
 */

public class MouseManager : MonoBehaviour
{
    //创建单例模式
    public static MouseManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            //如果检查发现已存在MouseManager的实例，遵从单例原则，删除当前创建的实例，使用之前已有的实例
        }
        else
        {
            Instance = this;
        }
    } 
    
    //接受摄像机发出的射线的撞击信息
    private RaycastHit _hitinfo;
    public Texture2D point, doorway, attack, target, arrow;
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //根据鼠标靠近的物体类型切换鼠标指针的贴图
        if (Physics.Raycast(ray, out _hitinfo)) 
        {
            //切换鼠标贴图
            switch (_hitinfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(point,new Vector2(0,0),CursorMode.Auto);
                    break;
            }
        }
    }
    //范型委托——事件
    public event Action<Vector3> OnMouseClicked;
    //System下的event关键字，声明Action类，传入Vector3 
    //其他的函数可以注册到Action的类中，当该类的实例被Invoke后，会将传入变量依次传给已注册的函数，令其执行
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && _hitinfo.collider != null)
        {//鼠标左键被按下 && 点击位置存在Object
            if (_hitinfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(_hitinfo.point);
                //? : if the event is not NULL ,then start the next function
            }
        }
    }
    private void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
}
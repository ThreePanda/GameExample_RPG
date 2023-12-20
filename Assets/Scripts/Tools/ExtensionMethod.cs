using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
//Extension Method
//拓展方法必须在non-generic class（非范型）中，即static class，其对应方法也需为static
public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    //声明一个类的拓展，首先引入的this后的类型即为拓展的对象类
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();
        
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;
    }
}

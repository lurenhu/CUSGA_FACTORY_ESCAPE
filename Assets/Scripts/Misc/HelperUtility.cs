using System.Collections.Generic;
using UnityEngine;

public class HelperUtility
{
    /// <summary>
    /// 将向量转换为以x轴为起点的角度值
    /// </summary>
     public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degree = radians * Mathf.Rad2Deg;

        return degree;
    }

    /// <summary>
    /// 通过所给角度，获取单位方向向量
    /// </summary>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    /// <summary>
    /// 判断A列表中的值是否都与B列表中的值匹配
    /// </summary>
    /// <param name="A">浮动列表</param>
    /// <param name="B">固定列表</param>
    /// <param name="eps">浮动范围</param>
    public static bool CheckFloatList(List<float> A, List<float> B, float eps)
    {
        Dictionary<float,float> checkMatch = new Dictionary<float, float>();

        for (int i = 0; i < A.Count; i++)
        {
            for (int j = 0; j < B.Count; j++)
            {
                if (Mathf.Abs(A[i] - B[j]) < eps && !checkMatch.ContainsKey(B[j]))
                {
                    checkMatch[B[j]] = A[i];
                }
            }
        }

        if (checkMatch.Count == B.Count)
            return true;
        else
            return false;
    } 
}

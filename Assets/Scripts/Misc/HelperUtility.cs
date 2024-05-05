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
                if (B[j] > -180 + eps && B[j] < 180 - eps)
                    if (Mathf.Abs(A[i] - B[j]) < eps && !checkMatch.ContainsKey(B[j]))
                    {
                        checkMatch[B[j]] = A[i];
                        continue;
                    }
                else if (B[j] >= 180 - eps)
                    if ((A[i] > B[j] -eps || A[i] < B[j] + eps - 360) && !checkMatch.ContainsKey(B[j]))
                    {
                        checkMatch[B[j]] = A[i];
                        continue;
                    }
                else if (B[j] <= -180 + eps)
                    if ((A[i] < B[j] + eps || A[i] > B[j] - eps + 360) && !checkMatch.ContainsKey(B[j]))
                    {
                        checkMatch[B[j]] = A[i];
                        continue;
                    }
            }
        }

        if (checkMatch.Count == B.Count)
            return true;
        else
            return false;
    } 

    /// <summary>
    /// 将屏幕坐标转化为世界坐标
    /// </summary>
    public static Vector3 TranslateScreenToWorld(Vector3 Position)
    {
        Vector3 cameraTranslatePos = Camera.main.ScreenToWorldPoint(Position);
        return new Vector3 (cameraTranslatePos.x,cameraTranslatePos.y,0);
    }

    /// <summary>
    /// 将世界坐标转化为屏幕坐标
    /// </summary>
    public static Vector3 TranslateWorldToScreen(Vector3 Position)
    {
        Vector3 cameraTranslatePos = Camera.main.WorldToScreenPoint(Position);
        return new Vector3 (cameraTranslatePos.x,cameraTranslatePos.y,0);
    }
}

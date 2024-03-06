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
}

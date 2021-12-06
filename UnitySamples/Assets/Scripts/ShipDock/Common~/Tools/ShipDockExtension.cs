using System.Collections.Generic;
using System.Text;
using UnityEngine;

static public class ShipDockExtension
{
    /// <summary>颜色参数所需长度</summary>
    private const int COLOR_PARAM_LEN = 3;
    /// <summary>颜色参数最大值</summary>
    private const int COLOR_MAX = 255;

    /// <summary>字符串创建器</summary>
    private static StringBuilder mBuilder;
    /// <summary>相机射线临时引用</summary>
    private static Ray rayForMainCamera;
    /// <summary>相机变换对象引用</summary>
    private static Transform cameraTF;

    /// <summary>
    /// 字符串拼接
    /// </summary>
    /// <param name="target">启示字符串</param>
    /// <param name="args">即将拼接的字符串数组</param>
    /// <returns></returns>
    public static string Append(this string target, params string[] args)
    {
        if (mBuilder == null)
        {
            mBuilder = new StringBuilder();
        }
        else { }

        mBuilder.Length = 0;
        mBuilder.Append(target);

        int max = args.Length;
        string temp;
        for (int i = 0; i < max; i++)
        {
            temp = args[i];
            mBuilder.Append(temp);
        }
        return mBuilder.ToString();
    }

    /// <summary>
    /// 将数组转换为指定字符串标识分隔的字符串
    /// </summary>
    /// <param name="target">目标字符串数组</param>
    /// <param name="symbol">用于分隔的字符串标识</param>
    /// <returns></returns>
    public static string Joins<T>(this T[] target, string symbol = ",")
    {
        string connector;
        string result = string.Empty;
        int max = target.Length;
        T temp;
        for (int i = 0; i < max; i++)
        {
            connector = (i == max - 1) ? string.Empty : symbol;
            temp = target[i];
            result = result.Append(temp.ToString(), connector);
        }
        return result;
    }

    /// <summary>
    /// 将列表转换为指定字符串标识分隔的字符串
    /// </summary>
    /// <param name="target">目标字符串数组</param>
    /// <param name="symbol">用于分隔的字符串标识</param>
    /// <returns></returns>
    public static string Joins<T>(this List<T> target, string symbol = ",")
    {
        string connector;
        string result = string.Empty;
        int max = target.Count;
        T temp;
        for (int i = 0; i < max; i++)
        {
            connector = (i == max - 1) ? string.Empty : symbol;
            temp = target[i];
            result = result.Append(temp.ToString(), connector);
        }
        return result;
    }

    public static List<T> Contact<T>(this List<T> target, List<T> list)
    {
        int max = (list != default) ? list.Count : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target;
    }
    
    public static List<T> Contact<T>(this List<T> target, T[] list)
    {
        int max = (list != default) ? list.Length : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target;
    }

    public static T[] ContactToArr<T>(this List<T> target, List<T> list)
    {
        int max = (list != default) ? list.Count : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target.ToArray();
    }

    public static T[] ContactToArr<T>(this List<T> target, T[] list)
    {
        int max = (list != default) ? list.Length : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target.ToArray();
    }

    public static void ContactToArr<T>(this T[] target, T[] list, out T[] result)
    {
        result = default;
        if (list == default || list.Length < 0)
        {
            return;
        }
        else { }

        int oldLen = target.Length;
        int contactLen = list.Length;
        int max = oldLen + contactLen;

        result = new T[max];
        for (int i = 0; i < max; i++)
        {
            if(i < oldLen)
            {
                result[i] = target[i];
            }
            else if(i >= oldLen && i < max)
            {
                result[i] = list[i - oldLen];
            }
            else { }
        }
    }

    public static Material GetMaterial(this Renderer target, bool isGetShareMat = true, bool isCheckMultMat = false, int index = -1)
    {
        if (isCheckMultMat && index >= 0)
        {
            Material[] list;
            if (isGetShareMat)
            {
                list = target.sharedMaterials;
                return list[index];
            }
            else
            {
                list = target.materials;
                return list[index];
            }
        }
        else
        {
            return isGetShareMat? target.sharedMaterial: target.material;
        }
    }

    public static void ResetMain(this Camera target)
    {
        cameraTF = default;
    }

    public static bool CameraRaycast(this Camera target, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask)
    {
        if (cameraTF == null)
        {
            cameraTF = Camera.main.transform;
        }
        else { }

        rayForMainCamera = new Ray(cameraTF.position, direction);
        bool result = Physics.Raycast(rayForMainCamera, out hitInfo, distance, layerMask);
#if UNITY_EDITOR
        Debug.DrawRay(cameraTF.position, direction, Color.yellow);
#endif
        return result;
    }

    public static Color SetAlpha(this Color target, float a = 1f)
    {
        return (target.a == a) ? target : new Color(target.r, target.g, target.b, a);
    }

    public static void SetChildOf(this GameObject target, Transform parent, bool isRotationReset = false)
    {
        SetChildOf(target.transform, parent, isRotationReset);
    }

    public static void SetChildOf(this Transform target, Transform parent, bool isRotationReset = false)
    {
        target.SetParent(parent);
        target.localPosition = Vector3.zero;
        target.localScale = Vector3.one;

        if (isRotationReset)
        {
            target.localRotation = Quaternion.identity;
        }
        else { }
    }

    public static Color ToColor(this int[] target)
    {
        if (target.Length < COLOR_PARAM_LEN)
        {
            return Color.white;
        }
        else { }

        float a = target.Length > 3 ? target[3] / COLOR_MAX : 1f;
        return new Color(target[0] / COLOR_MAX, target[1] / COLOR_MAX, target[2] / COLOR_MAX, a);
    }
}
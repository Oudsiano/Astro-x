using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static Vector3 WithX(this Vector3 value, float x)
        {
            value.x = x;
            return value;
        }
        
        public static Vector3 AddY(this Vector3 value, float y)
        {
            value.y += y;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAnchoredYPosition(this RectTransform rectTransform, float yPosition)
        {
            var position = rectTransform.anchoredPosition;
            position.y = yPosition;
            rectTransform.anchoredPosition = position;
        }

        public static void AddYPosition(this Transform transform, float y)
        {
            transform.position = transform.position.AddY(y);
        }

        public static void MoveToLocalZero(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static T GetRandom<T>(this T[] array)
        {
            if (array.Length == 0) return default;
            return array[Random.Range(0, array.Length)];
        }

        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }
    }
}
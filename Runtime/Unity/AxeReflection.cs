#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using Unity.Burst;
using UnityEditor;
using UnityEngine;

namespace AxeEngine.Editor
{
    [BurstCompile]
    public static class AxeReflection
    {
        public static Type[] GetAllAxeTypes()
        {
            return TypeCache.GetTypesWithAttribute<AxeProperty>().Where(t => t.IsPublic).ToArray();
        }

        public static FieldInfo[] GetAllFields(this Type type) => type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        public static bool IsSupportedType(this Type type) => type.IsBool() || type.IsInteger() || type.IsFloat() || type.IsString() || type.IsVector2() ||
                                                              type.IsVector3() || type.IsVector4() || type.IsColor() || type.IsCurve() || type.IsUnityObject();

        public static bool IsBool(this Type type) => type == typeof(bool);
        public static bool IsInteger(this Type type) => type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong);
        public static bool IsFloat(this Type type) => type == typeof(float) || type == typeof(double);
        public static bool IsString(this Type type) => type == typeof(string);
        public static bool IsUnityObject(this Type type) => InheritsFrom(type, typeof(MonoBehaviour)) || InheritsFrom(type, typeof(UnityEngine.Object)) || InheritsFrom(type, typeof(ScriptableObject));
        public static bool IsVector2(this Type type) => type == typeof(Vector2);
        public static bool IsVector3(this Type type) => type == typeof(Vector3);
        public static bool IsVector4(this Type type) => type == typeof(Vector4) || type == typeof(Quaternion);
        public static bool IsColor(this Type type) => type == typeof(Color) || type == typeof(Color32);
        public static bool IsCurve(this Type type) => type == typeof(AnimationCurve);
        public static bool IsArray(this Type type) => type.IsArray || type.GenericTypeArguments.Length == 1;

        private static bool InheritsFrom(this Type type, Type baseType)
        {
            // null does not have base type
            if (type == null)
            {
                return false;
            }

            // only interface or object can have null base type
            if (baseType == null)
            {
                return type.IsInterface || type == typeof(object);
            }

            // check implemented interfaces
            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            // check all base types
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }
    }
}
#endif
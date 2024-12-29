using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AxeEngine.Editor
{
    public static class AxeReflection
    {
        public static List<Type> GetAllAxeTypes()
        {
            List<Type> data = new List<Type>();
            foreach (var assembly in GetAvailableAssemblies())
            {
                data.AddRange(assembly
                    .GetTypes()
                    .Where(myType => myType.IsPublic && myType.GetCustomAttribute<AxeProperty>() != null).ToList());
            }

            return data;
        }

        public static FieldInfo[] GetAllFields(this Type type) => type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        public static bool IsSupportedType(this Type type) => type.IsBool() || type.IsInteger() || type.IsFloat() || type.IsString() || type.IsVector2() ||
                                                              type.IsVector3() || type.IsVector4() || type.IsColor() || type.IsCurve() || type.IsUnityObject();

        public static bool IsBool(this Type type) => type == typeof(bool);
        public static bool IsInteger(this Type type) => type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong);
        public static bool IsFloat(this Type type) => type == typeof(float) || type == typeof(double);
        public static bool IsString(this Type type) => type == typeof(string);
        public static bool IsUnityObject(this Type type) => type.BaseType == typeof(UnityEngine.Object) || type.BaseType == typeof(ScriptableObject);
        public static bool IsVector2(this Type type) => type == typeof(Vector2);
        public static bool IsVector3(this Type type) => type == typeof(Vector3);
        public static bool IsVector4(this Type type) => type == typeof(Vector4) || type == typeof(Quaternion);
        public static bool IsColor(this Type type) => type == typeof(Color) || type == typeof(Color32);
        public static bool IsCurve(this Type type) => type == typeof(AnimationCurve);
        public static bool IsArray(this Type type) => type.IsArray || type.GenericTypeArguments.Length == 1;

        private static IEnumerable<Assembly> GetAvailableAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
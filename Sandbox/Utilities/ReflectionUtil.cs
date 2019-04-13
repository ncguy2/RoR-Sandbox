using System.Reflection;

// ReSharper disable UnusedMember.Global

namespace Sandbox.Utilities {
    public static class ReflectionUtil {
        public static void SetPrivateField(this object obj, string fieldName, object value) {
            FieldInfo prop = obj.GetType()
                                .GetField(fieldName,
                                          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop?.SetValue(obj, value);
        }

        public static T GetPrivateField<T>(this object obj, string fieldName) {
            FieldInfo prop = obj.GetType()
                                .GetField(fieldName,
                                          BindingFlags.NonPublic | BindingFlags.Instance);
            return (T) prop?.GetValue(obj);
        }

        public static void SetPrivateProperty(this object obj, string propertyName, object value) {
            PropertyInfo prop = obj.GetType()
                                   .GetProperty(propertyName,
                                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop?.SetValue(obj, value, null);
        }

        public static void InvokePrivateMethod(this object obj, string methodName, params object[] methodParams) {
            MethodInfo dynMethod = obj.GetType()
                                      .GetMethod(
                                          methodName,
                                          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            dynMethod?.Invoke(obj, methodParams);
        }
    }
}
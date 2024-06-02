using System.Reflection;

namespace BackendTripRecruitmentTask.UnitTests.Helpers;

public static class PropertyHelper
{
    public static void SetProperty<T>(T obj, string propertyName, object value)
    {
        var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo == null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'.");

        if (!propertyInfo.CanWrite)
            throw new ArgumentException($"Property '{propertyName}' on type '{typeof(T).Name}' is read-only.");

        propertyInfo.SetValue(obj, value);
    }
}
using System;
using System.Globalization;
using System.Reflection;

namespace AwesomeAssertions.Specs;

public static class RandomDataExtensions
{
    public static void InitializeWithRandomData(this object sut)
    {
        foreach (PropertyInfo propertyInfo in sut.GetType().GetProperties())
        {
            if (!propertyInfo.CanWrite)
            {
                continue;
            }

            if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(sut, new Random().Next(100).ToString(CultureInfo.InvariantCulture));
                continue;
            }

            if (propertyInfo.PropertyType == typeof(bool))
            {
                propertyInfo.SetValue(sut, new Random().Next(2) == 0);
                continue;
            }

            propertyInfo.SetValue(sut, new Random().Next());
        }
    }
}

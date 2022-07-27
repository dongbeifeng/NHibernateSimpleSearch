// Copyright 2022 王建军
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using NHibernate.Mapping.ByCode;
using NHibernate.UserTypes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NHibernateUtils;

public static class MapperUtils
{
    /// <summary>
    /// 映射 <see cref="ICompositeUserType"/> 类型的属性。
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TCompositeUserType"></typeparam>
    /// <param name="propertyNames"></param>
    /// <returns></returns>
    public static Action<IPropertyMapper> MapPropertyOfCompositeUserType<TClass, TCompositeUserType>(params string[] propertyNames) where TCompositeUserType : ICompositeUserType
    {
        return prop =>
        {
            prop.Type<TCompositeUserType>();
            var columns = propertyNames.Select(propertyName => MapColumn<TClass>(propertyName)).ToArray();
            prop.Columns(columns);
        };
    }


    /// <summary>
    /// 映射 <see cref="ICompositeUserType"/> 类型的字典键。
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TCompositeUserType"></typeparam>
    /// <param name="propertyNames"></param>
    /// <returns></returns>
    public static Action<IMapKeyMapper> MapMapKeyOfCompositeUserType<TClass, TCompositeUserType>(params string[] propertyNames) where TCompositeUserType : ICompositeUserType
    {
        return key =>
        {
            key.Type<TCompositeUserType>();
            var columns = propertyNames.Select(propertyName => MapColumn<TClass>(propertyName)).ToArray();
            key.Columns(columns);
        };
    }

    /// <summary>
    /// 映射 <see cref="ICompositeUserType"/> 类型的元素。
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TCompositeUserType"></typeparam>
    /// <param name="propertyNames"></param>
    /// <returns></returns>
    public static Action<IElementMapper> MapElementOfCompositeUserType<TClass, TCompositeUserType>(params string[] propertyNames) where TCompositeUserType : ICompositeUserType
    {
        return element =>
        {
            element.Type<TCompositeUserType>();
            var columns = propertyNames.Select(propertyName => MapColumn<TClass>(propertyName)).ToArray();
            element.Columns(columns);
        };
    }


    internal static Action<IColumnMapper> MapColumn<TClass>(string propertyName)
    {
        var (notnull, length) = GetColumnAttributes(typeof(TClass), propertyName);

        return (c =>
        {
            c.Name(propertyName);

            if (notnull)
            {
                c.NotNullable(true);
            }

            if (length.HasValue)
            {
                c.Length(length.Value);
            }
        });
    }


    internal static (bool notnull, int? length) GetColumnAttributes(Type type, string propertyName)
    {
        PropertyInfo? p = type.GetProperty(propertyName);
        if (p is null)
        {
            throw new();
        }

        var notnull = IsValueTypeAndNotNullable(p.PropertyType) || p.IsDefined(typeof(RequiredAttribute), true);
        int? length = p.GetCustomAttribute<MaxLengthAttribute>(true)?.Length;

        return (notnull, length);
    }



    internal static bool IsValueTypeAndNotNullable(Type type)
    {
        if (type.IsValueType == false)
        {
            return false;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return false;
        }
        return true;
    }


}

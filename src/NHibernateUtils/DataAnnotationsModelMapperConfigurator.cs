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
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NHibernateUtils;

/// <summary>
/// 实现以下约定：
/// 如果属性是值类型且不是可空类型，则将数据库字段指定为 not null；
/// 如果属性或多对一关联上标记了 <see cref="RequiredAttribute"/>，则将数据库字段指定为 not null；
/// 如果属性上标记了 <see cref="MaxLengthAttribute"/>，则用它指定数据库字段的长度；
/// </summary>
public class DataAnnotationsModelMapperConfigurator : IModelMapperConfigurator
{
    public void Configure(ModelMapper mapper)
    {
        // 属性
        mapper.BeforeMapProperty += HandleBeforeMapProperty;
        // 多对一
        mapper.BeforeMapManyToOne += HandleBeforeMapManyToOne;
    }

    internal void HandleBeforeMapProperty(IModelInspector modelInspector, PropertyPath member, IPropertyMapper propertyCustomizer)
    {
        // 不可空值类型对应的列也不可空
        if (member.LocalMember is PropertyInfo p)
        {
            if (MapperUtils.IsValueTypeAndNotNullable(p.PropertyType))
            {
                propertyCustomizer.NotNullable(true);
            }
        }

        // RequiredAttribute 列不可空
        if (member.LocalMember.IsDefined(typeof(RequiredAttribute), true))
        {
            propertyCustomizer.NotNullable(true);
        }

        // 长度
        var attr = member.LocalMember.GetCustomAttribute<MaxLengthAttribute>(true);
        if (attr != null)
        {
            propertyCustomizer.Length(attr.Length);
        }
    }

    internal void HandleBeforeMapManyToOne(IModelInspector modelInspector, PropertyPath member, IManyToOneMapper propertyCustomizer)
    {
        // 应用 RequiredAttribute 列不可空
        if (member.LocalMember.IsDefined(typeof(RequiredAttribute), true))
        {
            propertyCustomizer.NotNullable(true);
        }
    }
}

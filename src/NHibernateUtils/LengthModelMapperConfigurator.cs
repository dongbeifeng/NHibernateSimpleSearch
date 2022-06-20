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
/// 如果属性上标记了 <see cref="MaxLengthAttribute"/>，则用它指定数据库字段的长度；
/// </summary>
public class LengthModelMapperConfigurator : IModelMapperConfigurator
{
    public void Configure(ModelMapper mapper)
    {
        // 属性
        mapper.BeforeMapProperty += HandleBeforeMapProperty;
    }

    internal void HandleBeforeMapProperty(IModelInspector modelInspector, PropertyPath member, IPropertyMapper propertyCustomizer)
    {
        // 长度
        var attr = member.LocalMember.GetCustomAttribute<MaxLengthAttribute>(true);
        if (attr != null)
        {
            propertyCustomizer.Length(attr.Length);
        }
    }
}

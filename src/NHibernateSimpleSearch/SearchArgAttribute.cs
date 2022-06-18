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

namespace NHibernateSimpleSearch;

/// <summary>
/// 用于标记查询参数。
/// </summary>
[System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class SearchArgAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public SearchArgAttribute(string sourceProperty, SearchMode seachMode = SearchMode.Auto)
    {
        SourceProperty = sourceProperty;
        SeachMode = seachMode;
    }

    public SearchArgAttribute(SearchMode seachMode = SearchMode.Auto)
    {
        SourceProperty = null;
        SeachMode = seachMode;
    }

    /// <summary>
    /// 获取查询的源属性。如果不指定，则使用与查询参数同名的源属性。
    /// </summary>
    public string? SourceProperty { get; }

    /// <summary>
    /// 获取查询方式。
    /// </summary>
    public SearchMode SeachMode { get; }
}

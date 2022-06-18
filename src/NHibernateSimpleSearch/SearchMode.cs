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
/// 表示查询方式。
/// </summary>
public enum SearchMode
{
    /// <summary>
    /// 根据查询参数类型自动确定查询方式。如果查询参数是字符串类型，则使用 <see cref="Like"/>，否则使用 <see cref="Equal"/>。
    /// </summary>
    Auto,

    /// <summary>
    /// 表示查询方式为：源属性 == 参数值。
    /// </summary>
    Equal,

    /// <summary>
    /// 表示查询方式为：源属性 != 参数值。
    /// </summary>
    NotEqual,

    /// <summary>
    /// 表示查询方式为：源属性 LIKE 参数值。
    /// </summary>
    Like,

    /// <summary>
    /// 表示查询方式为：源属性 NOT LIKE 参数值。
    /// </summary>
    NotLike,

    /// <summary>
    /// 表示查询方式为：源属性 > 参数值。
    /// </summary>
    GreaterThan,

    /// <summary>
    /// 表示查询方式为：源属性 >= 参数值。
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// 表示查询方式为：源属性 < 参数值。
    /// </summary>
    LessThan,

    /// <summary>
    /// 表示查询方式为：源属性 <= 参数值。
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// 表示查询方式为：源属性 IN 参数值。查询参数应为数组类型。
    /// </summary>
    In,


    /// <summary>
    /// 表示查询方式为：源属性 NOT IN 参数值。查询参数应为数组类型。
    /// </summary>
    NotIn,

    /// <summary>
    /// 表示查询方式是一个表达式，表达式属性名为 参数名 + Expr 后缀，
    /// 例如查询参数 LocationCode 的表达式属性名为 LocationCodeExpr。
    /// </summary>
    Expression,

}

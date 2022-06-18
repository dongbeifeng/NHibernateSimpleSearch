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

using System.Text.RegularExpressions;

namespace NHibernateSimpleSearch;

/// <summary>
/// 代替 <see cref="NHibernate.Linq.SqlMethods.Like(string, string)"/>，可以为在内存中运行的查询提供简单的模糊查找功能。
/// </summary>
public static class SqlMethods
{
    /// <summary>
    /// 代替 <see cref="NHibernate.Linq.SqlMethods.Like(string, string)"/>，提供内存里的简单模糊查询。
    /// </summary>
    /// <param name="input"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static bool Like(this string? input, string pattern)
    {
        if (input == null)
        {
            return false;
        }
        pattern = Regex.Escape(pattern)
            .Replace("*", ".*")
            .Replace("%", ".*")
            .Replace("_", ".");
        // pattern = Regex.Escape(pattern).Replace("*", ".*").Replace("?", ".");
        return Regex.IsMatch(input, pattern);
    }
}

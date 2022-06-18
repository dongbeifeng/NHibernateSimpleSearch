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

namespace NHibernateUtils;

/// <summary>
/// 表示分页的列表。页码基于 1。
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="CurrentPage">当前页码。</param>
/// <param name="List">本页数据。</param>
/// <param name="PageSize">页大小。</param>
/// <param name="Total">记录总数。</param>
public record PagedList<T>(List<T> List, int CurrentPage, int PageSize, int Total);

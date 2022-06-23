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

using NHibernate.Linq;

namespace NHibernateUtils;

public static class ChunkExtensions
{
    /// <summary>
    /// 将数据分块。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="nhQuery">NHibernate 查询对象</param>
    /// <param name="chunkSize"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<List<TSource>> ToChunksAsync<TSource>(this IQueryable<TSource> nhQuery, int chunkSize)
    {
        if (nhQuery is null)
        {
            throw new ArgumentNullException(nameof(nhQuery));
        }
        if (chunkSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "块大小不能小于 1。");
        }

        int chunkIndex = -1;
        while (true)
        {
            chunkIndex++;
            var chunk = await nhQuery.Skip(chunkIndex * chunkSize).Take(chunkSize).ToListAsync().ConfigureAwait(false);
            if (chunk.Count == 0)
            {
                yield break;
            }

            yield return chunk;

            if (chunk.Count < chunkSize)
            {
                yield break;
            }
        }
    }

    /// <summary>
    /// 按指定的块大小加载查询。
    /// 这个方法是为提升分配库存操作的数据库查询效率引入的。通常有远大于需求数量的库存数据，
    /// 如果全部读取到数据库，则大量数据用不到，造成浪费，如果逐条读取，则到数据库的往返次数过多，引起性能下降。
    /// 
    /// 这个方法允许程序以一定的块大小加载数据，从而提升性能。
    /// 调用方使用 foreach 循环遍历返回的枚举数，并指定块大小，在遇到 break 语句之前，
    /// 数据按块大小被加载进内存，遇到 break 语句后，后续的块不会加载。
    /// 以下代码以每块 10 个的大小加载数据，在遍历到第 25 个元素的时候停止，只会加载 3 个数据块，30 条数据：
    /// <code>
    ///    int count = 0;
    ///    var source = Enumerable.Range(1, 100).AsQueryable();
    ///    await foreach (var item in source.LoadInChunk(10))
    ///    {
    ///        count++;
    ///        if (count >= 25)
    ///        {
    ///            break;
    ///        }
    ///    }
    /// </code>
    /// 此方法是和 <see cref="INhQueryProvider"/> 接口绑定的
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="nhQuery">NHibernate 查询对象</param>
    /// <param name="chunkSize"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<TSource> LoadInChunksAsync<TSource>(this IQueryable<TSource> nhQuery, int chunkSize)
    {
        if (nhQuery is null)
        {
            throw new ArgumentNullException(nameof(nhQuery));
        }
        if (chunkSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "块大小不能小于 1。");
        }

        await foreach (var chunk in nhQuery.ToChunksAsync(chunkSize).ConfigureAwait(false))
        {
            foreach (var item in chunk)
            {
                yield return item;
            }
        }
    }

}

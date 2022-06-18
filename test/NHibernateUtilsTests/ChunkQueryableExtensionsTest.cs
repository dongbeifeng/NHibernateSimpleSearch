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

using Xunit;

namespace NHibernateUtils.Tests;

public class ChunkQueryableExtensionsTest
{
    class Foo
    {
        public int Index { get; set; }

        public int Quantity { get; set; }

        public bool IsRead { get; set; }
    }

    [Fact]
    public async Task ToChunksAsyncTest()
    {
        TestingQueryable<int> q = new TestingQueryable<int>(Enumerable.Range(1, 23));

        Queue<List<int>> chunks = new Queue<List<int>>();
        chunks.Enqueue(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        chunks.Enqueue(new List<int> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
        chunks.Enqueue(new List<int> { 21, 22, 23,});
        await foreach (var chunk in q.ToChunksAsync(10).ConfigureAwait(false))
        {
            Assert.True(chunk.SequenceEqual(chunks.Dequeue()));
        }
    }


    [Fact]
    public async Task LoadInChunkAsyncTest()
    {
        var data = Enumerable.Range(1, 23).Select(x => new Foo { Index = x, Quantity = 1 }).ToArray();
        TestingQueryable<Foo> q = new TestingQueryable<Foo>(data);

        int sum = 0;
        await foreach (var item in q.LoadInChunksAsync(10).ConfigureAwait(false))
        {
            sum += item.Quantity;
            item.IsRead = true;
            if (sum >= 17)
            {
                break;
            }
        }

        Assert.True(data[16].IsRead);
        Assert.False(data[17].IsRead);
    }


}

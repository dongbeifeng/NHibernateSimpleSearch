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
using Xunit;

namespace NHibernateUtils.Tests;

public class TestingQueryableTest
{

    class Foo
    {
        public string Bar { get; set; }

        public string? Baz { get; set; }
    }


    [Fact]
    public async Task SingleOrDefaultAsyncTest()
    {
        List<Foo> list = new List<Foo>
        {
            new Foo { Bar = "Bar1", Baz = "Baz1" },
            new Foo { Bar = "Bar2", Baz = "Baz2" },
            new Foo { Bar = "Bar3", Baz = "Baz3" },
        };

        TestingQueryable<Foo> q = new TestingQueryable<Foo>(list);

        var foo = await q.Where(x => x.Bar == "Bar2").SingleOrDefaultAsync();
        Assert.Same(list[1], foo);
    }

    [Fact]
    public void SingleOrDefaultTest()
    {
        List<Foo> list = new List<Foo>
        {
            new Foo { Bar = "Bar1", Baz = "Baz1" },
            new Foo { Bar = "Bar2", Baz = "Baz2" },
            new Foo { Bar = "Bar3", Baz = "Baz3" },
        };

        TestingQueryable<Foo> q = new TestingQueryable<Foo>(list);

        var foo = q.Where(x => x.Bar == "Bar2").SingleOrDefault();
        Assert.Same(list[1], foo);
    }


    [Fact]
    public async Task ToListAsyncTest()
    {
        List<Foo> list = new List<Foo>
        {
            new Foo { Bar = "Bar1", Baz = "Baz1" },
            new Foo { Bar = "Bar2", Baz = "Baz2" },
            new Foo { Bar = "Bar3", Baz = "Baz3" },
        };

        TestingQueryable<Foo> q = new TestingQueryable<Foo>(list);

        var foos = await q.Where(x => x.Bar == "Bar2").ToListAsync();
        Assert.Single(foos);
        Assert.Same(list[1], foos.Single());

    }

    [Fact]
    public void ToListTest()
    {
        List<Foo> list = new List<Foo>
        {
            new Foo { Bar = "Bar1", Baz = "Baz1" },
            new Foo { Bar = "Bar2", Baz = "Baz2" },
            new Foo { Bar = "Bar3", Baz = "Baz3" },
        };

        TestingQueryable<Foo> q = new TestingQueryable<Foo>(list);

        var foos = q.Where(x => x.Bar == "Bar2").ToList();
        Assert.Single(foos);
        Assert.Same(list[1], foos.Single());
    }

    [Fact]
    public void ToList_KeepsOrder()
    {
        List<Foo> list = new List<Foo>
        {
            new Foo { Bar = "Bar3", Baz = "Baz3" },
            new Foo { Bar = "Bar1", Baz = "Baz1" },
            new Foo { Bar = "Bar2", Baz = "Baz2" },
        };

        TestingQueryable<Foo> q = new TestingQueryable<Foo>(list);

        var foos = q.ToList();
        Assert.Same(list[0], foos[0]);
        Assert.Same(list[1], foos[1]);
        Assert.Same(list[2], foos[2]);
    }


    [Fact]
    public void OrderByTest()
    {
        List<Foo> list = new List<Foo>
        {
            new Foo { Bar = "Bar2", Baz = "Baz2" },
            new Foo { Bar = "Bar1", Baz = "Baz1" },
            new Foo { Bar = "Bar3", Baz = "Baz3" },
        };
        
        TestingQueryable<Foo> q = new TestingQueryable<Foo>(list);

        var list2 = q.OrderBy(x => x.Bar).ToList();
        Assert.Equal("Bar1", list2[0].Bar);
        Assert.Equal("Bar2", list2[1].Bar);
        Assert.Equal("Bar3", list2[2].Bar);

    }


    [Fact]
    public void OrderByDescendingTest()
    {
        List<Foo> list = new List<Foo>
        {
            new Foo { Bar = "Bar2", Baz = "Baz2" },
            new Foo { Bar = "Bar1", Baz = "Baz1" },
            new Foo { Bar = "Bar3", Baz = "Baz3" },
        };

        TestingQueryable<Foo> q = new TestingQueryable<Foo>(list);

        var list2 = q.OrderByDescending(x => x.Bar).ToList();
        Assert.Equal("Bar3", list2[0].Bar);
        Assert.Equal("Bar2", list2[1].Bar);
        Assert.Equal("Bar1", list2[2].Bar);

    }

}

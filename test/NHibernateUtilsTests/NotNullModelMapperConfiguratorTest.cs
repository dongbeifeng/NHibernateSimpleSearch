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
using Xunit;
using NSubstitute;
using static NSubstitute.Substitute;
using System.ComponentModel.DataAnnotations;

namespace NHibernateUtils.Tests;

public class NotNullModelMapperConfiguratorTest
{
    class Foo
    {
        [Required]
        public Foo? Rel1 { get; set; }

        public Foo? Rel2 { get; set; }

        public int Prop1 { get; set; }

        public int? Prop2 { get; set; }

        [Required]
        public string? Prop3 { get; set; }

        public string? Prop4 { get; set; }

    }


    [Fact]
    public void IsValueTypeAndNotNullableTest()
    {
        Assert.True(NotNullModelMapperConfigurator.IsValueTypeAndNotNullable(typeof(int)));
        Assert.False(NotNullModelMapperConfigurator.IsValueTypeAndNotNullable(typeof(int?)));
        Assert.False(NotNullModelMapperConfigurator.IsValueTypeAndNotNullable(typeof(string)));
    }

    [Theory]
    [InlineData("Rel1", true)]
    [InlineData("Rel2", false)]
    public void HandleBeforeMapManyToOneTest(string propName, bool notNullable)
    {
        NotNullModelMapperConfigurator sut = new NotNullModelMapperConfigurator();

        var map = For<IManyToOneMapper>();
        PropertyPath member = new PropertyPath(null, typeof(Foo).GetProperty(propName));
        sut.HandleBeforeMapManyToOne(For<IModelInspector>(), member, map);
        if (notNullable)
        {
            map.Received().NotNullable(true);
        }
        else
        {
            map.DidNotReceive().NotNullable(true);
        }
    }

    [Theory]
    [InlineData("Prop1", true)]
    [InlineData("Prop2", false)]
    [InlineData("Prop3", true)]
    [InlineData("Prop4", false)]
    public void HandleHandleBeforeMapPropertyTest(string propName, bool notNullable)
    {
        NotNullModelMapperConfigurator sut = new NotNullModelMapperConfigurator();

        var map = For<IPropertyMapper>();
        PropertyPath member = new PropertyPath(null, typeof(Foo).GetProperty(propName));
        sut.HandleBeforeMapProperty(For<IModelInspector>(), member, map);
        if (notNullable)
        {
            map.Received().NotNullable(true);
        }
        else
        {
            map.DidNotReceive().NotNullable(true);
        }
    }

}

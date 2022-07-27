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

public class DataAnnotationsModelMapperConfiguratorTest
{
    class Foo
    {

        public int Prop1 { get; set; }

        public int? Prop2 { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Prop3 { get; set; }

        public string? Prop4 { get; set; }

        [Required]
        public Foo? Prop5 { get; set; }

        public Foo? Prop6 { get; set; }
    }


    [Theory]
    [InlineData("Prop5", true)]
    [InlineData("Prop6", false)]
    public void HandleBeforeMapManyToOneTest(string propName, bool notNullable)
    {
        DataAnnotationsModelMapperConfigurator sut = new DataAnnotationsModelMapperConfigurator();

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
    [InlineData("Prop1", true, null)]
    [InlineData("Prop2", false, null)]
    [InlineData("Prop3", true, 30)]
    [InlineData("Prop4", false, null)]
    public void HandleHandleBeforeMapPropertyTest(string propName, bool expected_notnull, int? expected_length)
    {
        DataAnnotationsModelMapperConfigurator sut = new DataAnnotationsModelMapperConfigurator();

        var map = For<IPropertyMapper>();
        PropertyPath member = new PropertyPath(null, typeof(Foo).GetProperty(propName));
        sut.HandleBeforeMapProperty(For<IModelInspector>(), member, map);
        if (expected_notnull)
        {
            map.Received().NotNullable(true);
        }
        else
        {
            map.DidNotReceive().NotNullable(true);
        }

        if (expected_length is not null)
        {
            map.Received().Length(expected_length.Value);
        }
        else
        {
            map.DidNotReceive().Length(Arg.Any<int>());
        }
    }



}

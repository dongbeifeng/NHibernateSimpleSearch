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

using NHibernate.Type;
using System.Security.Principal;
using Xunit;

namespace NHibernateUtils.Tests;

public class AuditInterceptorTest
{
    class Foo
    {
        [CreationTime]
        public DateTime ctime { get; set; } = DateTime.MinValue;

        [ModificationTime]
        public DateTime mtime { get; set; } = DateTime.MinValue;

        [CreationUser] 
        public string? cuser { get; set; }

        [ModificationUser]
        public string? muser { get; set; }


        public string? Baz { get; set; }
    }

    class Bar
    {
        public DateTime ctime { get; set; } = DateTime.MinValue;

        public DateTime mtime { get; set; } = DateTime.MinValue;

        public string? cuser { get; set; }

        public string? muser { get; set; }


        public string? Baz { get; set; }
    }


    [Fact]
    public void OnFlushDirtyTest()
    {
        AuditInterceptor interceptor = new AuditInterceptor(new GenericPrincipal(new GenericIdentity("wangjianjun"), null));

        string[] propertiesNames = new[] { "ctime", "mtime", "cuser", "muser", "Baz" };
        IType[] types = new IType[] { TypeFactory.GetDateTimeType(4), TypeFactory.GetDateTimeType(4), TypeFactory.GetStringType(10), TypeFactory.GetStringType(10), TypeFactory.GetStringType(10) };

        {
            Foo foo = new Foo();
            object?[] currentState = new object?[] { foo.ctime, foo.mtime, foo.cuser, foo.muser, foo.Baz };
            object?[] previousState = new object?[] { foo.ctime, foo.mtime, foo.cuser, foo.muser, foo.Baz };
            var b = interceptor.OnFlushDirty(
                foo,
                "FOO",
                currentState,
                previousState,
                propertiesNames,
                types
                );

            Assert.True(b);
            Assert.Equal(DateTime.MinValue, currentState[0]);
            Assert.InRange((DateTime)currentState[1], DateTime.Now.AddSeconds(-3), DateTime.Now.AddSeconds(3));
            Assert.Null(currentState[2]);
            Assert.Equal("wangjianjun", currentState[3]);
            Assert.Null(currentState[4]);
        }

        {
            Bar bar = new Bar();
            object?[] currentState = new object?[] { bar.ctime, bar.mtime, bar.cuser, bar.muser, bar.Baz };
            object?[] previousState = new object?[] { bar.ctime, bar.mtime, bar.cuser, bar.muser, bar.Baz };
            var b = interceptor.OnFlushDirty(
                bar,
                "BAR",
                currentState,
                previousState,
                propertiesNames,
                types
                );

            Assert.False(b);
            Assert.Equal(DateTime.MinValue, currentState[0]);
            Assert.Equal(DateTime.MinValue, currentState[1]);
            Assert.Null(currentState[2]);
            Assert.Null(currentState[3]);
            Assert.Null(currentState[4]);

        }

    }

    [Fact]
    public void OnSaveTest()
    {
        AuditInterceptor interceptor = new AuditInterceptor(new GenericPrincipal(new GenericIdentity("wangjianjun"), null));
        string[] propertiesNames = new[] { "ctime", "mtime", "cuser", "muser", "Baz" };
        IType[] types = new IType[] { TypeFactory.GetDateTimeType(4), TypeFactory.GetDateTimeType(4), TypeFactory.GetStringType(10), TypeFactory.GetStringType(10), TypeFactory.GetStringType(10) };

        {
            Foo foo = new Foo();
            object?[] state = new object?[] { foo.ctime, foo.mtime, foo.cuser, foo.muser, foo.Baz! };
            var b = interceptor.OnSave(
                foo,
                "FOO",
                state,
                propertiesNames,
                types
                );

            Assert.True(b);
            Assert.InRange((DateTime)state[0], DateTime.Now.AddSeconds(-3), DateTime.Now.AddSeconds(3));
            Assert.InRange((DateTime)state[1], DateTime.Now.AddSeconds(-3), DateTime.Now.AddSeconds(3));
            Assert.Equal("wangjianjun", state[2]);
            Assert.Equal("wangjianjun", state[3]);
            Assert.Null(state[4]);
        }

        {
            Bar bar = new Bar();
            object?[] state = new object?[] { bar.ctime, bar.mtime, bar.cuser, bar.muser, bar.Baz! };
            var b = interceptor.OnFlushDirty(
                bar,
                "BAR",
                state,
                state,
                propertiesNames,
                types
                );

            Assert.False(b);
            Assert.Equal(DateTime.MinValue, state[0]);
            Assert.Equal(DateTime.MinValue, state[1]);
            Assert.Null(state[2]);
            Assert.Null(state[3]);
            Assert.Null(state[4]);

        }


    }

}

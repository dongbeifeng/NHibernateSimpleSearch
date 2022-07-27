using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NHibernate.UserTypes;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Xunit;

namespace NHibernateUtils.Tests
{
    public class MapperUtilsTests
    {

        class Foo
        {
            [Required]
            [MaxLength(15)]
            public string? A { get; set; }

            public string? B { get; set; }

            public int C { get; set; }

            public int? D { get; set; }
        }

        class FooType : ICompositeUserType
        {
            public string[] PropertyNames => throw new NotImplementedException();

            public IType[] PropertyTypes => throw new NotImplementedException();

            public Type ReturnedClass => throw new NotImplementedException();

            public bool IsMutable => throw new NotImplementedException();

            public object Assemble(object cached, ISessionImplementor session, object owner)
            {
                throw new NotImplementedException();
            }

            public object DeepCopy(object value)
            {
                throw new NotImplementedException();
            }

            public object Disassemble(object value, ISessionImplementor session)
            {
                throw new NotImplementedException();
            }

            public new bool Equals(object x, object y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(object x)
            {
                throw new NotImplementedException();
            }

            public object GetPropertyValue(object component, int property)
            {
                throw new NotImplementedException();
            }

            public object NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
            {
                throw new NotImplementedException();
            }

            public void NullSafeSet(DbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
            {
                throw new NotImplementedException();
            }

            public object Replace(object original, object target, ISessionImplementor session, object owner)
            {
                throw new NotImplementedException();
            }

            public void SetPropertyValue(object component, int property, object value)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void IsValueTypeAndNotNullableTest()
        {
            Assert.True(MapperUtils.IsValueTypeAndNotNullable(typeof(int)));
            Assert.False(MapperUtils.IsValueTypeAndNotNullable(typeof(int?)));
            Assert.False(MapperUtils.IsValueTypeAndNotNullable(typeof(string)));
        }

        [Theory]
        [InlineData("A", true, 15)]
        [InlineData("B", false, null)]
        [InlineData("C", true, null)]
        [InlineData("D", false, null)]
        public void GetColumnAttributesTest(string fooProp, bool expected_notnull, int? expected_length)
        {
            var (notnull, length) = MapperUtils.GetColumnAttributes(typeof(Foo), fooProp);
            Assert.Equal(expected_notnull, notnull);
            Assert.Equal(expected_length, length);
        }

        [Fact()]
        public void MapColumnTest()
        {
            {
                var mapper = MapperUtils.MapColumn<Foo>("A");
                IColumnMapper col = Substitute.For<IColumnMapper>();
                mapper(col);
                col.Received().NotNullable(true);
                col.Received().Length(15);
            }

            {
                var mapper = MapperUtils.MapColumn<Foo>("B");
                IColumnMapper col = Substitute.For<IColumnMapper>();
                mapper(col);
                col.DidNotReceive().NotNullable(true);
                col.DidNotReceive().Length(Arg.Any<int>());
            }

            {
                var mapper = MapperUtils.MapColumn<Foo>("C");
                IColumnMapper col = Substitute.For<IColumnMapper>();
                mapper(col);
                col.Received().NotNullable(true);
                col.DidNotReceive().Length(Arg.Any<int>());
            }

            {
                var mapper = MapperUtils.MapColumn<Foo>("D");
                IColumnMapper col = Substitute.For<IColumnMapper>();
                mapper(col);
                col.DidNotReceive().NotNullable(true);
                col.DidNotReceive().Length(Arg.Any<int>());
            }
        }

        [Fact()]
        public void MapPropertyOfCompositeUserTypeTest()
        {
            var mapper = MapperUtils.MapPropertyOfCompositeUserType<Foo, FooType>("A", "B", "C", "D");
            IPropertyMapper prop = Substitute.For<IPropertyMapper>();
            mapper(prop);
            prop.Received().Type<FooType>();
            prop.Received().Columns(Arg.Any<Action<IColumnMapper>[]>());
        }
    }
}
using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

#if PCL
using System.Windows.Markup;

using System.Xaml;
using System.Xaml.Schema;
#else
using System.Windows.Markup;
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
#endif

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlNodeListTest
	{
		[Test]
		public void ConstructorNull()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlNodeList(null));
		}

		[Test]
		public void NegativeSize()
		{
			Assert.Throws<ArgumentOutOfRangeException> (() => new XamlNodeList(new XamlSchemaContext(), -100));
		}

		[Test]
		public void ReadWriteListShouldRoundtrip()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);

			var reader = new XamlObjectReader(new TestClass4 { Foo = "foo", Bar = "bar" }, sc);
			XamlServices.Transform(reader, list.Writer);

			var writer = new XamlObjectWriter(sc);
			var listReader = list.GetReader();
			XamlServices.Transform(listReader, writer);

			ClassicAssert.IsNotNull(writer.Result, "#1");
            ClassicAssert.IsInstanceOf<TestClass4>(writer.Result, "#2");

			ClassicAssert.AreEqual("foo", ((TestClass4)writer.Result).Foo, "#3");
			ClassicAssert.AreEqual("bar", ((TestClass4)writer.Result).Bar, "#4");

			// try reading a 2nd time, we should not get the same reader
			writer = new XamlObjectWriter(sc);
			var listReader2 = list.GetReader();
            ClassicAssert.AreNotSame(listReader, listReader2, "#5");
			XamlServices.Transform(listReader2, writer);

			ClassicAssert.IsNotNull(writer.Result, "#6");
            ClassicAssert.IsInstanceOf<TestClass4>(writer.Result, "#7");

			ClassicAssert.AreEqual("foo", ((TestClass4)writer.Result).Foo, "#8");
			ClassicAssert.AreEqual("bar", ((TestClass4)writer.Result).Bar, "#9");
		}

		[Test]
		public void WriterShouldThrowExceptionIfNotClosed()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);
			list.Writer.WriteStartObject(sc.GetXamlType(typeof(TestClass4)));
			list.Writer.WriteEndObject();
			Assert.Throws<XamlException> (() => list.GetReader());
		}

		[Test]
		public void WriterShouldNotThrowExceptionIfClosed()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);
			list.Writer.WriteStartObject(sc.GetXamlType(typeof(TestClass4)));
			list.Writer.WriteEndObject();
			list.Writer.Close();
			list.GetReader();
		}
	}
}


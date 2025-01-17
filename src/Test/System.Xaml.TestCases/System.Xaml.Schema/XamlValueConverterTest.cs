//
// Copyright (C) 2010 Novell Inc. http://novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;
using System.ComponentModel;
using NUnit.Framework.Legacy;

#if PCL
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
#else
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
#endif

namespace MonoTests.System.Xaml.Schema
{
	[TestFixture]
	public class XamlValueConverterTest
	{
		[Test]
		public void ConstructorNullConverterTypeTargetType ()
		{
			// either of them must be non-null.
			Assert.Throws<ArgumentException> (() => new XamlValueConverter<TypeConverter> (null, null));
		}

		[Test]
		public void ConstructorNullConverterTypeTargetTypeNull ()
		{
			// either of them must be non-null.
			Assert.Throws<ArgumentException> (() => new XamlValueConverter<TypeConverter> (null, null, null));
		}

		[Test]
		public void ConstructorOnlyWithName ()
		{
			// ok
			new XamlValueConverter<TypeConverter> (null, null, "Foo");
		}

		[Test]
		public void ConstructorNullConverterType ()
		{
			// ok
			var c = new XamlValueConverter<TypeConverter> (null, XamlLanguage.Int32);
			ClassicAssert.IsNull (c.ConverterInstance, "#1");
		}

		[Test]
		public void ConstructorNullTargetType ()
		{
			// ok
			var c = new XamlValueConverter<TypeConverter> (typeof (Int32Converter), null);
			ClassicAssert.IsTrue (c.ConverterInstance is Int32Converter, "#1");
		}

		[Test]
		public void ConstructorNullName ()
		{
			// ok
			new XamlValueConverter<TypeConverter> (typeof (Int32Converter), XamlLanguage.Int32, null);
		}

		[Test]
		public void ConverterTargetMismatch ()
		{
			// ok
			var c = new XamlValueConverter<TypeConverter> (typeof (Int32Converter), XamlLanguage.String, null);
			ClassicAssert.IsTrue (c.ConverterInstance is Int32Converter, "#1");
		}

		[Test]
		public void InconsistentConverterType ()
		{
			var c = new XamlValueConverter<TypeConverter> (typeof (int), XamlLanguage.String, null);
			Assert.Throws<XamlSchemaException> (() => { var t = c.ConverterInstance; }, "#1");
		}

		[Test]
		public void ObjectType ()
		{
			// This test asserts that XamlLanguage.Object.TypeConverter.ConverterType is null for different reason.
			var c = new XamlValueConverter<TypeConverter> (typeof (TypeConverter), XamlLanguage.Object, null);
			ClassicAssert.IsNotNull (c.ConverterInstance, "#1");
			#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNull (XamlLanguage.Object.TypeConverter.ConverterInstance, "#2");
			#endif
		}

		[Test]
		public void Equality ()
		{
			// ok
			var c1 = new XamlValueConverter<TypeConverter> (null, XamlLanguage.Int32);
			var c2 = new XamlValueConverter<TypeConverter> (null, XamlLanguage.Int32);
			var c3 = new XamlValueConverter<TypeConverter> (typeof (Int32Converter), XamlLanguage.Int32);
			var c4 = new XamlValueConverter<TypeConverter> (typeof (Int32Converter), XamlLanguage.Int32, null);
			var c5 = new XamlValueConverter<TypeConverter> (typeof (Int32Converter), XamlLanguage.Int32, "Foo");
			ClassicAssert.IsTrue (c1 == c2, "#1");
			ClassicAssert.IsFalse (c1 == c3, "#2");
			ClassicAssert.IsTrue (c3 == c4, "#3");
			ClassicAssert.IsFalse (c4 == c5, "#4");
		}
		
		[Test]
		public void TestToString ()
		{
			ClassicAssert.AreEqual ("Int32Converter(Int32)", new XamlValueConverter<TypeConverter> (typeof (Int32Converter), XamlLanguage.Int32).ToString (), "#1");
			ClassicAssert.AreEqual ("Foo", new XamlValueConverter<TypeConverter> (typeof (Int32Converter), XamlLanguage.Int32, "Foo").ToString (), "#2");
			ClassicAssert.AreEqual ("Int32Converter", new XamlValueConverter<TypeConverter> (typeof (Int32Converter), null).ToString (), "#1");
			ClassicAssert.AreEqual ("Int32", new XamlValueConverter<TypeConverter> (null, XamlLanguage.Int32).ToString (), "#3"); // huh, really? no difference from ConverterType?
		}
	}
}

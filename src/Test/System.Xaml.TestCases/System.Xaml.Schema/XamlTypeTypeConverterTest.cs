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
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
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

namespace MonoTests.System.Xaml.Schema
{
#if HAS_TYPE_CONVERTER

	[TestFixture]
	public class XamlTypeTypeConverterTest
	{
		XamlTypeTypeConverter c = new XamlTypeTypeConverter ();
		XamlSchemaContext sctx = new XamlSchemaContext (null, null);

		[Test]
		public void CanConvertFrom ()
		{
			ClassicAssert.IsFalse (c.CanConvertFrom (null, typeof (XamlType)), "#1");
			ClassicAssert.IsTrue (c.CanConvertFrom (null, typeof (string)), "#2");
			ClassicAssert.IsFalse (c.CanConvertFrom (null, typeof (int)), "#3");
			ClassicAssert.IsFalse (c.CanConvertFrom (null, typeof (object)), "#4");
			ClassicAssert.IsFalse (c.CanConvertFrom (new DummyValueSerializerContext (), typeof (XamlType)), "#5");
		}

		[Test]
		public void CanConvertTo ()
		{
			ClassicAssert.IsFalse (c.CanConvertTo (null, typeof (XamlType)), "#1");
			ClassicAssert.IsTrue (c.CanConvertTo (null, typeof (string)), "#2");
			ClassicAssert.IsFalse (c.CanConvertTo (null, typeof (int)), "#3");
			ClassicAssert.IsFalse (c.CanConvertTo (null, typeof (object)), "#4");
			ClassicAssert.IsFalse (c.CanConvertTo (new DummyValueSerializerContext (), typeof (XamlType)), "#5");
		}

		// ConvertFrom() is not supported in either way.

		[Test]
		public void ConvertFrom ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertFrom (null, null, XamlLanguage.String));
		}

		[Test]
		public void ConvertFrom2 ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertFrom (null, null, "System.Int32"));
		}

		[Test]
		public void ConvertXamlTypeToXamlType ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (null, null, XamlLanguage.String, typeof (XamlType)), "#1");
		}

		[Test]
		public void ConvertXamlTypeToXamlType2 ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (new DummyValueSerializerContext (), null, XamlLanguage.String, typeof (XamlType)), "#1");
		}

		[Test]
		public void ConvertXamlTypeToType ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (null, null, XamlLanguage.String, typeof (Type)));
		}

		[Test]
		public void ConvertXamlTypeToString ()
		{
			// ... so, it does not seem to just call XamlType.ToString(), but rather first try to use UnderlyingType if possible.
			ClassicAssert.AreEqual ("System.String", c.ConvertTo (null, null, XamlLanguage.String, typeof (string)), "#1"); // huh?
			ClassicAssert.AreEqual ("System.Windows.Markup.TypeExtension".Fixup(), c.ConvertTo (null, null, XamlLanguage.Type, typeof (string)), "#1"); // huh?
			ClassicAssert.AreEqual ("{urn:foo}Foo", c.ConvertTo (null, null, new XamlType ("urn:foo", "Foo", null, sctx), typeof (string)), "#2");
		}

		[Test]
		public void ConvertStringToString ()
		{
			ClassicAssert.AreEqual ("foo", c.ConvertTo (null, CultureInfo.InvariantCulture, "foo", typeof (string)), "#1");
		}

		[Test]
		public void ConvertIntToString ()
		{
			ClassicAssert.AreEqual ("5", c.ConvertTo (null, null, 5, typeof (string)), "#1");
		}

		[Test]
		public void ConvertStringToXamlType ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (new DummyValueSerializerContext (), null, "System.String", typeof (XamlType)), "#1");
		}
	}
#endif
}
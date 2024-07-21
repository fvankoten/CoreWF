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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

using CategoryAttribute = NUnit.Framework.CategoryAttribute;

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlDirectiveTest
	{
		XamlSchemaContext sctx = new XamlSchemaContext (new XamlSchemaContextSettings ());

		[Test]
		public void ConstructorNameNull ()
		{
			// wow, it is allowed.
			var d = new XamlDirective (String.Empty, null);
			ClassicAssert.IsNull (d.Name, "#1");
		}

		[Test]
		public void ConstructorNamespaceNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlDirective (null, "Foo"));
		}

		[Test]
		public void ConstructorNamespaceXamlNS ()
		{
			new XamlDirective (XamlLanguage.Xaml2006Namespace, "Foo");
		}

#if HAS_TYPE_CONVERTER
		[Test]
		public void ConstructorComplexParamsTypeNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlDirective (new string [] {"urn:foo"}, "Foo", null, null, AllowedMemberLocations.Any));
		}

		[Test]
		public void ConstructorComplexParamsNullNamespaces ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlDirective (null, "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any));
		}

		[Test]
		public void ConstructorComplexParamsEmptyNamespaces ()
		{
			new XamlDirective (new string [0], "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any);
		}

		[Test]
		public void ConstructorComplexParams ()
		{
			new XamlDirective (new string [] {"urn:foo"}, "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any);
		}
#endif
		[Test]
		public void DefaultValuesWithName ()
		{
			var d = new XamlDirective ("urn:foo", "Foo");
			ClassicAssert.AreEqual (AllowedMemberLocations.Any, d.AllowedLocation, "#1");
			ClassicAssert.IsNull (d.DeclaringType, "#2");
			ClassicAssert.IsNotNull (d.Invoker, "#3");
			ClassicAssert.IsNull (d.Invoker.UnderlyingGetter, "#3-2");
			ClassicAssert.IsNull (d.Invoker.UnderlyingSetter, "#3-3");
			ClassicAssert.IsTrue (d.IsUnknown, "#4");
			ClassicAssert.IsTrue (d.IsReadPublic, "#5");
			ClassicAssert.IsTrue (d.IsWritePublic, "#6");
			ClassicAssert.AreEqual ("Foo", d.Name, "#7");
			ClassicAssert.IsTrue (d.IsNameValid, "#8");
			ClassicAssert.AreEqual ("urn:foo", d.PreferredXamlNamespace, "#9");
			ClassicAssert.IsNull (d.TargetType, "#10");
			ClassicAssert.IsNotNull (d.Type, "#11");
			ClassicAssert.AreEqual (typeof (object), d.Type.UnderlyingType, "#11-2");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNull (d.TypeConverter, "#12");
#endif
			ClassicAssert.IsNull (d.ValueSerializer, "#13");
			ClassicAssert.IsNull (d.DeferringLoader, "#14");
			ClassicAssert.IsNull (d.UnderlyingMember, "#15");
			ClassicAssert.IsFalse (d.IsReadOnly, "#16");
			ClassicAssert.IsFalse (d.IsWriteOnly, "#17");
			ClassicAssert.IsFalse (d.IsAttachable, "#18");
			ClassicAssert.IsFalse (d.IsEvent, "#19");
			ClassicAssert.IsTrue (d.IsDirective, "#20");
			ClassicAssert.IsNotNull (d.DependsOn, "#21");
			ClassicAssert.AreEqual (0, d.DependsOn.Count, "#21-2");
			ClassicAssert.IsFalse (d.IsAmbient, "#22");
			// TODO: ClassicAssert.AreEqual (DesignerSerializationVisibility.Visible, d.SerializationVisibility, "#23");
		}

#if HAS_TYPE_CONVERTER
		[Test]
		public void DefaultValuesWithComplexParams ()
		{
			var d = new XamlDirective (new string [0], "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any);
			ClassicAssert.AreEqual (AllowedMemberLocations.Any, d.AllowedLocation, "#1");
			ClassicAssert.IsNull (d.DeclaringType, "#2");
			ClassicAssert.IsNotNull (d.Invoker, "#3");
			ClassicAssert.IsNull (d.Invoker.UnderlyingGetter, "#3-2");
			ClassicAssert.IsNull (d.Invoker.UnderlyingSetter, "#3-3");
			ClassicAssert.IsFalse (d.IsUnknown, "#4"); // different from another test
			ClassicAssert.IsTrue (d.IsReadPublic, "#5");
			ClassicAssert.IsTrue (d.IsWritePublic, "#6");
			ClassicAssert.AreEqual ("Foo", d.Name, "#7");
			ClassicAssert.IsTrue (d.IsNameValid, "#8");
			ClassicAssert.AreEqual (null, d.PreferredXamlNamespace, "#9"); // different from another test (as we specified empty array above)
			ClassicAssert.IsNull (d.TargetType, "#10");
			ClassicAssert.IsNotNull (d.Type, "#11");
			ClassicAssert.AreEqual (typeof (object), d.Type.UnderlyingType, "#11-2");
			ClassicAssert.IsNull (d.TypeConverter, "#12");
			ClassicAssert.IsNull (d.ValueSerializer, "#13");
			ClassicAssert.IsNull (d.DeferringLoader, "#14");
			ClassicAssert.IsNull (d.UnderlyingMember, "#15");
			ClassicAssert.IsFalse (d.IsReadOnly, "#16");
			ClassicAssert.IsFalse (d.IsWriteOnly, "#17");
			ClassicAssert.IsFalse (d.IsAttachable, "#18");
			ClassicAssert.IsFalse (d.IsEvent, "#19");
			ClassicAssert.IsTrue (d.IsDirective, "#20");
			ClassicAssert.IsNotNull (d.DependsOn, "#21");
			ClassicAssert.AreEqual (0, d.DependsOn.Count, "#21-2");
			ClassicAssert.IsFalse (d.IsAmbient, "#22");
			//TODO: ClassicAssert.AreEqual (DesignerSerializationVisibility.Visible, d.SerializationVisibility, "#23");
		}
#endif
	}
}

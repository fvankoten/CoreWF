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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
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
using XamlReader = System.Xaml.XamlReader;
using NUnit.Framework.Legacy;
#if NETSTANDARD
#endif

namespace MonoTests.System.Xaml
{
	// FIXME: enable DeferringLoader tests.
	[TestFixture]
	public class XamlTypeTest
	{
		XamlSchemaContext sctx = new XamlSchemaContext (null, null);

		[Test]
		public void ConstructorTypeNullType ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType (null, sctx));
		}

		[Test]
		public void ConstructorTypeNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType (typeof (int), null));
		}

		[Test]
		public void ConstructorSimpleType ()
		{
			var t = new XamlType (typeof (int), sctx);
			ClassicAssert.AreEqual ("Int32", t.Name, "#1");
			ClassicAssert.AreEqual (typeof (int), t.UnderlyingType, "#2");
			ClassicAssert.IsNotNull (t.BaseType, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			ClassicAssert.AreEqual ("ValueType", t.BaseType.Name, "#3-2");
			ClassicAssert.AreEqual ("clr-namespace:System;assembly=System.Private.CoreLib", t.BaseType.PreferredXamlNamespace, "#3-3");
			// It is likely only for primitive types such as int.
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, t.PreferredXamlNamespace, "#4");

			t = new XamlType (typeof (XamlXmlReader), sctx);
			ClassicAssert.AreEqual ("XamlXmlReader", t.Name, "#11");
			ClassicAssert.AreEqual (typeof (XamlXmlReader), t.UnderlyingType, "#12");
			ClassicAssert.IsNotNull (t.BaseType, "#13");
			ClassicAssert.AreEqual (typeof (XamlReader), t.BaseType.UnderlyingType, "#13-2");
			ClassicAssert.AreEqual ("clr-namespace:System.Xaml;assembly=System.Xaml".Fixup(), t.BaseType.PreferredXamlNamespace, "#13-3");
			ClassicAssert.AreEqual ("clr-namespace:System.Xaml;assembly=System.Xaml".Fixup(), t.PreferredXamlNamespace, "#14");
		}

		[Test]
		public void ConstructorNullTypeInvoker ()
		{
			// allowed.
			new XamlType (typeof (int), sctx, null);
		}

		[Test]
		public void ConstructorNamesNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType (String.Empty, null, null, sctx));
		}

		[Test]
		public void ConstructorNamesNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType ("System", "Int32", null, null));
		}

		[Test]
		public void ConstructorNames ()
		{
			// null typeArguments is allowed.
			new XamlType ("System", "Int32", null, sctx);
		}

		[Test]
		public void ConstructorNameNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => new MyXamlType (null, null, sctx));
		}

		[Test]
		public void ConstructorNameNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new MyXamlType ("System.Int32", null, null));
		}

		[Test]
		public void ConstructorNameInvalid ()
		{
			// ... all allowed.
			new XamlType (String.Empty, ".", null, sctx);
			new XamlType (String.Empty, "<>", null, sctx);
			new XamlType (String.Empty, "", null, sctx);
		}

		[Test]
		public void ConstructorNameWithFullName ()
		{
			// null typeArguments is allowed.
			var t = new MyXamlType ("System.Int32", null, sctx);
			ClassicAssert.AreEqual ("System.Int32", t.Name, "#1");
			ClassicAssert.IsNull (t.UnderlyingType, "#2");
			ClassicAssert.IsNotNull (t.BaseType, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			ClassicAssert.AreEqual ("Object", t.BaseType.Name, "#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, t.BaseType.PreferredXamlNamespace, "#3-3");
			ClassicAssert.IsNull (t.BaseType.BaseType, "#3-4");
			ClassicAssert.AreEqual (String.Empty, t.PreferredXamlNamespace, "#4");
			ClassicAssert.IsFalse (t.IsArray, "#5");
			ClassicAssert.IsFalse (t.IsGeneric, "#6");
			ClassicAssert.IsTrue (t.IsPublic, "#7");
			ClassicAssert.AreEqual (0, t.GetAllMembers ().Count, "#8");
		}

		[Test]
		public void NoSuchTypeByName ()
		{
			var t = new MyXamlType ("System.NoSuchType", null, sctx);
			ClassicAssert.AreEqual ("System.NoSuchType", t.Name, "#1");
			ClassicAssert.IsNull (t.UnderlyingType, "#2");
			ClassicAssert.IsNotNull (t.BaseType, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			ClassicAssert.AreEqual ("Object", t.BaseType.Name, "#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, t.BaseType.PreferredXamlNamespace, "#3-3");
			ClassicAssert.AreEqual (String.Empty, t.PreferredXamlNamespace, "#4");
		}

		[Test]
		public void NoSuchTypeByNames ()
		{
			var t = new XamlType ("urn:foo", "System.NoSuchType", null, sctx);
			ClassicAssert.AreEqual ("System.NoSuchType", t.Name, "#1");
			ClassicAssert.IsNull (t.UnderlyingType, "#2");
			ClassicAssert.IsNotNull (t.BaseType, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			ClassicAssert.AreEqual ("Object", t.BaseType.Name, "#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, t.BaseType.PreferredXamlNamespace, "#3-3");
			ClassicAssert.AreEqual ("urn:foo", t.PreferredXamlNamespace, "#4");
		}

		[Test]
		public void EmptyTypeArguments ()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("results in NRE on .NET 4.0 RTM, but should be treated the same");
			var t1 = new MyXamlType ("System.Int32", null, sctx);
			var t2 = new MyXamlType ("System.Int32", new XamlType [0], sctx);
			ClassicAssert.IsTrue (t1 == t2, "#1");
			ClassicAssert.IsTrue (t1.Equals (t2), "#2");
		}

		[Test]
		public void EmptyTypeArguments2 ()
		{
			var t1 = new XamlType ("System", "Int32", null, sctx);
			var t2 = new XamlType ("System", "Int32", new XamlType [0], sctx);
			ClassicAssert.IsNull (t1.TypeArguments, "#1");
			ClassicAssert.IsNull (t2.TypeArguments, "#2");
			ClassicAssert.IsTrue (t1 == t2, "#3");
			ClassicAssert.IsTrue (t1.Equals (t2), "#4");
		}

		[Test]
		public void EqualityAcrossConstructors ()
		{
			var t1 = new XamlType (typeof (int), sctx);
			var t2 = new XamlType (t1.PreferredXamlNamespace, t1.Name, null, sctx);
			// not sure if it always returns false for different .ctor comparisons...
			ClassicAssert.IsFalse (t1 == t2, "#3");
			
			ClassicAssert.AreNotEqual (XamlLanguage.Type, new XamlSchemaContext ().GetXamlType (typeof (Type)), "#4");
		}

		[Test]
		public void ArrayAndCollection ()
		{
			var t = new XamlType (typeof (int), sctx);
			ClassicAssert.IsFalse (t.IsArray, "#1.1");
			ClassicAssert.IsFalse (t.IsCollection, "#1.2");
			ClassicAssert.IsNull (t.ItemType, "#1.3");

			t = new XamlType (typeof (ArrayList), sctx);
			ClassicAssert.IsFalse (t.IsArray, "#2.1");
			ClassicAssert.IsTrue (t.IsCollection, "#2.2");
			ClassicAssert.IsNotNull (t.ItemType, "#2.3");
			ClassicAssert.AreEqual ("Object", t.ItemType.Name, "#2.4");

			t = new XamlType (typeof (int []), sctx);
			ClassicAssert.IsTrue (t.IsArray, "#3.1");
			ClassicAssert.IsFalse (t.IsCollection, "#3.2");
			ClassicAssert.IsNotNull (t.ItemType, "#3.3");
			ClassicAssert.AreEqual (typeof (int), t.ItemType.UnderlyingType, "#3.4");

			t = new XamlType (typeof (IList), sctx);
			ClassicAssert.IsFalse (t.IsArray, "#4.1");
			ClassicAssert.IsTrue (t.IsCollection, "#4.2");
			ClassicAssert.IsNotNull (t.ItemType, "#4.3");
			ClassicAssert.AreEqual (typeof (object), t.ItemType.UnderlyingType, "#4.4");

			t = new XamlType (typeof (ICollection), sctx); // it is not a XAML collection.
			ClassicAssert.IsFalse (t.IsArray, "#5.1");
			ClassicAssert.IsFalse (t.IsCollection, "#5.2");
			ClassicAssert.IsNull (t.ItemType, "#5.3");

			t = new XamlType (typeof (ArrayExtension), sctx);
			ClassicAssert.IsFalse (t.IsArray, "#6.1");
			ClassicAssert.IsFalse (t.IsCollection, "#6.2");
			ClassicAssert.IsNull (t.ItemType, "#6.3");
		}

		[Test]
		public void Dictionary ()
		{
			var t = new XamlType (typeof (int), sctx);
			ClassicAssert.IsFalse (t.IsDictionary, "#1.1");
			ClassicAssert.IsFalse (t.IsCollection, "#1.1-2");
			ClassicAssert.IsNull (t.KeyType, "#1.2");
			t = new XamlType (typeof (Hashtable), sctx);
			ClassicAssert.IsTrue (t.IsDictionary, "#2.1");
			ClassicAssert.IsFalse (t.IsCollection, "#2.1-2");
			ClassicAssert.IsNotNull (t.KeyType, "#2.2");
			ClassicAssert.IsNotNull (t.ItemType, "#2.2-2");
			ClassicAssert.AreEqual ("Object", t.KeyType.Name, "#2.3");
			ClassicAssert.AreEqual ("Object", t.ItemType.Name, "#2.3-2");
			t = new XamlType (typeof (Dictionary<int,string>), sctx);
			ClassicAssert.IsTrue (t.IsDictionary, "#3.1");
			ClassicAssert.IsFalse (t.IsCollection, "#3.1-2");
			ClassicAssert.IsNotNull (t.KeyType, "#3.2");
			ClassicAssert.IsNotNull (t.ItemType, "#3.2-2");
			ClassicAssert.AreEqual ("Int32", t.KeyType.Name, "#3.3");
			ClassicAssert.AreEqual ("String", t.ItemType.Name, "#3.3-2");

			var ml = t.GetAllMembers ();
			ClassicAssert.AreEqual (2, ml.Count, "#3.4");
			ClassicAssert.IsTrue (ml.Any (mi => mi.Name == "Keys"), "#3.4-2");
			ClassicAssert.IsTrue (ml.Any (mi => mi.Name == "Values"), "#3.4-3");
			ClassicAssert.IsNotNull (t.GetMember ("Keys"), "#3.4-4");
			ClassicAssert.IsNotNull (t.GetMember ("Values"), "#3.4-5");
		}

		public class TestClass1
		{
		}
	
		class TestClass2
		{
			internal TestClass2 () {}
		}

		[Test]
		public void IsConstructible ()
		{
			// ... is it?
			ClassicAssert.IsTrue (new XamlType (typeof (int), sctx).IsConstructible, "#1");
			// ... is it?
			ClassicAssert.IsFalse (new XamlType (typeof (TestClass1), sctx).IsConstructible, "#2");
			ClassicAssert.IsFalse (new XamlType (typeof (TestClass2), sctx).IsConstructible, "#3");
			ClassicAssert.IsTrue (new XamlType (typeof (object), sctx).IsConstructible, "#4");
		}

		class AttachableClass
		{
			#pragma warning disable 67
			public event EventHandler<EventArgs> SimpleEvent;
			#pragma warning restore 67
			public void AddSimpleHandler (object o, EventHandler h)
			{
			}
		}

		// hmm, what can we use to verify this method?
		[Test]
		public void GetAllAttachableMembers ()
		{
			var xt = new XamlType (typeof (AttachableClass), sctx);
			var l = xt.GetAllAttachableMembers ();
			ClassicAssert.AreEqual (0, l.Count, "#1");
		}

		[Test]
		public void DefaultValuesType ()
		{
			var t = new XamlType (typeof (int), sctx);
			ClassicAssert.IsNotNull (t.Invoker, "#1");
			ClassicAssert.IsTrue (t.IsNameValid, "#2");
			ClassicAssert.IsFalse (t.IsUnknown, "#3");
			ClassicAssert.AreEqual ("Int32", t.Name, "#4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, t.PreferredXamlNamespace, "#5");
			ClassicAssert.IsNull (t.TypeArguments, "#6");
			ClassicAssert.AreEqual (typeof (int), t.UnderlyingType, "#7");
			ClassicAssert.IsFalse (t.ConstructionRequiresArguments, "#8");
			ClassicAssert.IsFalse (t.IsArray, "#9");
			ClassicAssert.IsFalse (t.IsCollection, "#10");
			ClassicAssert.IsTrue (t.IsConstructible, "#11");
			ClassicAssert.IsFalse (t.IsDictionary, "#12");
			ClassicAssert.IsFalse (t.IsGeneric, "#13");
			ClassicAssert.IsFalse (t.IsMarkupExtension, "#14");
			ClassicAssert.IsFalse (t.IsNameScope, "#15");
			ClassicAssert.IsFalse (t.IsNullable, "#16");
			ClassicAssert.IsTrue (t.IsPublic, "#17");
			ClassicAssert.IsFalse (t.IsUsableDuringInitialization, "#18");
			ClassicAssert.IsFalse (t.IsWhitespaceSignificantCollection, "#19");
			ClassicAssert.IsFalse (t.IsXData, "#20");
			ClassicAssert.IsFalse (t.TrimSurroundingWhitespace, "#21");
			ClassicAssert.IsFalse (t.IsAmbient, "#22");
			ClassicAssert.IsNull (t.AllowedContentTypes, "#23");
			ClassicAssert.IsNull (t.ContentWrappers, "#24");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNotNull (t.TypeConverter, "#25");
			ClassicAssert.IsTrue (t.TypeConverter.ConverterInstance is Int32Converter, "#25-2");
#endif
			ClassicAssert.IsNull (t.ValueSerializer, "#26");
			ClassicAssert.IsNull (t.ContentProperty, "#27");
			//ClassicAssert.IsNull (t.DeferringLoader, "#28");
			ClassicAssert.IsNull (t.MarkupExtensionReturnType, "#29");
			ClassicAssert.AreEqual (sctx, t.SchemaContext, "#30");
		}

		[Test]
		public void DefaultValuesType2 ()
		{
			var t = new XamlType (typeof (Type), sctx);
			ClassicAssert.IsNotNull (t.Invoker, "#1");
			ClassicAssert.IsTrue (t.IsNameValid, "#2");
			ClassicAssert.IsFalse (t.IsUnknown, "#3");
			ClassicAssert.AreEqual ("Type", t.Name, "#4");
			// Note that Type is not a standard type. An instance of System.Type is usually represented as TypeExtension.
			ClassicAssert.AreEqual ("clr-namespace:System;assembly=System.Private.CoreLib", t.PreferredXamlNamespace, "#5");
			ClassicAssert.IsNull (t.TypeArguments, "#6");
			ClassicAssert.AreEqual (typeof (Type), t.UnderlyingType, "#7");
			ClassicAssert.IsTrue (t.ConstructionRequiresArguments, "#8"); // yes, true.
			ClassicAssert.IsFalse (t.IsArray, "#9");
			ClassicAssert.IsFalse (t.IsCollection, "#10");
			ClassicAssert.IsFalse (t.IsConstructible, "#11"); // yes, false.
			ClassicAssert.IsFalse (t.IsDictionary, "#12");
			ClassicAssert.IsFalse (t.IsGeneric, "#13");
			ClassicAssert.IsFalse (t.IsMarkupExtension, "#14");
			ClassicAssert.IsFalse (t.IsNameScope, "#15");
			ClassicAssert.IsTrue (t.IsNullable, "#16");
			ClassicAssert.IsTrue (t.IsPublic, "#17");
			ClassicAssert.IsFalse (t.IsUsableDuringInitialization, "#18");
			ClassicAssert.IsFalse (t.IsWhitespaceSignificantCollection, "#19");
			ClassicAssert.IsFalse (t.IsXData, "#20");
			ClassicAssert.IsFalse (t.TrimSurroundingWhitespace, "#21");
			ClassicAssert.IsFalse (t.IsAmbient, "#22");
			ClassicAssert.IsNull (t.AllowedContentTypes, "#23");
			ClassicAssert.IsNull (t.ContentWrappers, "#24");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNotNull (t.TypeConverter, "#25"); // TypeTypeConverter
#endif
			ClassicAssert.IsNull (t.ValueSerializer, "#26");
			ClassicAssert.IsNull (t.ContentProperty, "#27");
			//ClassicAssert.IsNull (t.DeferringLoader, "#28");
			ClassicAssert.IsNull (t.MarkupExtensionReturnType, "#29");
			ClassicAssert.AreEqual (sctx, t.SchemaContext, "#30");
		}

		[Test]
		public void DefaultValuesName ()
		{
			var t = new XamlType ("urn:foo", ".", null, sctx);

			ClassicAssert.IsNotNull (t.Invoker, "#1");
			ClassicAssert.IsFalse (t.IsNameValid, "#2");
			ClassicAssert.IsTrue (t.IsUnknown, "#3");
			ClassicAssert.AreEqual (".", t.Name, "#4");
			ClassicAssert.AreEqual ("urn:foo", t.PreferredXamlNamespace, "#5");
			ClassicAssert.IsNull (t.TypeArguments, "#6");
			ClassicAssert.IsNull (t.UnderlyingType, "#7");
			ClassicAssert.IsFalse (t.ConstructionRequiresArguments, "#8");
			ClassicAssert.IsFalse (t.IsArray, "#9");
			ClassicAssert.IsFalse (t.IsCollection, "#10");
			ClassicAssert.IsTrue (t.IsConstructible, "#11");
			ClassicAssert.IsFalse (t.IsDictionary, "#12");
			ClassicAssert.IsFalse (t.IsGeneric, "#13");
			ClassicAssert.IsFalse (t.IsMarkupExtension, "#14");
			ClassicAssert.IsFalse (t.IsNameScope, "#15");
			ClassicAssert.IsTrue (t.IsNullable, "#16"); // different from int
			ClassicAssert.IsTrue (t.IsPublic, "#17");
			ClassicAssert.IsFalse (t.IsUsableDuringInitialization, "#18");
			ClassicAssert.IsTrue (t.IsWhitespaceSignificantCollection, "#19"); // somehow true ...
			ClassicAssert.IsFalse (t.IsXData, "#20");
			ClassicAssert.IsFalse (t.TrimSurroundingWhitespace, "#21");
			ClassicAssert.IsFalse (t.IsAmbient, "#22");
			ClassicAssert.IsNull (t.AllowedContentTypes, "#23");
			ClassicAssert.IsNull (t.ContentWrappers, "#24");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNull (t.TypeConverter, "#25");
#endif
			ClassicAssert.IsNull (t.ValueSerializer, "#26");
			ClassicAssert.IsNull (t.ContentProperty, "#27");
			//ClassicAssert.IsNull (t.DeferringLoader, "#28");
			ClassicAssert.IsNull (t.MarkupExtensionReturnType, "#29");
			ClassicAssert.AreEqual (sctx, t.SchemaContext, "#30");
		}

		[Test]
		public void DefaultValuesCustomType ()
		{
			var t = new MyXamlType ("System.Int32", null, sctx);

			ClassicAssert.IsNotNull (t.Invoker, "#1");
			ClassicAssert.IsFalse (t.IsNameValid, "#2");
			ClassicAssert.IsTrue (t.IsUnknown, "#3");
			ClassicAssert.AreEqual ("System.Int32", t.Name, "#4");
			ClassicAssert.AreEqual (String.Empty, t.PreferredXamlNamespace, "#5");
			ClassicAssert.IsNull (t.TypeArguments, "#6");
			ClassicAssert.IsNull (t.UnderlyingType, "#7");
			ClassicAssert.IsFalse (t.ConstructionRequiresArguments, "#8");
			ClassicAssert.IsFalse (t.IsArray, "#9");
			ClassicAssert.IsFalse (t.IsCollection, "#10");
			ClassicAssert.IsTrue (t.IsConstructible, "#11");
			ClassicAssert.IsFalse (t.IsDictionary, "#12");
			ClassicAssert.IsFalse (t.IsGeneric, "#13");
			ClassicAssert.IsFalse (t.IsMarkupExtension, "#14");
			ClassicAssert.IsFalse (t.IsNameScope, "#15");
			ClassicAssert.IsTrue (t.IsNullable, "#16"); // different from int
			ClassicAssert.IsTrue (t.IsPublic, "#17");
			ClassicAssert.IsFalse (t.IsUsableDuringInitialization, "#18");
			ClassicAssert.IsTrue (t.IsWhitespaceSignificantCollection, "#19"); // somehow true ...
			ClassicAssert.IsFalse (t.IsXData, "#20");
			ClassicAssert.IsFalse (t.TrimSurroundingWhitespace, "#21");
			ClassicAssert.IsFalse (t.IsAmbient, "#22");
			ClassicAssert.IsNull (t.AllowedContentTypes, "#23");
			ClassicAssert.IsNull (t.ContentWrappers, "#24");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNull (t.TypeConverter, "#25");
#endif
			ClassicAssert.IsNull (t.ValueSerializer, "#26");
			ClassicAssert.IsNull (t.ContentProperty, "#27");
			//ClassicAssert.IsNull (t.DeferringLoader, "#28");
			ClassicAssert.IsNull (t.MarkupExtensionReturnType, "#29");
			ClassicAssert.AreEqual (sctx, t.SchemaContext, "#30");
		}

		[Ambient]
		[ContentProperty ("Name")]
		[WhitespaceSignificantCollection]
		[UsableDuringInitialization (true)]
		public class TestClass3
		{
			public TestClass3 (string name)
			{
				Name = name;
			}
			
			public string Name { get; set; }
		}

		[Test]
		public void DefaultValuesSeverlyAttributed ()
		{
			var t = new XamlType (typeof (TestClass3), sctx);
			ClassicAssert.IsNotNull (t.Invoker, "#1");
			ClassicAssert.IsFalse (t.IsNameValid, "#2"); // see #4
			ClassicAssert.IsFalse (t.IsUnknown, "#3");
			ClassicAssert.AreEqual ("XamlTypeTest+TestClass3", t.Name, "#4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, t.PreferredXamlNamespace, "#5");
			ClassicAssert.IsNull (t.TypeArguments, "#6");
			ClassicAssert.AreEqual (typeof (TestClass3), t.UnderlyingType, "#7");
			ClassicAssert.IsTrue (t.ConstructionRequiresArguments, "#8");
			ClassicAssert.IsFalse (t.IsArray, "#9");
			ClassicAssert.IsFalse (t.IsCollection, "#10");
			ClassicAssert.IsFalse (t.IsConstructible, "#11");
			ClassicAssert.IsFalse (t.IsDictionary, "#12");
			ClassicAssert.IsFalse (t.IsGeneric, "#13");
			ClassicAssert.IsFalse (t.IsMarkupExtension, "#14");
			ClassicAssert.IsFalse (t.IsNameScope, "#15");
			ClassicAssert.IsTrue (t.IsNullable, "#16");
			ClassicAssert.IsTrue (t.IsPublic, "#17");
			ClassicAssert.IsTrue (t.IsUsableDuringInitialization, "#18");
			ClassicAssert.IsTrue (t.IsWhitespaceSignificantCollection, "#19");
			ClassicAssert.IsFalse (t.IsXData, "#20");
			ClassicAssert.IsFalse (t.TrimSurroundingWhitespace, "#21");
			ClassicAssert.IsTrue (t.IsAmbient, "#22");
			ClassicAssert.IsNull (t.AllowedContentTypes, "#23");
			ClassicAssert.IsNull (t.ContentWrappers, "#24");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNull (t.TypeConverter, "#25");
#endif
			ClassicAssert.IsNull (t.ValueSerializer, "#26");
			ClassicAssert.IsNotNull (t.ContentProperty, "#27");
			ClassicAssert.AreEqual ("Name", t.ContentProperty.Name, "#27-2");
			// ClassicAssert.IsNull (t.DeferringLoader, "#28");
			ClassicAssert.IsNull (t.MarkupExtensionReturnType, "#29");
			ClassicAssert.AreEqual (sctx, t.SchemaContext, "#30");
		}

		[Test]
		public void DefaultValuesArgumentAttributed ()
		{
			var t = new XamlType (typeof (ArgumentAttributed), sctx);
			ClassicAssert.IsNotNull (t.Invoker, "#1");
			ClassicAssert.IsTrue (t.IsNameValid, "#2");
			ClassicAssert.IsFalse (t.IsUnknown, "#3");
			ClassicAssert.AreEqual ("ArgumentAttributed", t.Name, "#4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, t.PreferredXamlNamespace, "#5");
			ClassicAssert.IsNull (t.TypeArguments, "#6");
			ClassicAssert.AreEqual (typeof (ArgumentAttributed), t.UnderlyingType, "#7");
			ClassicAssert.IsTrue (t.ConstructionRequiresArguments, "#8");
			ClassicAssert.IsFalse (t.IsArray, "#9");
			ClassicAssert.IsFalse (t.IsCollection, "#10");
			ClassicAssert.IsTrue (t.IsConstructible, "#11");
			ClassicAssert.IsFalse (t.IsDictionary, "#12");
			ClassicAssert.IsFalse (t.IsGeneric, "#13");
			ClassicAssert.IsFalse (t.IsMarkupExtension, "#14");
			ClassicAssert.IsFalse (t.IsNameScope, "#15");
			ClassicAssert.IsTrue (t.IsNullable, "#16");
			ClassicAssert.IsTrue (t.IsPublic, "#17");
			ClassicAssert.IsFalse (t.IsUsableDuringInitialization, "#18");
			ClassicAssert.IsFalse (t.IsWhitespaceSignificantCollection, "#19");
			ClassicAssert.IsFalse (t.IsXData, "#20");
			ClassicAssert.IsFalse (t.TrimSurroundingWhitespace, "#21");
			ClassicAssert.IsFalse (t.IsAmbient, "#22");
			ClassicAssert.IsNull (t.AllowedContentTypes, "#23");
			ClassicAssert.IsNull (t.ContentWrappers, "#24");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNull (t.TypeConverter, "#25");
#endif
			ClassicAssert.IsNull (t.ValueSerializer, "#26");
			ClassicAssert.IsNull (t.ContentProperty, "#27");
			// ClassicAssert.IsNull (t.DeferringLoader, "#28");
			ClassicAssert.IsNull (t.MarkupExtensionReturnType, "#29");
			ClassicAssert.AreEqual (sctx, t.SchemaContext, "#30");

			var members = t.GetAllMembers ();
			ClassicAssert.AreEqual (2, members.Count, "#31");
			string [] names = {"Arg1", "Arg2"};
			foreach (var member in members)
				ClassicAssert.IsTrue (Array.IndexOf (names, member.Name) >= 0, "#32");
		}

#if HAS_TYPE_CONVERTER
		[Test]
		public void TypeConverter ()
		{
			ClassicAssert.IsNull (new XamlType (typeof (List<object>), sctx).TypeConverter, "#1");
			ClassicAssert.IsNull (new XamlType (typeof (Dictionary<object, object>), sctx).TypeConverter, "#1");
			ClassicAssert.IsNotNull (new XamlType (typeof (object), sctx).TypeConverter, "#2");
#if !WINDOWS_UWP
			ClassicAssert.IsTrue (new XamlType (typeof (Uri), sctx).TypeConverter.ConverterInstance.GetType().Name == "UriTypeConverter", "#3");
#endif
			ClassicAssert.IsTrue (new XamlType (typeof (TimeSpan), sctx).TypeConverter.ConverterInstance is TimeSpanConverter, "#4");
			ClassicAssert.IsNull (new XamlType (typeof (XamlType), sctx).TypeConverter, "#5");
			ClassicAssert.IsTrue (new XamlType (typeof (char), sctx).TypeConverter.ConverterInstance is CharConverter, "#6");
		}
				
		[Test]
		public void TypeConverter_Type ()
		{
			TypeConveter_TypeOrTypeExtension (typeof (Type));
		}
		
		[Test]
		public void TypeConverter_TypeExtension ()
		{
			TypeConveter_TypeOrTypeExtension (typeof (TypeExtension));
		}
		
		void TypeConveter_TypeOrTypeExtension (Type type)
		{
			var xtc = new XamlType (type, sctx).TypeConverter;
			ClassicAssert.IsNotNull (xtc, "#7");
			var tc = xtc.ConverterInstance;
			ClassicAssert.IsNotNull (tc, "#7-2");
			ClassicAssert.IsFalse (tc.CanConvertTo (typeof (Type)), "#7-3");
			ClassicAssert.IsFalse (tc.CanConvertTo (typeof (XamlType)), "#7-4");
			ClassicAssert.IsTrue (tc.CanConvertTo (typeof (string)), "#7-5");
			ClassicAssert.AreEqual ("{http://schemas.microsoft.com/winfx/2006/xaml}TypeExtension", tc.ConvertToString (XamlLanguage.Type), "#7-6");
			ClassicAssert.IsFalse (tc.CanConvertFrom (typeof (Type)), "#7-7");
			ClassicAssert.IsFalse (tc.CanConvertFrom (typeof (XamlType)), "#7-8");
			// .NET returns true for type == typeof(Type) case here, which does not make sense. Disabling it now.
			//ClassicAssert.IsFalse (tc.CanConvertFrom (typeof (string)), "#7-9");
			try {
				tc.ConvertFromString ("{http://schemas.microsoft.com/winfx/2006/xaml}TypeExtension");
				Assert.Fail ("failure");
			} catch (NotSupportedException) {
			}
		}

#endif

		[Test]
		public void GetXamlNamespaces ()
		{
			var xt = new XamlType (typeof (string), new XamlSchemaContext (null, null));
			var l = xt.GetXamlNamespaces ().ToList ();
			l.Sort ();
			ClassicAssert.AreEqual (2, l.Count, "#1-1");
			ClassicAssert.AreEqual ("clr-namespace:System;assembly=System.Private.CoreLib", l [0], "#1-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, l [1], "#1-3");

			xt = new XamlType (typeof (TypeExtension), new XamlSchemaContext (null, null));
			l = xt.GetXamlNamespaces ().ToList ();
			l.Sort ();
			ClassicAssert.AreEqual (3, l.Count, "#2-1");
			ClassicAssert.AreEqual ("clr-namespace:System.Windows.Markup;assembly=System.Xaml".Fixup(), l [0], "#2-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, l [1], "#2-3");
			//ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, l [2], "#2-4"); // ??

			xt = new XamlType (typeof (List<string>), new XamlSchemaContext (null, null));
			l = xt.GetXamlNamespaces ().ToList ();
			l.Sort ();
			ClassicAssert.AreEqual (1, l.Count, "#3-1");
			ClassicAssert.AreEqual ("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib", l [0], "#3-2");
		}
		
		[Test]
		public void GetAliasedProperty ()
		{
			XamlMember xm;
			var xt = new XamlType (typeof (SeverlyAliasedClass), new XamlSchemaContext (null, null));
			xm = xt.GetAliasedProperty (XamlLanguage.Key);
			ClassicAssert.IsNotNull (xm, "#1");
			xm = xt.GetAliasedProperty (XamlLanguage.Name);
			ClassicAssert.IsNotNull (xm, "#2");
			xm = xt.GetAliasedProperty (XamlLanguage.Uid);
			ClassicAssert.IsNotNull (xm, "#3");
			xm = xt.GetAliasedProperty (XamlLanguage.Lang);
			ClassicAssert.IsNotNull (xm, "#4");
			
			xt = new XamlType (typeof (Dictionary<int,string>), xt.SchemaContext);
			ClassicAssert.IsNull (xt.GetAliasedProperty (XamlLanguage.Key), "#5");
		}

		[Test]
		public void GetAliasedPropertyOnAllTypes ()
		{
			foreach (var xt in XamlLanguage.AllTypes)
				foreach (var xd in XamlLanguage.AllDirectives)
					ClassicAssert.IsNull (xt.GetAliasedProperty (xd), xt.Name + " and " + xd.Name);
		}

		[DictionaryKeyProperty ("Key")]
		[RuntimeNameProperty ("RuntimeTypeName")]
		[UidProperty ("UUID")]
		[XmlLangProperty ("XmlLang")]
		public class SeverlyAliasedClass
		{
			public string Key { get; set; }
			public string RuntimeTypeName { get; set; }
			public string UUID { get; set; }
			public string XmlLang { get; set; }
		}

		[Test]
		public void ToStringTest ()
		{
			ClassicAssert.AreEqual ("{http://schemas.microsoft.com/winfx/2006/xaml}String", XamlLanguage.String.ToString (), "#1");
			ClassicAssert.AreEqual ("{http://schemas.microsoft.com/winfx/2006/xaml}TypeExtension", XamlLanguage.Type.ToString (), "#2");
			ClassicAssert.AreEqual ("{http://schemas.microsoft.com/winfx/2006/xaml}ArrayExtension", XamlLanguage.Array.ToString (), "#3");
		}

		[Test]
		public void GetPositionalParameters ()
		{
			IList<XamlType> l;
			l = XamlLanguage.Type.GetPositionalParameters (1);
			ClassicAssert.IsNotNull (l, "#1");
			ClassicAssert.AreEqual (1, l.Count, "#2");
			ClassicAssert.AreEqual (typeof (Type), l [0].UnderlyingType, "#3"); // not TypeExtension but Type.
			ClassicAssert.AreEqual ("Type", l [0].Name, "#4");
		}

		[Test]
		public void GetPositionalParametersWrongCount ()
		{
			ClassicAssert.IsNull (XamlLanguage.Type.GetPositionalParameters (2), "#1");
		}

		[Test]
		public void GetPositionalParametersNoMemberExtension ()
		{
			// wow, so it returns some meaningless method parameters.
			ClassicAssert.IsNotNull (new XamlType (typeof (MyXamlType), sctx).GetPositionalParameters (3), "#1");
		}
		
		[Test]
		public void ListMembers ()
		{
			var xt = new XamlType (typeof (List<int>), sctx);
			var ml = xt.GetAllMembers ().ToArray ();
			ClassicAssert.AreEqual (1, ml.Length, "#1");
			ClassicAssert.IsNotNull (xt.GetMember ("Capacity"), "#2");
		}
		
		[Test]
		public void ComplexPositionalParameters ()
		{
			new XamlType (typeof (ComplexPositionalParameterWrapper), sctx);
		}
		
		[Test]
		public void CustomArrayExtension ()
		{
			var xt = new XamlType (typeof (MyArrayExtension), sctx);
			var xm = xt.GetMember ("Items");
			ClassicAssert.IsNotNull (xt.GetAllMembers ().FirstOrDefault (m => m.Name == "Items"), "#0");
			ClassicAssert.IsNotNull (xm, "#1");
			ClassicAssert.IsFalse (xm.IsReadOnly, "#2"); // Surprisingly it is False. Looks like XAML ReadOnly is true only if it lacks set accessor. Having private member does not make it ReadOnly.
			ClassicAssert.IsTrue (xm.Type.IsCollection, "#3");
			ClassicAssert.IsFalse (xm.Type.IsConstructible, "#4");
		}
		
		[Test]
		public void ContentIncluded ()
		{
			var xt = new XamlType (typeof (ContentIncludedClass), sctx);
			var xm = xt.GetMember ("Content");
			ClassicAssert.AreEqual (xm, xt.ContentProperty, "#1");
			ClassicAssert.IsTrue (xt.GetAllMembers ().Contains (xm), "#2");
		}
		
		[Test]
		public void NamedItem ()
		{
			var xt = new XamlType (typeof (NamedItem), sctx);
			var e = xt.GetAllMembers ().GetEnumerator ();
			ClassicAssert.IsTrue (e.MoveNext (), "#1");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), e.Current, "#2");
			ClassicAssert.IsTrue (e.MoveNext (), "#3");
			ClassicAssert.AreEqual (xt.GetMember ("References"), e.Current, "#4");
			ClassicAssert.IsFalse (e.MoveNext (), "#5");
		}

		[Test]
		public void CanAssignTo ()
		{
			foreach (var xt1 in XamlLanguage.AllTypes)
				foreach (var xt2 in XamlLanguage.AllTypes)
					ClassicAssert.AreEqual (xt1.UnderlyingType.IsAssignableFrom (xt2.UnderlyingType), xt2.CanAssignTo (xt1), "{0} to {1}", xt1, xt2);
			ClassicAssert.IsTrue (XamlLanguage.Type.CanAssignTo (XamlLanguage.Object), "x#1"); // specific test
			ClassicAssert.IsFalse (new MyXamlType ("MyFooBar", null, sctx).CanAssignTo (XamlLanguage.String), "x#2"); // custom type to string -> false
			ClassicAssert.IsTrue (new MyXamlType ("MyFooBar", null, sctx).CanAssignTo (XamlLanguage.Object), "x#3"); // custom type to object -> true!
		}

		[Test]
		public void IsXData ()
		{
			ClassicAssert.IsFalse (XamlLanguage.XData.IsXData, "#1"); // yes, it is false.
			ClassicAssert.IsTrue (sctx.GetXamlType (typeof (XmlSerializable)).IsXData, "#2");
		}
		
		[Test]
		public void XDataMembers ()
		{
			var xt = sctx.GetXamlType (typeof (XmlSerializableWrapper));
			ClassicAssert.IsNotNull (xt.GetMember ("Value"), "#1"); // it is read-only, so if wouldn't be retrieved if it were not XData.

			ClassicAssert.IsNotNull (XamlLanguage.XData.GetMember ("XmlReader"), "#2"); // it is returned, but ignored by XamlObjectReader.
			ClassicAssert.IsNotNull (XamlLanguage.XData.GetMember ("Text"), "#3");
		}

		[Test]
		public void AttachableProperty ()
		{
			var xt = new XamlType (typeof (Attachable), sctx);
			var apl = xt.GetAllAttachableMembers ();
			ClassicAssert.IsTrue (apl.Any (ap => ap.Name == "Foo"), "#1");
			ClassicAssert.IsTrue (apl.Any (ap => ap.Name == "Protected"), "#2");
			// oh? SetBaz() has non-void return value, but it seems ignored.
			ClassicAssert.IsTrue (apl.Any (ap => ap.Name == "Baz"), "#3");
			ClassicAssert.AreEqual (4, apl.Count, "#4");
			ClassicAssert.IsTrue (apl.All (ap => ap.IsAttachable), "#5");
			var x = apl.First (ap => ap.Name == "X");
			ClassicAssert.IsTrue (x.IsEvent, "#6");
		}

		[Test]
		public void AttachablePropertySetValueNullObject ()
		{
			var xt = new XamlType (typeof (Attachable), sctx);
			var apl = xt.GetAllAttachableMembers ();
			var foo = apl.First (ap => ap.Name == "Foo");
			ClassicAssert.IsTrue (foo.IsAttachable, "#7");
			Assert.Throws<ArgumentNullException> (() => foo.Invoker.SetValue (null, "xxx"));
		}

		[Test]
		public void AttachablePropertySetValueSuccess ()
		{
			var xt = new XamlType (typeof (Attachable), sctx);
			var apl = xt.GetAllAttachableMembers ();
			var foo = apl.First (ap => ap.Name == "Foo");
			ClassicAssert.IsTrue (foo.IsAttachable, "#7");
			var obj = new object ();
			foo.Invoker.SetValue (obj, "xxx"); // obj is non-null, so valid.
			// FIXME: this line should be unnecessary.
			AttachablePropertyServices.RemoveProperty (obj, new AttachableMemberIdentifier (foo.Type.UnderlyingType, foo.Name));
		}

		[Test]
		public void ReadOnlyPropertyContainer ()
		{
			var xt = new XamlType (typeof (ReadOnlyPropertyContainer), sctx);
			var xm = xt.GetMember ("Bar");
			ClassicAssert.IsNotNull (xm, "#1");
			ClassicAssert.IsFalse (xm.IsWritePublic, "#2");
		}

		[Test]
		public void UnknownType ()
		{
			var xt = new XamlType ("urn:foo", "MyUnknown", null, sctx);
			ClassicAssert.IsTrue (xt.IsUnknown, "#1");
			ClassicAssert.IsNotNull (xt.BaseType, "#2");
			ClassicAssert.IsFalse (xt.BaseType.IsUnknown, "#3");
			ClassicAssert.AreEqual (typeof (object), xt.BaseType.UnderlyingType, "#4");
		}

		[Test] // wrt bug #680385
		public void DerivedListMembers ()
		{
			var xt = sctx.GetXamlType (typeof (XamlTest.Configurations));
			ClassicAssert.IsTrue (xt.GetAllMembers ().Any (xm => xm.Name == "Active"), "#1"); // make sure that the member name is Active, not Configurations.Active ...
		}

		[Test]
		public void EnumType ()
		{
			var xt = sctx.GetXamlType (typeof (EnumValueType));
			ClassicAssert.IsTrue (xt.IsConstructible, "#1");
			ClassicAssert.IsFalse (xt.IsNullable, "#2");
			ClassicAssert.IsFalse (xt.IsUnknown, "#3");
			ClassicAssert.IsFalse (xt.IsUsableDuringInitialization, "#4");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNotNull (xt.TypeConverter, "#5");
#endif
		}

		[Test]
		public void CollectionContentProperty ()
		{
			var xt = sctx.GetXamlType (typeof (CollectionContentProperty));
			var p = xt.ContentProperty;
			ClassicAssert.IsNotNull (p, "#1");
			ClassicAssert.AreEqual ("ListOfItems", p.Name, "#2");
		}

		[Test]
		public void AmbientPropertyContainer ()
		{
			var xt = sctx.GetXamlType (typeof (SecondTest.ResourcesDict));
			ClassicAssert.IsTrue (xt.IsAmbient, "#1");
			var l = xt.GetAllMembers ().ToArray ();
			ClassicAssert.AreEqual (2, l.Length, "#2");
			// FIXME: enable when string representation difference become compatible
			// ClassicAssert.AreEqual ("System.Collections.Generic.Dictionary(System.Object, System.Object).Keys", l [0].ToString (), "#3");
			// ClassicAssert.AreEqual ("System.Collections.Generic.Dictionary(System.Object, System.Object).Values", l [1].ToString (), "#4");
		}

		[Test]
		public void NullableContainer ()
		{
			var xt = sctx.GetXamlType (typeof (NullableContainer));
			ClassicAssert.IsFalse (xt.IsGeneric, "#1");
			ClassicAssert.IsTrue (xt.IsNullable, "#2");
			var xm = xt.GetMember ("TestProp");
			ClassicAssert.IsTrue (xm.Type.IsGeneric, "#3");
			ClassicAssert.IsTrue (xm.Type.IsNullable, "#4");
			ClassicAssert.AreEqual ("clr-namespace:System;assembly=System.Private.CoreLib", xm.Type.PreferredXamlNamespace, "#5");
			ClassicAssert.AreEqual (1, xm.Type.TypeArguments.Count, "#6");
			ClassicAssert.AreEqual (XamlLanguage.Int32, xm.Type.TypeArguments [0], "#7");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNotNull (xm.Type.TypeConverter, "#8");
			ClassicAssert.IsNotNull (xm.Type.TypeConverter.ConverterInstance, "#9");
#endif

			var obj = new NullableContainer ();
			xm.Invoker.SetValue (obj, 5);
			xm.Invoker.SetValue (obj, null);
		}

		[Test]
		public void DerivedCollectionAndDictionary ()
		{
			var xt = sctx.GetXamlType (typeof (IList<int>));
			ClassicAssert.IsTrue (xt.IsCollection, "#1");
			ClassicAssert.IsFalse (xt.IsDictionary, "#2");
			xt = sctx.GetXamlType (typeof (IDictionary<EnumValueType,int>));
			ClassicAssert.IsTrue (xt.IsDictionary, "#3");
			ClassicAssert.IsFalse (xt.IsCollection, "#4");
		}

		[Test]
		public void NullableTypeShouldUseProperValueSerializer()
		{
			var val = DateTime.Today;
			var xt = sctx.GetXamlType(typeof(DateTime?));
			ClassicAssert.IsTrue(xt.IsNullable, "#1");
			ClassicAssert.AreEqual(xt.BaseType, sctx.GetXamlType(typeof(ValueType)), "#2");
			ClassicAssert.IsNull(xt.ValueSerializer, "#3");
#if HAS_TYPE_CONVERTER
			ClassicAssert.IsNotInstanceOf<DateTimeConverter>(xt.TypeConverter.ConverterInstance, "#4");
#if PCL
			ClassicAssert.IsInstanceOf(typeof(global::System.Xaml.XamlSchemaContext).Assembly.GetType("System.Xaml.ComponentModel.PortableXamlDateTimeConverter"), xt.TypeConverter.ConverterInstance, "#4");
#endif
#endif
		}

		[Test]
		public void BaseClassPropertiesShouldHaveProperNamespaces()
		{
			var xtnamebase = sctx.GetXamlType(typeof(TestClass5WithName));
			var xtderived = sctx.GetXamlType(typeof(NamespaceTest2.TestClassWithDifferentBaseNamespace));
			ClassicAssert.IsNotNull(xtderived);
			var xmname = xtderived.GetMember("TheName");
			ClassicAssert.IsNotNull(xmname);
            ClassicAssert.AreSame(xtnamebase, xmname.TargetType);
            ClassicAssert.AreSame(xtnamebase, xmname.DeclaringType);
			// note that the preferred namespace of the name member does not reflect the type we got the 
			// member from, but the type it is declared on.
			ClassicAssert.AreEqual(xtnamebase.PreferredXamlNamespace, xmname.PreferredXamlNamespace);

            ClassicAssert.AreSame(xmname, xtderived.GetAliasedProperty(XamlLanguage.Name));
			// not important: Assert.AreNotSame(xmname, xtnamebase.GetAliasedProperty(XamlLanguage.Name));
			ClassicAssert.AreEqual(xmname, xtnamebase.GetAliasedProperty(XamlLanguage.Name));
		}
	}

	class MyXamlType : XamlType
	{
		public MyXamlType (string fullName, IList<XamlType> typeArguments, XamlSchemaContext context)
			: base (fullName, typeArguments, context)
		{
		}
	}
}

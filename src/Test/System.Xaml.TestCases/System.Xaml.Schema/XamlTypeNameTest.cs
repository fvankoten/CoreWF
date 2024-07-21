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
	[TestFixture]
	public class XamlTypeNameTest
	{
		[Test]
		public void ConstructorDefault ()
		{
			var xtn = new XamlTypeName ();
			ClassicAssert.IsNotNull (xtn.TypeArguments, "#1");
		}

		[Test]
		public void ConstructorXamlTypeNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlTypeName (null));
		}

		[Test]
		public void ConstructorNameNull ()
		{
			// allowed.
			var xtn = new XamlTypeName ("urn:foo", null);
			ClassicAssert.IsNotNull (xtn.TypeArguments, "#1");
		}

		[Test]
		public void ConstructorNamespaceNull ()
		{
			// allowed.
			var xtn = new XamlTypeName (null, "FooBar");
			ClassicAssert.IsNotNull (xtn.TypeArguments, "#1");
		}

		[Test]
		public void ConstructorName ()
		{
			var n = new XamlTypeName ("urn:foo", "FooBar");
			ClassicAssert.IsNotNull (n.TypeArguments, "#1");
			ClassicAssert.AreEqual (0, n.TypeArguments.Count, "#2");
		}

		[Test]
		public void ConstructorTypeArgumentsNull ()
		{
			var n = new XamlTypeName ("urn:foo", "FooBar", (XamlTypeName []) null);
			ClassicAssert.IsNotNull (n.TypeArguments, "#1");
			ClassicAssert.AreEqual (0, n.TypeArguments.Count, "#2");
		}

		[Test]
		public void ConstructorTypeArgumentsNullEntry()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore(".NET causes NRE on ToString().It is not really intended and should raise an error when constructed");
			Assert.Throws<ArgumentNullException> (() => {
				var type = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] { null });
				Assert.DoesNotThrow (() => type.ToString ());
			});
		}

		[Test]
		public void ConstructorTypeArguments ()
		{
			new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] {new XamlTypeName ("urn:bar", "FooBarBaz")});
		}

		[Test]
		public void ConstructorTypeArgumentsEmpty ()
		{
			var n = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [0]);
			ClassicAssert.IsNotNull (n.TypeArguments, "#1");
			ClassicAssert.AreEqual (0, n.TypeArguments.Count, "#2");
		}

		[Test]
		public void ToStringDefault ()
		{
			var n = new XamlTypeName ();
			Assert.Throws<InvalidOperationException> (() => n.ToString ());
		}

		[Test]
		public void ToStringNameNull ()
		{
			var n = new XamlTypeName ("urn:foo", null);
			Assert.Throws<InvalidOperationException> (() => n.ToString ());
		}

		[Test]
		public void ToStringNamespaceNull ()
		{
			// allowed.
			var n = new XamlTypeName (null, "FooBar");
			Assert.Throws<InvalidOperationException> (() => n.ToString ());
		}

		[Test]
		public void ToStringTypeArgumentsNull ()
		{
			var n = new XamlTypeName ("urn:foo", "FooBar", (XamlTypeName []) null);
			ClassicAssert.AreEqual ("{urn:foo}FooBar", n.ToString (), "#1");
		}

		[Test]
		public void ToStringTypeArgumentsNullEntry ()
		{
			#if PCL
			Assert.Throws<ArgumentNullException> (() => {
			#else
			Assert.Throws<NullReferenceException> (() => {
			#endif
				var n = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] { null, new XamlTypeName ("urn:bar", "FooBarBaz") });
				ClassicAssert.AreEqual ("{urn:foo}FooBar()", n.ToString (), "#1");
			});
		}

		[Test]
		public void ToStringTypeArguments ()
		{
			var n = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] {new XamlTypeName ("urn:bar", "FooBarBaz")});
			ClassicAssert.AreEqual ("{urn:foo}FooBar({urn:bar}FooBarBaz)", n.ToString (), "#1");
		}

		[Test]
		public void ToStringTypeArguments2 ()
		{
			var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			ClassicAssert.AreEqual ("{urn:foo}Foo({urn:bar}Bar, {urn:baz}Baz)", n.ToString (), "#1");
		}

		[Test]
		public void ToStringEmptyNamespace ()
		{
			var n = new XamlTypeName (string.Empty, "Foo");
			ClassicAssert.AreEqual ("{}Foo", n.ToString (), "#1");
		}

		[Test]
		public void ToStringXamlTypePredefined ()
		{
			var n = new XamlTypeName (XamlLanguage.Int32);
			ClassicAssert.AreEqual ("{http://schemas.microsoft.com/winfx/2006/xaml}Int32", n.ToString (), "#1");
		}

		[Test]
		public void ToStringNamespaceLookupInsufficient ()
		{
			var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			var lookup = new MyNamespaceLookup ();
			lookup.Add ("a", "urn:foo");
			lookup.Add ("c", "urn:baz");
			// it fails because there is missing mapping for urn:bar.
			Assert.Throws<InvalidOperationException> (() => n.ToString (lookup), "#1");
		}

		[Test]
		public void ToStringNullLookup ()
		{
			var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			ClassicAssert.AreEqual ("{urn:foo}Foo({urn:bar}Bar, {urn:baz}Baz)", n.ToString (null), "#1");
		}

		[Test]
		public void ToStringNamespaceLookup ()
		{
			var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			var lookup = new MyNamespaceLookup ();
			lookup.Add ("a", "urn:foo");
			lookup.Add ("b", "urn:bar");
			lookup.Add ("c", "urn:baz");
			ClassicAssert.AreEqual ("a:Foo(b:Bar, c:Baz)", n.ToString (lookup), "#1");
			ClassicAssert.AreEqual ("b:Bar, c:Baz", XamlTypeName.ToString (n.TypeArguments, lookup), "#2");
		}

		// This test shows that MarkupExtension names are not replaced at XamlTypeName.ToString(), while XamlXmlWriter writes like "x:Null".
		[Test]
		public void ToStringNamespaceLookup2 ()
		{
			var lookup = new MyNamespaceLookup ();
			lookup.Add ("x", XamlLanguage.Xaml2006Namespace);
			ClassicAssert.AreEqual ("x:NullExtension", new XamlTypeName (XamlLanguage.Null).ToString (lookup), "#1");
			// WHY is TypeExtension not the case?
			//ClassicAssert.AreEqual ("x:TypeExtension", new XamlTypeName (XamlLanguage.Type).ToString (lookup), "#2");
			ClassicAssert.AreEqual ("x:ArrayExtension", new XamlTypeName (XamlLanguage.Array).ToString (lookup), "#3");
			ClassicAssert.AreEqual ("x:StaticExtension", new XamlTypeName (XamlLanguage.Static).ToString (lookup), "#4");
			ClassicAssert.AreEqual ("x:Reference", new XamlTypeName (XamlLanguage.Reference).ToString (lookup), "#5");
		}

		[Test]
		public void StaticToStringNullLookup ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ToString (new XamlTypeName [] {new XamlTypeName ("urn:foo", "bar")}, null));
		}

		[Test]
		public void StaticToStringNullTypeNameList ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ToString (null, new MyNamespaceLookup ()));
		}

		[Test]
		public void StaticToStringEmptyArray ()
		{
			ClassicAssert.AreEqual ("", XamlTypeName.ToString (new XamlTypeName [0], new MyNamespaceLookup ()), "#1");
		}

		class MyNamespaceLookup : INamespacePrefixLookup
		{
			Dictionary<string,string> dic = new Dictionary<string,string> ();

			public void Add (string prefix, string ns)
			{
				dic [ns] = prefix;
			}

			public string LookupPrefix (string ns)
			{
				string p;
				return dic.TryGetValue (ns, out p) ? p : null;
			}
		}

		XamlTypeName dummy;

		[Test]
		public void TryParseNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.TryParse (null, new MyNSResolver (), out dummy));
		}

		[Test]
		public void TryParseNullResolver ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.TryParse ("Foo", null, out dummy));
		}

		[Test]
		public void TryParseEmptyName ()
		{
			ClassicAssert.IsFalse (XamlTypeName.TryParse (String.Empty, new MyNSResolver (), out dummy), "#1");
		}

		[Test]
		public void TryParseColon ()
		{
			var r = new MyNSResolver ();
			r.Add ("a", "urn:foo");
			ClassicAssert.IsFalse (XamlTypeName.TryParse (":", r, out dummy), "#1");
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("a:", r, out dummy), "#2");
			ClassicAssert.IsFalse (XamlTypeName.TryParse (":b", r, out dummy), "#3");
		}

		[Test]
		public void TryParseInvalidName ()
		{
			var r = new MyNSResolver ();
			r.Add ("a", "urn:foo");
			r.Add ("#", "urn:bar");
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("$%#___!", r, out dummy), "#1");
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("a:#$#", r, out dummy), "#2");
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("#:foo", r, out dummy), "#3");
		}

		[Test]
		public void TryParseNoFillEmpty ()
		{
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("Foo", new MyNSResolver (true), out dummy), "#1");
		}

		[Test]
		public void TryParseFillEmpty ()
		{
			var r = new MyNSResolver ();
			ClassicAssert.IsTrue (XamlTypeName.TryParse ("Foo", r, out dummy), "#1");
			ClassicAssert.IsNotNull (dummy, "#2");
			ClassicAssert.AreEqual (String.Empty, dummy.Namespace, "#2-2");
			ClassicAssert.AreEqual ("Foo", dummy.Name, "#2-3");
		}

		[Test]
		public void TryParseAlreadyQualified ()
		{
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("{urn:foo}Foo", new MyNSResolver (), out dummy), "#1");
		}

		[Test]
		public void TryParseResolveFailure ()
		{
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("x:Foo", new MyNSResolver (), out dummy), "#1");
		}

		[Test]
		public void TryParseResolveSuccess ()
		{
			var r = new MyNSResolver ();
			r.Add ("x", "urn:foo");
			ClassicAssert.IsTrue (XamlTypeName.TryParse ("x:Foo", r, out dummy), "#1");
			ClassicAssert.IsNotNull (dummy, "#2");
			ClassicAssert.AreEqual ("urn:foo", dummy.Namespace, "#2-2");
			ClassicAssert.AreEqual ("Foo", dummy.Name, "#2-3");
		}

		[Test]
		public void TryParseInvalidGenericName ()
		{
			var r = new MyNSResolver ();
			r.Add ("x", "urn:foo");
			ClassicAssert.IsFalse (XamlTypeName.TryParse ("x:Foo()", r, out dummy), "#1");
		}

		[Test]
		public void TryParseGenericName ()
		{
			var r = new MyNSResolver ();
			r.Add ("x", "urn:foo");
			ClassicAssert.IsTrue (XamlTypeName.TryParse ("x:Foo(x:Foo,x:Bar)", r, out dummy), "#1");
			ClassicAssert.AreEqual (2, dummy.TypeArguments.Count, "#2");
		}

		[Test]
		public void ParseListNullNames ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ParseList (null, new MyNSResolver ()));
		}

		[Test]
		public void ParseListNullResolver ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ParseList ("foo", null));
		}

		[Test]
		public void ParseListInvalid ()
		{
			Assert.Throws<FormatException> (() => XamlTypeName.ParseList ("foo bar", new MyNSResolver ()));
		}

		[Test]
		public void ParseListInvalid2 ()
		{
			Assert.Throws<FormatException> (() => XamlTypeName.ParseList ("foo,", new MyNSResolver ()));
		}

		[Test]
		public void ParseListInvalid3 ()
		{
			Assert.Throws<FormatException> (() => XamlTypeName.ParseList ("", new MyNSResolver ()));
		}

		[Test]
		public void ParseListValid ()
		{
			var l = XamlTypeName.ParseList ("foo,  bar", new MyNSResolver ());
			ClassicAssert.AreEqual (2, l.Count, "#1");
			ClassicAssert.AreEqual ("{}foo", l [0].ToString (), "#2");
			ClassicAssert.AreEqual ("{}bar", l [1].ToString (), "#3");
			l = XamlTypeName.ParseList ("foo,bar", new MyNSResolver ());
			ClassicAssert.AreEqual ("{}foo", l [0].ToString (), "#4");
			ClassicAssert.AreEqual ("{}bar", l [1].ToString (), "#5");
		}
		
		[Test]
		public void GenericArrayName ()
		{
			var ns = new MyNSResolver ();
			ns.Add ("s", "urn:foo");
			var xn = XamlTypeName.Parse ("s:Nullable(s:Int32)[,,]", ns);
			ClassicAssert.AreEqual ("urn:foo", xn.Namespace, "#1");
			// note that array suffix comes here.
			ClassicAssert.AreEqual ("Nullable[,,]", xn.Name, "#2");
			// note that array suffix is detached from Name and appended after generic type arguments.
			ClassicAssert.AreEqual ("{urn:foo}Nullable({urn:foo}Int32)[,,]", xn.ToString (), "#3");
		}

		[Test]
		public void GenericGenericName ()
		{
			var ns = new MyNSResolver ();
			ns.Add ("s", "urn:foo");
			ns.Add ("", "urn:bar");
			ns.Add ("x", XamlLanguage.Xaml2006Namespace);
			var xn = XamlTypeName.Parse ("List(KeyValuePair(x:Int32, s:DateTime))", ns);
			ClassicAssert.AreEqual ("urn:bar", xn.Namespace, "#1");
			ClassicAssert.AreEqual ("List", xn.Name, "#2");
			ClassicAssert.AreEqual ("{urn:bar}List({urn:bar}KeyValuePair({http://schemas.microsoft.com/winfx/2006/xaml}Int32, {urn:foo}DateTime))", xn.ToString (), "#3");
		}

		class MyNSResolver : IXamlNamespaceResolver
		{
			public MyNSResolver ()
				: this (false)
			{
			}

			public MyNSResolver (bool returnNullForEmpty)
			{
				if (!returnNullForEmpty)
					dic.Add (String.Empty, String.Empty);
			}

			Dictionary<string,string> dic = new Dictionary<string,string> ();

			public void Add (string prefix, string ns)
			{
				dic [prefix] = ns;
			}

			public string GetNamespace (string prefix)
			{
				string ns;
				return dic.TryGetValue (prefix, out ns) ? ns : null;
			}
			
			public IEnumerable<NamespaceDeclaration> GetNamespacePrefixes ()
			{
				foreach (var p in dic)
					yield return new NamespaceDeclaration (p.Value, p.Key);
			}
		}
	}
}

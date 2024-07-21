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
using System.IO;
using System.Windows.Markup;
using NUnit.Framework.Legacy;

#if PCL
using System.Xaml;
using System.Xaml.Schema;
#else
using System.Xaml;
using System.Xaml.Schema;
#endif

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlSchemaContextTest
	{
		XamlSchemaContext NewStandardContext ()
		{
			return new XamlSchemaContext (new Assembly [] {typeof (XamlSchemaContext).GetTypeInfo().Assembly });
		}

		XamlSchemaContext NewThisAssemblyContext ()
		{
			return new XamlSchemaContext (new Assembly [] {GetType ().GetTypeInfo().Assembly });
		}

		[Test]
		public void ConstructorNullAssemblies ()
		{
			// allowed.
			var ctx = new XamlSchemaContext ((Assembly []) null);
			ClassicAssert.IsFalse (ctx.FullyQualifyAssemblyNamesInClrNamespaces, "#1");
			ClassicAssert.IsFalse (ctx.SupportMarkupExtensionsWithDuplicateArity, "#2");
			ClassicAssert.IsNull (ctx.ReferenceAssemblies, "#3");
		}

		[Test]
		public void ConstructorNullSettings ()
		{
			// allowed.
			new XamlSchemaContext ((XamlSchemaContextSettings) null);
		}

		[Test]
		public void ConstructorNoAssembly ()
		{
			new XamlSchemaContext (new Assembly [0]);
		}

		[Test]
		public void Constructor ()
		{
			var ctx = new XamlSchemaContext (new Assembly [] {typeof (XamlSchemaContext).GetTypeInfo().Assembly });
			ClassicAssert.AreEqual (1, ctx.ReferenceAssemblies.Count, "#1");
		}

		[Test]
		public void GetAllXamlNamespaces ()
		{
			var ctx = new XamlSchemaContext (null, null);
			var arr = ctx.GetAllXamlNamespaces ().ToArray ();
			ClassicAssert.AreEqual (6, arr.Length, "#1");
			ClassicAssert.IsTrue (arr.Contains (XamlLanguage.Xaml2006Namespace), "#1-2");
			ClassicAssert.IsTrue (arr.Contains ("urn:mono-test"), "#1-3");
            ClassicAssert.IsTrue(arr.Contains("urn:mono-test2"), "#1-4");
            ClassicAssert.IsTrue(arr.Contains("urn:bar"), "#1-5");

            ctx = NewStandardContext ();
			arr = ctx.GetAllXamlNamespaces ().ToArray ();
			ClassicAssert.AreEqual (1, arr.Length, "#2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, arr [0], "#2-2");

			ctx = NewThisAssemblyContext ();
			arr = ctx.GetAllXamlNamespaces ().ToArray ();
			ClassicAssert.AreEqual (5, arr.Length, "#3");
			ClassicAssert.IsTrue (arr.Contains ("urn:mono-test"), "#3-2");
			ClassicAssert.IsTrue (arr.Contains ("urn:mono-test2"), "#3-3");
            ClassicAssert.IsTrue(arr.Contains("urn:bar"), "#3-4");
        }

        [Test]
		public void GetPreferredPrefixNull ()
		{
			var ctx = new XamlSchemaContext (null, null);
			Assert.Throws<ArgumentNullException> (() => ctx.GetPreferredPrefix (null));
		}

		[Test]
		public void GetPreferredPrefix ()
		{
			var ctx = new XamlSchemaContext (null, null);
			ClassicAssert.AreEqual ("x", ctx.GetPreferredPrefix (XamlLanguage.Xaml2006Namespace), "#1");
			ClassicAssert.AreEqual ("p", ctx.GetPreferredPrefix ("urn:4mbw93w89mbh"), "#2"); // ... WTF "p" ?
			ClassicAssert.AreEqual ("p", ctx.GetPreferredPrefix ("urn:etbeoesmj"), "#3"); // ... WTF "p" ?
		}

		[Test]
		public void TryGetCompatibleXamlNamespaceNull ()
		{
			var ctx = new XamlSchemaContext (null, null);
			string dummy;
			Assert.Throws<ArgumentNullException> (() => ctx.TryGetCompatibleXamlNamespace (null, out dummy));
		}

		[Test]
		public void TryGetCompatibleXamlNamespace ()
		{
			var ctx = new XamlSchemaContext (null, null);
			string dummy;
			ClassicAssert.IsFalse (ctx.TryGetCompatibleXamlNamespace (String.Empty, out dummy), "#1");
			ClassicAssert.IsNull (dummy, "#1-2"); // this shows the fact that the out result value for false case is not trustworthy.

			ctx = NewThisAssemblyContext ();
			ClassicAssert.IsFalse (ctx.TryGetCompatibleXamlNamespace (String.Empty, out dummy), "#2");

            // test XmlnsCompatibleWith when subsuming namespace is defined, should find both.
            ClassicAssert.IsTrue (ctx.TryGetCompatibleXamlNamespace ("urn:bar", out dummy), "#3");
			ClassicAssert.IsTrue (ctx.TryGetCompatibleXamlNamespace ("urn:foo", out dummy), "#4");
			ClassicAssert.AreEqual ("urn:bar", dummy, "#5");

            // should not find a compatible namespace when XmlnsCompatibleWith is used with undefined subsuming namespace
            ClassicAssert.IsFalse(ctx.TryGetCompatibleXamlNamespace("urn:bar2", out dummy), "#6");
            ClassicAssert.IsFalse(ctx.TryGetCompatibleXamlNamespace("urn:foo2", out dummy), "#7");
        }

        /*
                    var settings = new XamlSchemaContextSettings () { FullyQualifyAssemblyNamesInClrNamespaces = true };
                    ctx = new XamlSchemaContext (new Assembly [] {typeof (XamlSchemaContext).Assembly }, settings);

                    ctx = new XamlSchemaContext (new Assembly [] {GetType ().Assembly }, settings);
                    arr = ctx.GetAllXamlNamespaces ().ToArray ();
                    ClassicAssert.AreEqual (2, arr.Length, "#5");
                    ClassicAssert.IsTrue (arr.Contains ("urn:mono-test"), "#5-2");
                    ClassicAssert.IsTrue (arr.Contains ("urn:mono-test2"), "#5-3");
                }
        */

        [Test]
		public void GetXamlTypeAndAllXamlTypes ()
		{
			var ctx = new XamlSchemaContext (new Assembly [] {typeof (string).GetTypeInfo().Assembly }); // build with corlib.
			ClassicAssert.AreEqual (0, ctx.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace).Count (), "#0"); // premise

			var xt = ctx.GetXamlType (typeof (string));
			ClassicAssert.IsNotNull (xt, "#1");
			ClassicAssert.AreEqual (typeof (string), xt.UnderlyingType, "#2");
			ClassicAssert.IsTrue (object.ReferenceEquals (xt, ctx.GetXamlType (typeof (string))), "#3");

			// non-primitive type example
			ClassicAssert.IsTrue (object.ReferenceEquals (ctx.GetXamlType (GetType ()), ctx.GetXamlType (GetType ())), "#4");

			// after getting these types, it still returns 0. So it's not all about caching.
			ClassicAssert.AreEqual (0, ctx.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace).Count (), "#5");
		}

		[Test]
		public void AddGetAllXamlTypesToEmpty ()
		{
			var ctx = NewStandardContext ();
			Assert.Throws<NotSupportedException> (() => ctx.GetAllXamlTypes ("urn:foo").Add (new XamlType (typeof (int), ctx)));
		}

		[Test]
		public void GetAllXamlTypesInXaml2006Namespace ()
		{
			var ctx = NewStandardContext ();

			// There are some special types that have non-default name: MemberDefinition, PropertyDefinition

			var l = ctx.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace);
			ClassicAssert.IsTrue (l.Count () > 40, "#1");
			ClassicAssert.IsTrue (l.Any (t => t.UnderlyingType == typeof (MemberDefinition)), "#2");
			ClassicAssert.IsTrue (l.Any (t => t.Name == "AmbientAttribute"), "#3");
			ClassicAssert.IsTrue (l.Any (t => t.Name == "XData"), "#4");
			ClassicAssert.IsTrue (l.Any (t => t.Name == "ArrayExtension"), "#5");
			ClassicAssert.IsTrue (l.Any (t => t.Name == "StaticExtension"), "#6");
			// FIXME: enable these tests when I sort out how these special names are filled.
			//ClassicAssert.IsTrue (l.Any (t => t.Name == "Member"), "#7");
			//ClassicAssert.IsTrue (l.Any (t => t.Name == "Property"), "#8");
			//ClassicAssert.IsFalse (l.Any (t => t.Name == "MemberDefinition"), "#9");
			//ClassicAssert.IsFalse (l.Any (t => t.Name == "PropertyDefinition"), "#10");
			//ClassicAssert.AreEqual ("MemberDefinition", new XamlType (typeof (MemberDefinition), new XamlSchemaContext (null, null)).Name);
			//ClassicAssert.AreEqual ("Member", l.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace).First (t => t.UnderlyingType == typeof (MemberDefinition)));
			ClassicAssert.IsFalse (l.Any (t => t.Name == "Array"), "#11");
			ClassicAssert.IsFalse (l.Any (t => t.Name == "Null"), "#12");
			ClassicAssert.IsFalse (l.Any (t => t.Name == "Static"), "#13");
			ClassicAssert.IsFalse (l.Any (t => t.Name == "Type"), "#14");
			ClassicAssert.IsTrue (l.Contains (XamlLanguage.Type), "#15");
			ClassicAssert.IsFalse (l.Contains (XamlLanguage.String), "#16"); // huh?
			ClassicAssert.IsFalse (l.Contains (XamlLanguage.Object), "#17"); // huh?
			ClassicAssert.IsTrue (l.Contains (XamlLanguage.Array), "#18");
			ClassicAssert.IsFalse (l.Contains (XamlLanguage.Uri), "#19");
		}

		[Test]
		public void GetXamlTypeByName ()
		{
			var ns = XamlLanguage.Xaml2006Namespace;
			var ctx = NewThisAssemblyContext ();
			//var ctx = NewStandardContext ();
			XamlType xt;

			ClassicAssert.IsNull (ctx.GetXamlType (new XamlTypeName ("urn:foobarbaz", "bar")));

			xt = ctx.GetXamlType (new XamlTypeName (ns, "Int32"));
			ClassicAssert.IsNotNull (xt, "#1");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Int32", new XamlTypeName [] {new XamlTypeName (ns, "Int32")}));
			ClassicAssert.IsNull (xt, "#1-2");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Uri"));
			ClassicAssert.IsNotNull (xt, "#2");

			// Compare those results to GetAllXamlTypesInXaml2006Namespace() results,
			// which asserts that types with those names are *not* included.
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Array"));
			ClassicAssert.IsNotNull (xt, "#3");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Property"));
			ClassicAssert.IsNotNull (xt, "#4");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Null"));
			ClassicAssert.IsNotNull (xt, "#5");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Static"));
			ClassicAssert.IsNotNull (xt, "#6");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Type"));
			ClassicAssert.IsNotNull (xt, "#7");
		}

		[Test]
		public void GetTypeForRuntimeType ()
		{
			var ctx = NewStandardContext ();

			// There are some special types that have non-default name: MemberDefinition, PropertyDefinition

			var xt = ctx.GetXamlType (typeof (Type));
			ClassicAssert.AreEqual ("Type", xt.Name, "#1-1");
			ClassicAssert.AreEqual (typeof (Type), xt.UnderlyingType, "#1-2");

			xt = ctx.GetXamlType (new XamlTypeName (XamlLanguage.Xaml2006Namespace, "Type")); // becomes TypeExtension, not Type
			ClassicAssert.AreEqual ("TypeExtension", xt.Name, "#2-1");
			ClassicAssert.AreEqual (typeof (TypeExtension), xt.UnderlyingType, "#2-2");
		}

		[TestCase(typeof(bool))]
		[TestCase(typeof(byte))]
		[TestCase(typeof(char))]
		[TestCase(typeof(DateTime))]
		[TestCase(typeof(decimal))]
		[TestCase(typeof(double))]
		[TestCase(typeof(Int16))]
		[TestCase(typeof(Int32))]
		[TestCase(typeof(Int64))]
		[TestCase(typeof(float))]
		[TestCase(typeof(string))]
		[TestCase(typeof(TimeSpan))]
		public void GetTypeFromXamlTypeNameWithClrName (Type type)
		{
			// ensure that this does *not* resolve clr type name.
			var xn = new XamlTypeName ("clr-namespace:System;assembly=mscorlib", type.Name);
			var ctx = NewStandardContext ();
			var xt = ctx.GetXamlType (xn);
			ClassicAssert.IsNull (xt, "#1");

			ctx = new XamlSchemaContext ();
			xt = ctx.GetXamlType (xn);
			ClassicAssert.IsNotNull (xt, "#2");
		}

		[Test]
		public void GetAbstractType()
		{
			var ctx = new XamlSchemaContext ();
			var xt = ctx.GetXamlType (typeof(AbstractObject));
			ClassicAssert.IsNotNull (xt, "#1");
		}

		[Test]
		public void GetAbstractTypeFromClrNamespace()
		{
			var ctx = new XamlSchemaContext();
			var tn = new XamlTypeName(Compat.TestAssemblyNamespace, "AbstractObject");
			var xt = ctx.GetXamlType(tn);
			ClassicAssert.IsNotNull(xt, "#1");
			ClassicAssert.IsNotNull(xt.UnderlyingType, "#2");
		}

		[Test]
		public void GetAbstractTypeFromUriNamespace()
		{
			var ctx = new XamlSchemaContext();
			var tn = new XamlTypeName("urn:mono-test", "AbstractObject");
			var xt = ctx.GetXamlType(tn);
			ClassicAssert.IsNotNull(xt, "#1");
			ClassicAssert.IsNotNull(xt.UnderlyingType, "#2");
		}

		[Test]
		public void AttachableMemberTypeShouldBeCorrectWhenReadOnly()
		{
			var ctx = new XamlSchemaContext();
			var xt = ctx.GetXamlType(typeof(AttachedWrapper4));
			ClassicAssert.IsNotNull(xt, "#1");
			var xm = xt.GetAttachableMember("SomeCollection");
			ClassicAssert.IsNotNull(xm, "#2");
			ClassicAssert.AreEqual(typeof(List<TestClass4>), xm.Type.UnderlyingType, "#3");
		}
		[Test]
		public void AttachableMemberTypeShouldBeCorrect()
		{
			var ctx = new XamlSchemaContext();
			var xt = ctx.GetXamlType(typeof(AttachedWrapper5));
			ClassicAssert.IsNotNull(xt, "#1");
			var xm = xt.GetAttachableMember("SomeCollection");
			ClassicAssert.IsNotNull(xm, "#2");
			ClassicAssert.AreEqual(typeof(List<TestClass4>), xm.Type.UnderlyingType, "#3");
		}

		[Test]
		public void PassesNullToGetXamlType_typeArguments_ForNoArguments()
		{
			var xml = File.ReadAllText(Compat.GetTestFile("Int32.xml")).UpdateXml();
			var ctx = new TestGetXamlTypeArgumentsNull();
			var reader = new XamlXmlReader(new StringReader(xml), ctx);
			var writer = new XamlObjectWriter(ctx);

			XamlServices.Transform(reader, writer);

			ClassicAssert.True(ctx.Invoked);
		}

		private class TestGetXamlTypeArgumentsNull : XamlSchemaContext
		{
			public bool Invoked { get; set; }

			protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
			{
				ClassicAssert.IsNull(typeArguments);
				Invoked = true;
				return base.GetXamlType(xamlNamespace, name, typeArguments);
			}
		}
	}
}

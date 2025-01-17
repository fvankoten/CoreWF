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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using System.Windows.Markup;
#if PCL

using System.Xaml;
using System.Xaml.Schema;
#else
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
#endif

using CategoryAttribute = NUnit.Framework.CategoryAttribute;
using XamlReader = System.Xaml.XamlReader;
using NUnit.Framework.Legacy;

// Some test result remarks:
// - TypeExtension: [ConstructorArgument] -> PositionalParameters
// - StaticExtension: almost identical to TypeExtension
// - Reference: [ConstructorArgument], [ContentProperty] -> only ordinal member.
// - ArrayExtension: [ConstrutorArgument], [ContentProperty] -> no PositionalParameters, Items.
// - NullExtension: no member.
// - MyExtension: [ConstructorArgument] -> only ordinal members...hmm?

namespace MonoTests.System.Xaml
{
	public partial class XamlReaderTestBase
	{
		protected void Read_String (XamlReader r)
		{
			ClassicAssert.AreEqual (XamlNodeType.None, r.NodeType, "#1");
			ClassicAssert.IsNull (r.Member, "#2");
			ClassicAssert.IsNull (r.Namespace, "#3");
			ClassicAssert.IsNull (r.Member, "#4");
			ClassicAssert.IsNull (r.Type, "#5");
			ClassicAssert.IsNull (r.Value, "#6");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			ClassicAssert.IsNotNull (r.Type, "#23");
			ClassicAssert.AreEqual (new XamlType (typeof (string), r.SchemaContext), r.Type, "#23-2");
			ClassicAssert.IsNull (r.Namespace, "#25");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.IsNotNull (r.Member, "#33");
			ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "#33-2");
			ClassicAssert.IsNull (r.Type, "#34");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("foo", r.Value, "#43");
			ClassicAssert.IsNull (r.Member, "#44");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");
			ClassicAssert.IsNull (r.Type, "#53");
			ClassicAssert.IsNull (r.Member, "#54");

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#62");
			ClassicAssert.IsNull (r.Type, "#63");

			ClassicAssert.IsFalse (r.Read (), "#71");
			ClassicAssert.IsTrue (r.IsEof, "#72");
		}

		protected void WriteNullMemberAsObject (XamlReader r, Action validateNullInstance)
		{
			ClassicAssert.AreEqual (XamlNodeType.None, r.NodeType, "#1");
			ClassicAssert.IsTrue (r.Read (), "#6");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#7");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "#7-2");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "#7-3");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#12-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#12-3");

			ClassicAssert.IsTrue (r.Read (), "#16");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#17");
			var xt = new XamlType (typeof (TestClass4), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#17-2");
//			ClassicAssert.IsTrue (r.Instance is TestClass4, "#17-3");
			ClassicAssert.AreEqual (2, xt.GetAllMembers ().Count, "#17-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#22");
			ClassicAssert.AreEqual (xt.GetMember ("Bar"), r.Member, "#22-2");

			ClassicAssert.IsTrue (r.Read (), "#26");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#27");
			ClassicAssert.AreEqual (XamlLanguage.Null, r.Type, "#27-2");
			if (validateNullInstance != null)
				validateNullInstance ();

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#32");

			ClassicAssert.IsTrue (r.Read (), "#36");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#37");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#42");
			ClassicAssert.AreEqual (xt.GetMember ("Foo"), r.Member, "#42-2");

			ClassicAssert.IsTrue (r.Read (), "#43");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#43-2");
			ClassicAssert.AreEqual (XamlLanguage.Null, r.Type, "#43-3");
			if (validateNullInstance != null)
				validateNullInstance ();

			ClassicAssert.IsTrue (r.Read (), "#44");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#44-2");

			ClassicAssert.IsTrue (r.Read (), "#46");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#47");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#52");

			ClassicAssert.IsFalse (r.Read (), "#56");
			ClassicAssert.IsTrue (r.IsEof, "#57");
		}
		
		protected void StaticMember (XamlReader r)
		{
			ClassicAssert.AreEqual (XamlNodeType.None, r.NodeType, "#1");
			ClassicAssert.IsTrue (r.Read (), "#6");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#7");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "#7-2");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "#7-3");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#12-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#12-3");

			ClassicAssert.IsTrue (r.Read (), "#16");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#17");
			var xt = new XamlType (typeof (TestClass5), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#17-2");
//			ClassicAssert.IsTrue (r.Instance is TestClass5, "#17-3");
			ClassicAssert.AreEqual (3, xt.GetAllMembers ().Count, "#17-4");
			ClassicAssert.IsTrue (xt.GetAllMembers ().Any (xm => xm.Name == "Bar"), "#17-5");
			ClassicAssert.IsTrue (xt.GetAllMembers ().Any (xm => xm.Name == "Baz"), "#17-6");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#22");
			ClassicAssert.AreEqual (xt.GetMember ("Bar"), r.Member, "#22-2");

			ClassicAssert.IsTrue (r.Read (), "#26");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#27");
			ClassicAssert.AreEqual (XamlLanguage.Null, r.Type, "#27-2");
//			ClassicAssert.IsNull (r.Instance, "#27-3");

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#32");

			ClassicAssert.IsTrue (r.Read (), "#36");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#37");
			// static Foo is not included in GetAllXembers() return value.
			// ReadOnly is not included in GetAllMembers() return value neither.
			// nonpublic Baz is a member, but does not appear in the reader.

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#52");

			ClassicAssert.IsFalse (r.Read (), "#56");
			ClassicAssert.IsTrue (r.IsEof, "#57");
		}

		protected void Skip (XamlReader r)
		{
			r.Skip ();
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Skip ();
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2");
			r.Skip ();
			ClassicAssert.IsTrue (r.IsEof, "#3");
		}

		protected void Skip2 (XamlReader r)
		{
			r.Read (); // NamespaceDeclaration
			r.Read (); // Type
			if (r is XamlXmlReader)
				ReadBase (r);
			r.Read (); // Member (Initialization)
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#1");
			r.Skip ();
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#2");
			r.Skip ();
			ClassicAssert.IsTrue (r.IsEof, "#3");
		}

		protected void Read_XmlDocument (XamlReader r)
		{
			for (int i = 0; i < 3; i++) {
				r.Read ();
				ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1-" + i);
			}
			r.Read ();

			ClassicAssert.AreEqual (new XamlType (typeof (XmlDocument), r.SchemaContext), r.Type, "#2");
			r.Read ();
			var l = new List<XamlMember> ();
			while (r.NodeType == XamlNodeType.StartMember) {
			// It depends on XmlDocument's implenentation details. It fails on mono only because XmlDocument.SchemaInfo overrides both getter and setter.
			//for (int i = 0; i < 5; i++) {
			//	ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-" + i);
				l.Add (r.Member);
				r.Skip ();
			}
			ClassicAssert.IsNotNull (l.FirstOrDefault (m => m.Name == "Value"), "#4-1");
			ClassicAssert.IsNotNull (l.FirstOrDefault (m => m.Name == "InnerXml"), "#4-2");
			ClassicAssert.IsNotNull (l.FirstOrDefault (m => m.Name == "Prefix"), "#4-3");
			ClassicAssert.IsNotNull (l.FirstOrDefault (m => m.Name == "PreserveWhitespace"), "#4-4");
			ClassicAssert.IsNotNull (l.FirstOrDefault (m => m.Name == "Schemas"), "#4-5");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#5");
			ClassicAssert.IsFalse (r.Read (), "#6");
		}

		protected void Read_NonPrimitive (XamlReader r)
		{
			ClassicAssert.AreEqual (XamlNodeType.None, r.NodeType, "#1");
			ClassicAssert.IsTrue (r.Read (), "#6");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#7");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "#7-2");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "#7-3");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#12-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#12-3");

			ClassicAssert.IsTrue (r.Read (), "#16");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#17");
			var xt = new XamlType (typeof (TestClass3), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#17-2");
//			ClassicAssert.IsTrue (r.Instance is TestClass3, "#17-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#22");
			ClassicAssert.AreEqual (xt.GetMember ("Nested"), r.Member, "#22-2");

			ClassicAssert.IsTrue (r.Read (), "#26");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#27");
			ClassicAssert.AreEqual (XamlLanguage.Null, r.Type, "#27-2");
//			ClassicAssert.IsNull (r.Instance, "#27-3");

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#32");

			ClassicAssert.IsTrue (r.Read (), "#36");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#37");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#42");

			ClassicAssert.IsFalse (r.Read (), "#46");
			ClassicAssert.IsTrue (r.IsEof, "#47");
		}

		protected void Read_TypeOrTypeExtension (XamlReader r, Action validateInstance, XamlMember ctorArgMember)
		{
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");
//			ClassicAssert.IsNull (r.Instance, "#14");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			ClassicAssert.IsNotNull (r.Type, "#23");
			ClassicAssert.AreEqual (XamlLanguage.Type, r.Type, "#23-2");
			ClassicAssert.IsNull (r.Namespace, "#25");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.IsNotNull (r.Member, "#33");
			ClassicAssert.AreEqual (ctorArgMember, r.Member, "#33-2");
			ClassicAssert.IsNull (r.Type, "#34");
//			ClassicAssert.IsNull (r.Instance, "#35");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.IsNotNull (r.Value, "#43");
			ClassicAssert.AreEqual ("x:Int32", r.Value, "#43-2");
			ClassicAssert.IsNull (r.Member, "#44");
//			ClassicAssert.IsNull (r.Instance, "#45");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");
			ClassicAssert.IsNull (r.Type, "#53");
			ClassicAssert.IsNull (r.Member, "#54");
//			ClassicAssert.IsNull (r.Instance, "#55");

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#62");
			ClassicAssert.IsNull (r.Type, "#63");

			ClassicAssert.IsFalse (r.Read (), "#71");
			ClassicAssert.IsTrue (r.IsEof, "#72");
		}

		protected void Read_TypeOrTypeExtension2 (XamlReader r, Action validateInstance, XamlMember ctorArgMember)
		{
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");

			var defns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;

			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (defns, r.Namespace.Namespace, "#13-3:" + r.Namespace.Prefix);

			ClassicAssert.IsTrue (r.Read (), "#16");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#17");
			ClassicAssert.IsNotNull (r.Namespace, "#18");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#18-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#18-3:" + r.Namespace.Prefix);

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			ClassicAssert.AreEqual (new XamlType (typeof (TypeExtension), r.SchemaContext), r.Type, "#23-2");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (ctorArgMember, r.Member, "#33-2");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("TestClass1", r.Value, "#43-2");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#62");

			ClassicAssert.IsFalse (r.Read (), "#71");
			ClassicAssert.IsTrue (r.IsEof, "#72");
		}

		protected void Read_Reference (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (Reference), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23-2");
//			ClassicAssert.IsTrue (r.Instance is Reference, "#26");
			ClassicAssert.IsNotNull (XamlLanguage.Type.SchemaContext, "#23-3");
			ClassicAssert.IsNotNull (r.SchemaContext, "#23-4");
			ClassicAssert.AreNotEqual (XamlLanguage.Type.SchemaContext, r.SchemaContext, "#23-5");
			ClassicAssert.AreNotEqual (XamlLanguage.Reference.SchemaContext, xt.SchemaContext, "#23-6");
			ClassicAssert.AreEqual (XamlLanguage.Reference, xt, "#23-7");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			// unlike TypeExtension there is no PositionalParameters.
			ClassicAssert.AreEqual (xt.GetMember ("Name"), r.Member, "#33-2");

			// It is a ContentProperty (besides [ConstructorArgument])
			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("FooBar", r.Value, "#43-2");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#62");

			ClassicAssert.IsFalse (r.Read (), "#71");
			ClassicAssert.IsTrue (r.IsEof, "#72");
		}

		protected void Read_NullOrNullExtension (XamlReader r, Action validateInstance)
		{
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");
//			ClassicAssert.IsNull (r.Instance, "#14");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			ClassicAssert.AreEqual (new XamlType (typeof (NullExtension), r.SchemaContext), r.Type, "#23-2");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#62");

			ClassicAssert.IsFalse (r.Read (), "#71");
			ClassicAssert.IsTrue (r.IsEof, "#72");
		}

		// almost identical to TypeExtension (only type/instance difference)
		protected void Read_StaticExtension (XamlReader r, XamlMember ctorArgMember)
		{
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");
//			ClassicAssert.IsNull (r.Instance, "#14");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			ClassicAssert.AreEqual (new XamlType (typeof (StaticExtension), r.SchemaContext), r.Type, "#23-2");
//			ClassicAssert.IsTrue (r.Instance is StaticExtension, "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (ctorArgMember, r.Member, "#33-2");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("FooBar", r.Value, "#43-2");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#62");

			ClassicAssert.IsFalse (r.Read (), "#71");
			ClassicAssert.IsTrue (r.IsEof, "#72");
		}

		protected void Read_ListInt32 (XamlReader r, Action validateInstance, List<int> obj)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");

			var defns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";

			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-3");
			ClassicAssert.AreEqual (defns, r.Namespace.Namespace, "ns#1-4");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (List<int>), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23");
			ClassicAssert.IsTrue (xt.IsCollection, "#27");
			if (validateInstance != null)
				validateInstance ();

			// This assumption on member ordering ("Type" then "Items") is somewhat wrong, and we might have to adjust it in the future.

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "#33");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			// The value is implementation details, not testable.
			//ClassicAssert.AreEqual ("3", r.Value, "#43");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			if (obj.Count > 0) { // only when items exist.

			ClassicAssert.IsTrue (r.Read (), "#72");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#72-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "#72-3");

			string [] values = {"5", "-3", "2147483647", "0"};
			for (int i = 0; i < 4; i++) {
				ClassicAssert.IsTrue (r.Read (), i + "#73");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#73-2");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#74-2");
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, i + "#74-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.IsNotNull (r.Value, i + "#75-2");
				ClassicAssert.AreEqual (values [i], r.Value, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#74-2");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#75-2");
			}

			ClassicAssert.IsTrue (r.Read (), "#81");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#82"); // XamlLanguage.Items
			
			} // end of "if count > 0".

			ClassicAssert.IsTrue (r.Read (), "#87");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#88");

			ClassicAssert.IsFalse (r.Read (), "#89");
		}

		protected void Read_ListType (XamlReader r, bool isObjectReader, bool order = true)
		{

			IEnumerable<NamespaceDeclaration> namespaces = new [] {
				new NamespaceDeclaration ("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib", string.Empty),
				new NamespaceDeclaration ("clr-namespace:" + Compat.Namespace + ";assembly=" + Compat.Namespace, Compat.Prefix),
				new NamespaceDeclaration ("clr-namespace:System;assembly=System.Private.CoreLib", "s"),
				new NamespaceDeclaration (XamlLanguage.Xaml2006Namespace, "x")
			};

			if (order)
				namespaces = namespaces.OrderBy(n => n.Prefix);
			int count = 0;
			foreach (var ns in namespaces) {
				count++;
				ClassicAssert.IsTrue (r.Read (), "ns#{0}-1", count);
				ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#{0}-2", count);
				ClassicAssert.IsNotNull (r.Namespace, "ns#{0}-3", count);
				ClassicAssert.AreEqual (ns.Prefix, r.Namespace.Prefix, "ns#{0}-3-2", count);
				ClassicAssert.AreEqual (ns.Namespace, r.Namespace.Namespace, "ns#{0}-3-3", count);
			}

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (List<Type>), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23");
			ClassicAssert.IsTrue (xt.IsCollection, "#27");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "#33");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("2", r.Value, "#43");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#72");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#72-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "#72-3");

			string [] values = {"x:Int32", $"Dictionary(s:Type, {Compat.Prefix}:XamlType)"};
			for (int i = 0; i < 2; i++) {
				ClassicAssert.IsTrue (r.Read (), i + "#73");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#73-2");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#74-2");
				// Here XamlObjectReader and XamlXmlReader significantly differs. (Lucky we can make this test conditional so simply)
				if (isObjectReader)
					ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, i + "#74-3");
				else
					ClassicAssert.AreEqual (XamlLanguage.Type.GetMember ("Type"), r.Member, i + "#74-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.IsNotNull (r.Value, i + "#75-2");
				ClassicAssert.AreEqual (values [i], r.Value, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#74-2");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#75-2");
			}

			ClassicAssert.IsTrue (r.Read (), "#81");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#82"); // XamlLanguage.Items
			
			ClassicAssert.IsTrue (r.Read (), "#87");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#88");

			ClassicAssert.IsFalse (r.Read (), "#89");
		}

		protected void Read_ListArray (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");

			var defns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";
			var defns2 = "clr-namespace:System;assembly=System.Private.CoreLib";
			//var defns3 = "clr-namespace:System.Xaml;assembly=System.Xaml";

			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-3");
			ClassicAssert.AreEqual (defns, r.Namespace.Namespace, "ns#1-4");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("s", r.Namespace.Prefix, "ns#2-3-2");
			ClassicAssert.AreEqual (defns2, r.Namespace.Namespace, "ns#2-3-3");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (List<Array>), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23");
			ClassicAssert.IsTrue (xt.IsCollection, "#27");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "#33");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("2", r.Value, "#43");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#72");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#72-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "#72-3");

			string [] values = {"x:Int32", "x:String"};
			for (int i = 0; i < 2; i++) {
				ClassicAssert.IsTrue (r.Read (), i + "#73");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#73-2");
				ClassicAssert.AreEqual (XamlLanguage.Array, r.Type, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#74-2");
				ClassicAssert.AreEqual (XamlLanguage.Array.GetMember ("Type"), r.Member, i + "#74-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.IsNotNull (r.Value, i + "#75-2");
				ClassicAssert.AreEqual (values [i], r.Value, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#74-2");

				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#75-2");
				ClassicAssert.AreEqual (XamlLanguage.Array.GetMember ("Items"), r.Member, i + "#75-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75-4");
				ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, i + "#75-5");
				ClassicAssert.IsTrue (r.Read (), i + "#75-7");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#75-8");
				ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, i + "#75-9");

				for (int j = 0; j < 3; j++) {
					ClassicAssert.IsTrue (r.Read (), i + "#76-" + j);
					ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#76-2"+ "-" + j);
					ClassicAssert.IsTrue (r.Read (), i + "#76-3"+ "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#76-4"+ "-" + j);
					ClassicAssert.IsTrue (r.Read (), i + "#76-5"+ "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, i + "#76-6"+ "-" + j);
					ClassicAssert.IsTrue (r.Read (), i + "#76-7"+ "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#76-8"+ "-" + j);
					ClassicAssert.IsTrue (r.Read (), i + "#76-9"+ "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#76-10"+ "-" + j);
				}

				ClassicAssert.IsTrue (r.Read (), i + "#77");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#77-2");

				ClassicAssert.IsTrue (r.Read (), i + "#78");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#78-2");

				ClassicAssert.IsTrue (r.Read (), i + "#79");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#79-2");

				ClassicAssert.IsTrue (r.Read (), i + "#80");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#80-2");
			}

			ClassicAssert.IsTrue (r.Read (), "#81");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#82"); // XamlLanguage.Items
			
			ClassicAssert.IsTrue (r.Read (), "#87");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#88");

			ClassicAssert.IsFalse (r.Read (), "#89");
		}

		protected void Read_ArrayList (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");

			var defns = "clr-namespace:System.Collections;assembly=System.Runtime.Extensions";

			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-3");
			ClassicAssert.AreEqual (defns, r.Namespace.Namespace, "ns#1-4");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (ArrayList), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23");
//			ClassicAssert.AreEqual (obj, r.Instance, "#26");
			ClassicAssert.IsTrue (xt.IsCollection, "#27");

			if (r is XamlXmlReader)
				ReadBase (r);

			// This assumption on member ordering ("Type" then "Items") is somewhat wrong, and we might have to adjust it in the future.

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "#33");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			// The value is implementation details, not testable.
			//ClassicAssert.AreEqual ("3", r.Value, "#43");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#72");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#72-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				ClassicAssert.IsTrue (r.Read (), i + "#73");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#73-2");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#74-2");
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, i + "#74-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.IsNotNull (r.Value, i + "#75-2");
				ClassicAssert.AreEqual (values [i], r.Value, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#74-2");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#75-2");
			}

			ClassicAssert.IsTrue (r.Read (), "#81");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#82"); // XamlLanguage.Items

			ClassicAssert.IsTrue (r.Read (), "#87");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#88");

			ClassicAssert.IsFalse (r.Read (), "#89");
		}

		protected void Read_ArrayOrArrayExtensionOrMyArrayExtension (XamlReader r, Action validateInstance, Type extType)
		{
			if (extType == typeof (MyArrayExtension)) {
				ClassicAssert.IsTrue (r.Read (), "#1");
				ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#2");
				ClassicAssert.IsNotNull (r.Namespace, "#3");
				ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "#3-2");
			}
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (extType, r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			// This assumption on member ordering ("Type" then "Items") is somewhat wrong, and we might have to adjust it in the future.

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (xt.GetMember ("Type"), r.Member, "#33");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("x:Int32", r.Value, "#43");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#62");
			ClassicAssert.AreEqual (xt.GetMember ("Items"), r.Member, "#63");

			ClassicAssert.IsTrue (r.Read (), "#71");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "#71-2");
			ClassicAssert.IsNull (r.Type, "#71-3");
			ClassicAssert.IsNull (r.Member, "#71-4");
			ClassicAssert.IsNull (r.Value, "#71-5");

			ClassicAssert.IsTrue (r.Read (), "#72");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#72-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				ClassicAssert.IsTrue (r.Read (), i + "#73");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#73-2");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#74-2");
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, i + "#74-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.IsNotNull (r.Value, i + "#75-2");
				ClassicAssert.AreEqual (values [i], r.Value, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#74-2");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#75-2");
			}

			ClassicAssert.IsTrue (r.Read (), "#81");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#82"); // XamlLanguage.Items

			ClassicAssert.IsTrue (r.Read (), "#83");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#84"); // GetObject

			ClassicAssert.IsTrue (r.Read (), "#85");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#86"); // ArrayExtension.Items

			ClassicAssert.IsTrue (r.Read (), "#87");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#88"); // ArrayExtension

			ClassicAssert.IsFalse (r.Read (), "#89");
		}

		// It gives Type member, not PositionalParameters... and no Items member here.
		protected void Read_ArrayExtension2 (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");
//			ClassicAssert.IsNull (r.Instance, "#14");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (ArrayExtension), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23-2");
//			ClassicAssert.IsTrue (r.Instance is ArrayExtension, "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (xt.GetMember ("Type"), r.Member, "#33-2");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			ClassicAssert.AreEqual ("x:Int32", r.Value, "#43-2");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#62");

			ClassicAssert.IsFalse (r.Read (), "#71");
			ClassicAssert.IsTrue (r.IsEof, "#72");
		}

		protected void Read_CustomMarkupExtension (XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1-2");
			r.Read ();
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			ClassicAssert.IsFalse (r.IsEof, "#1");
			var xt = r.Type;

			if (r is XamlXmlReader)
				ReadBase (r);

			r.Read ();
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#2-1");
			ClassicAssert.IsFalse (r.IsEof, "#2-2");
			ClassicAssert.AreEqual (xt.GetMember ("Bar"), r.Member, "#2-3");

			ClassicAssert.IsTrue (r.Read (), "#2-4");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#2-5");
			ClassicAssert.AreEqual ("v2", r.Value, "#2-6");

			ClassicAssert.IsTrue (r.Read (), "#2-7");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#2-8");

			r.Read ();
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-1");
			ClassicAssert.IsFalse (r.IsEof, "#3-2");
			ClassicAssert.AreEqual (xt.GetMember ("Baz"), r.Member, "#3-3");

			ClassicAssert.IsTrue (r.Read (), "#3-4");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#3-5");
			ClassicAssert.AreEqual ("v7", r.Value, "#3-6");

			ClassicAssert.IsTrue (r.Read (), "#3-7");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#3-8");
			
			r.Read ();
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#4-1");
			ClassicAssert.IsFalse (r.IsEof, "#4-2");
			ClassicAssert.AreEqual (xt.GetMember ("Foo"), r.Member, "#4-3");
			ClassicAssert.IsTrue (r.Read (), "#4-4");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#4-5");
			ClassicAssert.AreEqual ("x:Int32", r.Value, "#4-6");

			ClassicAssert.IsTrue (r.Read (), "#4-7");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#4-8");

			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#5-2");

			ClassicAssert.IsFalse (r.Read (), "#6");
		}

		protected void Read_CustomMarkupExtension2 (XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension2)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "#4");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual ("MonoTests.System.Xaml.MyExtension2", r.Value, "#6");
			ClassicAssert.IsTrue (r.Read (), "#7"); // EndMember
			ClassicAssert.IsTrue (r.Read (), "#8"); // EndObject
			ClassicAssert.IsFalse (r.Read (), "#9");
		}

		protected void Read_CustomMarkupExtension3 (XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension3)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "#4");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual ("MonoTests.System.Xaml.MyExtension3", r.Value, "#6");
			ClassicAssert.IsTrue (r.Read (), "#7"); // EndMember
			ClassicAssert.IsTrue (r.Read (), "#8"); // EndObject
			ClassicAssert.IsFalse (r.Read (), "#9");
		}

		protected void Read_CustomMarkupExtension4 (XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension4)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "#4");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual ("MonoTests.System.Xaml.MyExtension4", r.Value, "#6");
			ClassicAssert.IsTrue (r.Read (), "#7"); // EndMember
			ClassicAssert.IsTrue (r.Read (), "#8"); // EndObject
			ClassicAssert.IsFalse (r.Read (), "#9");
		}

		protected void Read_CustomMarkupExtension5 (XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension5)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "#4");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual ("foo", r.Value, "#6");
			ClassicAssert.IsTrue (r.Read (), "#7");
			ClassicAssert.AreEqual ("bar", r.Value, "#8");
			ClassicAssert.IsTrue (r.Read (), "#9");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#10");
			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#12");
			ClassicAssert.IsFalse (r.Read (), "#13");
		}

		protected void Read_CustomMarkupExtension6 (XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension6)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (xt.GetMember ("Foo"), r.Member, "#4"); // this is the difference between MyExtension5 and MyExtension6: it outputs constructor arguments as normal members
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual ("foo", r.Value, "#6");
			ClassicAssert.IsTrue (r.Read (), "#7"); // EndMember
			ClassicAssert.IsTrue (r.Read (), "#8"); // EndObject
			ClassicAssert.IsFalse (r.Read (), "#9");
		}

		protected void Read_CustomExtensionWithChildExtension(XamlReader r)
		{
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read();
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(ValueWrapper)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			ClassicAssert.IsTrue(r.Read(), "#3");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(xt.GetMember("StringValue"), r.Member, "#4");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(MyExtension2)), xt = r.Type, "#2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(xt.GetMember("Foo"), r.Member, "#3-2");

			// Child TypeExtension
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(TypeExtension)), xt = r.Type, "#2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "#3-2");
			ClassicAssert.AreEqual("TestClass1", r.Value, "#3-2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, "#3-2");

			// Finish up MyExtension2
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, "#3-2");

			ClassicAssert.IsTrue(r.Read(), "#7"); // EndMember
			ClassicAssert.IsTrue(r.Read(), "#8"); // EndObject
			ClassicAssert.IsFalse(r.Read(), "#9");
			ClassicAssert.AreEqual(XamlNodeType.None, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.IsEof, "#3-2");
		}


		protected void Read_CustomExtensionWithPositionalChildExtension(XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); 
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (ValueWrapper)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (xt.GetMember ("StringValue"), r.Member, "#4");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension2)), xt = r.Type, "#2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(XamlLanguage.PositionalParameters, r.Member, "#3-2");

			// Child TypeExtension
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (TypeExtension)), xt = r.Type, "#2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#3-2");
			ClassicAssert.AreEqual ("TestClass1", r.Value, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#3-2");

			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "#3-2");
			ClassicAssert.AreEqual("Value For Bar", r.Value, "#3-2");

			// Finish up MyExtension2
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#3-2");

			ClassicAssert.IsTrue (r.Read (), "#7"); // EndMember
			ClassicAssert.IsTrue (r.Read (), "#8"); // EndObject
			ClassicAssert.IsFalse (r.Read (), "#9");
			ClassicAssert.AreEqual (XamlNodeType.None, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.IsEof, "#3-2");
		}

		protected void Read_CustomExtensionWithChildExtensionAndNamedProperty(XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); 
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (ValueWrapper)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-1");
			ClassicAssert.AreEqual (xt.GetMember ("StringValue"), r.Member, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#4");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#4-1");
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension2)), xt = r.Type, "#4-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#5-1");
			ClassicAssert.AreEqual(xt.GetMember("Foo"), r.Member, "#5-2");

			// Child TypeExtension
			ClassicAssert.IsTrue (r.Read (), "#6");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#6-1");
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (TypeExtension)), r.Type, "#6-2");
			ClassicAssert.IsTrue (r.Read (), "#7");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#7-1");
			ClassicAssert.IsTrue (r.Read (), "#8");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#8-1");
			ClassicAssert.AreEqual ("TestClass1", r.Value, "#8-2");
			ClassicAssert.IsTrue (r.Read (), "#9");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#9-1");
			ClassicAssert.IsTrue (r.Read (), "#10");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#10-1");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#11-1");
			ClassicAssert.IsTrue (r.Read (), "#12");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#12-1");
			ClassicAssert.AreEqual(xt.GetMember("Bar"), r.Member, "#12-2");

			ClassicAssert.IsTrue(r.Read(), "#13");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "#13-1");
			ClassicAssert.AreEqual("Value For Bar", r.Value, "#13-2");

			// Finish up MyExtension2
			ClassicAssert.IsTrue (r.Read (), "#14");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#14-1");
			ClassicAssert.IsTrue (r.Read (), "#15");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#15-1");

			ClassicAssert.IsTrue (r.Read (), "#16"); // EndMember
			ClassicAssert.IsTrue (r.Read (), "#17"); // EndObject
			ClassicAssert.IsFalse (r.Read (), "#18");
			ClassicAssert.AreEqual (XamlNodeType.None, r.NodeType, "#18-1");
			ClassicAssert.IsTrue (r.IsEof, "#19");
		}


		protected void Read_CustomExtensionWithCommasInPositionalValue(XamlReader r)
		{
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read();
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(ValueWrapper)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			ClassicAssert.IsTrue(r.Read(), "#3");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(xt.GetMember("StringValue"), r.Member, "#4");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(MyExtension6)), xt = r.Type, "#2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(XamlLanguage.PositionalParameters, r.Member, "#4");

			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "#3-2");
			ClassicAssert.AreEqual("Some Value, With Commas", r.Value, "#3-2");


			// Finish up MyExtension7
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, "#3-2");

			ClassicAssert.IsTrue(r.Read(), "#7"); // EndMember
			ClassicAssert.IsTrue(r.Read(), "#8"); // EndObject
			ClassicAssert.IsFalse(r.Read(), "#9");
			ClassicAssert.AreEqual(XamlNodeType.None, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.IsEof, "#3-2");
		}

        // cf. https://msdn.microsoft.com/en-us/library/ee200269.aspx
        //     "If the next character is a "\" (Unicode code point 005C), consume that "\" without adding 
        //     it to the text value, then consume the following character and append that to the value."
        protected void Load_CustomExtensionWithEscapeChars(XamlReader r)
        {
            ValueWrapper o = XamlServices.Load(r) as ValueWrapper;
            ClassicAssert.IsNotNull(o, "Null, or not a ValueWrapper");
            ClassicAssert.AreEqual("Quoted string: 'test'; Embedded braces: {test}; Two Backslashes: \\\\ (test after last escape)", o.StringValue, "Escape character not parsed properly");
        }

        protected void Read_CustomExtensionWithPositionalAndNamed(XamlReader r)
		{
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read();
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(ValueWrapper)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			ClassicAssert.IsTrue(r.Read(), "#3");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(xt.GetMember("StringValue"), r.Member, "#4");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#5-2");
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(MyExtension6)), xt = r.Type, "#5-3");

			// positional parameter
			ReadMemberWithValue(r, XamlLanguage.PositionalParameters, "#6", "SomeValue");

			// non-positional parameter
			ReadMemberWithValue(r, xt.GetMember("Foo"), "#7", "OtherValue");

			// Finish up MyExtension7
			ClassicAssert.IsTrue(r.Read(), "#8");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, "#8-2");

			ClassicAssert.IsTrue(r.Read(), "#9"); // EndMember
			ClassicAssert.IsTrue(r.Read(), "#10"); // EndObject
			ClassicAssert.IsFalse(r.Read(), "#11");
			ClassicAssert.AreEqual(XamlNodeType.None, r.NodeType, "#11-2");
			ClassicAssert.IsTrue(r.IsEof, "#12");
		}

		protected void Read_CustomExtensionWithCommasInNamedValue(XamlReader r)
		{
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read(); // ns
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read();
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(ValueWrapper)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			ClassicAssert.IsTrue(r.Read(), "#3");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(xt.GetMember("StringValue"), r.Member, "#4");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(r.SchemaContext.GetXamlType(typeof(MyExtension6)), xt = r.Type, "#2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(xt.GetMember("Foo"), r.Member, "#4");

			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "#3-2");
			ClassicAssert.AreEqual("Some Value, With Commas", r.Value, "#3-2");


			// Finish up MyExtension7
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, "#3-2");

			ClassicAssert.IsTrue(r.Read(), "#7"); // EndMember
			ClassicAssert.IsTrue(r.Read(), "#8"); // EndObject
			ClassicAssert.IsFalse(r.Read(), "#9");
			ClassicAssert.AreEqual(XamlNodeType.None, r.NodeType, "#3-2");
			ClassicAssert.IsTrue(r.IsEof, "#3-2");
		}

		public void Read_CustomExtensionWithPositonalAfterExplicitProperty(XamlReader r)
		{
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); // ns
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#1");
			r.Read (); 
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#2-0");
			var xt = r.Type;
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (ValueWrapper)), xt, "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#3");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (xt.GetMember ("StringValue"), r.Member, "#4");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (MyExtension2)), xt = r.Type, "#2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.AreEqual(XamlLanguage.PositionalParameters, r.Member, "#3-2");

			// Child TypeExtension
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#3-2");
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (TypeExtension)), xt = r.Type, "#2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#3-2");
			ClassicAssert.AreEqual ("TestClass1", r.Value, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#3-2");

			ClassicAssert.IsTrue(r.Read(), "#5");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "#3-2");
			ClassicAssert.AreEqual("Value For Bar", r.Value, "#3-2");

			// Finish up MyExtension2
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.Read (), "#5");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#3-2");

			ClassicAssert.IsTrue (r.Read (), "#7"); // EndMember
			ClassicAssert.IsTrue (r.Read (), "#8"); // EndObject
			ClassicAssert.IsFalse (r.Read (), "#9");
			ClassicAssert.AreEqual (XamlNodeType.None, r.NodeType, "#3-2");
			ClassicAssert.IsTrue (r.IsEof, "#3-2");
		}


		protected void Read_ArgumentAttributed (XamlReader r, object obj)
		{
			Read_CommonClrType (r, obj, new KeyValuePair<string,string> ("x", XamlLanguage.Xaml2006Namespace));

			if (r is XamlXmlReader)
				ReadBase (r);

			var args = Read_AttributedArguments_String (r, new string [] {"arg1", "arg2"});
			ClassicAssert.AreEqual ("foo", args [0], "#1");
			ClassicAssert.AreEqual ("bar", args [1], "#2");
		}

		protected void Read_Dictionary (XamlReader r, bool includeSystemNamespace)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib", r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			if (includeSystemNamespace)
				ReadNamespace(r, "sys", "clr-namespace:System;assembly=System.Private.CoreLib", "#3");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (Dictionary<string,object>), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");
//			ClassicAssert.AreEqual (obj, r.Instance, "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "smitems#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smitems#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "smitems#3");

			for (int i = 0; i < 3; i++) {

				// start of an item
				ClassicAssert.IsTrue (r.Read (), "soi#1-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "soi#1-2." + i);
				var xt2 = new XamlType (typeof (double), r.SchemaContext);
				ClassicAssert.AreEqual (xt2, r.Type, "soi#1-3." + i);

				ClassicAssert.IsTrue (r.Read (), "smi#1-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smi#1-2." + i);
				ClassicAssert.AreEqual (XamlLanguage.Key, r.Member, "smi#1-3." + i);

				ClassicAssert.IsTrue (r.Read (), "svi#1-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "svi#1-2." + i);
				ClassicAssert.AreEqual (i == 0 ? "Foo" : i == 1 ? "Bar" : "Woo", r.Value, "svi#1-3." + i);

				ClassicAssert.IsTrue (r.Read (), "emi#1-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emi#1-2." + i);

				ClassicAssert.IsTrue (r.Read (), "smi#2-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smi#2-2." + i);
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "smi#2-3." + i);

				ClassicAssert.IsTrue (r.Read (), "svi#2-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "svi#2-2." + i);
				ClassicAssert.AreEqual (i == 0 ? "5" : i == 1 ? "-6.5" : "123.45", r.Value, "svi#2-3." + i); // converted to string(!)

				ClassicAssert.IsTrue (r.Read (), "emi#2-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emi#2-2." + i);

				ClassicAssert.IsTrue (r.Read (), "eoi#1-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eoi#1-2." + i);
				// end of an item
			}

			ClassicAssert.IsTrue (r.Read (), "emitems#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emitems#2"); // XamlLanguage.Items

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2"); // Dictionary

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_Dictionary2 (XamlReader r, XamlMember ctorArgMember, bool order = true)
		{
			IEnumerable<NamespaceDeclaration> namespaces = new [] {
				new NamespaceDeclaration ("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib", string.Empty),
				new NamespaceDeclaration ("clr-namespace:" + Compat.Namespace + ";assembly=" + Compat.Namespace, Compat.Prefix),
				new NamespaceDeclaration ("clr-namespace:System;assembly=System.Private.CoreLib", "s"),
				new NamespaceDeclaration (XamlLanguage.Xaml2006Namespace, "x")
			};

			if (order)
				namespaces = namespaces.OrderBy(n => n.Prefix);
			int count = 0;
			foreach (var ns in namespaces)
			{
				count++;
				ClassicAssert.IsTrue (r.Read (), "ns#{0}-1", count);
				ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#{0}-2", count);
				ClassicAssert.IsNotNull (r.Namespace, "ns#{0}-3", count);
				ClassicAssert.AreEqual (ns.Prefix, r.Namespace.Prefix, "ns#{0}-4", count);
				ClassicAssert.AreEqual (ns.Namespace, r.Namespace.Namespace, "ns#{0}-5", count);
			}

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (Dictionary<string,Type>), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");
//			ClassicAssert.AreEqual (obj, r.Instance, "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "smitems#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smitems#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "smitems#3");

			for (int i = 0; i < 2; i++) {

				// start of an item
				ClassicAssert.IsTrue (r.Read (), "soi#1-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "soi#1-2." + i);
				var xt2 = XamlLanguage.Type;
				ClassicAssert.AreEqual (xt2, r.Type, "soi#1-3." + i);

				if (r is XamlObjectReader) {
					Read_Dictionary2_ConstructorArgument (r, ctorArgMember, i);
					Read_Dictionary2_Key (r, i);
				} else {
					Read_Dictionary2_Key (r, i);
					Read_Dictionary2_ConstructorArgument (r, ctorArgMember, i);
				}

				ClassicAssert.IsTrue (r.Read (), "eoi#1-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eoi#1-2." + i);
				// end of an item
			}

			ClassicAssert.IsTrue (r.Read (), "emitems#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emitems#2"); // XamlLanguage.Items

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2"); // Dictionary

			ClassicAssert.IsFalse (r.Read (), "end");
		}
		
		void Read_Dictionary2_ConstructorArgument (XamlReader r, XamlMember ctorArgMember, int i)
		{
			ClassicAssert.IsTrue (r.Read (), "smi#1-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smi#1-2." + i);
			ClassicAssert.AreEqual (ctorArgMember, r.Member, "smi#1-3." + i);

			ClassicAssert.IsTrue (r.Read (), "svi#1-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "svi#1-2." + i);
			ClassicAssert.AreEqual (i == 0 ? "x:Int32" : "Dictionary(s:Type, " + Compat.Prefix + ":XamlType)", r.Value, "svi#1-3." + i);

			ClassicAssert.IsTrue (r.Read (), "emi#1-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emi#1-2." + i);
		}

		void Read_Dictionary2_Key (XamlReader r, int i)
		{
			ClassicAssert.IsTrue (r.Read (), "smi#2-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smi#2-2." + i);
			ClassicAssert.AreEqual (XamlLanguage.Key, r.Member, "smi#2-3." + i);

			ClassicAssert.IsTrue (r.Read (), "svi#2-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "svi#2-2." + i);
			ClassicAssert.AreEqual (i == 0 ? "Foo" : "Bar", r.Value, "svi#2-3." + i);

			ClassicAssert.IsTrue (r.Read (), "emi#2-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emi#2-2." + i);
		}

		protected void PositionalParameters1 (XamlReader r)
		{
			// ns1 > T:PositionalParametersClass1 > M:_PositionalParameters > foo > 5 > EM:_PositionalParameters > ET:PositionalParametersClass1

			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (PositionalParametersClass1), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");
//			ClassicAssert.AreEqual (obj, r.Instance, "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sposprm#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sposprm#2");
			ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "sposprm#3");

			ClassicAssert.IsTrue (r.Read (), "sva#1-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "sva#1-2");
			ClassicAssert.AreEqual ("foo", r.Value, "sva#1-3");

			ClassicAssert.IsTrue (r.Read (), "sva#2-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "sva#2-2");
			ClassicAssert.AreEqual ("5", r.Value, "sva#2-3");

			ClassicAssert.IsTrue (r.Read (), "eposprm#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eposprm#2"); // XamlLanguage.PositionalParameters

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}
		
		protected void PositionalParameters2 (XamlReader r)
		{
			// ns1 > T:PositionalParametersWrapper > M:Body > T:PositionalParametersClass1 > M:_PositionalParameters > foo > 5 > EM:_PositionalParameters > ET:PositionalParametersClass1

			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (PositionalParametersWrapper), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");
//			ClassicAssert.AreEqual (obj, r.Instance, "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sm#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#1-2");
			ClassicAssert.AreEqual (xt.GetMember ("Body"), r.Member, "sm#1-3");

			xt = new XamlType (typeof (PositionalParametersClass1), r.SchemaContext);
			ClassicAssert.IsTrue (r.Read (), "so#2-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2");
			ClassicAssert.AreEqual (xt, r.Type, "so#2-3");
//			ClassicAssert.AreEqual (obj.Body, r.Instance, "so#2-4");

			ClassicAssert.IsTrue (r.Read (), "sposprm#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sposprm#2");
			ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "sposprm#3");

			ClassicAssert.IsTrue (r.Read (), "sva#1-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "sva#1-2");
			ClassicAssert.AreEqual ("foo", r.Value, "sva#1-3");

			ClassicAssert.IsTrue (r.Read (), "sva#2-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "sva#2-2");
			ClassicAssert.AreEqual ("5", r.Value, "sva#2-3");

			ClassicAssert.IsTrue (r.Read (), "eposprm#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eposprm#2"); // XamlLanguage.PositionalParameters

			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2");

			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eo#1-2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}
		
		protected void ComplexPositionalParameters (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (ComplexPositionalParameterWrapper), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");
//			ClassicAssert.AreEqual (obj, r.Instance, "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sm#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#1-2");
			ClassicAssert.AreEqual (xt.GetMember ("Param"), r.Member, "sm#1-3");

			xt = r.SchemaContext.GetXamlType (typeof (ComplexPositionalParameterClass));
			ClassicAssert.IsTrue (r.Read (), "so#2-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2");
			ClassicAssert.AreEqual (xt, r.Type, "so#2-3");
//			ClassicAssert.AreEqual (obj.Param, r.Instance, "so#2-4");

			ClassicAssert.IsTrue (r.Read (), "sarg#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sarg#2");
			ClassicAssert.AreEqual (XamlLanguage.Arguments, r.Member, "sarg#3");

			ClassicAssert.IsTrue (r.Read (), "so#3-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#3-2");
			xt = r.SchemaContext.GetXamlType (typeof (ComplexPositionalParameterValue));
			ClassicAssert.AreEqual (xt, r.Type, "so#3-3");

			ClassicAssert.IsTrue (r.Read (), "sm#3-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#3-2");
			ClassicAssert.AreEqual (xt.GetMember ("Foo"), r.Member, "sm#3-3");
			ClassicAssert.IsTrue (r.Read (), "v#3-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#3-2");
			ClassicAssert.AreEqual ("foo", r.Value, "v#3-3");

			ClassicAssert.IsTrue (r.Read (), "em#3-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#3-2");
			ClassicAssert.IsTrue (r.Read (), "eo#3-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#3-2");

			ClassicAssert.IsTrue (r.Read (), "earg#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "earg#2"); // XamlLanguage.Arguments

			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2");

			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eo#1-2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_ListWrapper (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "#1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#2");
			ClassicAssert.IsNotNull (r.Namespace, "#3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "#3-2");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (ListWrapper), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23");
//			ClassicAssert.AreEqual (obj, r.Instance, "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#62");
			ClassicAssert.AreEqual (xt.GetMember ("Items"), r.Member, "#63");

			ClassicAssert.IsTrue (r.Read (), "#71");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "#71-2");
			ClassicAssert.IsNull (r.Type, "#71-3");
			ClassicAssert.IsNull (r.Member, "#71-4");
			ClassicAssert.IsNull (r.Value, "#71-5");

			ClassicAssert.IsTrue (r.Read (), "#72");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#72-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				ClassicAssert.IsTrue (r.Read (), i + "#73");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#73-2");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#74-2");
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, i + "#74-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.IsNotNull (r.Value, i + "#75-2");
				ClassicAssert.AreEqual (values [i], r.Value, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#74-2");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#75-2");
			}

			ClassicAssert.IsTrue (r.Read (), "#81");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#82"); // XamlLanguage.Items

			ClassicAssert.IsTrue (r.Read (), "#83");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#84"); // GetObject

			ClassicAssert.IsTrue (r.Read (), "#85");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#86"); // ListWrapper.Items

			ClassicAssert.IsTrue (r.Read (), "#87");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#88"); // ListWrapper

			ClassicAssert.IsFalse (r.Read (), "#89");
		}

		protected void Read_ListWrapper2 (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "#1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#2");
			ClassicAssert.IsNotNull (r.Namespace, "#3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "#3-2");

			ClassicAssert.IsTrue (r.Read (), "#6");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#7");
			ClassicAssert.IsNotNull (r.Namespace, "#8");
			ClassicAssert.AreEqual ("scg", r.Namespace.Prefix, "#8-2");
			ClassicAssert.AreEqual ("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib", r.Namespace.Namespace, "#8-3");

			ClassicAssert.IsTrue (r.Read (), "#11");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "#12");
			ClassicAssert.IsNotNull (r.Namespace, "#13");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "#13-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "#13-3");

			ClassicAssert.IsTrue (r.Read (), "#21");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#22");
			var xt = new XamlType (typeof (ListWrapper2), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "#23");
//			ClassicAssert.AreEqual (obj, r.Instance, "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "#61");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#62");
			ClassicAssert.AreEqual (xt.GetMember ("Items"), r.Member, "#63");

			ClassicAssert.IsTrue (r.Read (), "#71");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#71-2");
			xt = r.SchemaContext.GetXamlType (typeof (List<int>));
			ClassicAssert.AreEqual (xt, r.Type, "#71-3");
			ClassicAssert.IsNull (r.Member, "#71-4");
			ClassicAssert.IsNull (r.Value, "#71-5");

			// Capacity
			ClassicAssert.IsTrue (r.Read (), "#31");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#32");
			ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "#33");

			ClassicAssert.IsTrue (r.Read (), "#41");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "#42");
			// The value is implementation details, not testable.
			//ClassicAssert.AreEqual ("3", r.Value, "#43");

			ClassicAssert.IsTrue (r.Read (), "#51");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#52");

			// Items
			ClassicAssert.IsTrue (r.Read (), "#72");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#72-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				ClassicAssert.IsTrue (r.Read (), i + "#73");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, i + "#73-2");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, i + "#74-2");
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, i + "#74-3");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.IsNotNull (r.Value, i + "#75-2");
				ClassicAssert.AreEqual (values [i], r.Value, i + "#73-3");
				ClassicAssert.IsTrue (r.Read (), i + "#74");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, i + "#74-2");
				ClassicAssert.IsTrue (r.Read (), i + "#75");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, i + "#75-2");
			}

			ClassicAssert.IsTrue (r.Read (), "#81");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#82"); // XamlLanguage.Items

			ClassicAssert.IsTrue (r.Read (), "#83");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#84"); // StartObject(of List<int>)

			ClassicAssert.IsTrue (r.Read (), "#85");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#86"); // ListWrapper.Items

			ClassicAssert.IsTrue (r.Read (), "#87");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#88"); // ListWrapper

			ClassicAssert.IsFalse (r.Read (), "#89");
		}
		
		protected void Read_ContentIncluded (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (ContentIncludedClass), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sposprm#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sposprm#2");
			ClassicAssert.AreEqual (xt.GetMember ("Content"), r.Member, "sposprm#3");

			ClassicAssert.IsTrue (r.Read (), "sva#1-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "sva#1-2");
			ClassicAssert.AreEqual ("foo", r.Value, "sva#1-3");

			ClassicAssert.IsTrue (r.Read (), "eposprm#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eposprm#2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}
		
		protected void Read_PropertyDefinition (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (PropertyDefinition), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "smod#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smod#2");
			ClassicAssert.AreEqual (xt.GetMember ("Modifier"), r.Member, "smod#3");

			ClassicAssert.IsTrue (r.Read (), "vmod#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vmod#2");
			ClassicAssert.AreEqual ("protected", r.Value, "vmod#3");

			ClassicAssert.IsTrue (r.Read (), "emod#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emod#2");

			ClassicAssert.IsTrue (r.Read (), "sname#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sname#2");
			ClassicAssert.AreEqual (xt.GetMember ("Name"), r.Member, "sname#3");

			ClassicAssert.IsTrue (r.Read (), "vname#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vname#2");
			ClassicAssert.AreEqual ("foo", r.Value, "vname#3");

			ClassicAssert.IsTrue (r.Read (), "ename#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "ename#2");

			ClassicAssert.IsTrue (r.Read (), "stype#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "stype#2");
			ClassicAssert.AreEqual (xt.GetMember ("Type"), r.Member, "stype#3");

			ClassicAssert.IsTrue (r.Read (), "vtype#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vtype#2");
			ClassicAssert.AreEqual ("x:String", r.Value, "vtype#3");

			ClassicAssert.IsTrue (r.Read (), "etype#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "etype#2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_StaticExtensionWrapper (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (StaticExtensionWrapper), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sprm#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sprm#2");
			ClassicAssert.AreEqual (xt.GetMember ("Param"), r.Member, "sprm#3");

			ClassicAssert.IsTrue (r.Read (), "so#2-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2");
			xt = new XamlType (typeof (StaticExtension), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#2-3");

			ClassicAssert.IsTrue (r.Read (), "smbr#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smbr#2");
			ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "smbr#3");

			ClassicAssert.IsTrue (r.Read (), "vmbr#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vmbr#2");
			ClassicAssert.AreEqual ("StaticExtensionWrapper.Foo", r.Value, "vmbr#3");

			ClassicAssert.IsTrue (r.Read (), "embr#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "embr#2");

			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2");

			ClassicAssert.IsTrue (r.Read (), "emod#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emod#1-2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_TypeExtensionWrapper (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (TypeExtensionWrapper), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sprm#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sprm#2");
			ClassicAssert.AreEqual (xt.GetMember ("Param"), r.Member, "sprm#3");

			ClassicAssert.IsTrue (r.Read (), "so#2-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2");
			xt = new XamlType (typeof (TypeExtension), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#2-3");

			ClassicAssert.IsTrue (r.Read (), "smbr#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smbr#2");
			ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "smbr#3");

			ClassicAssert.IsTrue (r.Read (), "vmbr#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vmbr#2");
			ClassicAssert.AreEqual (String.Empty, r.Value, "vmbr#3");

			ClassicAssert.IsTrue (r.Read (), "embr#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "embr#2");

			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2");

			ClassicAssert.IsTrue (r.Read (), "emod#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emod#1-2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_EventContainer (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (EventContainer), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_NamedItems (XamlReader r, bool isObjectReader)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			var xt = new XamlType (typeof (NamedItem), r.SchemaContext);

			// foo
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sxname#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sxname#2");
			ClassicAssert.AreEqual (XamlLanguage.Name, r.Member, "sxname#3");

			ClassicAssert.IsTrue (r.Read (), "vxname#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vxname#2");
			ClassicAssert.AreEqual ("__ReferenceID0", r.Value, "vxname#3");

			ClassicAssert.IsTrue (r.Read (), "exname#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "exname#2");

			ClassicAssert.IsTrue (r.Read (), "sname#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sname#2");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), r.Member, "sname#3");

			ClassicAssert.IsTrue (r.Read (), "vname#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vname#2");
			ClassicAssert.AreEqual ("foo", r.Value, "vname#3");

			ClassicAssert.IsTrue (r.Read (), "ename#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "ename#2");

			ClassicAssert.IsTrue (r.Read (), "sItems#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sItems#2");
			ClassicAssert.AreEqual (xt.GetMember ("References"), r.Member, "sItems#3");

			ClassicAssert.IsTrue (r.Read (), "goc#1-1");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "goc#1-2");

			ClassicAssert.IsTrue (r.Read (), "smbr#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smbr#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "smbr#3");

			// bar
			ClassicAssert.IsTrue (r.Read (), "soc#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "soc#1-2");
			ClassicAssert.AreEqual (xt, r.Type, "soc#1-3");

			ClassicAssert.IsTrue (r.Read (), "smbrc#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smbrc#2");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), r.Member, "smbrc#3");

			ClassicAssert.IsTrue (r.Read (), "vmbrc#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vmbrc#2");
			ClassicAssert.AreEqual ("bar", r.Value, "vmbrc#3");

			ClassicAssert.IsTrue (r.Read (), "embrc#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "embrc#2");

			ClassicAssert.IsTrue (r.Read (), "sItemsc#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sItemsc#2");
			ClassicAssert.AreEqual (xt.GetMember ("References"), r.Member, "sItemsc#3");

			ClassicAssert.IsTrue (r.Read (), "god#2-1");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "god#2-2");

			ClassicAssert.IsTrue (r.Read (), "smbrd#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smbrd#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "smbrd#3");

			ClassicAssert.IsTrue (r.Read (), "sod#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "sod#1-2");
			ClassicAssert.AreEqual (XamlLanguage.Reference, r.Type, "sod#1-3");

			ClassicAssert.IsTrue (r.Read (), "smbrd#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smbrd#2");
			if (isObjectReader)
				ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "smbrd#3");
			else
				ClassicAssert.AreEqual (XamlLanguage.Reference.GetMember ("Name"), r.Member, "smbrd#3");

			ClassicAssert.IsTrue (r.Read (), "vmbrd#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vmbrd#2");
			ClassicAssert.AreEqual ("__ReferenceID0", r.Value, "vmbrd#3");

			ClassicAssert.IsTrue (r.Read (), "embrd#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "embrd#2");

			ClassicAssert.IsTrue (r.Read (), "eod#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eod#2-2");

			ClassicAssert.IsTrue (r.Read (), "eItemsc#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eItemsc#2");

			ClassicAssert.IsTrue (r.Read (), "eoc#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eoc#2-2");

			ClassicAssert.IsTrue (r.Read (), "emod#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emod#1-2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			// baz
			ClassicAssert.IsTrue (r.Read (), "so3#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so3#1-2");
			ClassicAssert.AreEqual (xt, r.Type, "so3#1-3");
			ClassicAssert.IsTrue (r.Read (), "sname3#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sname3#2");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), r.Member, "sname3#3");

			ClassicAssert.IsTrue (r.Read (), "vname3#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vname3#2");
			ClassicAssert.AreEqual ("baz", r.Value, "vname3#3");

			ClassicAssert.IsTrue (r.Read (), "ename3#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "ename3#2");

			ClassicAssert.IsTrue (r.Read (), "eo3#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo3#1-2");

			ClassicAssert.IsTrue (r.Read (), "em2#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em2#1-2");

			ClassicAssert.IsTrue (r.Read (), "eo2#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo2#1-2");

			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_NamedItems2 (XamlReader r, bool isObjectReader)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			var xt = new XamlType (typeof (NamedItem2), r.SchemaContext);

			// i1
			ClassicAssert.IsTrue (r.Read (), "so1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so1#2");
			ClassicAssert.AreEqual (xt, r.Type, "so1#3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), r.Member, "sm1#3");

			ClassicAssert.IsTrue (r.Read (), "v1#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v1#2");
			ClassicAssert.AreEqual ("i1", r.Value, "v1#3");

			ClassicAssert.IsTrue (r.Read (), "em1#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em1#2");

			ClassicAssert.IsTrue (r.Read (), "srefs1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "srefs1#2");
			ClassicAssert.AreEqual (xt.GetMember ("References"), r.Member, "srefs1#3");

			ClassicAssert.IsTrue (r.Read (), "go1#1");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "go1#2");

			ClassicAssert.IsTrue (r.Read (), "sitems1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sitems1#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sitems1#3");

			// i2
			ClassicAssert.IsTrue (r.Read (), "so2#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so2#1-2");
			ClassicAssert.AreEqual (xt, r.Type, "so2#1-3");
			ClassicAssert.IsTrue (r.Read (), "sm2#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm2#2");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), r.Member, "sm2#3");

			ClassicAssert.IsTrue (r.Read (), "v2#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v2#2");
			ClassicAssert.AreEqual ("i2", r.Value, "v2#3");

			ClassicAssert.IsTrue (r.Read (), "em2#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em2#2");

			ClassicAssert.IsTrue (r.Read (), "srefs2#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "srefs2#2");
			ClassicAssert.AreEqual (xt.GetMember ("References"), r.Member, "srefs2#3");

			ClassicAssert.IsTrue (r.Read (), "go2#1");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "go2#2");

			ClassicAssert.IsTrue (r.Read (), "sItems1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sItems1#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sItems1#3");

			ClassicAssert.IsTrue (r.Read (), "so3#1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so3#2");
			ClassicAssert.AreEqual (xt, r.Type, "so3#3");
			ClassicAssert.IsTrue (r.Read (), "sm3#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm3#2");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), r.Member, "sm3#3");

			ClassicAssert.IsTrue (r.Read (), "v3#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v3#2");
			ClassicAssert.AreEqual ("i3", r.Value, "v3#3");

			ClassicAssert.IsTrue (r.Read (), "em3#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em3#2");

			ClassicAssert.IsTrue (r.Read (), "eo3#1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo3#2");

			ClassicAssert.IsTrue (r.Read (), "eitems2#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eitems2#2");

			ClassicAssert.IsTrue (r.Read (), "ego2#1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "ego2#2");

			ClassicAssert.IsTrue (r.Read (), "erefs2#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "erefs2#2");

			ClassicAssert.IsTrue (r.Read (), "eo2#1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo2#2");

			// i4
			ClassicAssert.IsTrue (r.Read (), "so4#1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so4#2");
			ClassicAssert.AreEqual (xt, r.Type, "so4#3");

			ClassicAssert.IsTrue (r.Read (), "sm4#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm4#2");
			ClassicAssert.AreEqual (xt.GetMember ("ItemName"), r.Member, "sm4#3");

			ClassicAssert.IsTrue (r.Read (), "v4#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v4#2");
			ClassicAssert.AreEqual ("i4", r.Value, "v4#3");

			ClassicAssert.IsTrue (r.Read (), "em4#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em4#2");

			ClassicAssert.IsTrue (r.Read (), "srefs4#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "srefs4#2");
			ClassicAssert.AreEqual (xt.GetMember ("References"), r.Member, "srefs4#3");

			ClassicAssert.IsTrue (r.Read (), "go4#1");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "go4#2");

			ClassicAssert.IsTrue (r.Read (), "sitems4#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sitems4#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sitems4#3");

			ClassicAssert.IsTrue (r.Read (), "sor1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "sor1#2");
			ClassicAssert.AreEqual (XamlLanguage.Reference, r.Type, "sor1#3");

			ClassicAssert.IsTrue (r.Read (), "smr1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smr1#2");
			if (isObjectReader)
				ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "smr1#3");
			else
				ClassicAssert.AreEqual (XamlLanguage.Reference.GetMember ("Name"), r.Member, "smr1#3");

			ClassicAssert.IsTrue (r.Read (), "vr1#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "vr1#2");
			ClassicAssert.AreEqual ("i3", r.Value, "vr1#3");

			ClassicAssert.IsTrue (r.Read (), "emr1#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emr1#2");

			ClassicAssert.IsTrue (r.Read (), "eor#1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eor#2");

			ClassicAssert.IsTrue (r.Read (), "eitems4#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eitems4#2");

			ClassicAssert.IsTrue (r.Read (), "ego4#1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "ego4#2");

			ClassicAssert.IsTrue (r.Read (), "erefs4#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "erefs4#2");

			ClassicAssert.IsTrue (r.Read (), "eo4#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo4#1-2");

			ClassicAssert.IsTrue (r.Read (), "eitems1#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "eitems1#2");

			ClassicAssert.IsTrue (r.Read (), "ego1#1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "ego1#2");

			ClassicAssert.IsTrue (r.Read (), "erefs1#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "erefs1#2");

			ClassicAssert.IsTrue (r.Read (), "eo1#1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo1#2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_XmlSerializableWrapper (XamlReader r, bool isObjectReader)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			// t:XmlSerializableWrapper
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (XmlSerializableWrapper), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Value
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("Value"), r.Member, "sm1#3");

			// x:XData
			ClassicAssert.IsTrue (r.Read (), "so#2-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2");
			ClassicAssert.AreEqual (XamlLanguage.XData, r.Type, "so#2-3");
			if (r is XamlObjectReader)
				ClassicAssert.IsNull (((XamlObjectReader) r).Instance, "xdata-instance");

			ClassicAssert.IsTrue (r.Read (), "sm2#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm2#2");
			ClassicAssert.AreEqual (XamlLanguage.XData.GetMember ("Text"), r.Member, "sm2#3");

			ClassicAssert.IsTrue (r.Read (), "v1#1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v1#2");
			var val = isObjectReader ? "<root />" : "<root xmlns=\"" + assns + "\" />";
			ClassicAssert.AreEqual (val, r.Value, "v1#3");

			ClassicAssert.IsTrue (r.Read (), "em2#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em2#2");

			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2");

			ClassicAssert.IsTrue (r.Read (), "em1#1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em1#2");

			// /t:XmlSerializableWrapper
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_XmlSerializable (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#1-5");

			// t:XmlSerializable
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (XmlSerializable), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// /t:XmlSerializable
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_ListXmlSerializable (XamlReader r)
		{
			while (true) {
				r.Read ();
				if (r.Member == XamlLanguage.Items)
					break;
				if (r.IsEof)
					Assert.Fail ("Items did not appear");
			}

			// t:XmlSerializable (yes...it is not XData!)
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (XmlSerializable), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			// /t:XmlSerializable
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			r.Close ();
		}

		protected void Read_TypeConverterOnListMember (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("http://www.domain.com/path", r.Namespace.Namespace, "ns#1-5");

			// t:TypeOtherAssembly
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (SecondTest.TypeOtherAssembly), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Values
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("Values"), r.Member, "sm1#3");

			// x:Value
			ClassicAssert.IsTrue (r.Read (), "v#1-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#1-2");
			ClassicAssert.AreEqual ("1, 2, 3", r.Value, "v#1-3");

			// /m:Values
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:TypeOtherAssembly
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_EnumContainer (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#1-5");

			// t:EnumContainer
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (EnumContainer), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:EnumProperty
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("EnumProperty"), r.Member, "sm1#3");

			// x:Value
			ClassicAssert.IsTrue (r.Read (), "v#1-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#1-2");
			ClassicAssert.AreEqual ("Two", r.Value, "v#1-3");

			// /m:EnumProperty
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:EnumContainer
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_CollectionContentProperty (XamlReader r, bool contentPropertyIsUsed)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("scg", r.Namespace.Prefix, "ns#2-4");
			assns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#2-5");

			ClassicAssert.IsTrue (r.Read (), "ns#3-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#3-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#3-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#3-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#3-5");

			// t:CollectionContentProperty
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (CollectionContentProperty), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:ListOfItems
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("ListOfItems"), r.Member, "sm1#3");

			// t:CollectionContentProperty
			xt = new XamlType (typeof (List<SimpleClass>), r.SchemaContext);
			ClassicAssert.IsTrue (r.Read (), "so#2-1");
			if (contentPropertyIsUsed)
				ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "so#2-2.1");
			else {
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2.2");
				ClassicAssert.AreEqual (xt, r.Type, "so#2-3");

				// m:Capacity
				ClassicAssert.IsTrue (r.Read (), "sm#2-1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#2-2");
				ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "sm#2-3");

				// r.Skip (); // LAMESPEC: .NET then skips to *post* Items node (i.e. at the first TestClass item)

				ClassicAssert.IsTrue (r.Read (), "v#1-1");

				ClassicAssert.IsTrue (r.Read (), "em#2-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#2-2");
			}

			ClassicAssert.IsTrue (r.Read (), "sm#3-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm#3-3");

			for (int i = 0; i < 4; i++) {
				// t:SimpleClass
				ClassicAssert.IsTrue (r.Read (), "so#3-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#3-2." + i);
				xt = new XamlType (typeof (SimpleClass), r.SchemaContext);
				ClassicAssert.AreEqual (xt, r.Type, "so#3-3." + i);

				// /t:SimpleClass
				ClassicAssert.IsTrue (r.Read (), "eo#3-1");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#3-2");
			}

			// /m:Items
			ClassicAssert.IsTrue (r.Read (), "em#3-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#3-2");

			// /t:CollectionContentProperty
			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2");

			// /m:ListOfItems
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:CollectionContentProperty
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_CollectionContentPropertyX (XamlReader r, bool contentPropertyIsUsed)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("sc", r.Namespace.Prefix, "ns#2-4");
			assns = "clr-namespace:System.Collections;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#2-5");

			ClassicAssert.IsTrue (r.Read (), "ns#x-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#x-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#x-3");
			ClassicAssert.AreEqual ("scg", r.Namespace.Prefix, "ns#x-4");
			assns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#x-5");

			ClassicAssert.IsTrue (r.Read (), "ns#3-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#3-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#3-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#3-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#3-5");

			// t:CollectionContentProperty
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (CollectionContentPropertyX), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:ListOfItems
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("ListOfItems"), r.Member, "sm1#3");

			// t:List<IEnumerable>
			xt = new XamlType (typeof (List<IEnumerable>), r.SchemaContext);
			ClassicAssert.IsTrue (r.Read (), "so#2-1");

			/*if (contentPropertyIsUsed)
				ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "so#2-2.1");
			else*/ {
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2.2");
				ClassicAssert.AreEqual (xt, r.Type, "so#2-3");

				// m:Capacity
				ClassicAssert.IsTrue (r.Read (), "sm#2-1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#2-2");
				ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "sm#2-3");

				// r.Skip (); // LAMESPEC: .NET then skips to *post* Items node (i.e. at the first TestClass item)

				ClassicAssert.IsTrue (r.Read (), "v#1-1");

				// /m:Capacity
				ClassicAssert.IsTrue (r.Read (), "em#2-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#2-2");
			}

			ClassicAssert.IsTrue (r.Read (), "sm#3-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm#3-3");

			if (!contentPropertyIsUsed) {

				ClassicAssert.IsTrue (r.Read (), "so#x-1");
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#x-2.2");
				xt = new XamlType (typeof (List<object>), r.SchemaContext);
				ClassicAssert.AreEqual (xt, r.Type, "so#x-3");

				// m:Capacity
				ClassicAssert.IsTrue (r.Read (), "sm#xx-1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#xx-2");
				ClassicAssert.AreEqual (xt.GetMember ("Capacity"), r.Member, "sm#xx-3");

				ClassicAssert.IsTrue (r.Read (), "v#x-1");

				// /m:Capacity
				ClassicAssert.IsTrue (r.Read (), "em#xx-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#xx-2");

				ClassicAssert.IsTrue (r.Read (), "sm#x-1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#x-2");
				ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm#x-3");
			}

			for (int i = 0; i < 4; i++) {
				// t:SimpleClass
				ClassicAssert.IsTrue (r.Read (), "so#3-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#3-2." + i);
				xt = new XamlType (typeof (SimpleClass), r.SchemaContext);
				ClassicAssert.AreEqual (xt, r.Type, "so#3-3." + i);

				// /t:SimpleClass
				ClassicAssert.IsTrue (r.Read (), "eo#3-1");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#3-2");
			}

			if (!contentPropertyIsUsed) {
				// /m:Items
				ClassicAssert.IsTrue (r.Read (), "em#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#x-2");

				// /t:List<IEnumerable>
				ClassicAssert.IsTrue (r.Read (), "eo#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#x-2");
			}

			// /m:Items
			ClassicAssert.IsTrue (r.Read (), "em#3-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#3-2");

			// /t:CollectionContentProperty
			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2");

			// /m:ListOfItems
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:CollectionContentProperty
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		#region ambient property test
		protected void Read_AmbientPropertyContainer (XamlReader r, bool extensionBased)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual ("http://www.domain.com/path", r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue (r.Read (), "ns#2-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

			// t:AmbientPropertyContainer
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (SecondTest.ResourcesDict), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			ClassicAssert.IsTrue (r.Read (), "sm#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#1-2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm#1-3");

			xt = new XamlType (typeof (SecondTest.TestObject), r.SchemaContext);
			for (int i = 0; i < 2; i++) {

				if (i == 1 && r is XamlObjectReader && extensionBased) {
					ReadReasourceExtension_AmbientPropertyContainer (r, i, extensionBased);
					continue;
				}

				// t:TestObject
				ClassicAssert.IsTrue (r.Read (), "so#2-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2-2." + i);
				ClassicAssert.AreEqual (xt, r.Type, "so#2-3." + i);

				if (!extensionBased) {
					if (i == 0 && r is XamlXmlReader) // order difference between XamlObjectReader and XamlXmlReader ...
						ReadName_AmbientPropertyContainer (r, i);

					ReadTestProperty_AmbientPropertyContainer (r, i, extensionBased);

					if (i == 0 && r is XamlObjectReader) // order difference between XamlObjectReader and XamlXmlReader ...
						ReadName_AmbientPropertyContainer (r, i);
				}

				if (r is XamlObjectReader && extensionBased) { 
					ReadTestProperty_AmbientPropertyContainer (r, i, extensionBased);
					ReadName_AmbientPropertyContainer (r, i);
				}

				ReadKey_AmbientPropertyContainer (r, i, extensionBased);

				if (extensionBased && i == 1)
					 ReadTestProperty_AmbientPropertyContainer (r, i, extensionBased);

				// /t:TestObject
				ClassicAssert.IsTrue (r.Read (), "eo#2-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2-2." + i);
			}

			// /m:Items
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:AmbientPropertyContainer
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_AmbientPropertyContainer3(XamlReader r, bool extensionBased)
		{
			ClassicAssert.IsTrue(r.Read(), "ns#1-1");
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull(r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual("", r.Namespace.Prefix, "ns#1-4");
			ClassicAssert.AreEqual("http://www.domain.com/path", r.Namespace.Namespace, "ns#1-5");

			ClassicAssert.IsTrue(r.Read(), "ns#2-1");
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2-2");
			ClassicAssert.IsNotNull(r.Namespace, "ns#2-3");
			ClassicAssert.AreEqual("x", r.Namespace.Prefix, "ns#2-4");
			ClassicAssert.AreEqual(XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#2-5");

		}

		void ReadKey_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			// m:Key
			ClassicAssert.IsTrue (r.Read (), "sm#4-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#4-2." + i);
			ClassicAssert.AreEqual (XamlLanguage.Key, r.Member, "sm#4-3." + i);

			if (!extensionBased || r is XamlObjectReader) {
				// t:String (as it is specific derived type as compared to the key object type in Dictionary<object,object>)
				ClassicAssert.IsTrue (r.Read (), "so#5-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#5-2." + i);
				ClassicAssert.AreEqual (XamlLanguage.String, r.Type, "so#5-3." + i);

				ClassicAssert.IsTrue (r.Read (), "sm#5-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#5-2." + i);
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "sm#5-3." + i);

				ClassicAssert.IsTrue (r.Read (), "v#5-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#5-2." + i);

				ClassicAssert.IsTrue (r.Read (), "em#5-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#5-2." + i);

				// /t:String
				ClassicAssert.IsTrue (r.Read (), "eo#5-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#5-2." + i);
			} else {
				// it is in attribute string without type in xml.
				ClassicAssert.IsTrue (r.Read (), "v#y-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#y-2." + i);
				ClassicAssert.AreEqual (i == 0 ? "TestDictItem" : "okay", r.Value, "v#y-3." + i);
			}

			// /m:Key
			ClassicAssert.IsTrue (r.Read (), "em#4-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#4-2." + i);
		}

		void ReadName_AmbientPropertyContainer (XamlReader r, int i)
		{
			// m:Name
			ClassicAssert.IsTrue (r.Read (), "sm#3-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#3-2." + i);
			ClassicAssert.AreEqual (XamlLanguage.Name, r.Member, "sm#3-3." + i);

			ClassicAssert.IsTrue (r.Read (), "v#3-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#3-2." + i);
			ClassicAssert.AreEqual ("__ReferenceID0", r.Value, "v#3-3." + i);

			// /m:Name
			ClassicAssert.IsTrue (r.Read (), "em#3-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#3-2." + i);
		}

		void ReadTestProperty_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			var xt = new XamlType (typeof (SecondTest.TestObject), r.SchemaContext);

			// m:TestProperty
			ClassicAssert.IsTrue (r.Read (), "sm#2-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#2-2." + i);
			ClassicAssert.AreEqual (xt.GetMember ("TestProperty"), r.Member, "sm#2-3." + i);

			if (i == 0) {
				// t:TestObject={x:Null}
				ClassicAssert.IsTrue (r.Read (), "so#3-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#3-2." + i);
				ClassicAssert.AreEqual (XamlLanguage.Null, r.Type, "so#3-3." + i);
				ClassicAssert.IsTrue (r.Read (), "eo#3-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#3-2." + i);
			} else if (extensionBased) {
				ReadReasourceExtension_AmbientPropertyContainer (r, i, extensionBased);
			} else {
				ReadReference_AmbientPropertyContainer (r, i, extensionBased);
			}

			// /m:TestProperty
			ClassicAssert.IsTrue (r.Read (), "em#2-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#2-2." + i);
		}

		void ReadReasourceExtension_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			// t:ResourceExtension
			var xt = r.SchemaContext.GetXamlType (typeof (SecondTest.ResourceExtension));
			ClassicAssert.IsTrue (r.Read (), "so#z-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#z-2." + i);
			ClassicAssert.AreEqual (xt, r.Type, "so#z-2." + i);

			if (r is XamlObjectReader) {

				// m:Arguments
				ClassicAssert.IsTrue (r.Read (), "sm#zz-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#zz-2." + i);
				ClassicAssert.AreEqual (XamlLanguage.Arguments, r.Member, "sm#zz-3." + i);

				ReadReference_AmbientPropertyContainer (r, i, extensionBased);

				// /m:Arguments
				ClassicAssert.IsTrue (r.Read (), "em#zz-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#zz-2." + i);

				ReadKey_AmbientPropertyContainer (r, i, extensionBased);

			} else {

				// m:PositionalParameters
				ClassicAssert.IsTrue (r.Read (), "sm#z-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#z-2." + i);
				ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "sm#z-3." + i);

				ClassicAssert.IsTrue (r.Read (), "v#z-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#z-2." + i);

				// /m:PositionalParameters
				ClassicAssert.IsTrue (r.Read (), "em#z-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#z-2." + i);
			}
			
			// /t:ResourceExtension
			ClassicAssert.IsTrue (r.Read (), "eo#z-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#z-2." + i);
		}

		void ReadReference_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			// x:Reference
			ClassicAssert.IsTrue (r.Read (), "so#zz2-1." + i);
			ClassicAssert.AreEqual (XamlLanguage.Reference, r.Type, "so#zz2-2." + i);

			// posparm
			ClassicAssert.IsTrue (r.Read (), "sm#zz2-1." + i);
			ClassicAssert.AreEqual (XamlLanguage.PositionalParameters, r.Member, "sm#zz2-3." + i);
			// value
			ClassicAssert.IsTrue (r.Read (), "v#zz2-1." + i);
			ClassicAssert.AreEqual ("__ReferenceID0", r.Value, "v#zz2-2." + i);
			// /posparm
			ClassicAssert.IsTrue (r.Read (), "em#zz2-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#zz2-2." + i);
			// /x:Reference
			ClassicAssert.IsTrue (r.Read (), "eo#zz2-1." + i);
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#zz2-2." + i);
		}
		#endregion

		protected void Read_NullableContainer (XamlReader r)
		{
			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ClassicAssert.AreEqual (assns, r.Namespace.Namespace, "ns#1-5");

			// t:NullableContainer
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (NullableContainer), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:TestProp
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("TestProp"), r.Member, "sm1#3");

			// x:Value
			ClassicAssert.IsTrue (r.Read (), "v#1-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#1-2");
			ClassicAssert.AreEqual ("5", r.Value, "v#1-3");

			// /m:TestProp
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:NullableContainer
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_DeferredLoadingContainerMember(XamlReader r)
		{
			ClassicAssert.IsTrue(r.Read(), "ns#1-1");
			ClassicAssert.AreEqual(XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull(r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual("", r.Namespace.Prefix, "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType().GetTypeInfo().Assembly.GetName().Name;
			ClassicAssert.AreEqual(assns, r.Namespace.Namespace, "ns#1-5");

			// t:DeferredLoadingContainerMember
			ClassicAssert.IsTrue(r.Read(), "so#1-1");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType(typeof(DeferredLoadingContainerMember), r.SchemaContext);
			ClassicAssert.AreEqual(xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase(r);

			ClassicAssert.IsTrue(r.Read(), "sm1#1");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual(xt.GetMember("Child"), r.Member, "sm1#3");

			ClassicAssert.IsTrue(r.Read(), "v#1-1");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, "v#1-2");
			ClassicAssert.AreEqual(xt = r.SchemaContext.GetXamlType(typeof(DeferredLoadingChild)), r.Type, "v#1-3");

			ClassicAssert.IsTrue(r.Read(), "sm1#1");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual(xt.GetMember("Foo"), r.Member, "sm1#3");

			ClassicAssert.IsTrue(r.Read(), "em#1-1");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "em#1-2");
			ClassicAssert.AreEqual("Some value", r.Value, "em#1-2");

			ClassicAssert.IsTrue(r.Read(), "em#1-1");
			ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, "em#1-2");

			ClassicAssert.IsTrue(r.Read(), "em#1-1");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, "em#1-2");

			ClassicAssert.IsTrue(r.Read(), "em#1-1");
			ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:NullableContainer
			ClassicAssert.IsTrue(r.Read(), "eo#1-1");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse(r.Read(), "end");
		}

		protected void Read_DirectListContainer (XamlReader r)
		{
			var assns1 = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			var assns2 = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			ReadNamespace (r, String.Empty, assns1, "ns#1");
			ReadNamespace (r, "scg", assns2, "ns#2");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#3");

			// t:DirectListContainer
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (DirectListContainer), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("Items"), r.Member, "sm1#3");

			// GetObject
			ClassicAssert.IsTrue (r.Read (), "go#1");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "go#2");

			// m:Items(GetObject)
			ClassicAssert.IsTrue (r.Read (), "sm2#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm2#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm2#3");

			xt = r.SchemaContext.GetXamlType (typeof (DirectListContent));
			for (int i = 0; i < 3; i++) {
				// t:DirectListContent
				ClassicAssert.IsTrue (r.Read (), "so#x-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#x-2." + i);
				ClassicAssert.AreEqual (xt, r.Type, "so#x-3." + i);

				// m:Value
				ClassicAssert.IsTrue (r.Read (), "sm#x1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#x2");
				ClassicAssert.AreEqual (xt.GetMember ("Value"), r.Member, "sm#x3");

				// x:Value
				ClassicAssert.IsTrue (r.Read (), "v#x-1");
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#x-2");
				ClassicAssert.AreEqual ("Hello" + (i + 1), r.Value, "v#x-3");

				// /m:Value
				ClassicAssert.IsTrue (r.Read (), "em#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#x-2");

				// /t:DirectListContent
				ClassicAssert.IsTrue (r.Read (), "eo#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#x-2");
			}

			// /m:Items(GetObject)
			ClassicAssert.IsTrue (r.Read (), "em#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#2-2");

			// /GetObject
			ClassicAssert.IsTrue (r.Read (), "ego#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "ego#2-2");

			// /m:Items
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:DirectListContainer
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_DirectDictionaryContainer (XamlReader r)
		{
			var assns1 = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ReadNamespace (r, String.Empty, assns1, "ns#1");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#2");

			// t:DirectDictionaryContainer
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (DirectDictionaryContainer), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (xt.GetMember ("Items"), r.Member, "sm1#3");

			// GetObject
			ClassicAssert.IsTrue (r.Read (), "go#1");
			ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "go#2");

			// m:Items(GetObject)
			ClassicAssert.IsTrue (r.Read (), "sm2#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm2#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm2#3");

			xt = r.SchemaContext.GetXamlType (typeof (int));
			for (int i = 0; i < 3; i++) {
				// t:DirectDictionaryContent
				ClassicAssert.IsTrue (r.Read (), "so#x-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#x-2." + i);
				ClassicAssert.AreEqual (xt, r.Type, "so#x-3." + i);

				// m:Key
				ClassicAssert.IsTrue (r.Read (), "sm#y1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#y2");
				ClassicAssert.AreEqual (XamlLanguage.Key, r.Member, "sm#y3");

				// x:Value
				ClassicAssert.IsTrue (r.Read (), "v#y-1");
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#y-2");
				ClassicAssert.AreEqual (((EnumValueType) i).ToString ().ToLower (), r.Value, "v#y-3");

				// /m:Key
				ClassicAssert.IsTrue (r.Read (), "em#y-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#y-2");

				// m:Value
				ClassicAssert.IsTrue (r.Read (), "sm#x1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#x2");
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "sm#x3");

				// x:Value
				ClassicAssert.IsTrue (r.Read (), "v#x-1");
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#x-2");
				ClassicAssert.AreEqual ("" + (i + 2) * 10, r.Value, "v#x-3");

				// /m:Value
				ClassicAssert.IsTrue (r.Read (), "em#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#x-2");

				// /t:DirectDictionaryContent
				ClassicAssert.IsTrue (r.Read (), "eo#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#x-2");
			}

			// /m:Items(GetObject)
			ClassicAssert.IsTrue (r.Read (), "em#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#2-2");

			// /GetObject
			ClassicAssert.IsTrue (r.Read (), "ego#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "ego#2-2");

			// /m:Items
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:DirectDictionaryContainer
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_DirectDictionaryContainer2 (XamlReader r)
		{
			ReadNamespace (r, String.Empty, "http://www.domain.com/path", "ns#1");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#2");

			// t:DirectDictionaryContainer
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (SecondTest.ResourcesDict2), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm1#3");

			xt = r.SchemaContext.GetXamlType (typeof (SecondTest.TestObject2));
			for (int i = 0; i < 2; i++) {
				// t:TestObject
				ClassicAssert.IsTrue (r.Read (), "so#x-1." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#x-2." + i);
				ClassicAssert.AreEqual (xt, r.Type, "so#x-3." + i);

				// m:Key
				ClassicAssert.IsTrue (r.Read (), "sm#y1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#y2");
				ClassicAssert.AreEqual (XamlLanguage.Key, r.Member, "sm#y3");

				// value
				ClassicAssert.IsTrue (r.Read (), "v#y-1");
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#y-2");
				ClassicAssert.AreEqual (i == 0 ? "1" : "two", r.Value, "v#y-3");

				// /m:Key
				ClassicAssert.IsTrue (r.Read (), "em#y-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#y-2");

				// m:TestProperty
				ClassicAssert.IsTrue (r.Read (), "sm#x1");
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#x2");
				ClassicAssert.AreEqual (xt.GetMember ("TestProperty"), r.Member, "sm#x3");

				// x:Value
				ClassicAssert.IsTrue (r.Read (), "v#x-1");
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#x-2");
				ClassicAssert.AreEqual (i == 0 ? "1" : "two", r.Value, "v#x-3");

				// /m:TestProperty
				ClassicAssert.IsTrue (r.Read (), "em#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#x-2");

				// /t:TestObject
				ClassicAssert.IsTrue (r.Read (), "eo#x-1");
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#x-2");
			}

			// /m:Items
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// /t:DirectDictionaryContainer
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		protected void Read_ContentPropertyContainer (XamlReader r)
		{
			ReadNamespace (r, String.Empty, "http://www.domain.com/path", "ns#1");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#2");

			// 1:: t:ContentPropertyContainer
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (typeof (SecondTest.ContentPropertyContainer), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// 2:: m:Items
			ClassicAssert.IsTrue (r.Read (), "sm1#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm1#2");
			ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "sm1#3");

			xt = r.SchemaContext.GetXamlType (typeof (SecondTest.SimpleType));
			for (int i = 0; i < 2; i++) {
				// 3:: t:SimpleType
				ClassicAssert.IsTrue (r.Read (), "so#x-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#x-2" + "." + i);
				ClassicAssert.AreEqual (xt, r.Type, "so#x-3" + "." + i);

				// 4:: m:Key
				ClassicAssert.IsTrue (r.Read (), "sm#y1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#y2" + "." + i);
				ClassicAssert.AreEqual (XamlLanguage.Key, r.Member, "sm#y3" + "." + i);

				// 4:: value
				ClassicAssert.IsTrue (r.Read (), "v#y-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "v#y-2" + "." + i);
				ClassicAssert.AreEqual (i == 0 ? "one" : "two", r.Value, "v#y-3" + "." + i);

				// 4:: /m:Key
				ClassicAssert.IsTrue (r.Read (), "em#y-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#y-2" + "." + i);

if (i == 0) {

				// 4-2:: m:Items(ContentProperty)
				ClassicAssert.IsTrue (r.Read (), "sm#x1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sm#x2" + "." + i);
				ClassicAssert.AreEqual (xt.GetMember ("Items"), r.Member, "sm#x3" + "." + i);

				// 5:: GetObject
				ClassicAssert.IsTrue (r.Read (), "go#z-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "go#z-2" + "." + i);

				// 6:: m:Items(GetObject)
				ClassicAssert.IsTrue (r.Read (), "smz#1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smz#2" + "." + i);
				ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "smz#3" + "." + i);

				for (int j = 0; j < 2; j++) {
					// 7:: t:SimpleType
					ClassicAssert.IsTrue (r.Read (), "soi#x-1" + "." + i + "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "soi#x-2" + "." + i + "-" + j);
					ClassicAssert.AreEqual (xt, r.Type, "soi#z-3" + "." + i + "-" + j);

					// 7:: /t:SimpleType
					ClassicAssert.IsTrue (r.Read (), "eoi#x-1" + "." + i + "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eoi#x-2" + "." + i + "-" + j);
				}

				// 6:: /m:Items(GetObject)
				ClassicAssert.IsTrue (r.Read (), "emz#x-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emz#x-2" + "." + i);

				// 5:: /GetObject
				ClassicAssert.IsTrue (r.Read (), "eo#z-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#z-2" + "." + i);

				// 4:: /m:Items(ContentProperty)
				ClassicAssert.IsTrue (r.Read (), "em#x1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#x2" + "." + i);

				// 4-2:: m:NonContentItems
				ClassicAssert.IsTrue (r.Read (), "smv#1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smv#2" + "." + i);
				ClassicAssert.AreEqual (xt.GetMember ("NonContentItems"), r.Member, "smv#3" + "." + i);

				// 5-2:: GetObject
				ClassicAssert.IsTrue (r.Read (), "go#z-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.GetObject, r.NodeType, "go#v-2" + "." + i);

				// 6-2:: m:Items
				ClassicAssert.IsTrue (r.Read (), "smw#1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "smw#2" + "." + i);
				ClassicAssert.AreEqual (XamlLanguage.Items, r.Member, "smw#3" + "." + i);

				for (int j = 0; j < 2; j++) {
					// 7-2:: t:SimpleType
					ClassicAssert.IsTrue (r.Read (), "soi2#x-1" + "." + i + "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "soi2#x-2" + "." + i + "-" + j);
					ClassicAssert.AreEqual (xt, r.Type, "soi2#z-3" + "." + i + "-" + j);

					// 7-2:: /t:SimpleType
					ClassicAssert.IsTrue (r.Read (), "eoi2#x-1" + "." + i + "-" + j);
					ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eoi2#x-2" + "." + i + "-" + j);
				}

				// 6-2:: /m:Items
				ClassicAssert.IsTrue (r.Read (), "emw#1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emw#2" + "." + i);

				// 5-2:: /GetObject
				ClassicAssert.IsTrue (r.Read (), "eo#v-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#v-2" + "." + i);

				// 4-2:: /m:NonContentItems
				ClassicAssert.IsTrue (r.Read (), "emv#1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "emv#2" + "." + i);

}

				// 3:: /t:SimpleType
				ClassicAssert.IsTrue (r.Read (), "eo#x-1" + "." + i);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#x-2" + "." + i);
			}

			// 2:: /m:Items
			ClassicAssert.IsTrue (r.Read (), "em#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "em#1-2");

			// 1:: /t:ContentPropertyContainer
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}
		
		protected void Read_AttachedProperty (XamlReader r, string additionalNamspace = null, Type wrapperType = null)
		{
			var at = new XamlType (typeof (Attachable), r.SchemaContext);

			ClassicAssert.IsTrue (r.Read (), "ns#1-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#1-2");
			ClassicAssert.IsNotNull (r.Namespace, "ns#1-3");
			ClassicAssert.AreEqual ("", r.Namespace.Prefix, "ns#1-4");

			if (additionalNamspace != null)
			{
				this.ReadNamespace(r, "ns", additionalNamspace, "ns#2");
			}

			// t:AttachedWrapper
			ClassicAssert.IsTrue (r.Read (), "so#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#1-2");
			var xt = new XamlType (wrapperType ?? typeof(AttachedWrapper), r.SchemaContext);
			ClassicAssert.AreEqual (xt, r.Type, "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ReadAttachedProperty (r, at.GetAttachableMember ("Foo"), "x", "x");

			// m:Value
			ClassicAssert.IsTrue (r.Read (), "sm#2-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "#sm#2-2");
			ClassicAssert.AreEqual (xt.GetMember ("Value"), r.Member, "sm#2-3");

			// t:Attached
			ClassicAssert.IsTrue (r.Read (), "so#2-1");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "#so#2-2");
			ClassicAssert.AreEqual (r.SchemaContext.GetXamlType (typeof (Attached)), r.Type, "so#2-3");

			ReadAttachedProperty (r, at.GetAttachableMember ("Foo"), "y", "y");

			ClassicAssert.IsTrue (r.Read (), "eo#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "#eo#2-2");

			// /m:Value
			ClassicAssert.IsTrue (r.Read (), "em#2-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "#em#2-2");

			// /t:AttachedWrapper
			ClassicAssert.IsTrue (r.Read (), "eo#1-1");
			ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#1-2");

			ClassicAssert.IsFalse (r.Read (), "end");
		}

		void ReadAttachedProperty (XamlReader r, XamlMember xm, string value, string label)
		{
			ClassicAssert.IsTrue (r.Read (), label + "#1-1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, label + "#1-2");
			ClassicAssert.AreEqual (xm, r.Member, label + "#1-3");

			ClassicAssert.IsTrue (r.Read (), label + "#2-1");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, label + "#2-2");
			ClassicAssert.AreEqual (value, r.Value, label + "2-3");

			ClassicAssert.IsTrue (r.Read (), label + "#3-1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, label + "#3-2");
		}

		protected void Read_CommonXamlPrimitive (object obj)
		{
			var r = new XamlObjectReader (obj);
			Read_CommonXamlType (r);
			Read_Initialization (r, obj);
		}

		// from StartMember of Initialization to EndMember
		protected string Read_Initialization (XamlReader r, object comparableValue)
		{
			ClassicAssert.IsTrue (r.Read (), "init#1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "init#2");
			ClassicAssert.IsNotNull (r.Member, "init#3");
			ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "init#3-2");
			ClassicAssert.IsTrue (r.Read (), "init#4");
			ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "init#5");
			ClassicAssert.AreEqual (typeof (string), r.Value.GetType (), "init#6");
			string ret = (string) r.Value;
			if (comparableValue != null)
				ClassicAssert.AreEqual (comparableValue.ToString (), r.Value, "init#6-2");
			ClassicAssert.IsTrue (r.Read (), "init#7");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "init#8");
			return ret;
		}

		protected object [] Read_AttributedArguments_String (XamlReader r, string [] argNames) // valid only for string arguments.
		{
			object [] ret = new object [argNames.Length];

			ClassicAssert.IsTrue (r.Read (), "attarg.Arguments.Start1");
			ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "attarg.Arguments.Start2");
			ClassicAssert.IsNotNull (r.Member, "attarg.Arguments.Start3");
			ClassicAssert.AreEqual (XamlLanguage.Arguments, r.Member, "attarg.Arguments.Start4");
			for (int i = 0; i < argNames.Length; i++) {
				string arg = argNames [i];
				ClassicAssert.IsTrue (r.Read (), "attarg.ArgStartObject1." + arg);
				ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "attarg.ArgStartObject2." + arg);
				ClassicAssert.AreEqual (typeof (string), r.Type.UnderlyingType, "attarg.ArgStartObject3." + arg);
				ClassicAssert.IsTrue (r.Read (), "attarg.ArgStartMember1." + arg);
				ClassicAssert.AreEqual (XamlNodeType.StartMember, r.NodeType, "attarg.ArgStartMember2." + arg);
				ClassicAssert.AreEqual (XamlLanguage.Initialization, r.Member, "attarg.ArgStartMember3." + arg); // (as the argument is string here by definition)
				ClassicAssert.IsTrue (r.Read (), "attarg.ArgValue1." + arg);
				ClassicAssert.AreEqual (XamlNodeType.Value, r.NodeType, "attarg.ArgValue2." + arg);
				ClassicAssert.AreEqual (typeof (string), r.Value.GetType (), "attarg.ArgValue3." + arg);
				ret [i] = r.Value;
				ClassicAssert.IsTrue (r.Read (), "attarg.ArgEndMember1." + arg);
				ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "attarg.ArgEndMember2." + arg);
				ClassicAssert.IsTrue (r.Read (), "attarg.ArgEndObject1." + arg);
				ClassicAssert.AreEqual (XamlNodeType.EndObject, r.NodeType, "attarg.ArgEndObject2." + arg);
			}
			ClassicAssert.IsTrue (r.Read (), "attarg.Arguments.End1");
			ClassicAssert.AreEqual (XamlNodeType.EndMember, r.NodeType, "attarg.Arguments.End2");
			return ret;
		}

		// from initial to StartObject
		protected void Read_CommonXamlType (XamlObjectReader r)
		{
			Read_CommonXamlType (r, delegate {
				ClassicAssert.IsNull (r.Instance, "ct#4");
				});
		}
		
		protected void Read_CommonXamlType (XamlReader r, Action validateInstance)
		{
			ClassicAssert.IsTrue (r.Read (), "ct#1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ct#2");
			ClassicAssert.IsNotNull (r.Namespace, "ct#3");
			ClassicAssert.AreEqual ("x", r.Namespace.Prefix, "ct#3-2");
			ClassicAssert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ct#3-3");
			if (validateInstance != null)
				validateInstance ();

			ClassicAssert.IsTrue (r.Read (), "ct#5");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "ct#6");
		}

		static readonly Type[] mscorlib_types = { typeof(IList<>), typeof(bool) };

		static readonly Assembly[] mscorlib_assemblies = mscorlib_types.Select(r => r.GetTypeInfo().Assembly).Distinct().ToArray();

		static string GetFixedAssemblyName(Type type)
		{
			if (mscorlib_assemblies.Contains(type.GetTypeInfo().Assembly))
				return "System.Private.CoreLib";
			return type.GetTypeInfo().Assembly.GetName().Name;
		}


		// from initial to StartObject
		protected void Read_CommonClrType (XamlReader r, object obj, params KeyValuePair<string,string> [] additionalNamespaces)
		{
			ClassicAssert.IsTrue (r.Read (), "ct#1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ct#2");
			ClassicAssert.IsNotNull (r.Namespace, "ct#3");
			ClassicAssert.AreEqual (String.Empty, r.Namespace.Prefix, "ct#3-2");
			ClassicAssert.AreEqual ("clr-namespace:" + obj.GetType ().Namespace + ";assembly=" + GetFixedAssemblyName(obj.GetType ()), r.Namespace.Namespace, "ct#3-3");

			foreach (var kvp in additionalNamespaces) {
				ClassicAssert.IsTrue (r.Read (), "ct#4." + kvp.Key);
				ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ct#5." + kvp.Key);
				ClassicAssert.IsNotNull (r.Namespace, "ct#6." + kvp.Key);
				ClassicAssert.AreEqual (kvp.Key, r.Namespace.Prefix, "ct#6-2." + kvp.Key);
				ClassicAssert.AreEqual (kvp.Value, r.Namespace.Namespace, "ct#6-3." + kvp.Key);
			}

			ClassicAssert.IsTrue (r.Read (), "ct#7");
			ClassicAssert.AreEqual (XamlNodeType.StartObject, r.NodeType, "ct#8");
		}


		protected void Read_DefaultValueMemberShouldBeOmittedString(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadNamespace(r, "x", XamlLanguage.Xaml2006Namespace, "ns2");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesString)), "o1", xt =>
			{
				ReadBase(r);

				ReadMember(r, xt.GetMember("NoDefaultValue"), "m1", xm =>
				{
					ReadObject(r, XamlLanguage.Null, "o2");
				});
			});

			ClassicAssert.IsFalse(r.Read(), "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedStringNonDefault(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesString)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "Hello");
				ReadMemberWithValue(r, xt.GetMember("NullDefaultValue"), "m2", "There");
				ReadMemberWithValue(r, xt.GetMember("SpecificDefaultValue"), "m3", "Friend");
			});

			ClassicAssert.IsFalse(r.Read(), "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedInt(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "0");
			});

			ClassicAssert.IsFalse(r.Read(), "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedIntNonDefault(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "1");
				ReadMemberWithValue(r, xt.GetMember("SpecificDefaultValue"), "m3", "3");
				ReadMemberWithValue(r, xt.GetMember("ZeroDefaultValue"), "m2", "2");
			});

			ClassicAssert.IsFalse(r.Read(), "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedNullableInt(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadNamespace(r, "x", XamlLanguage.Xaml2006Namespace, "ns2");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesNullableInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMember(r, xt.GetMember("NoDefaultValue"), "m1", xm =>
				{
					ReadObject(r, XamlLanguage.Null, "o2");
				});
			});

			ClassicAssert.IsFalse(r.Read(), "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedNullableIntNonDefault(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesNullableInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "1");
				ReadMemberWithValue(r, xt.GetMember("NullDefaultValue"), "m2", "2");
				ReadMemberWithValue(r, xt.GetMember("SpecificDefaultValue"), "m3", "3");
				ReadMemberWithValue(r, xt.GetMember("ZeroDefaultValue"), "m4", "4");
			});

			ClassicAssert.IsFalse(r.Read(), "end");
		}

		protected void ReadBase (XamlReader r)
		{
			if (!(r is XamlXmlReader))
				return;
#if !PCL
			if (Type.GetType ("Mono.Runtime") == null)
				return;
#endif
            // we include the xml declaration, MS.NET does not?
            ClassicAssert.IsTrue(r.Read(), "sbase#1");
            ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, "sbase#2");
            ClassicAssert.AreEqual(XamlLanguage.Base, r.Member, "sbase#3");

            ClassicAssert.IsTrue(r.Read(), "vbase#1");
            ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, "vbase#2");
            ClassicAssert.IsTrue(r.Value is string, "vbase#3");

            ClassicAssert.IsTrue(r.Read(), "ebase#1");
            ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, "ebase#2");
        }

		protected void ReadNamespace (XamlReader r, string prefix, string ns, string label)
		{
			ClassicAssert.IsTrue (r.Read (), label + "-1");
			ClassicAssert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, label + "-2");
			ClassicAssert.IsNotNull (r.Namespace, label + "-3");
			ClassicAssert.AreEqual (prefix, r.Namespace.Prefix, label + "-4");
			ClassicAssert.AreEqual (ns, r.Namespace.Namespace, label + "-5");
		}

		protected void ReadMemberWithValue (XamlReader r, XamlMember member, string label, params object[] values)
		{
			ReadMember(r, member, label, m => {
				for (int i = 0; i < values.Length; i++)
				{
					ReadValue(r, values[i], label + "-v" + i);
				}
			});
		}

		protected void ReadObject(XamlReader r, XamlType type, string label, Action<XamlType> readContent = null)
		{
			ClassicAssert.IsTrue(r.Read(), label + "-1");
			ClassicAssert.AreEqual(XamlNodeType.StartObject, r.NodeType, label + "-2");
			ClassicAssert.AreEqual(type, r.Type, label + "3");

			if (readContent != null)
				readContent(type);

			ClassicAssert.IsTrue(r.Read(), label + "4");
			ClassicAssert.AreEqual(XamlNodeType.EndObject, r.NodeType, label + "5");
		}

		protected void ReadValue(XamlReader r, object value, string label)
		{
			ClassicAssert.IsTrue(r.Read(), label + "-1");
			ClassicAssert.AreEqual(XamlNodeType.Value, r.NodeType, label + "-2");
			ClassicAssert.AreEqual(value, r.Value, label + "-3");
		}

		protected void ReadMember (XamlReader r, XamlMember member, string label, Action<XamlMember> readContent = null)
		{
			ClassicAssert.IsTrue(r.Read(), label + "-1");
			ClassicAssert.AreEqual(XamlNodeType.StartMember, r.NodeType, label + "-2");
			ClassicAssert.AreEqual(member, r.Member, label + "-3");
			if (readContent != null)
				readContent(member);
			ClassicAssert.IsTrue(r.Read(), label + "-5");
			ClassicAssert.AreEqual(XamlNodeType.EndMember, r.NodeType, label + "-6");
		}
	}
}

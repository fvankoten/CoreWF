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
using MonoTests.System.Xaml;
using System.Globalization;
using System.ComponentModel;
using CategoryAttribute = NUnit.Framework.CategoryAttribute;
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


namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class ValueSerializerContextTest
	{
		public static void RunCanConvertFromTest(ITypeDescriptorContext context, Type sourceType) => runCanConvertFrom?.Invoke(context, sourceType);
		public static void RunConvertFromTest(ITypeDescriptorContext context, CultureInfo culture, object value) => runConvertFrom?.Invoke(context, culture, value);
		public static void RunCanConvertToTest(ITypeDescriptorContext context, Type destinationType) => runCanConvertTo?.Invoke(context, destinationType);
		public static void RunConvertToTest(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) => runConvertTo?.Invoke(context, culture, value, destinationType);

		static Action<ITypeDescriptorContext, Type> runCanConvertFrom;
		static Action<ITypeDescriptorContext, CultureInfo, object> runConvertFrom;
		static Action<ITypeDescriptorContext, Type> runCanConvertTo;
		static Action<ITypeDescriptorContext, CultureInfo, object, Type> runConvertTo;

		[SetUp]
		public void SetUp()
		{
			runCanConvertFrom = null;
			runConvertFrom = null;
			runCanConvertTo = null;
			runConvertTo = null;
		}

		void SetupReaderService()
		{
			var obj = new TestValueSerialized();
			var xr = new XamlObjectReader(obj);
			while (!xr.IsEof)
				xr.Read();
		}

		void SetupWriterService()
		{
			var obj = new TestValueSerialized();
			var ctx = new XamlSchemaContext();
			var xw = new XamlObjectWriter(ctx);
			var xt = ctx.GetXamlType(obj.GetType());
			xw.WriteStartObject(xt);
			xw.WriteStartMember(XamlLanguage.Initialization);
			xw.WriteValue("v");
			xw.WriteEndMember();
			xw.Close();
		}

		[Test]
		public void ReaderServiceTest()
		{
			bool ranConvertTo = false;
			bool ranCanConvertTo = false;
			runCanConvertTo = (context, destinationType) =>
			{
				ClassicAssert.IsNotNull(context, "#1");
				ClassicAssert.AreEqual(typeof(string), destinationType, "#2");
				//ClassicAssert.IsNull(Provider.GetService(typeof(IXamlNameResolver)), "#3");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlNameProvider)), "#4");
				//ClassicAssert.IsNull(Provider.GetService(typeof(IXamlNamespaceResolver)), "#5");
				ClassicAssert.IsNotNull(context.GetService(typeof(INamespacePrefixLookup)), "#6");
				//ClassicAssert.IsNull(Provider.GetService(typeof(IXamlTypeResolver)), "#7");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlSchemaContextProvider)), "#8");
				ClassicAssert.IsNull(context.GetService(typeof(IAmbientProvider)), "#9");
				ClassicAssert.IsNull(context.GetService(typeof(IAttachedPropertyStore)), "#10");
				ClassicAssert.IsNull(context.GetService(typeof(IDestinationTypeProvider)), "#11");
				ClassicAssert.IsNull(context.GetService(typeof(IXamlObjectWriterFactory)), "#12");
				ranCanConvertTo = true;
			};
			runConvertTo = (context, culture, value, destinationType) =>
			{
				ClassicAssert.IsNotNull(context, "#13");
				ClassicAssert.AreEqual(CultureInfo.InvariantCulture, culture, "#14");
				ClassicAssert.AreEqual(typeof(string), destinationType, "#15");
				//ClassicAssert.IsNull(Provider.GetService(typeof(IXamlNameResolver)), "#16");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlNameProvider)), "#17");
				//ClassicAssert.IsNull(Provider.GetService(typeof(IXamlNamespaceResolver)), "#18");
				ClassicAssert.IsNotNull(context.GetService(typeof(INamespacePrefixLookup)), "#19");
				//ClassicAssert.IsNull(Provider.GetService(typeof(IXamlTypeResolver)), "#20");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlSchemaContextProvider)), "#21");
				ClassicAssert.IsNull(context.GetService(typeof(IAmbientProvider)), "#22");
				ClassicAssert.IsNull(context.GetService(typeof(IAttachedPropertyStore)), "#23");
				ClassicAssert.IsNull(context.GetService(typeof(IDestinationTypeProvider)), "#24");
				ClassicAssert.IsNull(context.GetService(typeof(IXamlObjectWriterFactory)), "#25");
				ranConvertTo = true;
			};
			SetupReaderService();
			ClassicAssert.IsTrue(ranConvertTo, "#26");
			ClassicAssert.IsTrue(ranCanConvertTo, "#27");
		}

		[Test]
		public void WriterServiceTest()
		{
			bool ranConvertFrom = false;
			bool ranCanConvertFrom = false;
			// need to test within the call, not outside of it
			runCanConvertFrom = (context, sourceType) =>
			{
				ClassicAssert.AreEqual(sourceType, typeof(string), "#1");
				if (Compat.IsPortableXaml)
				{
					// only System.Xaml provides the context here (extended functionality)
					ClassicAssert.IsNotNull(context, "#2");
					ClassicAssert.IsNotNull(context.GetService(typeof(IXamlNameResolver)), "#3");
					//ClassicAssert.IsNull (Provider.GetService (typeof(IXamlNameProvider)), "#4");
					ClassicAssert.IsNotNull(context.GetService(typeof(IXamlNamespaceResolver)), "#5");
					//ClassicAssert.IsNull (Provider.GetService (typeof(INamespacePrefixLookup)), "#6");
					ClassicAssert.IsNotNull(context.GetService(typeof(IXamlTypeResolver)), "#7");
					ClassicAssert.IsNotNull(context.GetService(typeof(IXamlSchemaContextProvider)), "#8");
					ClassicAssert.IsNotNull(context.GetService(typeof(IAmbientProvider)), "#9");
					ClassicAssert.IsNull(context.GetService(typeof(IAttachedPropertyStore)), "#10");
					ClassicAssert.IsNotNull(context.GetService(typeof(IDestinationTypeProvider)), "#11");
					ClassicAssert.IsNotNull(context.GetService(typeof(IXamlObjectWriterFactory)), "#12");
				}
				ranCanConvertFrom = true;
			};
			runConvertFrom = (context, culture, value) =>
			{
				ClassicAssert.IsNotNull(context, "#13");
				ClassicAssert.AreEqual(CultureInfo.InvariantCulture, culture, "#14");
				ClassicAssert.AreEqual("v", value, "#15");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlNameResolver)), "#16");
				//ClassicAssert.IsNull (Provider.GetService (typeof(IXamlNameProvider)), "#17");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlNamespaceResolver)), "#18");
				//ClassicAssert.IsNull (Provider.GetService (typeof(INamespacePrefixLookup)), "#19");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlTypeResolver)), "#20");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlSchemaContextProvider)), "#21");
				ClassicAssert.IsNotNull(context.GetService(typeof(IAmbientProvider)), "#22");
				ClassicAssert.IsNull(context.GetService(typeof(IAttachedPropertyStore)), "#23");
				ClassicAssert.IsNotNull(context.GetService(typeof(IDestinationTypeProvider)), "#24");
				ClassicAssert.IsNotNull(context.GetService(typeof(IXamlObjectWriterFactory)), "#25");
				ranConvertFrom = true;
			};
			SetupWriterService();
			ClassicAssert.IsTrue(ranConvertFrom, "#26");
			ClassicAssert.IsTrue(ranCanConvertFrom, "#27");
		}

		[Test]
		public void NameResolver()
		{
			bool ranConvertFrom = false;
			runConvertFrom = (context, culture, sourceType) =>
			{
				var nr = (IXamlNameResolver)context.GetService(typeof(IXamlNameResolver));
				ClassicAssert.IsNull(nr.Resolve("random"), "nr#1");
				//var ft = nr.GetFixupToken (new string [] {"random"}); -> causes internal error.
				//var ft = nr.GetFixupToken (new string [] {"random"}, true); -> causes internal error
				//var ft = nr.GetFixupToken (new string [0], false);
				//ClassicAssert.IsNotNull (ft, "nr#2");
				ranConvertFrom = true;
			};

			SetupWriterService();

			ClassicAssert.IsTrue(ranConvertFrom, "#2");
		}
	}
}

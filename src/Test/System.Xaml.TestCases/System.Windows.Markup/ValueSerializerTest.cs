﻿//
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
using NUnit.Framework;
using MonoTests.System.Xaml;
using System.Windows.Markup;
using NUnit.Framework.Legacy;

#if PCL

using System.Xaml;
using System.Xaml.Schema;
#else
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
#endif

using Category = NUnit.Framework.CategoryAttribute;

namespace MonoTests.System.Windows.Markup
{
	[TestFixture]
	public class ValueSerializerTest
	{
		static ValueSerializerTest()
		{
			std_types = new List<XamlType>(XamlLanguage.AllTypes);
			std_types.Sort((t1, t2) => String.CompareOrdinal(t1.Name, t2.Name));
		}

		static readonly List<XamlType> std_types;
		object[] test_values = { null, true, "test", 3, 'x', 5.5, -1.414m, (byte)255, new Uri("urn:foo"), new NullExtension(), new object(), new PropertyDefinition(), new Reference("Foo"), new StaticExtension(), TimeSpan.FromMinutes(5), new TypeExtension("TypeExt"), new XData() { Text = "test xdata" } }; // can we instantiate MemberDefinition?
		string[] test_strings = { String.Empty, "True", "test", "3", "x", "5.5", "-1.414", "255", "urn:foo", "System.Windows.Markup.NullExtension", "System.Object", "System.Windows.Markup.PropertyDefinition", "System.Windows.Markup.Reference", "System.Windows.Markup.StaticExtension", "00:05:00", "System.Windows.Markup.TypeExtension", "System.Windows.Markup.XData" };

		[Test]
		public void SerializerInAllTypes()
		{
			// var sctx = new XamlSchemaContext (new Assembly [] { typeof (XamlType).Assembly });
			foreach (var t in std_types)
			{
				if (t != XamlLanguage.String)
				{
                    ClassicAssert.IsNull(t.ValueSerializer, "IsNull? " + t.Name);
					continue;
				}
				var v = t.ValueSerializer.ConverterInstance;
				foreach (var val in test_values)
                    ClassicAssert.IsTrue(v.CanConvertToString(val, null), t.Name + "_" + (val != null ? val.GetType() : null));
			}
		}

		static readonly Type[] no_ser_types = { typeof(object), typeof(ArrayExtension), typeof(MemberDefinition), typeof(NullExtension), typeof(PropertyDefinition), typeof(Reference), typeof(StaticExtension), typeof(TypeExtension), typeof(XData) };

		[Test]
		public void GetSerializerForAllTypes()
		{
			// Serializers from GetSerializerFor() returns very 
			// different results from predefined ValueSerializer.
			foreach (var t in std_types)
			{
				var v = ValueSerializer.GetSerializerFor(t.UnderlyingType, null);
				if (no_ser_types.Any(ti => ti == t.UnderlyingType))
				{
                    ClassicAssert.IsNull(v, "NoSerializer_" + t.Name);
					continue;
				}
				else if (v == null)
					Assert.Fail("Missing serializer for " + t.Name);

				// String ValueSerializer is the only exceptional one that mostly fails ConvertToString().
				// For remaining types, ConvertToString() should succeed.
				// What is funny or annoying here is, that always return true for CanConvertToString() while everything fails at ConvertToString() on .NET.
				if (t.UnderlyingType == typeof(string))
					continue;
#if NETSTANDARD
				if (t.UnderlyingType == typeof(Uri))
					continue;
#endif

				int i = 0;
				foreach (var val in test_values)
				{
                    ClassicAssert.IsTrue(v.CanConvertToString(val, null), t.Name + "_" + (val != null ? val.GetType() : null));
					ClassicAssert.AreEqual(test_strings[i++].Fixup(), v.ConvertToString(val, null), "value-" + t.Name + "_" + val);
				}

				// The funny thing also applies to CanConvertToString() and ConvertToString().

				i = 0;
				foreach (var str in test_strings)
				{
                    ClassicAssert.IsTrue(v.CanConvertFromString(str.Fixup(), null), t.Name + "_" + str);
					// FIXME: add tests for this large matrix someday.
					//ClassicAssert.AreEqual (test_values [i++], v.ConvertFromString (str, null), "value-" + t.Name + "_" + str);
				}
			}
		}

		[Test]
		public void GetSerializerFor()
		{
            ClassicAssert.IsNull(ValueSerializer.GetSerializerFor(typeof(Array)), "#1");
            ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(Uri)), "#2");
            ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(Type)), "#3"); // has no TypeConverter (undocumented behavior)
            ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(string)), "#4"); // documented as special
			ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(DateTime)), "#5"); // documented as special
			ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(bool)), "#6"); // has no TypeConverter (undocumented behavior)
			ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(byte)), "#7"); // has no TypeConverter (undocumented behavior)
			ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(char)), "#8"); // has no TypeConverter (undocumented behavior)
			ClassicAssert.IsNull(ValueSerializer.GetSerializerFor(typeof(DBNull)), "#9"); // TypeCode.DBNull
			ClassicAssert.IsNull(ValueSerializer.GetSerializerFor(typeof(object)), "#10");
			ClassicAssert.IsNotNull(ValueSerializer.GetSerializerFor(typeof(TimeSpan)), "#11"); // has no TypeConverter (undocumented behavior), TypeCode.Object -> unexpectedly has non-null serializer!

			/* TODO: not sure why this isn't true in System.Xaml
			ClassicAssert.IsNull (ValueSerializer.GetSerializerFor (typeof (DateTimeOffset)), "#12"); // has no TypeConverter (undocumented behavior), TypeCode.Object -> expected
			*/

			ClassicAssert.IsNull (ValueSerializer.GetSerializerFor (typeof (MyExtension)), "#13");
			ClassicAssert.IsNotNull (ValueSerializer.GetSerializerFor (typeof (MyExtension4)), "#14"); // has TypeConverter.
			ClassicAssert.IsNull (ValueSerializer.GetSerializerFor (typeof (XamlType)), "#15"); // While there is XamlTypeTypeConverter, it is not used on XamlType.
		}

		[Test]
		public void DefaultImplementation ()
		{
			var v = new MyValueSerializer ();

			foreach (var val in test_values) {
				ClassicAssert.IsFalse (v.CanConvertToString (val, null), "CanConvertTo." + val);
				try {
					v.ConvertToString (val, null);
					ClassicAssert.Fail ("ConvertTo." + val);
				} catch (NotSupportedException) {
				}
			}

			// The funny thing also applies to CanConvertToString() and ConvertToString().

			foreach (var str in test_strings) {
				ClassicAssert.IsFalse (v.CanConvertFromString (str, null), "CanConvertFrom." + str);
				try {
					v.ConvertFromString (str, null);
					ClassicAssert.Fail ("ConvertFrom." + str);
				} catch (NotSupportedException) {
				}
			}
			
			ClassicAssert.AreEqual (typeof (NotSupportedException), v.CallGetConvertFromException (null).GetType (), "#1");
			ClassicAssert.AreEqual (typeof (NotSupportedException), v.CallGetConvertToException (null, typeof (int)).GetType (), "#2");
			ClassicAssert.IsFalse (v.TypeReferences (null, null).GetEnumerator ().MoveNext (), "#3");
		}

		[Test]
		public void StringValueSerializer ()
		{
			var vs = ValueSerializer.GetSerializerFor (typeof (string));
			ClassicAssert.AreEqual (String.Empty, vs.ConvertToString (String.Empty, null), "#1"); // it does not convert String.Empty to "\"\""
		}

		class MyValueSerializer : ValueSerializer
		{
			public Exception CallGetConvertFromException (object value)
			{
				return GetConvertFromException (value);
			}

			public Exception CallGetConvertToException (object value, Type destinationType)
			{
				return GetConvertToException (value, destinationType);
			}
		}
	}
}

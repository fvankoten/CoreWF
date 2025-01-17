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

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlReaderSettingsTest
	{
		[Test]
		public void DefaultValues ()
		{
			var s = new XamlReaderSettings ();
			ClassicAssert.IsFalse (s.AllowProtectedMembersOnRoot, "#1");
			ClassicAssert.IsNull (s.BaseUri, "#2");
			ClassicAssert.IsFalse (s.IgnoreUidsOnPropertyElements, "#3");
			ClassicAssert.IsNull (s.LocalAssembly, "#4");
			ClassicAssert.IsFalse (s.ProvideLineInfo, "#5");
			ClassicAssert.IsFalse (s.ValuesMustBeString, "#6");
		}

		[Test]
		public void CopyConstructorNull ()
		{
			new XamlReaderSettings (null);
		}

		[Test]
		public void CopyConstructor ()
		{
			var s = new XamlReaderSettings ();
			s.AllowProtectedMembersOnRoot = true;
			s.IgnoreUidsOnPropertyElements = true;
			s.ProvideLineInfo = true;
			s.ValuesMustBeString = true;
			s.BaseUri = new Uri ("urn:foo");
			s.LocalAssembly = typeof (object).GetTypeInfo().Assembly;

			s = new XamlReaderSettings (s);

			ClassicAssert.IsTrue (s.AllowProtectedMembersOnRoot, "#1");
			ClassicAssert.IsTrue (s.BaseUri.Equals (new Uri ("urn:foo")), "#2");
			ClassicAssert.IsTrue (s.IgnoreUidsOnPropertyElements, "#3");
			ClassicAssert.AreEqual (typeof (int).GetTypeInfo().Assembly, s.LocalAssembly, "#4");
			ClassicAssert.IsTrue (s.ProvideLineInfo, "#5");
			ClassicAssert.IsTrue (s.ValuesMustBeString, "#6");
		}
	}
}

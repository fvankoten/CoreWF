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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
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

using Category = NUnit.Framework.CategoryAttribute;

namespace MonoTests.System.Windows.Markup
{
	[TestFixture]
	public class ReferenceTest
	{
		[Test]
		public void ConstructorNullName ()
		{
			new Reference ((string) null); // it is somehow allowed
		}

		[Test]
		public void ProvideValueWithoutTypeOrName ()
		{
			var reference = new Reference ();
			Assert.Throws<ArgumentNullException> (() => reference.ProvideValue (null));
		}

		[Test]
		public void ProvideValueWithNameWithoutResolver ()
		{
			var x = new Reference ("X");
			Assert.Throws<ArgumentNullException> (() => x.ProvideValue (null)); // serviceProvider is required.
		}

		[Test]
		public void ProvideValueWithNameWithProviderNoResolver ()
		{
			var x = new Reference ("X");
			Assert.Throws<InvalidOperationException> (() => x.ProvideValue (new NameServiceProvider (false, false)));
		}

		[Test]
		public void ProvideValueWithNameWithProviderResolveFail ()
		{
			var x = new Reference ("X");
			var r = new NameServiceProvider (true, false);
			ClassicAssert.AreEqual ("BAR", x.ProvideValue (r), "#1");
		}

		[Test]
		public void ProvideValueWithNameWithProviderResolveSuccess ()
		{
			var x = new Reference ("Y");
			var r = new NameServiceProvider (true, true);
			ClassicAssert.AreEqual ("FOO", x.ProvideValue (r), "#1");
		}

		class NameServiceProvider : IServiceProvider
		{
			Resolver resolver;

			public NameServiceProvider (bool worksFine, bool resolvesFine)
			{
				resolver = worksFine ? new Resolver (resolvesFine) : null;
			}

			public object GetService (Type serviceType)
			{
				ClassicAssert.AreEqual (typeof (IXamlNameResolver), serviceType, "TypeToResolve");
				return resolver;
			}
		}

		class Resolver : IXamlNameResolver
		{
			bool resolves;

			public Resolver (bool resolvesFine)
			{
				resolves = resolvesFine;
			}

			public IEnumerable<KeyValuePair<string, object>> GetAllNamesAndValuesInScope ()
			{
				throw new Exception ();
			}
			
			public object GetFixupToken (IEnumerable<string> names)
			{
				throw new NotImplementedException ();
			}
			
			// only X (which 'failed' to resolve) calls this
			public object GetFixupToken (IEnumerable<string> names, bool canAssignDirectly)
			{
				ClassicAssert.IsTrue (canAssignDirectly, "canAssignDirectly");
				ClassicAssert.AreEqual (1, names.Count (), "Count");
				ClassicAssert.AreEqual ("X", names.First (), "name0");
				return "BAR";
			}
			
			public bool IsFixupTokenAvailable {
				get { throw new NotImplementedException (); }
			}
			
			#pragma warning disable 67
			public event EventHandler OnNameScopeInitializationComplete;
			#pragma warning restore 67

			// both X and Y calls this.
			public object Resolve (string name)
			{
				return resolves ? "FOO" : null;
			}

			public object Resolve (string name, out bool isFullyInitialized)
			{
				throw new NotImplementedException ();
			}
		}
	}
}

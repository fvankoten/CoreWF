﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections;
using n = NUnit.Framework;
using NUnit.Framework.Legacy;

#if PCL
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;

#else
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
using System.ComponentModel;
#endif

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlObjectWriterCollectionTests
	{
		/// <summary>
		/// Test adding a different type to a custom collection using an explicitly implemented IList.Add(object) method
		/// </summary>
		[Test] // works on both MS.NET and System.Xaml, but no way to make use of type converters
        public void TestCustomCollectionAddOverride ()
		{
			var xaml = @"<CollectionParentCustomAddOverride xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'><OtherItem/></CollectionParentCustomAddOverride>".UpdateXml ();
			var parent = (CollectionParentCustomAddOverride)XamlServices.Load (new StringReader (xaml));

			ClassicAssert.IsNotNull (parent, "#1");
			ClassicAssert.IsInstanceOf<CollectionParentCustomAddOverride> (parent, "#2");
			ClassicAssert.AreEqual (1, parent.Items.Count, "#3");
			var item = parent.Items.FirstOrDefault ();
			ClassicAssert.IsNotNull (item, "#4");
			ClassicAssert.AreEqual ("FromOther", item.Name, "#5");
		}

		/// <summary>
		/// Test adding a different type to a generic collection using the typeconverter on the list item type.
		/// </summary>
		[Test, n.Category (Categories.NotOnSystemXaml)] // doesn't work in MS.NET, but theoretically should
        public void TestListCollectionItemConverter ()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("doesn't work in MS.NET, it does not use type converters for items in a list");

			var xaml = @"<CollectionParentGenericList xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'><OtherItem/></CollectionParentGenericList>".UpdateXml ();
			var parent = (CollectionParentGenericList)XamlServices.Load (new StringReader (xaml));

			ClassicAssert.IsNotNull (parent, "#1");
			ClassicAssert.IsInstanceOf<CollectionParentGenericList> (parent, "#2");
			ClassicAssert.AreEqual (1, parent.Items.Count, "#3");
			var item = parent.Items.FirstOrDefault ();
			ClassicAssert.IsNotNull (item, "#4");
			ClassicAssert.AreEqual ("FromOther", item.Name, "#5");
		}

		/// <summary>
		/// Test adding a different type to a custom collection that implements a generic collection using the typeconverter of the list item type.
		/// </summary>
		[Test, n.Category (Categories.NotOnSystemXaml)] // New in System.Xaml, doesn't work in MS.NET, but should
        public void TestCustomCollectionItemConverter ()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("New in System.Xaml, doesn't work in MS.NET, but should");

			var xaml = @"<CollectionParentCustomNoOverride xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'><OtherItem/></CollectionParentCustomNoOverride>".UpdateXml ();
			var parent = (CollectionParentCustomNoOverride)XamlServices.Load (new StringReader (xaml));

			ClassicAssert.IsNotNull (parent, "#1");
			ClassicAssert.IsInstanceOf<CollectionParentCustomNoOverride> (parent, "#2");
			ClassicAssert.AreEqual (1, parent.Items.Count, "#3");
			var item = parent.Items.FirstOrDefault ();
			ClassicAssert.IsNotNull (item, "#4");
			ClassicAssert.AreEqual ("FromOther", item.Name, "#5");
		}
	}
}

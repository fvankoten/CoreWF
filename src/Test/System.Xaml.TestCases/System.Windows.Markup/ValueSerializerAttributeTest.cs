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
    public class ValueSerializerAttributeTest
    {
        private static string TestTypeTypeName = typeof(TestType).AssemblyQualifiedName;
        private class TestType { }

        [Test]
        public void ConstructedWithType()
        {
            var vsa = new ValueSerializerAttribute(typeof(TestType));
            ClassicAssert.AreEqual(typeof(TestType), vsa.ValueSerializerType, "#1");
            ClassicAssert.AreEqual(typeof(TestType).AssemblyQualifiedName, vsa.ValueSerializerTypeName, "#2");
        }

        [Test]
        public void ConstructedWithTypeName()
        {
            var vsa = new ValueSerializerAttribute(TestTypeTypeName);
            ClassicAssert.AreEqual(typeof(TestType).AssemblyQualifiedName, vsa.ValueSerializerTypeName, "#1");
            ClassicAssert.AreEqual(typeof(TestType), vsa.ValueSerializerType, "#1");
        }
    }
}

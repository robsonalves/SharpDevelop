﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseTestClassTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = 
				"import unittest\r\n" +
				"\r\n" +
				"class simpleTest(unittest.TestCase):\r\n" +
				"    def testSuccess(self):\r\n" +
				"        assert True\r\n" +
				"\r\n" +
				"    def testFailure(self):\r\n" +
				"        assert False\r\n" +
				"\r\n";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
			}
		}
		
		[Test]
		public void SimpleTestFirstBaseTypeIsUnitTestTestCase()
		{
			IReturnType baseType = c.BaseTypes[0];
			string actualBaseTypeName = baseType.FullyQualifiedName;
			string expectedBaseTypeName = "unittest.TestCase";
			Assert.AreEqual(expectedBaseTypeName, actualBaseTypeName);
		}
	}
}

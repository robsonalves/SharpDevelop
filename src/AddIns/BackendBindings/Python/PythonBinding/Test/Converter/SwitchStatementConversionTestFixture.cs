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
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class SwitchStatementConversionTestFixture
	{	
		[Test]
		public void SwitchStatement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public int Run(int i)\r\n" +
							"    {\r\n" +
							"        switch (i) {\r\n" +
							"            case 7:\r\n" +
							"                i = 4;\r\n" +
							"                break;\r\n" +
							"            case 10:\r\n" +
							"                return 0;\r\n" +
							"            case 9:\r\n" +
							"                return 2;\r\n" +
							"            case 8:\r\n" +
							"                break;\r\n" +
							"            default:\r\n" +
							"                return -1;\r\n" +
							"        }\r\n" +
							"        return i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"  def Run(self, i):\r\n" +
									"    if i == 7:\r\n" +
									"      i = 4\r\n" +
									"    elif i == 10:\r\n" +
									"      return 0\r\n" +
									"    elif i == 9:\r\n" +
									"      return 2\r\n" +
									"    elif i == 8:\r\n" +
									"      pass\r\n" +
									"    else:\r\n" +
									"      return -1\r\n" +
									"    return i";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code, code);
		}	
		
		[Test]
		public void CaseFallThrough()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public int Run(int i)\r\n" +
							"    {\r\n" +
							"        switch (i) {\r\n" +
							"            case 10:\r\n" +
							"            case 11:\r\n" +
							"                return 0;\r\n" +
							"            case 9:\r\n" +
							"                return 2;\r\n" +
							"            default:\r\n" +
							"                return -1;\r\n" +
							"        }\r\n" +
							"    }\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"  def Run(self, i):\r\n" +
									"    if i == 10 or i == 11:\r\n" +
									"      return 0\r\n" +
									"    elif i == 9:\r\n" +
									"      return 2\r\n" +
									"    else:\r\n" +
									"      return -1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}		
	}
}

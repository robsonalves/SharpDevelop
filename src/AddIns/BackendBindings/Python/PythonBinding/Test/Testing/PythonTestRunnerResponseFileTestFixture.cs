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
using System.IO;
using System.Text;
using ICSharpCode.Core.Services;
using ICSharpCode.PythonBinding;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestRunnerResponseFileTestFixture
	{
		PythonTestRunnerResponseFile responseFile;
		StringBuilder responseFileText;
		StringWriter writer;
		
		[SetUp]
		public void Init()
		{
			responseFileText = new StringBuilder();
			writer = new StringWriter(responseFileText);
			responseFile = new PythonTestRunnerResponseFile(writer);
		}
		
		[Test]
		public void WriteTestAddsTestNameToResponseFile()
		{
			responseFile.WriteTest("MyTests");
			
			Assert.AreEqual("MyTests\r\n", responseFileText.ToString());
		}
		
		[Test]
		public void WritePathAddsQuotedSysPathCommandLineArgument()
		{
			responseFile.WritePath(@"c:\mytests");
			
			string expectedText = "/p:\"c:\\mytests\"\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteResultsFileNameAddsQuotedResultsFileNameCommandLineArgument()
		{
			responseFile.WriteResultsFileName(@"c:\temp\tmp66.tmp");
			
			string expectedText = "/r:\"c:\\temp\\tmp66.tmp\"\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void DisposeMethodDisposesTextWriterPassedInConstructor()
		{
			responseFile.Dispose();
			Assert.Throws<ObjectDisposedException>(delegate { writer.Write("test"); });
		}
		
		[Test]
		public void WriteTestsWritesSelectedTestMethodNameWhenMethodSelected()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.FullyQualifiedName = "MyNamespace.MyTests.MyTestMethod";
			SelectedTests tests = new SelectedTests(new MockCSharpProject(), null, null, method);
			
			responseFile.WriteTests(tests);
			
			string expectedText = "MyNamespace.MyTests.MyTestMethod\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsWritesSelectedTestClassNameWhenOnlyClassSelected()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.FullyQualifiedName = "MyNamespace.MyTests";
			SelectedTests tests = new SelectedTests(new MockCSharpProject(), null, c, null);
			
			responseFile.WriteTests(tests);
			
			string expectedText = "MyNamespace.MyTests\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsWritesSelectedTestNamespaceWhenOnlyNamespaceSelected()
		{
			SelectedTests tests = new SelectedTests(new MockCSharpProject(), "MyNamespace", null, null);
			responseFile.WriteTests(tests);
			
			string expectedText = "MyNamespace\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsWritesNamespacesForAllFilesInProjectWhenOnlyProjectSelected()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			FileProjectItem item = new FileProjectItem(project, ItemType.Compile);
			item.FileName = @"c:\projects\mytests\nonTestClass.py";
			ProjectService.AddProjectItem(project, item);
			
			item = new FileProjectItem(project, ItemType.Compile);
			item.FileName = @"c:\projects\mytests\TestClass.py";
			ProjectService.AddProjectItem(project, item);
			
			SelectedTests tests = new SelectedTests(project);
			
			responseFile.WriteTests(tests);
			
			string expectedText =
				"nonTestClass\r\n" +
				"TestClass\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsWritesNothingToResponseFileWhenMethodAndClassAndNamespaceAndProjectIsNull()
		{
			SelectedTests tests = new SelectedTests(null);
			responseFile.WriteTests(tests);
			Assert.AreEqual(String.Empty, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsDoesNotThrowNullReferenceExceptionWhenNonFileProjectItemInProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			WebReferenceUrl webRef = new WebReferenceUrl(project);
			webRef.Include = "test";
			ProjectService.AddProjectItem(project, webRef);
			
			FileProjectItem item = new FileProjectItem(project, ItemType.Compile);
			item.FileName = @"c:\projects\mytests\myTests.py";
			ProjectService.AddProjectItem(project, item);
			
			SelectedTests tests = new SelectedTests(project);
			responseFile.WriteTests(tests);
			
			string expectedText = "myTests\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsWritesDirectoriesForReferencedProjectsToSysPathCommandLineArguments()
		{
			MockCSharpProject referencedProject = new MockCSharpProject();
			referencedProject.FileName = @"c:\projects\pyproject\pyproject.pyproj";
			
			MockCSharpProject unitTestProject = new MockCSharpProject();
			ProjectReferenceProjectItem projectRef = new ProjectReferenceProjectItem(unitTestProject, referencedProject);
			projectRef.FileName = @"c:\projects\pyproject\pyproject.pyproj";
			ProjectService.AddProjectItem(unitTestProject, projectRef);
			
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.FullyQualifiedName = "MyNamespace.MyTests.MyTestMethod";
			
			SelectedTests tests = new SelectedTests(unitTestProject, null, null, method);
			responseFile.WriteTests(tests);
			
			string expectedText =
				"/p:\"c:\\projects\\pyproject\"\r\n" +
				"MyNamespace.MyTests.MyTestMethod\r\n";
			
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
	}
}

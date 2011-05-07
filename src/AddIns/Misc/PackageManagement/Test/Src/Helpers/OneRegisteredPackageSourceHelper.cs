﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class OneRegisteredPackageSourceHelper
	{
		public RegisteredPackageSources RegisteredPackageSources;
		public TestablePackageManagementOptions Options;
		public FakeSettings FakeSettings;
		public PackageSource PackageSource = new PackageSource("http://sharpdevelop.com", "Test Package Source");
		
		public OneRegisteredPackageSourceHelper()
		{
			CreateOneRegisteredPackageSource();
		}
		
		void CreateOneRegisteredPackageSource()
		{
			Properties properties = new Properties();
			Options = new TestablePackageManagementOptions();
			FakeSettings = Options.FakeSettings;
			RegisteredPackageSources = Options.PackageSources;
			AddOnePackageSource();
		}
		
		public void AddOnePackageSource()
		{
			RegisteredPackageSources.Clear();
			RegisteredPackageSources.Add(PackageSource);
		}
		
		public void AddTwoPackageSources()
		{			
			AddOnePackageSource();
			RegisteredPackageSources.Add(new PackageSource("http://second.codeplex.com", "second"));
		}
	}
}

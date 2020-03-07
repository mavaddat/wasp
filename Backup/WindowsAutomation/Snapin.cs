// Copyright (c) 2007 Joel Bennett
// This File is license under the Ms Reciprocal License

// Copyright Grant- Subject to the terms of this license, each contributor grants you 
// a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
// prepare derivative works of its contribution, and distribute its contribution or any 
// derivative works that you create.

// Conditions and Limitations

// Reciprocal Grants- For any file you distribute that contains code from the software
// (in source code or binary format), you must provide recipients the source code to that 
// file along with a copy of this license, which license will govern that file. You may 
// license other files that are entirely your own work and do not contain code from the 
// software under any terms you choose.

// *****************************************************************************
// NOTE: For current and complete licensing information please see:
// http://www.codeplex.com/WASP/Project/License.aspx
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.ComponentModel;

namespace Huddled.Wasp
{
	[RunInstaller(true)]
	public class WindowsAutomationSnapIn : PSSnapIn
	{
		public override string Name
		{
			get { return "WASP"; }
		}
		public override string Vendor
		{
			get { return "HuddledMasses.org"; }
		}
		public override string VendorResource
		{
            get { return "WASP, HuddledMasses.org"; }
		}
		public override string Description
		{
			get { return "The Windows Automation Snapin Project has PowerShell cmdlets for enumerating, moving, and managing application windows, as well as sending clicks and keystrokes"; }
		}
		public override string DescriptionResource
		{
			get { return "WASP, Registers the Windows Automation Snapin cmdlets"; }
		}
	}
}

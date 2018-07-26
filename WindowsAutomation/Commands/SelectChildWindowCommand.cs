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
using System.Collections;
using System.Diagnostics;

namespace Huddled.Wasp
{
   [Cmdlet(VerbsCommon.Select, "ChildWindow", DefaultParameterSetName = "ProcessName")]
	public class SelectChildWindowCommand : WindowCmdletBase
   {
		public override void ProcessWindow(WindowHandle hWnd)
		{
			WindowHandle found = (WindowHandle)NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, null);
			while (!found.Equals(IntPtr.Zero))
			{
				if (NativeMethods.IsWindow(found.Handle) && 
					 NativeMethods.IsWindowVisible(found.Handle) && 
					(found.GetParentOrOwner() == hWnd.Handle)
					)
				{
					WriteObject( found );
				}
				found = NativeMethods.FindWindowEx(IntPtr.Zero, found, null, null);
			}
		}
		// select-control -window 0 | ? { $_.GetParent() -eq (select-window notepad) }
   }
}

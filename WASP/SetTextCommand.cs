// Copyright (c) 2009 Joel Bennett
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
// http://wasp.codeplex.com/license
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Windows.Automation;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Huddled.Wasp
{
	[Cmdlet(VerbsCommon.Set, "Text", DefaultParameterSetName = "Click")]
	public class SetTextCommand : PSCmdlet
	{
       [Parameter()]
       public string Text { get; set; }

      [Parameter(ValueFromPipeline=true)]
      public AutomationElement Target { get; set; }

      protected override void ProcessRecord()
      {
         Target.SetFocus();
         object textPattern = null;
         object valuePattern = null;



         if (Target.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
         {
            ((ValuePattern)valuePattern).SetValue(Text);
         }
         else if (Target.TryGetCurrentPattern(TextPattern.Pattern, out textPattern))
         {
            SendKeys.SendWait("^{HOME}");
            SendKeys.SendWait("^+{END}");
            SendKeys.SendWait("{DEL}");
            SendKeys.SendWait(Text);
         }
         else throw new NotSupportedException("Setting the text on this element is not supported");
      }

   }
}

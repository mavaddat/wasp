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

using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace Huddled.Wasp
{
	//[Cmdlet(VerbsDiagnostic.Test, "Window")]
	[Cmdlet("Nothing", "Window", DefaultParameterSetName = "Default", SupportsShouldProcess = true)]
   public abstract class AutomationElementCmdletBase : Cmdlet
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		[Parameter(Position = 100, Mandatory = true,
			ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
			HelpMessage = "A list of AutomationElements for this cmdlet to act on.")]
      [Alias("Element")]
      public AutomationElement[] Window { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Passthru")]
		[Parameter]
		public SwitchParameter Passthru { get; set; }

      public abstract void ProcessAutomationElement(AutomationElement element);

		protected override void ProcessRecord()
		{
         foreach (AutomationElement w in Window)
			{
				if (ShouldProcess(w.ToString()))
				{
               ProcessAutomationElement(w);
				}

				if (Passthru.IsPresent && Passthru.ToBool())
				{
					WriteObject(w);
				}
			}
		}
	}
}

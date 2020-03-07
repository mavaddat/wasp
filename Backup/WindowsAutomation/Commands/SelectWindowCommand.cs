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
   [Cmdlet(VerbsCommon.Select, "Window", DefaultParameterSetName = "ProcessName")]
   public class SelectWindowCommand : PSCmdlet
   {

      #region Parameters
      [Alias("ForegroundWindow")]
      [Parameter(Position = 0,
          ParameterSetName = "ForegroundWindow",
          ValueFromPipeline = false)]
      public SwitchParameter ActiveWindow { get; set; }

		// we have to use arrays for PowerShell. :-(
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		[Alias("Name", "Exe", "Application", "Program")]
      [Parameter(Position = 0,
        ParameterSetName = "ProcessName",
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "The name of one or more process to find windows from. Wildcards are permitted.")]
      public string[] ProcessName
      {
         set
         {
            processNames = value;
         }
         get
         {
            return processNames;
         }
      }

		// we have to use arrays for PowerShell. :-(
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		[Alias("WindowTitle", "Caption", "WindowName", "MainWindowTitle", "Text")]
      [Parameter(Position = 0,
          ParameterSetName = "WindowTitle",
          ValueFromPipelineByPropertyName = true,
          HelpMessage = "The title text of one or more windows. Wildcards are permitted.")]
      public string[] Title
      {
         set
         {
            windowTitles = value;
         }
         get
         {
            return windowTitles;
         }
      }

		// we have to use arrays for PowerShell. :-(
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		[Alias("WindowClass")]
		[Parameter(Position = 0,
          ParameterSetName = "WindowClass",
          ValueFromPipelineByPropertyName = true,
          HelpMessage = "The window class name of one or more windows. Wildcards are permitted.")]
      public string[] Class
      {
         set
         {
            windowClasses = value;
         }
         get
         {
            return windowClasses;
         }
      }

		[Parameter()]
		public SwitchParameter ToolWindows { get; set; }


      /// <summary>
      /// Define the InputObject parameter. If the input is a stream 
      /// of (collections of)Process objects, the Name and Id parameters 
      /// are ignored, and the Process objects are taken directly.  This 
      /// allows the cmdlet to deal with processes that have wildcard 
      /// characters in their name.
      /// <value>Process objects</value>
      /// </summary>
		// we have to use arrays for PowerShell. :-(
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		[Parameter(Position = 0,
          Mandatory = false,
       ParameterSetName = "InputObject",
       ValueFromPipeline = true,
          ValueFromPipelineByPropertyName = true,
       HelpMessage = "Allows the input to be a filtered set of output from Get-Process")]
      public Process[] InputObject
      {
         get { return processes; }
         set { processes = value; }
      }
      #endregion

      private Process[] processes;
      private string[] processNames;
      private string[] windowTitles;
      private string[] windowClasses;

      /// <summary>
      /// Processes the record.
      /// </summary>
      protected override void ProcessRecord()
      {
         switch (ParameterSetName)
         {
            case "ForegroundWindow":
               WriteObject(new Window(NativeMethods.GetForegroundWindow()));
               break;
            case "ProcessName":
               if (processNames == null) { goto default; }
               foreach (Window w in GetWindowsByProcessName(processNames))
               {
                  WriteObject(w);
               }
               break;
            case "WindowTitle":
               foreach (Window w in GetWindowsByWindowTitle(windowTitles))
               {
                  WriteObject(w);
               }
               break;
            case "InputObject":
               foreach (Process proc in processes)
               {
                  foreach (Window w in GetWindowsOfProcess(proc))
                  {
                     WriteObject(w);
                  }

               }
               break;
            case "WindowClass":
               foreach (Window w in GetWindowsByClass(windowClasses))
               {
                  WriteObject(w);
               }
               break;

            default:

               WriteVerbose("Enumerating all windows");
               foreach (Window w in Huddled.Wasp.WindowFinder.GetTopLevelWindows())
               {
                  WriteObject(w);
               }
               break;
         } // switch (ParameterSetName...
      }

      /// <summary>Gets the windows which match the specified class name(s)</summary>
      /// <param name="titles">The window classes (wildcards allowed)</param>
      /// <returns>Each of the matching top level windows</returns>
      private IEnumerable<Window> GetWindowsByClass(string[] Classes)
      {
			Huddled.Wasp.WindowFinder.IncludeToolWindowsWithAppWindows = ToolWindows.ToBool();
         var windows = Huddled.Wasp.WindowFinder.GetTopLevelWindows();
			Huddled.Wasp.WindowFinder.IncludeToolWindowsWithAppWindows = false;

         foreach (string cls in Classes)
         {
            WildcardPattern wildcard = new WildcardPattern(cls, WildcardOptions.IgnoreCase | WildcardOptions.Compiled);
            foreach (Huddled.Wasp.Window window in windows)
            {
               if (wildcard.IsMatch(window.Class))
               {
                  yield return window;
               }
            }
         }
      }

      /// <summary>Gets the windows which match the specified titles</summary>
      /// <param name="titles">The window titles (wildcards allowed)</param>
      /// <returns>Each of the matching top level windows</returns>
      private IEnumerable<Window> GetWindowsByWindowTitle(string[] titles)
      {
         foreach (Huddled.Wasp.Window window in Huddled.Wasp.WindowFinder.GetTopLevelWindows())
         {
            foreach (string title in titles)
            {
               WildcardPattern wildcard = new WildcardPattern(title, WildcardOptions.IgnoreCase | WildcardOptions.Compiled);
               if (wildcard.IsMatch(window.Title))
               {
                  yield return window;
               }
            }
         }
      }

      /// <summary>Gets the windows of processes which match the specified names.</summary>
      /// <param name="processes">The process names (wildcards allowed)</param>
      /// <returns>Each of the top level windows owned by a matching process</returns>
      private IEnumerable<Window> GetWindowsByProcessName(string[] names)
      {
         foreach (Process proc in Process.GetProcesses())
         {
            WriteVerbose("Attempting to fetch processes by name.");
            foreach (string name in names)
            {
               // Use the process name to perform wildcard expansion.  
               // If the process name does not contain a wildcard pattern then it is used as it is.
               WildcardPattern wildcard = new WildcardPattern(name, WildcardOptions.IgnoreCase);

               // It's probably a process name
               if (wildcard.IsMatch(proc.ProcessName))
               {
                  foreach (Window w in GetWindowsOfProcess(proc))
                  {
                     yield return w;
                  }
               }
            }
         }
      }

      /// <summary>Gets the windows of a process</summary>
      /// <param name="proc">The process</param>
      /// <returns>Each of the top level windows owned by a process</returns>
      private IEnumerable<Window> GetWindowsOfProcess(Process proc)
      {
         WriteVerbose("Found Matching Process(es), fetching threads.");
         foreach (Window w in Huddled.Wasp.WindowFinder.GetProcessWindows(proc))
         {
            yield return w;
         }
      }

   }
}

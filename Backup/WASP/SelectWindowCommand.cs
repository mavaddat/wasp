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

namespace Huddled.Wasp
{
   [Cmdlet(VerbsCommon.Select, "Window", DefaultParameterSetName = "ChildElements")]
   public class SelectWindowCommand : PSCmdlet
   {
      #region Parameters
      [Alias("ForegroundWindow")]
      [Parameter(Position = 0,
          ParameterSetName = "ForegroundWindow",
          ValueFromPipeline = false)]
      public SwitchParameter ActiveWindow { set; get; }

      //[Parameter()]
      //public SwitchParameter ToolWindows { get; set; }

      [Parameter()]
      public SwitchParameter NoWildcards { get; set; }

      // we have to use arrays for PowerShell. :-(
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      [Alias("Name", "Exe", "Application", "Program")]
      [Parameter(Position = 0, //ValueFromPipelineByPropertyName = true,
        HelpMessage = "The name of one or more process to find windows from. Wildcards are permitted.")]
      public string[] ProcessName
      {
         set
         {
            _processNames = value;
         }
         get
         {
            return _processNames;
         }
      }

      // we have to use arrays for PowerShell. :-(
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      [Alias("WindowTitle", "Caption", "WindowName", "MainWindowTitle", "Text")]
      [Parameter(Position = 1, //ValueFromPipelineByPropertyName = true,
          HelpMessage = "The title text of one or more windows. Wildcards are permitted.")]
      public string[] Title
      {
         set
         {
            _windowTitles = value;
         }
         get
         {
            return _windowTitles;
         }
      }

      // we have to use arrays for PowerShell. :-(
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      [Alias("WindowClass")]
      [Parameter(Position = 2, //ValueFromPipelineByPropertyName = true,
          HelpMessage = "The window class name of one or more windows. Wildcards are permitted.")]
      public string[] Class
      {
         set
         {
            _windowClasses = value;
         }
         get
         {
            return _windowClasses;
         }
      }

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
      [Parameter(Position = 20, Mandatory = false,
          ParameterSetName = "InputObject",
          ValueFromPipelineByPropertyName = true,
          HelpMessage = "Allows the input to be a filtered set of output from Get-Process")]
      [Alias("InputObject", "Id")]
      public int[] ProcessId
      {
         get { return _processes; }
         set { _processes = value; }
      }

      [Parameter(Position = 20, Mandatory = false,
         ParameterSetName = "ChildElements",
         ValueFromPipelineByPropertyName = true,
         HelpMessage = "Allows the input to be a filtered set of output from Get-Process")]
      [Alias("Element")]
      public AutomationElement ParentElement
      {
         get { return _parent; }
         set { _parent = value; }
      }
      #endregion

      private AutomationElement _parent = AutomationElement.RootElement;

      private List<int> _processIds;
      private int[] _processes;
      private string[] _processNames;
      private string[] _windowTitles;
      private string[] _windowClasses;
      private WildcardPattern[] _processNamePatterns;
      private WildcardPattern[] _windowTitlePatterns;
      private WildcardPattern[] _windowClassPatterns;
      //LocalizedControlType
      private Condition _condition = Condition.TrueCondition;
      AutomationElementCollection _windows;
      protected override void BeginProcessing()
      {
         

         _processNamePatterns = StringsToWildcards(_processNames);
         _windowTitlePatterns = StringsToWildcards(_windowTitles);
         _windowClassPatterns = StringsToWildcards(_windowClasses);

         if (_processes != null || _processNamePatterns != null)
         {
            _processIds = GetProcessIds(_processNamePatterns);
            if (_processes != null)
            {
               _processIds.AddRange(_processes);
            }
         }

         if (NoWildcards.ToBool())
         {
            _condition = new AndCondition(new[] { 
               (_windowTitles == null ) ? Condition.TrueCondition :
                  CreateOrCondition(AutomationElementIdentifiers.NameProperty, _windowTitles), 
               (_windowClasses == null ) ? Condition.TrueCondition :
                  CreateOrCondition(AutomationElementIdentifiers.ClassNameProperty, _windowClasses),
               (_processIds == null ) ? Condition.TrueCondition :
                  CreateProcessCondition(_processIds.ToArray())
            });
         }

         //var titlePatterns = StringsToWildcards(_windowTitles);
         //var classPatterns = StringsToWildcards(_windowClasses);
         base.BeginProcessing();
      }

      protected override void ProcessRecord()
      {
         _windows = _parent.FindAll(TreeScope.Children, _condition);
         
         if (_processes != null)
         {
            _processIds.AddRange(_processes);
         }

         foreach (AutomationElement window in _windows)
         {
            bool include = true;
            if (!NoWildcards.ToBool())
            {
               if (_windowClassPatterns != null)
               {
                  include = false;
                  string windowClass = (string)window.GetCurrentPropertyValue(AutomationElementIdentifiers.ClassNameProperty);
                  foreach (var wildcard in _windowClassPatterns)
                  {
                     if (wildcard.IsMatch(windowClass))
                     {
                        include = true;
                        break;
                     }
                  }
               }

               if (include && _windowTitlePatterns != null)
               {
                  include = false;
                  string windowTitle = (string)window.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty);
                  foreach (var wildcard in _windowTitlePatterns)
                  {
                     if (wildcard.IsMatch(windowTitle))
                     {
                        include = true;
                        break;
                     }
                  }
               }
            }
            if (include)
            {
               if (_processIds != null)
               {
                  int windowProcId = (int)window.GetCurrentPropertyValue(AutomationElementIdentifiers.ProcessIdProperty);
                  foreach (var id in _processIds)
                  {
                     if (windowProcId == id)
                     {
                        WriteObject(new Window(window));
                     }
                  }
               }
               else
               {
                  WriteObject(new Window(window));
               }
            }
         }
         base.ProcessRecord();
      }

      private static WildcardPattern[] StringsToWildcards(string[] patterns)
      {
         if(patterns == null) { return null; }

         WildcardPattern[] results = new WildcardPattern[patterns.Length];
         int p = 0;
         foreach (var pattern in patterns)
         {
            results[p++] = new WildcardPattern(pattern, WildcardOptions.IgnoreCase);
         }
         return results;
      }

      private static Condition CreateProcessCondition(int[] processIds)
      {
         if (processIds != null && processIds.Length > 0)
         {
            PropertyCondition[] processConditions = new PropertyCondition[processIds.Length];
            int p = 0;
            foreach (var id in processIds)
            {
               processConditions[p++] = new PropertyCondition(AutomationElement.ProcessIdProperty, id, PropertyConditionFlags.IgnoreCase);
            }
            return new OrCondition(processConditions);
         }
         else if (processIds.Length == 0)
         {
            return Condition.FalseCondition;
         }
         else return Condition.TrueCondition;
      }

      private static List<int> GetProcessIds(WildcardPattern[] processNamesPatterns)
      {
         var processIds = new List<int>();
         if (processNamesPatterns != null && processNamesPatterns.Length > 0)
         {
            foreach (Process proc in Process.GetProcesses())
            {
               foreach (var wildcard in processNamesPatterns)
               {
                  // It's probably a process name
                  if (wildcard.IsMatch(proc.ProcessName))
                  {
                     processIds.Add(proc.Id);
                  }
               }
            }
         }
         return processIds;
      }

      private static Condition CreateOrCondition(AutomationProperty property, object[] propertyValues)
      {
         if (propertyValues != null && propertyValues.Length > 0)
         {
            PropertyCondition[] conditions = new PropertyCondition[propertyValues.Length];
            int p = 0;
            foreach (var val in propertyValues)
            {
               conditions[p++] = new PropertyCondition(property, val);
            }
            return new OrCondition(conditions);
         }
         else if (propertyValues.Length == 0)
         {
            return Condition.FalseCondition;
         }
         else return Condition.TrueCondition;
      }

      private static Condition CreateOrCondition(AutomationProperty property, string[] propertyValues)
      {
         if (propertyValues != null && propertyValues.Length > 0)
         {
            PropertyCondition[] conditions = new PropertyCondition[propertyValues.Length];
            int p = 0;
            foreach (var val in propertyValues)
            {
               conditions[p++] = new PropertyCondition(property, val, PropertyConditionFlags.IgnoreCase);
            }
            return new OrCondition(conditions);
         }
         else if (propertyValues.Length == 0)
         {
            return Condition.FalseCondition;
         }
         else return Condition.TrueCondition;
      }

   }
}

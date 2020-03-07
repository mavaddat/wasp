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
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace Huddled.Wasp
{
    [Cmdlet(VerbsCommon.Select, "Control", DefaultParameterSetName = "Name")]
    public class SelectControlCommand : Cmdlet
    {

        [Alias("WindowPointer", "hWnd", "WindowHandle", "Control", "ControlHandle", "Handle", "ControlPointer")]
        [Parameter(Mandatory = true,  ParameterSetName = "Name",  ValueFromPipeline = true)]
        [Parameter(Mandatory = true,  ParameterSetName = "Class", ValueFromPipeline = true)]
        public WindowHandle Window { get; set; }

        [Parameter(Mandatory = true,  Position = 1,  ParameterSetName = "Class")]
        public string Class { 
            get{
                return (_class == null) ? null : _class.ToString();
            }
            set
            {
                _class = new WildcardPattern(value, WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
            }
        }

        WildcardPattern _class;

		  [Alias("Caption", "Text")]
        [Parameter(Mandatory = false, Position = 5,  ParameterSetName = "Class")]
        [Parameter(Mandatory = false,                ParameterSetName = "Name")]
        public string Title {
            get
            {
                return (_title == null) ? null : _title.ToString();
            }
            set
            {
                _title = new WildcardPattern(value, WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
            }
        }
        WildcardPattern _title;

        ////[Parameter(Mandatory = false, ParameterSetName = "ClassPipeline", Position = 1, ValueFromPipeline = false)]
        ////[Parameter(Mandatory = false, ParameterSetName = "ClassArgument", Position = 2, ValueFromPipeline = false)]
        [Parameter(Mandatory = false, Position = 10, ParameterSetName = "Name")]
        [Parameter(Mandatory = false, Position = 10, ParameterSetName = "Class")]
        public int Index
        { 
            get{
                return _index.GetValueOrDefault(-1);
            }
            set{
                _index = value;
            }
        }
		  private Nullable<int> _index = null;


		  [Parameter()]
		  public SwitchParameter Recurse { get; set; }


        protected override void ProcessRecord()
        {

            int index = 0;
				WindowFinder.ChildFinder finder = new WindowFinder.ChildFinder(Window);
				var children = finder.FindAll();
				foreach(var found in children) 
			   {

				//IntPtr found = NativeMethods.FindWindowEx(Window, IntPtr.Zero, null, null);
				//while (!found.Equals(IntPtr.Zero))
				//{
					if( (Recurse.ToBool() || found.GetParent() == Window.Handle)
						 && (_class == null || _class.IsMatch(found.GetClassName() ?? ""))
					    && (_title == null || _title.IsMatch(found.GetWindowText() ?? ""))
                )
                {

                    if (_index.HasValue)
                    {
                        if (index == _index)
                        {
                            WriteObject(new Control(found));
                            break;
                        }
                        index++;
                    }
                    else
                    {
                        WriteObject(new Control(found));
                    }
                }
            }
        }
    }
}

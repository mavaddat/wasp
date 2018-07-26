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
using System.Drawing;
using Huddled.Wasp.NativeTypes;

namespace Huddled.Wasp
{
    [Cmdlet(VerbsCommon.Set, "WindowPosition", DefaultParameterSetName = "Default", SupportsShouldProcess = true)]
    public class SetWindowPositionCommand : WindowCmdletBase
    {
        [Alias("X")]
		  [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Default")]
        public int Left { get; set; }

        [Alias("Y")]
		  [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Default")]
        public int Top { get; set; }

		  [Parameter(Position = 3, Mandatory = false, ParameterSetName = "Default")]
        public int Width { get; set; }

		  [Parameter(Position = 4, Mandatory = false, ParameterSetName = "Default")]
        public int Height { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "Maximize")]
        public SwitchParameter Maximize { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "Minimize")]
        public SwitchParameter Minimize { get; set; }

		  [Parameter(ParameterSetName = "Default")]
        public SwitchParameter Restore { get; set; }


        public override void ProcessWindow(WindowHandle hWnd)
        {
            SWP swpParam = SWP.NOACTIVATE | SWP.NOZORDER;

            if (Maximize.IsPresent)
            {
                hWnd.Maximize();
            }
            else if (Minimize.IsPresent)
            {
                hWnd.Minimize();
            }
            else
            {
                if (Restore.IsPresent || hWnd.GetIsMaximized() || hWnd.GetIsMinimized())
                {
                    hWnd.Restore();
                }

                #region default the x,y,w,h
                if ((Width == 0) || (Height == 0) || (Left == 0) || (Top == 0))
                {
                    NativeMethods.RECT bounds;
                    NativeMethods.GetWindowRect(hWnd, out bounds);

                    if ((Width == 0) && (Height == 0))
                    {
                        swpParam |= SWP.NOSIZE;
                    }
                    else if ((Left == 0) && (Top == 0))
                    {
                        swpParam |= SWP.NOMOVE;
                    }
                    if (Width == 0)
                    {
                        Width = bounds.Width;
                    }
                    if (Height == 0)
                    {
                        Height = bounds.Height;
                    }
                    if (Left == 0)
                    {
                        Left = bounds.Left;
                    }
                    if (Top == 0)
                    {
                        Top = bounds.Top;
                    }
                }
                #endregion

                NativeMethods.SetWindowPos(hWnd, IntPtr.Zero, Left, Top, Width, Height, swpParam);
            }
        }
    }
}

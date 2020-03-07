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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Huddled.Wasp.NativeTypes;
using System.Drawing;

namespace Huddled.Wasp
{
	[Cmdlet(VerbsCommunications.Send, "Click", DefaultParameterSetName = "Default", SupportsShouldProcess = true)]
	public class SendClickCommand : WindowCmdletBase
	{
		public enum MouseButton
		{
			Left, Right, Middle, Fourth, Fifth
		}

		[Parameter(Position=1)]
		[Alias("X")]
		public int Left { get; set; }

		[Parameter(Position=2)]
		[Alias("Y")]
		public int Top { get; set; }

		[Parameter(Position = 3)]
		public MouseButton Button { get; set; }

		[Parameter()]
		public SwitchParameter AltButton { get; set; }

		[Parameter()]
		public SwitchParameter ShiftButton { get; set; }

		[Parameter()]
		public SwitchParameter ControlButton { get; set; }

		[Parameter()]
		public SwitchParameter WinButton { get; set; }

		[Parameter()]
		public SwitchParameter DoubleClick { get; set; }

		[Parameter()]
		public SwitchParameter ForceClient { get; set; }

		public override void ProcessWindow(WindowHandle hWnd)
		{
			if (hWnd > 0) hWnd.Activate();

			if (AltButton)
			{
				//NativeMethods.PostMessage(hWnd, WM.KeyDown, Keys.Alt, 0);
				NativeMethods.KeyboardEvent(Keys.Alt, KeyEventFlags.KeyDown);
			}
			if (ShiftButton)
			{
				NativeMethods.KeyboardEvent(Keys.Shift, KeyEventFlags.KeyDown);
			}
			if (ControlButton)
			{
				NativeMethods.KeyboardEvent(Keys.Control, KeyEventFlags.KeyDown);
			}
			if (WinButton)
			{
				NativeMethods.KeyboardEvent(Keys.LWin, KeyEventFlags.KeyDown);
			}

			// I can't explain how safe this is ;P
			// We're going to create our window message by subtracting the "NonClient" value
			WindowHitTestRegions hitTest = WindowHitTestRegions.ClientArea;
			int PointsOnScreen = 0;
			if (!ForceClient.ToBool())
			{
				IntPtr ht;
				Point screenRelative = hWnd.PointToScreen(Left, Top);
				PointsOnScreen = NativeMethods.MakeLong(screenRelative.Y, screenRelative.X);
				// NOTICE: MakeLong(Y,X)  (stupid POINTS struct)
				if (NativeMethods.SendMessageTimeout(hWnd, WM.NonClientHitTest, IntPtr.Zero, PointsOnScreen, SMTO.AbortIfHung, 1000, out ht))
				{
					hitTest = (WindowHitTestRegions)ht;
				}
			}

			if (hitTest == WindowHitTestRegions.ClientArea)
			{

				switch (this.Button)
				{
					case MouseButton.Left:
						NativeMethods.PostMessage(hWnd, (WM)(WM.LeftButtonDown), (int)MK.LButton, NativeMethods.MakeLong(Left, Top));
						NativeMethods.PostMessage(hWnd, WM.LeftButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.LeftButtonDoubleClick, (int)MK.LButton, NativeMethods.MakeLong(Left, Top));
							NativeMethods.PostMessage(hWnd, WM.LeftButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						}
						break;
					case MouseButton.Right:
						NativeMethods.PostMessage(hWnd, WM.RightButtonDown, (int)MK.RButton, NativeMethods.MakeLong(Left, Top));
						NativeMethods.PostMessage(hWnd, WM.RightButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.RightButtonDoubleClick, (int)MK.RButton, NativeMethods.MakeLong(Left, Top));
							NativeMethods.PostMessage(hWnd, WM.RightButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						}
						break;
					case MouseButton.Middle:
						NativeMethods.PostMessage(hWnd, WM.MiddleButtonDown, (int)MK.MButton, NativeMethods.MakeLong(Left, Top));
						NativeMethods.PostMessage(hWnd, WM.MiddleButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.MiddleButtonDoubleClick, (int)MK.MButton, NativeMethods.MakeLong(Left, Top));
							NativeMethods.PostMessage(hWnd, WM.MiddleButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						}
						break;
					case MouseButton.Fourth:
						NativeMethods.PostMessage(hWnd, WM.XButtonDown, NativeMethods.MakeLong((int)MK.XButton1, 1), NativeMethods.MakeLong(Left, Top));
						NativeMethods.PostMessage(hWnd, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.XButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton1, 1), NativeMethods.MakeLong(Left, Top));
							NativeMethods.PostMessage(hWnd, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						}
						break;
					case MouseButton.Fifth:
						NativeMethods.PostMessage(hWnd, WM.XButtonDown, NativeMethods.MakeLong((int)MK.XButton2, 2), NativeMethods.MakeLong(Left, Top));
						NativeMethods.PostMessage(hWnd, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.XButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton2, 2), NativeMethods.MakeLong(Left, Top));
							NativeMethods.PostMessage(hWnd, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
						}
						break;
					default:
						break;
				}
			}
			else // deal with non-client clicks ...
			{
				switch (this.Button)
				{
					case MouseButton.Left:
						NativeMethods.PostMessage(hWnd, WM.NonClientLeftButtonDown, (int)hitTest, PointsOnScreen);
						NativeMethods.PostMessage(hWnd, WM.NonClientLeftButtonUp, (int)hitTest, PointsOnScreen);
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.NonClientLeftButtonDoubleClick, (int)hitTest, PointsOnScreen);
							NativeMethods.PostMessage(hWnd, WM.NonClientLeftButtonUp, (int)hitTest, PointsOnScreen);
						}
						break;
					case MouseButton.Right:
						NativeMethods.PostMessage(hWnd, WM.NonClientRightButtonDown, (int)hitTest, PointsOnScreen);
						NativeMethods.PostMessage(hWnd, WM.NonClientRightButtonUp, (int)hitTest, PointsOnScreen);
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.NonClientRightButtonDoubleClick, (int)hitTest, PointsOnScreen);
							NativeMethods.PostMessage(hWnd, WM.NonClientRightButtonUp, (int)hitTest, PointsOnScreen);
						}
						break;
					case MouseButton.Middle:
						NativeMethods.PostMessage(hWnd, WM.NonClientMiddleButtonDown, (int)hitTest, PointsOnScreen);
						NativeMethods.PostMessage(hWnd, WM.NonClientMiddleButtonUp, (int)hitTest, PointsOnScreen);
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.NonClientMiddleButtonDoubleClick, (int)hitTest, PointsOnScreen);
							NativeMethods.PostMessage(hWnd, WM.NonClientMiddleButtonUp, (int)hitTest, PointsOnScreen);
						}
						break;
					case MouseButton.Fourth:
						NativeMethods.PostMessage(hWnd, WM.NonClientXButtonDown, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
						NativeMethods.PostMessage(hWnd, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.NonClientXButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
							NativeMethods.PostMessage(hWnd, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
						}
						break;
					case MouseButton.Fifth:
						NativeMethods.PostMessage(hWnd, WM.NonClientXButtonDown, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
						NativeMethods.PostMessage(hWnd, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
						if (DoubleClick.ToBool())
						{
							NativeMethods.PostMessage(hWnd, WM.NonClientXButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
							NativeMethods.PostMessage(hWnd, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
						}
						break;
					default:
						break;
				}

			}

			if (AltButton)
			{
				//NativeMethods.PostMessage(hWnd, WM.KeyDown, Keys.Alt, 0);
				NativeMethods.KeyboardEvent(Keys.Alt, KeyEventFlags.KeyUp);
			}
			if (ShiftButton)
			{
				NativeMethods.KeyboardEvent(Keys.Shift, KeyEventFlags.KeyUp);
			}
			if (ControlButton)
			{
				NativeMethods.KeyboardEvent(Keys.Control, KeyEventFlags.KeyUp);
			}
			if (WinButton)
			{
				NativeMethods.KeyboardEvent(Keys.LWin, KeyEventFlags.KeyUp);
			}

		}
	}
}

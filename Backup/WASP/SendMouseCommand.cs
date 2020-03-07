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

namespace Huddled.Wasp
{
	[Cmdlet(VerbsCommunications.Send, "Mouse", DefaultParameterSetName = "Click")]
	public class SendMouseCommand : PSCmdlet
	{
		[Parameter(Mandatory=true, ValueFromPipeline=true, ValueFromPipelineByPropertyName=true)]
		public AutomationElement Window { get; set; }
		
		[Parameter()]
		public Int16 Count { get { return _count; } set { _count = value; } }
		private Int16 _count = 1;

		//[Parameter(Mandatory = true, ParameterSetName = "Click")]
		//public SwitchParameter Click { get; set; }


		[Parameter(Mandatory = true)]
		[Alias("StartPosition")]
		public Point Position { get; set; }


		//// This would go with Keyboard_Event, but has been superceded by SendInput
		//[DllImport("user32.dll")]
		//public static extern void Mouse_Event
		//(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
		//public enum MouseEventType : int
		//{
		//   LeftDown = 0x02,
		//   LeftUp = 0x04,
		//   RightDown = 0x08,
		//   RightUp = 0x10
		//}

		internal static class SafeNativeMethods
		{
			[DllImport("user32.dll", SetLastError = true)]
         public static extern uint SendInput(uint inputCount, INPUT[] inputArray, int inputSize);

			[DllImport("user32.dll")]
         public static extern IntPtr GetMessageExtraInfo();
			
			#region SendInput Constants

         [Flags()]
         public enum KEYEVENTF
         {
            ExtendedKey = 0x0001,
            KeyUp       = 0x0002,
            Unicode     = 0x0004,
            Scancode    = 0x0008,
         }
         public const uint XBUTTON1 = 0x0001;
         public const uint XBUTTON2 = 0x0002;

         [Flags()] //http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx
         public enum MouseFlags : uint
         {
            Move        = 0x0001,  // mouse move 
            LeftDown    = 0x0002,  // left button down
            LeftUp      = 0x0004,  // left button up
            RightDown   = 0x0008,  // right button down
            RightUp     = 0x0010,  // right button up
            MiddleDown  = 0x0020,  // middle button down
            MiddleUp    = 0x0040,  // middle button up
            XDown       = 0x0080,  // x button down 
            XUp         = 0x0100,  // x button down
            Wheel       = 0x0800,  // wheel button rolled
            HWheel      = 0x1000,  // horizontal wheel roll
            NOCOALESCE  = 0x2000,
            VirtualDesk = 0x4000,  // map to entire virtual desktop
            Absolute    = 0x8000,  // absolute move
         }

			#endregion

			#region SendInputTypes
			[StructLayout(LayoutKind.Sequential)]
         public struct MOUSEINPUT //http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx
			{
            public MOUSEINPUT(Point point, MouseFlags flags, bool absolute)
            {
               dx = (int)point.X;
               dy = (int)point.Y;
               dwFlags = (uint)(flags | (absolute ? MouseFlags.Absolute : 0));
               time = 0;
               mouseData = 0;
               dwExtraInfo = IntPtr.Zero;
            }
				Int32 dx;
				Int32 dy;
				UInt32 mouseData;
            UInt32 dwFlags;
            UInt32 time;
				IntPtr dwExtraInfo;
			}

			[StructLayout(LayoutKind.Sequential)]
         public struct KEYBDINPUT
			{
				ushort wVk;
				ushort wScan;
				uint dwFlags;
				uint time;
				IntPtr dwExtraInfo;
			}

			[StructLayout(LayoutKind.Sequential)]
         public struct HARDWAREINPUT
			{
				uint uMsg;
				ushort wParamL;
				ushort wParamH;
			}

         public enum InputType : int
         {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
         }

			[StructLayout(LayoutKind.Explicit)]
			public struct INPUT
			{
            public INPUT(MOUSEINPUT mouseInput)
            {
               type = InputType.Mouse;
               mi = mouseInput;
               ki = new KEYBDINPUT();
            }

            public INPUT(KEYBDINPUT keyboardInput)
            {
               type = InputType.Keyboard;
               ki = keyboardInput;
               mi = new MOUSEINPUT();
            }

            // The Type field isn't for me, duh ...
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [FieldOffset(0)]
            InputType type;
				//[FieldOffset(IntPtr.Size)] //*
				[FieldOffset(8)]
				MOUSEINPUT mi;
				//[FieldOffset(IntPtr.Size)] //*
				[FieldOffset(8)]
				KEYBDINPUT ki;
            ////[FieldOffset(IntPtr.Size)] //*
            //[FieldOffset(8)]
            //HARDWAREINPUT hi;
			}
			#endregion SendInput Types

		}



		protected override void ProcessRecord()
		{
			// Invoke the thing...
			(Window.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern).Invoke();
			Point pt = Window.GetClickablePoint();

         var click = new[] { 
            new SafeNativeMethods.INPUT(
               new SafeNativeMethods.MOUSEINPUT(pt, SafeNativeMethods.MouseFlags.LeftDown, true)),
            new SafeNativeMethods.INPUT(
               new SafeNativeMethods.MOUSEINPUT(pt, SafeNativeMethods.MouseFlags.LeftUp, true))
         };

			// Send any number of clicks
			for (int iNumberOfClicks = 1; iNumberOfClicks <= _count; iNumberOfClicks++)
         {
            WriteDebug( SafeNativeMethods.SendInput(2, click, Marshal.SizeOf(click[0])).ToString(System.Globalization.CultureInfo.CurrentCulture) );
         }


			base.ProcessRecord();
		}
	}
}

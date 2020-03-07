//// Copyright (c) 2007 Joel Bennett
//// This File is license under the Ms Reciprocal License

//// Copyright Grant- Subject to the terms of this license, each contributor grants you 
//// a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
//// prepare derivative works of its contribution, and distribute its contribution or any 
//// derivative works that you create.

//// Conditions and Limitations

//// Reciprocal Grants- For any file you distribute that contains code from the software
//// (in source code or binary format), you must provide recipients the source code to that 
//// file along with a copy of this license, which license will govern that file. You may 
//// license other files that are entirely your own work and do not contain code from the 
//// software under any terms you choose.

//// *****************************************************************************
//// NOTE: For current and complete licensing information please see:
//// http://www.codeplex.com/WASP/Project/License.aspx
////

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Management.Automation;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;
////using Huddled.Wasp.NativeTypes;
//using System.Drawing;
//using System.Windows.Automation;
//using System.Windows;

//namespace Huddled.Wasp
//{
//   [Cmdlet(VerbsCommunications.Send, "Click", DefaultParameterSetName = "Default", SupportsShouldProcess = true)]
//   public class SendClickCommand : AutomationElementCmdletBase
//   {
//      public enum MouseButton
//      {
//         Left, Right, Middle, Fourth, Fifth
//      }

//      [Parameter(Position = 1)]
//      [Alias("X")]
//      public int Left { get; set; }

//      [Parameter(Position = 2)]
//      [Alias("Y")]
//      public int Top { get; set; }

//      [Parameter(Position = 3)]
//      public MouseButton Button { get; set; }

//      [Parameter()]
//      public SwitchParameter AltButton { get; set; }

//      [Parameter()]
//      public SwitchParameter ShiftButton { get; set; }

//      [Parameter()]
//      public SwitchParameter ControlButton { get; set; }

//      [Parameter()]
//      public SwitchParameter WinButton { get; set; }

//      [Parameter()]
//      public SwitchParameter DoubleClick { get; set; }

//      [Parameter()]
//      public SwitchParameter ForceClient { get; set; }

//      public override void ProcessAutomationElement(AutomationElement element)
//      {
//         element.SetFocus();

//         if (AltButton)
//         {
//            //NativeMethods.PostMessage(hWnd, WM.KeyDown, Keys.Alt, 0);
//            NativeMethods.KeyboardEvent(Keys.Alt, NativeMethods.KeyEventFlags.KeyDown);
//         }
//         if (ShiftButton)
//         {
//            NativeMethods.KeyboardEvent(Keys.Shift, NativeMethods.KeyEventFlags.KeyDown);
//         }
//         if (ControlButton)
//         {
//            NativeMethods.KeyboardEvent(Keys.Control, NativeMethods.KeyEventFlags.KeyDown);
//         }
//         if (WinButton)
//         {
//            NativeMethods.KeyboardEvent(Keys.LWin, NativeMethods.KeyEventFlags.KeyDown);
//         }

//         // I can't explain how safe this is ;P
//         // We're going to create our window message by subtracting the "NonClient" value
//         NativeMethods.WindowHitTestRegions hitTest = NativeMethods.WindowHitTestRegions.ClientArea;
//         int PointsOnScreen = 0;
//         if (!ForceClient.ToBool())
//         {
//            IntPtr ht;
//            Point screenRelative = new Point(Left, Top);//TODO: element.PointToScreen(Left, Top);
//            PointsOnScreen = NativeMethods.MakeLong((int)screenRelative.Y, (int)screenRelative.X);

//            // NOTICE: MakeLong(Y,X)  (stupid POINTS struct)
//            if (NativeMethods.SendMessageTimeout(element, NativeMethods.WM.NonClientHitTest, IntPtr.Zero, PointsOnScreen, NativeMethods.SMTO.AbortIfHung, 1000, out ht))
//            {
//               hitTest = (WindowHitTestRegions)ht;
//            }
//         }

//         if (hitTest == WindowHitTestRegions.ClientArea)
//         {

//            switch (this.Button)
//            {
//               case MouseButton.Left:
//                  NativeMethods.PostMessage(element, (WM)(WM.LeftButtonDown), (int)MK.LButton, NativeMethods.MakeLong(Left, Top));
//                  NativeMethods.PostMessage(element, WM.LeftButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.LeftButtonDoubleClick, (int)MK.LButton, NativeMethods.MakeLong(Left, Top));
//                     NativeMethods.PostMessage(element, WM.LeftButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  }
//                  break;
//               case MouseButton.Right:
//                  NativeMethods.PostMessage(element, WM.RightButtonDown, (int)MK.RButton, NativeMethods.MakeLong(Left, Top));
//                  NativeMethods.PostMessage(element, WM.RightButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.RightButtonDoubleClick, (int)MK.RButton, NativeMethods.MakeLong(Left, Top));
//                     NativeMethods.PostMessage(element, WM.RightButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  }
//                  break;
//               case MouseButton.Middle:
//                  NativeMethods.PostMessage(element, WM.MiddleButtonDown, (int)MK.MButton, NativeMethods.MakeLong(Left, Top));
//                  NativeMethods.PostMessage(element, WM.MiddleButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.MiddleButtonDoubleClick, (int)MK.MButton, NativeMethods.MakeLong(Left, Top));
//                     NativeMethods.PostMessage(element, WM.MiddleButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  }
//                  break;
//               case MouseButton.Fourth:
//                  NativeMethods.PostMessage(element, WM.XButtonDown, NativeMethods.MakeLong((int)MK.XButton1, 1), NativeMethods.MakeLong(Left, Top));
//                  NativeMethods.PostMessage(element, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.XButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton1, 1), NativeMethods.MakeLong(Left, Top));
//                     NativeMethods.PostMessage(element, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  }
//                  break;
//               case MouseButton.Fifth:
//                  NativeMethods.PostMessage(element, WM.XButtonDown, NativeMethods.MakeLong((int)MK.XButton2, 2), NativeMethods.MakeLong(Left, Top));
//                  NativeMethods.PostMessage(element, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.XButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton2, 2), NativeMethods.MakeLong(Left, Top));
//                     NativeMethods.PostMessage(element, WM.XButtonUp, 0, NativeMethods.MakeLong(Left, Top));
//                  }
//                  break;
//               default:
//                  break;
//            }
//         }
//         else // deal with non-client clicks ...
//         {
//            switch (this.Button)
//            {
//               case MouseButton.Left:
//                  NativeMethods.PostMessage(element, WM.NonClientLeftButtonDown, (int)hitTest, PointsOnScreen);
//                  NativeMethods.PostMessage(element, WM.NonClientLeftButtonUp, (int)hitTest, PointsOnScreen);
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.NonClientLeftButtonDoubleClick, (int)hitTest, PointsOnScreen);
//                     NativeMethods.PostMessage(element, WM.NonClientLeftButtonUp, (int)hitTest, PointsOnScreen);
//                  }
//                  break;
//               case MouseButton.Right:
//                  NativeMethods.PostMessage(element, WM.NonClientRightButtonDown, (int)hitTest, PointsOnScreen);
//                  NativeMethods.PostMessage(element, WM.NonClientRightButtonUp, (int)hitTest, PointsOnScreen);
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.NonClientRightButtonDoubleClick, (int)hitTest, PointsOnScreen);
//                     NativeMethods.PostMessage(element, WM.NonClientRightButtonUp, (int)hitTest, PointsOnScreen);
//                  }
//                  break;
//               case MouseButton.Middle:
//                  NativeMethods.PostMessage(element, WM.NonClientMiddleButtonDown, (int)hitTest, PointsOnScreen);
//                  NativeMethods.PostMessage(element, WM.NonClientMiddleButtonUp, (int)hitTest, PointsOnScreen);
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.NonClientMiddleButtonDoubleClick, (int)hitTest, PointsOnScreen);
//                     NativeMethods.PostMessage(element, WM.NonClientMiddleButtonUp, (int)hitTest, PointsOnScreen);
//                  }
//                  break;
//               case MouseButton.Fourth:
//                  NativeMethods.PostMessage(element, WM.NonClientXButtonDown, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
//                  NativeMethods.PostMessage(element, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.NonClientXButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
//                     NativeMethods.PostMessage(element, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton1, (int)hitTest), PointsOnScreen);
//                  }
//                  break;
//               case MouseButton.Fifth:
//                  NativeMethods.PostMessage(element, WM.NonClientXButtonDown, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
//                  NativeMethods.PostMessage(element, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
//                  if (DoubleClick.ToBool())
//                  {
//                     NativeMethods.PostMessage(element, WM.NonClientXButtonDoubleClick, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
//                     NativeMethods.PostMessage(element, WM.NonClientXButtonUp, NativeMethods.MakeLong((int)MK.XButton2, (int)hitTest), PointsOnScreen);
//                  }
//                  break;
//               default:
//                  break;
//            }

//         }

//         if (AltButton)
//         {
//            //NativeMethods.PostMessage(hWnd, WM.KeyDown, Keys.Alt, 0);
//            NativeMethods.KeyboardEvent(Keys.Alt, KeyEventFlags.KeyUp);
//         }
//         if (ShiftButton)
//         {
//            NativeMethods.KeyboardEvent(Keys.Shift, KeyEventFlags.KeyUp);
//         }
//         if (ControlButton)
//         {
//            NativeMethods.KeyboardEvent(Keys.Control, KeyEventFlags.KeyUp);
//         }
//         if (WinButton)
//         {
//            NativeMethods.KeyboardEvent(Keys.LWin, KeyEventFlags.KeyUp);
//         }

//      }

//      private static class NativeMethods
//      {
//         public static int MakeLong(int HiWord, int LoWord)
//         {
//            return (HiWord << 16) | (LoWord & 0xffff);
//         }

//         [return: MarshalAs(UnmanagedType.Bool)]
//         [DllImport("User32", SetLastError = true)]
//         public static extern bool PostMessage(IntPtr hWnd, WM Msg, SC wParam, IntPtr lParam);
//         [return: MarshalAs(UnmanagedType.Bool)]
//         [DllImport("User32", SetLastError = true)]
//         public static extern bool PostMessage(IntPtr hWnd, WM Msg, int wParam, int lParam);
//         [return: MarshalAs(UnmanagedType.Bool)]
//         [DllImport("User32", SetLastError = true)]
//         public static extern bool PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);


//         [return: MarshalAs(UnmanagedType.Bool)]
//         [DllImport("User32", SetLastError = true, CharSet = CharSet.Auto)]
//         public static extern bool SendMessageTimeout(IntPtr windowHandle,
//             WM Msg,
//            IntPtr wParam,
//             IntPtr lParam,
//             SMTO flags,
//             uint timeout,
//             out IntPtr result);

//         [return: MarshalAs(UnmanagedType.Bool)]
//         [DllImport("User32", SetLastError = true, CharSet = CharSet.Auto)]
//         public static extern bool SendMessageTimeout(IntPtr windowHandle,
//             WM Msg,
//            IntPtr wParam,
//             int lParam,
//             SMTO flags,
//             uint timeout,
//             out IntPtr result);

//         [return: MarshalAs(UnmanagedType.Bool)]
//         [DllImport("User32", SetLastError = true, CharSet = CharSet.Auto)]
//         public static extern bool SendMessageTimeout(IntPtr windowHandle,
//             WM Msg,
//            IntPtr wParam,
//             System.Text.StringBuilder lParam,
//             SMTO flags,
//             uint timeout,
//             out IntPtr result);


//         [DllImport("User32", EntryPoint = "keybd_event")]
//         private static extern void keybd_event(byte VirtualKeyCode, byte ScanCode, int Flags, int dwExtraInfo);
//         public static void KeyboardEvent(System.Windows.Forms.Keys VirtualKeyCode, KeyEventFlags Flags)
//         {
//            keybd_event((byte)VirtualKeyCode, (byte)0, (int)Flags, 0);
//         }

//         public enum WM : uint
//         {
//            LeftButtonDown = 0x201,
//            LeftButtonUp = 0x202,
//            LeftButtonDoubleClick = 0x203,

//            RightButtonDown = 0x204,
//            RightButtonUp = 0x205,
//            RightButtonDoubleClick = 0x206,

//            MiddleButtonDown = 0x207,
//            MiddleButtonUp = 0x208,
//            MiddleButtonDoubleClick = 0x209,

//            XButtonDoubleClick = 0x20D,
//            XButtonDown = 0x20B,
//            XButtonUp = 0x20C,

//            KeyDown = 0x100,
//            KeyFirst = 0x100,
//            KeyLast = 0x108,
//            KeyUp = 0x101,

//            NonClientHitTest = 0x084,

//            //NCACTIVATE         = 0x086,
//            //NCCALCSIZE         = 0x083,
//            //NCCREATE           = 0x081,
//            //NCDESTROY          = 0x082,
//            //NCMOUSEMOVE        = 0x0A0,
//            //NCPAINT				= 0x085,

//            NonClientLeftButtonDown = 0x0A1,
//            NonClientLeftButtonUp = 0x0A2,
//            NonClientLeftButtonDoubleClick = 0x0A3,

//            NonClientRightButtonDown = 0x0A4,
//            NonClientRightButtonUp = 0x0A5,
//            NonClientRightButtonDoubleClick = 0x0A6,

//            NonClientMiddleButtonDown = 0x0A7,
//            NonClientMiddleButtonUp = 0x0A8,
//            NonClientMiddleButtonDoubleClick = 0x0A9,

//            NonClientXButtonDown = 0x0AB,
//            NonClientXButtonUp = 0x0AC,
//            NonClientXButtonDoubleClick = 0x0AD,

//            Activate = 0x006,
//            ActivateApp = 0x01C,
//            SysCommand = 0x112,
//            GetText = 0x00D,
//            GetTextLength = 0x00E,
//         }

//         public enum SC : uint
//         {
//            Minimize = 0xf020,
//            Maximize = 0xf030,
//            Restore = 0xf120,
//            Close = 0xf060

//            //Size = 0xf000,
//            //Move = 0xf010,
//            //NextWindow = 0xf040,
//            //PrevWindow   =0xf050,
//            //VScroll      =0xf070,
//            //HScroll      =0xf080,
//            //MouseMenu    =0xf090,
//            //KeyMenu      =0xf100,
//            //Arrange      =0xf110,
//            //TaskList     =0xf130,
//            //ScreenSave   =0xf140,
//            //hotkey       =0xf150,
//            //default      =0xf160,
//            //monitorpower =0xf170,
//            //contexthelp  =0xf180,
//            //separator    =0xf00f
//         }

//         [Flags]
//         public enum KeyEventFlags : int
//         {
//            KeyDown = 0,
//            ExtendedKey = 0x1,
//            KeyUp = 0x2
//         }

//         public enum WindowHitTestRegions
//         {
//            /// <summary>HTERROR: On the screen background or on a dividing line between windows
//            /// (same as HTNOWHERE, except that the DefWindowProc function produces a system
//            /// beep to indicate an error).</summary>
//            Error = -2,
//            /// <summary>HTTRANSPARENT: In a window currently covered by another window in the
//            /// same thread (the message will be sent to underlying windows in the same thread
//            /// until one of them returns a code that is not HTTRANSPARENT).</summary>
//            TransparentOrCovered = -1,
//            /// <summary>HTNOWHERE: On the screen background or on a dividing line between
//            /// windows.</summary>
//            NoWhere = 0,
//            /// <summary>HTCLIENT: In a client area.</summary>
//            ClientArea = 1,
//            /// <summary>HTCAPTION: In a title bar.</summary>
//            TitleBar = 2,
//            /// <summary>HTSYSMENU: In a window menu or in a Close button in a child window.</summary>
//            SystemMenu = 3,
//            /// <summary>HTGROWBOX: In a size box (same as HTSIZE).</summary>
//            GrowBox = 4,
//            /// <summary>HTMENU: In a menu.</summary>
//            Menu = 5,
//            /// <summary>HTHSCROLL: In a horizontal scroll bar.</summary>
//            HorizontalScrollBar = 6,
//            /// <summary>HTVSCROLL: In the vertical scroll bar.</summary>
//            VerticalScrollBar = 7,
//            /// <summary>HTMINBUTTON: In a Minimize button. </summary>
//            MinimizeButton = 8,
//            /// <summary>HTMAXBUTTON: In a Maximize button.</summary>
//            MaximizeButton = 9,
//            /// <summary>HTLEFT: In the left border of a resizable window (the user can click
//            /// the mouse to resize the window horizontally).</summary>
//            LeftSizeableBorder = 10,
//            /// <summary>HTRIGHT: In the right border of a resizable window (the user can click
//            /// the mouse to resize the window horizontally).</summary>
//            RightSizeableBorder = 11,
//            /// <summary>HTTOP: In the upper-horizontal border of a window.</summary>
//            TopSizeableBorder = 12,
//            /// <summary>HTTOPLEFT: In the upper-left corner of a window border.</summary>
//            TopLeftSizeableCorner = 13,
//            /// <summary>HTTOPRIGHT: In the upper-right corner of a window border.</summary>
//            TopRightSizeableCorner = 14,
//            /// <summary>HTBOTTOM: In the lower-horizontal border of a resizable window (the
//            /// user can click the mouse to resize the window vertically).</summary>
//            BottomSizeableBorder = 15,
//            /// <summary>HTBOTTOMLEFT: In the lower-left corner of a border of a resizable
//            /// window (the user can click the mouse to resize the window diagonally).</summary>
//            BottomLeftSizeableCorner = 16,
//            /// <summary>HTBOTTOMRIGHT: In the lower-right corner of a border of a resizable
//            /// window (the user can click the mouse to resize the window diagonally).</summary>
//            BottomRightSizeableCorner = 17,
//            /// <summary>HTBORDER: In the border of a window that does not have a sizing
//            /// border.</summary>
//            NonSizableBorder = 18,
//            /// <summary>HTOBJECT: Unknown...No Documentation Found</summary>
//            Object = 19,
//            /// <summary>HTCLOSE: In a Close button.</summary>
//            CloseButton = 20,
//            /// <summary>HTHELP: In a Help button.</summary>
//            HelpButton = 21,
//            /// <summary>HTSIZE: In a size box (same as HTGROWBOX). (Same as GrowBox).</summary>
//            SizeBox = GrowBox,
//            /// <summary>HTREDUCE: In a Minimize button. (Same as MinimizeButton).</summary>
//            ReduceButton = MinimizeButton,
//            /// <summary>HTZOOM: In a Maximize button. (Same as MaximizeButton).</summary>
//            ZoomButton = MaximizeButton
//         }

//         [Flags]
//         public enum SMTO : uint
//         {
//            Normal = 0x0000,
//            Block = 0x0001,
//            AbortIfHung = 0x0002,
//            NoTimeOutIfNotHung = 0x0008
//         }

//      }
//   }
//}

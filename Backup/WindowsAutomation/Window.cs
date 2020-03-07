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
using System.Drawing;
using System.Diagnostics;
using Huddled.Wasp.NativeTypes;

namespace Huddled.Wasp
{
	/// <summary>
	/// A wrapper for IntPtr for window handles
    /// </summary>  
    public class WindowHandle
    {
        private string _text = null;
        private string _class = null;

        public IntPtr Handle { get; set; }
        public WindowHandle(IntPtr handle)
        {
            Handle = handle;
            _text = NativeMethods.GetWindowText( handle );
            _class = NativeMethods.GetClassName( handle );
		  }


		  #region Were Static 
		  public String GetClassName()
		  {
			  return NativeMethods.GetClassName(this);
		  }

		  public Window GetMainWindow()
		  {
			  return (Window)((Window)this).GetProcess().MainWindowHandle;
		  }


		  /// <summary>
		  /// Get the parent of this window. Null if this is a top-level window
		  /// </summary>
		  /// <returns>The parent <see cref="Window"/>, or <c>null</c> if there is no parent.</returns>
		  public IntPtr GetParentOrOwner()
		  {
			  return NativeMethods.GetParent(this.Handle);
		  }

		  public IntPtr GetParent()
		  {
			  return NativeMethods.GetAncestor(this.Handle, GA.Parent);
		  }

		  /// <summary>
		  /// Gets the children of this window, if any.
		  /// <remarks>These windows will probably not be top-level windows.</remarks>
		  /// </summary>
		  /// <value>The child Windows</value>
		  public List<WindowHandle> GetChildren()
		  {
			  var cf = new Huddled.Wasp.WindowFinder.ChildFinder(this);
			  return (List<WindowHandle>)cf.FindAll();
		  }
		  /// <summary>
		  /// Get the Process ID (PID) from a window handle
		  /// </summary>
		  /// <returns>The process id</returns>
		  public int GetProcessId()
		  {
			  int processId = 0;
			  NativeMethods.GetWindowThreadProcessId(this, ref processId);
			  return processId;
		  }



		  /// <summary>
		  /// Gets a value indicating whether the <see cref="Window"/> is minimized.
		  /// </summary>
		  /// <value><c>true</c> if minimized; otherwise, <c>false</c>.</value>
		  public bool GetIsMinimized()
		  {
			  return NativeMethods.IsIconic(this);
		  }

		  /// <summary>
		  /// Gets a value indicating whether the <see cref="Window"/> is maximized.
		  /// </summary>
		  /// <value><c>true</c> if maximized; otherwise, <c>false</c>.</value>
		  public bool GetIsMaximized()
		  {
			  return NativeMethods.IsZoomed(this);
		  }

		  public bool GetIsActive()
		  {
			  return NativeMethods.GetForegroundWindow() == this;
		  }


		  /// <summary>
		  /// Maximizes this Window.
		  /// </summary>
		  /// <remarks>This method posts a message to the window and returns.  If the window
		  /// is busy, or the application is hung, it may fail to respond.</remarks>
		  public void Maximize()
		  {
			  NativeMethods.PostMessage(this, WM.SysCommand, SC.Maximize, IntPtr.Zero);
			  //Win32.NativeMethods.ShowWindow(process.Handle, Win32.SW.Maximize);
		  }

		  /// <summary>
		  /// Minimizes this Window.
		  /// </summary>
		  /// <remarks>This method posts a message to the window and returns.  If the window
		  /// is busy, or the application is hung, it may fail to respond.</remarks>
		  public void Minimize()
		  {
			  NativeMethods.PostMessage(this, WM.SysCommand, SC.Minimize, IntPtr.Zero);
			  //Win32.NativeMethods.ShowWindow(process.Handle, Win32.SW.Minimize);
		  }

		  /// <summary>
		  /// Restores this Window.
		  /// </summary>
		  /// <remarks>This method posts a message to the window and returns.  If the window
		  /// is busy, or the application is hung, it may fail to respond.</remarks>
		  public void Restore()
		  {
			  NativeMethods.PostMessage(this, WM.SysCommand, SC.Restore, IntPtr.Zero);
			  //Win32.NativeMethods.ShowWindow(process.Handle, Win32.SW.Restore);
		  }

		  /// <summary>
		  /// Gets the text of the window
		  /// </summary>
		  /// <returns>The Window Text (usually the caption)</returns>
		  public string GetWindowText()
		  {
			  return NativeMethods.GetWindowText(this);
		  }

		  ///// <summary>
		  ///// Sets the text of the window
		  ///// </summary>
		  ///// <param name="text">The Text to set (not all windows accept setting their text)</param>
		  //public void SetWindowText( string text)
		  //{
		  //    NativeMethods.SetWindowText(this, text); 
		  //}


		  public Point PointToScreen(Point origin)
		  {
			  Rectangle pos = GetPosition();
			  return new Point(origin.X + pos.Left, origin.Y + pos.Top);
		  }

		  public Point PointToScreen(int X, int Y)
		  {
			  Rectangle pos = GetPosition();
			  return new Point(X + pos.Left, Y + pos.Top);
		  }
		  /// <summary>
		  /// Get the position of the window.
		  /// </summary>
		  /// <returns>A <see cref="System.Drawing.Rectangle"/> representing the current position of the window</returns>
		  public Rectangle GetPosition()
		  {
			  return NativeMethods.GetWindowRectangle(this);
		  }

		  /// <summary>
		  /// Set the position of the window
		  /// </summary>

		  /// <param name="value">A <see cref="System.Drawing.Rectangle"/> representing the new position of the window</param>
		  public void SetPosition( Rectangle value)
		  {
			  SetPosition(value.X, value.Y, value.Width, value.Height);
		  }



		  /// <summary>
		  /// Moves the window to the specified location and size
		  /// </summary>
		  /// <param name="x">The horizontal location.</param>
		  /// <param name="y">The vertical location.</param>
		  /// <param name="width">The width.</param>
		  /// <param name="height">The height.</param>
		  public void SetPosition( int x, int y, int width, int height)
		  {
			  SWP param = SWP.NOACTIVATE;
			  if (width + height <= 0)
			  {
				  param |= SWP.NOSIZE;
			  }

			  if (this.GetIsMaximized() || this.GetIsMaximized()) this.Restore();
			  NativeMethods.SetWindowPos(this, IntPtr.Zero, x, y, width, height, SWP.NOACTIVATE);
		  }


		  /// <summary>
		  /// Resizes the window to the specified width and height.
		  /// </summary>
		  /// <param name="width">The width.</param>
		  /// <param name="height">The height.</param>
		  public void SetSize( int width, int height)
		  {
			  if (this.GetIsMaximized() || this.GetIsMaximized()) this.Restore();

			  NativeMethods.SetWindowPos(this, IntPtr.Zero, 0, 0, width, height, SWP.NOMOVE | SWP.NOACTIVATE);
		  }




		  /// <summary>
		  /// Activates this Window.
		  /// </summary>
		  /// <remarks>This method posts a message to the window and returns.  If the window
		  /// is busy, or the application is hung, it may fail to respond.</remarks>
		  public void Activate()
		  {
			  NativeMethods.SetForegroundWindow(this);
			  //NativeMethods.PostMessage(this.Handle, WM.Activate, IntPtr.Zero, IntPtr.Zero);
		  }

		  /// <summary>
		  /// Closes this Window.
		  /// </summary>
		  /// <remarks>This method posts a message to the window and returns.  If the window
		  /// is busy, or the application is hung, it may fail to respond.</remarks>
		  public void Close()
		  {
			  NativeMethods.PostMessage(this, WM.SysCommand, SC.Close, IntPtr.Zero);
		  }

		  #region AppWindow Tests
		  /// <summary>
		  /// Determines whether the specified WindowHandle is real TaskWindow:
		  /// That is, one that should show up on the TaskBar
		  /// </summary>
		  /// <returns>
		  /// 	<c>true</c> if the specified window is a TaskWindow; otherwise, <c>false</c>.
		  /// </returns>
		  public bool IsAppWindow()
		  {
			  return WindowFinder.IsAppWindow(this.Handle);
		  }
		  /// <summary>
		  /// Determines whether the specified WindowHandle is real TaskWindow:
		  /// That is, one that should show up on the TaskBar
		  /// </summary>
		  /// <param name="includeToolWindows">If set to <c>true</c> include Tool windows.</param>
		  /// <returns>
		  /// 	<c>true</c> if the specified window is a TaskWindow; otherwise, <c>false</c>.
		  /// </returns>
		  public bool IsAppWindow( bool includeToolWindows)
		  {
			  return WindowFinder.IsAppWindow(this.Handle, includeToolWindows);
		  }
		  #endregion  AppWindow Tests

		  #endregion


		  #region type conversion
		  public static implicit operator IntPtr(WindowHandle wnd)
        {
            return wnd.Handle;
        }

        public static implicit operator WindowHandle(IntPtr value)
        {
            return new WindowHandle(value);
        }

        public static implicit operator int(WindowHandle wnd)
        {
            return wnd.Handle.ToInt32();
        }

        public static implicit operator WindowHandle(int value)
        {
            return new WindowHandle((IntPtr)value);
        }
        #endregion

        /// <summary>
        /// Gets the Text of the window
        /// </summary>
        public string Title
        {
            get
            {
                return _text;
            }
            //set
            //{
            //    this.SetWindowText(value);
            //}
        }

        /// <summary>
        /// Gets the ClassName for the window
        /// </summary>
        public string Class
        {
            get
            {
                return _class;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "{0}[@Class={1}, @Handle=0x{2:X8}]",
                Title, Class, Handle.ToInt32()
                );
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as WindowHandle);
        }
        public bool Equals(WindowHandle wnd)
        {
            return (Handle == wnd.Handle) && (Class == wnd.Class) && (Title == wnd.Title);
        }
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
    }

    /// <summary>
    /// An implementation of <see cref="WindowHandle"/> that's specific to 
    /// top-level windows (ie: what users think of as "windows").
    /// </summary>
    public class Window : WindowHandle
    {
        public Window( IntPtr handle ) : base( handle ) {}
        
        #region Properties
        private Process _proc;
        private int _processId;


        /// <summary>
        /// The name of the process for the window
        /// </summary>
        public string ProcessName
        {
            get
            {
                return GetProcess().ProcessName;
            }
        }

        /// <summary>
        /// Get the <see cref="Process"/> object for this window
        /// </summary>
        /// <returns>The <see cref="Process"/></returns>
		  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		  public Process GetProcess()
        {
            if (_proc == null)
            {
                _proc = Process.GetProcessById(ProcessId);
            }
            return _proc;
        }

        /// <summary>
        /// Get the <see cref="Process"/> Id for this window
        /// </summary>
        public int ProcessId
        {
            get
            {
                if (_processId == 0)
                {
                    _processId = this.GetProcessId();
                }
                return _processId;
            }
        }

        /// <summary>Get whether or not the window is the active one</summary>
        public bool IsActive
        {
            get
            {
                return this.GetIsActive();
            }
        }
        #endregion Properties
        /// <summary>
        /// Closes the main window for the application.
        /// </summary>
        /// <returns>True on success, False otherwise</returns>
		  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		  public bool CloseMainWindow()
        {
            return GetProcess().CloseMainWindow();
        }

        /// <summary>
        /// Close the process, terminating it.
        /// </summary>
		  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		  public void CloseProcess()
        {
            Process p = GetProcess();
            if (!p.CloseMainWindow())
                p.Close();
        }

		  #region type conversion
		  public static implicit operator IntPtr(Window wnd)
		  {
			  return wnd.Handle;
		  }

		  public static implicit operator Window(IntPtr value)
		  {
			  return new Window(value);
		  }

		  public static implicit operator int(Window wnd)
		  {
			  return wnd.Handle.ToInt32();
		  }

		  public static implicit operator Window(int value)
		  {
			  return new Window((IntPtr)value);
		  }

		  public static implicit operator Window(Control wnd)
		  {
			  return new Window(wnd.Handle);
		  }

		  public static implicit operator Control(Window value)
		  {
			  return new Control(value.Handle);
		  }		
		  #endregion
    }

}

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
using Huddled.Wasp.NativeTypes;
using System.IO;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Diagnostics;

namespace Huddled.Wasp
{

   public static class Constants
   {

      /// <summary>
      /// Defines an invalid handle (-1).
      /// </summary>
      public static readonly IntPtr InvalidHandle = new IntPtr(-1);
      //public static readonly IntPtr WM_GETTEXT = new IntPtr(13);

      public const string StringAny = "*";

      public const char CharAt = '@';
      public const char CharComma = ',';
      public const char CharSlash = '/';
      public const char CharEquals = '=';

      /// <summary>
      /// Defines the opening bracket character.
      /// </summary>
      public const char CharOpeningBracket = '[';
      /// <summary>
      /// Defines the closing bracket character.
      /// </summary>
      public const char CharClosingBracket = ']';

      /// <summary>
      /// Defines the characters that are considered quotes.
      /// </summary>
		public static char[] CharsQuotes
		{
			get { return new[] { '\'', '\"' }; }
		}
      /// <summary>
      /// Defines the characters that follow the end of a string.
      /// </summary>
		public static char[] CharsEnding
		{
			get { return new[] { CharOpeningBracket, CharClosingBracket, CharSlash, CharComma }; }
		}
   }

   public static class WindowFinder
   {

      /// <summary>
      /// Returns a collection of Window objects matching the given expression.
      /// </summary>
      /// <param name="expression">An expression representing the desired group of windows.</param>
      /// <returns>A collection of Window objects matching the given expression.</returns>
      public static List<Window> GetWindows(string expression, bool ignoreCase)
      {
         if (string.IsNullOrEmpty(expression)) throw new ArgumentNullException("expression");

         IntPtr hWnd = IntPtr.Zero;
         List<Window> windows = new List<Window>();
         foreach (WindowPattern wp in ParseWindowXPath(expression))
         {
            hWnd = (wp.Parent == 0 ? hWnd : (IntPtr)wp.Parent);
            IntPtr hWndChild = IntPtr.Zero;
            do
            {
               hWnd = NativeMethods.FindWindowEx(
                    hWnd,
                    hWndChild,
                    WildcardPattern.ContainsWildcardCharacters(wp.Class) ? null : wp.Class,
                    WildcardPattern.ContainsWildcardCharacters(wp.Title) ? null : wp.Title);

               if (hWnd != IntPtr.Zero) windows.Add(new Window(hWnd));

            } while (hWndChild != IntPtr.Zero);
         }
         return windows;
      }

      private struct WindowPattern
      {
         public string Class;
         public string Title;
         //            public int Handle;
         public int Parent;
         public int Index;
      }

      #region ParseWindowPath
      /// <summary>
      /// The regular expression pattern for matching window xpath
      /// <remarks>
      /// The one standout problem is that we're not handling "OR" ...
      /// We just assume everything is ANDed together
      /// </remarks>
      /// </summary>
      private static Regex WindowRePattern = new Regex(@"
        (?<title>[^/\[]*)(\[                     #] The window title is everything up to the [
        ((?<index>\d+)                           # The index is just a number
        |(@C[^=]*=(?<windowclass>[^],&\s]*))     # The class is an attribute and starts with @C.*=
        |(?:@H[^=]*=(?<windowhandle>[^],&\s]*))  # The handle is an attribute and starts with @H.*=
        |[,&\s])                                 # Each is separated by & or a comma (and optional white space)
        *\])?                                    # We might just have a title, with no [@attributes]
        (/|$)                                    # A path separator, or the end of the path
        ", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
      private struct wg
      {
         public const string title = "title";
         public const string index = "index";
         public const string classname = "class";
         public const string handle = "handle";
      }

      /// <summary>
      /// Parses an XPath-like string and returns a queue of WindowPattern objects
      /// which may be traversed to find the appropriate window....
      /// </summary>
      /// <param name="XPath">The array of strings to be parsed.</param>
      /// <returns>TODO</returns>
      private static IEnumerable<WindowPattern> ParseWindowXPath(string XPath)
      {
         if (string.IsNullOrEmpty(XPath)) throw new ArgumentNullException("XPath");
         int parent = 0;
         foreach (Match m in WindowRePattern.Matches(XPath))
         {
            WindowPattern result = new WindowPattern();

            result.Parent = parent;
            result.Class = m.Groups[wg.classname].Success ? m.Groups[wg.classname].Value : null;
            result.Title = m.Groups[wg.title].Success ? m.Groups[wg.title].Value : null;
            //result.Handle =
            parent = m.Groups[wg.handle].Success ? int.Parse(m.Groups[wg.handle].Value) : 0;
            result.Index = m.Groups[wg.index].Success ? int.Parse(m.Groups[wg.index].Value) : 0;

            yield return result;
         }
      }
      #endregion

      #region Window Listings
      private static List<Window> staticWindowList = new List<Window>();
      /// <summary>
      /// Callback method for listing the top-level application windows.  It populates the 
      /// staticWindowList, which chouls be empty prior to starting an enumeration.
      /// </summary>
      /// <param name="hWnd">Handle to a window</param>
      /// <param name="param"></param>
      /// <returns></returns>
      static bool TopLevelCallback(IntPtr hWnd, int param)
      {
         if (IsAppWindow(hWnd))
         {
            Window window = new Window(hWnd);
            if (param > 0)
            {
               int processId = 0;
               NativeMethods.GetWindowThreadProcessId(window, ref processId);
               if (param == processId)
               {
                  staticWindowList.Add(window);
               }
            }
            else
            {
               staticWindowList.Add(window);
            }
         }
         return true; // we want ALL the windows, so we return them all
      }

      /// <summary>
      /// Gets the top level windows.
      /// </summary>
      /// <value>The top level windows.</value>
      public static List<Window> GetTopLevelWindows()
      {
         
			lock (staticWindowList)
			{
				staticWindowList.Clear();
				NativeMethods.EnumWindows(new NativeMethods.EnumWindowsCallback(TopLevelCallback), 0);
	         return staticWindowList;
			}
      }

      /// <summary>
      /// Return all windows of a given process.
      /// </summary>
      /// <param name="process">The process to enumerate windows from</param>
      /// <value>The top level windows</value>
      public static List<Window> GetProcessWindows(Process process)
      {
         return GetProcessWindows(process.Id);
      }
      /// <summary>
      /// Return all windows of a given process.
      /// </summary>
      /// <param name="process">The id of the process to enumerate windows from</param>
      /// <value>The top level windows</value>
      public static List<Window> GetProcessWindows(int processId)
      {
			lock (staticWindowList)
			{
				staticWindowList.Clear();
				NativeMethods.EnumWindows(new NativeMethods.EnumWindowsCallback(TopLevelCallback), processId);
				return staticWindowList;
			}
         
      }


      // NOTE: This doesn't seem to work correctly (on Vista?) so I'm ditching it for now.
      ///// <summary>
      ///// Return all windows of a given thread
      ///// </summary>
      ///// <param name="threadId">The thread id</param>
      ///// <returns></returns>
      //public static List<Window> GetThreadWindows(int threadId)
      //{
      //    staticWindowList = new List<Window>();
      //    NativeMethods.EnumThreadWindows(threadId, new NativeMethods.EnumWindowsCallback(TopLevelCallback), 0);
      //    return staticWindowList;
      //}


      /// <summary>
      /// The deskop window
      /// </summary>
      public static Window GetDesktopWindow()
      {
         return new Window(NativeMethods.GetDesktopWindow());
      }

      /// <summary>
      /// The current foreground window
      /// </summary>
      public static Window GetForegroundWindow()
      {
         return new Window(NativeMethods.GetForegroundWindow());
      }

      #endregion


      #region Child Windows

      public class ChildFinder
      {
         // internal, for (temporary) use with callback methods
         private List<WindowHandle> childWindowList = new List<WindowHandle>();
         private IntPtr parent = IntPtr.Zero;
         private int instance;
         private int count;

         public ChildFinder(IntPtr window)
         {
            parent = window;
			}

         public List<WindowHandle> FindAll()
         {
				lock (childWindowList)
            {
					childWindowList.Clear();
               count = 0;
               instance = new Random().Next();
               NativeMethods.EnumChildWindows(parent, new NativeMethods.EnumWindowsCallback(ChildCallback), instance);
					return childWindowList;
            }
			}

         public List<WindowHandle> FindCount(int count)
         {
            lock (childWindowList)
            {
               this.count = count;
               childWindowList.Clear();
               instance = new Random().Next();
               NativeMethods.EnumChildWindows(parent, new NativeMethods.EnumWindowsCallback(ChildCallback), instance);
               return childWindowList;
            }
         }

         private bool ChildCallback(IntPtr hWnd, int param)
         {
            if (param == instance)
            {
               childWindowList.Add((WindowHandle)hWnd);
            }
            return --count <= 0; // if count starts at or below zero, this goes forever
         }

      }
      /// <summary>
      /// Gets the children of this window, if any.
      /// <remarks>These windows will probably not be top-level windows.</remarks>
      /// </summary>
      /// <value>The child Windows</value>
      public static List<WindowHandle> GetChildren(this WindowHandle wnd)
      {
         ChildFinder cf = new ChildFinder(wnd);
         return (List<WindowHandle>)cf.FindAll();
      }
      #endregion

      #region AppWindow Tests
      /// <summary>
      /// Determines whether the specified WindowHandle is real TaskWindow:
      /// That is, one that should show up on the TaskBar
      /// </summary>
      /// <param name="hWnd">The Window Handle</param>
      /// <returns>
      /// 	<c>true</c> if the specified window is a TaskWindow; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsAppWindow(IntPtr hWnd)
      {
			return IsAppWindow(hWnd, IncludeToolWindowsWithAppWindows);
      }

		public static bool IncludeToolWindowsWithAppWindows = false;
      /// <summary>
      /// Determines whether the specified WindowHandle is real TaskWindow:
      /// That is, one that should show up on the TaskBar
      /// </summary>
      /// <param name="hWnd">The Window Handle</param>
      /// <param name="includeToolWindows">If set to <c>true</c> include Tool windows.</param>
      /// <returns>
      /// 	<c>true</c> if the specified window is a TaskWindow; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsAppWindow(IntPtr hWnd, bool includeToolWindows)
      {
         bool result = true;

         if (NativeMethods.IsWindow(hWnd) && NativeMethods.IsWindowVisible(hWnd))
         {
            WS_EX stylex = (WS_EX)NativeMethods.GetWindowLong(hWnd, GWL.ExStyle);
            if (WS_EX.APPWINDOW == (stylex & WS_EX.APPWINDOW))
            {
               result = true;
            }
            else
            {
               WS style = (WS)NativeMethods.GetWindowLong(hWnd, GWL.Style);
               // if it's visible and doesn't have an owner ... it's PROBABLY an app
               if (IntPtr.Zero.Equals(NativeMethods.GetWindowLongish(hWnd, GWL.hWndParent))
                   && IntPtr.Zero.Equals(NativeMethods.GetWindow(hWnd, GW.Owner)) // doublechecking...
                   && NativeMethods.IsWindowVisible(hWnd)
                   && !(WS.CHILD == (style & WS.CHILD))
                   )
               {
                  // If it is an WS_EX_APPWINDOW force a pass?
                  if (WS_EX.TOOLWINDOW == (stylex & WS_EX.TOOLWINDOW) && !includeToolWindows)
                  {
                     result = false;
                  }
                  else
                  {
                     result = true;
                  }
               }
               else
               {
                  result = false;
               }
            }
            if (result)
            {
               string className = Huddled.Wasp.NativeMethods.GetClassName(hWnd);

               if (className.Equals("WindowsScreensaverClass") || className.Equals("tooltips_class32"))
               {
                  result = false;
               }
            }
         }
         else
         {
            result = false;
         }


         return result;
      }
      #endregion AppWindow Tests
   }

}

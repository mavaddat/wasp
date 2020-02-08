## WASP
![](Home_WASP.png)WASP is a PowerShell snapin for Windows Automation tasks like selecting windows and controls and sending mouse and keyboard events. WASP has [automation cmdlets](#automation-cmdlets) like Select-Window, Select-Control, Send-Keys, Send-Click, Get-WindowPosition, Set-WindowPosition, Set-WindowActive, Remove-Window ... etc.

Its goal is to enable Windows GUI Automation scripting from inside PowerShell without resorting to specialized scripting tools. 

Just to be clear, don't expect any "click to record" functionality &hellip; but, *do* expect to be able to automatically tile windows, send mouse clicks and keystrokes, and in general, automate those tasks that you would normally not be able to do from a console.

### Some Usage Examples
#### Author's standard demo:

##### Open a couple windows
    notepad.exe
    explorer.exe
##### list the windows
    Select-Window | ft –auto
##### Activate Notepad
    Select-Window notepad* | Set-WindowActive
##### Close Explorer
    Select-Window explorer | Select -First 1 | Remove-WIndow
##### Run a few more copies of notepad
    notepad; notepad; notepad; notepad;
##### Move them around so we can see them all ... (Note the use of foreach with the incrementation)
    $i = 1;$t = 100; Select-Window notepad | ForEach { Set-WindowPosition -X 20 -Y (($i++)*$t) -Window $_ }
##### Put some text into them ...
    Select-Window notepad | Send-Keys "this is a test"
##### Close the first notepad window by pressing ALT+F4, and pressing Alt+N
###### In this case, you don't have to worry about shifting focus to the popup, because it's modal
###### THE PROBLEM with sending keys like that is:<br/>&Tab;&Tab;if there is no confirmation dialog because the file is unchanged, the Alt+N still gets sent
    Select-Window notepad | Select -First 1 | Send-Keys "%{F4}%n"
##### Close the next notepad window ... 
##### By asking nicely (Remove-Window) and then hitting "n" for "Don't Save"
##### If there are no popups, Select-ChldWindow returns nothing, and that's the end of it
    Select-Window notepad | Select -First 1 | Remove-Window -Passthru | `
      Select-ChildWindow | Send-Keys "n"
##### Close the next notepad window the hard way 
##### Just to show off that our "Window" objects have a ProcessID and can be piped to kill
    Select-Window notepad | Select -First 1 | kill
##### A different way to confirm Don't Save (use CLICK instead of keyboard)
##### Notice how I dive in through several layers of Select-Control to find the button?
##### This can only work experimentally: 
##### use SPY++, or run the line repeatedly, adding "|Select-Control" until you see the one you want
    Select-Window notepad | Select -First 1 | Remove-Window -Passthru | `
      Select-childwindow | select-control| select-control| select-control -title "Do&n't Save" | Send-Click
##### But now we have the new -Recurse parameter, so it's easy.  Just find the window you want and ...
    Select-Window notepad | Select -First 1 | Remove-Window -Passthru | `
      Select-childwindow | select-control -title "Do&n't Save"  -recurse | Send-Click

#### Author's favourite use: Re-dock all Visual Studio Tool windows

###### NOTE: 0, 0 is the resize corner, so you need to specify X,Y coordinates for the click:
    Select-Window devenv | Select-ChildWindow | Send-Click 10 10 -Double 


### Automation cmdlets
* `Select-Window` - Pick windows by process name or window caption (with wildcard support)
* `Select-ChildWindow` - Pick all owned **windows** of another window (e.g., dialogs, tool windows)
* `Select-Control` - Pick controls (children) of a specific window, by class and/or name and/or index (with wildcard support) -- NOTE: the "Window" can be specified as "-Window 0" to get all parentless windows, which includes windows, dialogs, tooltips, etc... With **-Window 0** this returns a true superset of the Select-Window output.
* `Send-Click` - send mouse clicks (any button, with any modifier keys)
* `Send-Keys` - [Windows.Forms.SendKeys](http://msdn2.microsoft.com/en-us/library/system.windows.forms.sendkeys) lets you send keys ... try this: `Select-Window notepad | Send-Keys "%(ea)Testing{Enter}{F5}"` (and for extra fun, try it with multiple notepad windows open).
* `Set-WindowActive` - Simply activates the window
* `Set-WindowPosition` - Set any one of (or all of) top, left, width, height on a window ... or maximize/minimize/restore
* `Get-WindowPosition` - Get the position (kind-of redundant, actually, since the Window object has it's position as a property)
* `Remove-Window` - Closes the specified window

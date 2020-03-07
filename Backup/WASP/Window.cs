using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Windows.Automation;
using System.Windows;

namespace Huddled.Wasp
{
   public class Window
	{

		AutomationElement _element;

		public Window (AutomationElement element)
		{
			_element = element;
		}

		#region cast operators
      public AutomationElement Element {
         get { return _element; }
         set { _element = value; }
      }

		public AutomationElement ToAutomationElement()
		{
			return _element;
		}

		//public Window FromAutomationElement(AutomationElement element)
		//{
		//   return new Window(element);
		//}

		public static implicit operator AutomationElement(Window window)
		{
			return window._element;
		}

		public static implicit operator Window(AutomationElement element)
		{
			return new Window(element);
		}
		#endregion cast operators

		public bool HasKeyboardFocus
		{
			get {
				return (bool)_element.GetCurrentPropertyValue( AutomationElementIdentifiers.HasKeyboardFocusProperty );
			}
		}
		public bool IsKeyboardFocusable {
			get {
				return (bool)_element.GetCachedPropertyValue(AutomationElementIdentifiers.IsKeyboardFocusableProperty);
			}
		}
		public string Text {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty);
			}
		}
		public bool CanRotate {
			get {
				return (bool)_element.GetCurrentPropertyValue(TransformPatternIdentifiers.CanRotateProperty);
			}
		}
		public WindowVisualState VisualState {
			get { 
				return (WindowVisualState)_element.GetCurrentPropertyValue( WindowPatternIdentifiers.WindowVisualStateProperty );
			}
		}
		
		public string ItemType {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.ItemTypeProperty);
			}
		}
		
		public bool IsPassword {
			get {
				return (bool)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.IsPasswordProperty);
			}
		}

		public string ItemStatus {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.ItemStatusProperty);
			}
		}
		
		public string HelpText {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.HelpTextProperty);
			}
		}
		
		public string ClassName {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.ClassNameProperty);
			}
		}
		
		public string AcceleratorKeys {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.AcceleratorKeyProperty);
			}
		}
		
		public string LocalizedControlType {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.LocalizedControlTypeProperty);
			}
		}
		
		public string LabeledBy {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.LabeledByProperty);
			}
		}
		
		public int ProcessId {
			get { 
				return (int)_element.GetCurrentPropertyValue( AutomationElementIdentifiers.ProcessIdProperty );
			}
		}
		
		public string FrameworkId {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.FrameworkIdProperty);
			}
		}
		
		public bool IsControl {
			get {
				return (bool)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.IsControlElementProperty);
			}
		}
		
		public bool CanMove {
			get {
				return (bool)_element.GetCurrentPropertyValue(TransformPatternIdentifiers.CanMoveProperty);
			}
		}
		
		public bool IsTopmost {
			get {
				return (bool)_element.GetCurrentPropertyValue(WindowPatternIdentifiers.IsTopmostProperty);
			}
		}
		
		public bool IsOffScreen {
			get {
				return (bool)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.IsOffscreenProperty);
			}
		}
		
		public bool IsRequired {
			get {
				return (bool)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.IsRequiredForFormProperty);
			}
		}
		
		public bool IsContent {
			get {
				return (bool)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.IsContentElementProperty);
			}
		}
		
		public string Id {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.AutomationIdProperty);
			}
		}
		
		public bool CanMaximize {
			get {
				return (bool)_element.GetCurrentPropertyValue(WindowPatternIdentifiers.CanMaximizeProperty);
			}
		}
		
		public bool CanResize {
			get {
				return (bool)_element.GetCurrentPropertyValue(TransformPatternIdentifiers.CanResizeProperty);
			}
		}
		
		public bool IsEnabled {
			get {
				return (bool)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty);
			}
		}
		
		public ControlType ControlType {
			get { 
				return (ControlType)_element.GetCurrentPropertyValue( AutomationElementIdentifiers.ControlTypeProperty );
			}
		}
		
		public Rect Bounds {
			get { 
				return (Rect)_element.GetCurrentPropertyValue( AutomationElementIdentifiers.BoundingRectangleProperty );
			}
		}
		
		public bool CanMinimize {
			get {
				return (bool)_element.GetCurrentPropertyValue(WindowPatternIdentifiers.CanMinimizeProperty);
			}
		}
		
		public bool IsModal {
			get {
				return (bool)_element.GetCurrentPropertyValue(WindowPatternIdentifiers.IsModalProperty);
			}
		}
		
		public string AccessKey {
			get {
				return (string)_element.GetCurrentPropertyValue(AutomationElementIdentifiers.AccessKeyProperty);
			}
		}
		
		public OrientationType Orientation {
			get { 
				return (OrientationType)_element.GetCurrentPropertyValue( AutomationElementIdentifiers.OrientationProperty );
			}
		}
		
		public string RuntimeId {
			get { 
				return (string)_element.GetCurrentPropertyValue( AutomationElementIdentifiers.RuntimeIdProperty );
			}
		}
		
		public WindowInteractionState WindowInteractionState {
			get { 
				return (WindowInteractionState)_element.GetCurrentPropertyValue( WindowPatternIdentifiers.WindowInteractionStateProperty );
			}
		}
		
		//public bool {
		//   get { 
		//      return _element.GetCurrentPropertyValue(  );
		//   }
		//}
	}
}

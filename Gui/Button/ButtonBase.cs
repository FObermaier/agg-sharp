﻿using MatterHackers.VectorMath;

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2007 Lars Brubaker
//                  larsbrubaker@gmail.com
//
// Permission to copy, use, modify, sell and distribute this software
// is granted provided this copyright notice appears in all copies.
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
// classes ButtonWidget
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace MatterHackers.Agg.UI
{
	public class ButtonBase : GuiWidget
	{
		private bool mouseDownOnButton = false;

		private event EventHandler PrivateClick;

		private List<EventHandler> ClickEventDelegates = new List<EventHandler>();

		public event EventHandler Click
		{
			//Wraps the PrivateClick event delegate so that we can track which events have been added and clear them if necessary
			add
			{
				PrivateClick += value;
				ClickEventDelegates.Add(value);
			}

			remove
			{
				PrivateClick -= value;
				ClickEventDelegates.Remove(value);
			}
		}

		public void UnbindClickEvents()
		{
			//Clears all event handlers from the Click event
			foreach (EventHandler eh in ClickEventDelegates)
			{
				PrivateClick -= eh;
			}
			ClickEventDelegates.Clear();
		}

		public bool MouseDownOnButton
		{
			get { return mouseDownOnButton; }
			set { mouseDownOnButton = value; }
		}

		public ButtonBase()
		{
		}

		public ButtonBase(double x, double y)
		{
			OriginRelativeParent = new Vector2(x, y);
		}

		public void ClickButton(MouseEventArgs mouseEvent)
		{
			if (PrivateClick != null)
			{
				PrivateClick(this, mouseEvent);
			}
		}

		protected void FixBoundsAndChildrenPositions()
		{
			SetBoundsToEncloseChildren();

			if (LocalBounds.Left != 0 || LocalBounds.Bottom != 0)
			{
				SuspendLayout();
				// let's make sure that a button has 0, 0 at the lower left
				// move the children so they will fit with 0, 0 at the lower left
				foreach (GuiWidget child in Children)
				{
					child.OriginRelativeParent = child.OriginRelativeParent + new Vector2(-LocalBounds.Left, -LocalBounds.Bottom);
				}
				ResumeLayout();

				SetBoundsToEncloseChildren();
			}
		}

		override public void OnMouseDown(MouseEventArgs mouseEvent)
		{
			if (PositionWithinLocalBounds(mouseEvent.X, mouseEvent.Y))
			{
				MouseDownOnButton = true;
			}
			else
			{
				MouseDownOnButton = false;
			}

			base.OnMouseDown(mouseEvent);
		}

		public override void OnEnabledChanged(EventArgs e)
		{
			if (Enabled == false)
			{
				mouseDownOnButton = false;
			}
			base.OnEnabledChanged(e);
		}

		override public void OnMouseUp(MouseEventArgs mouseEvent)
		{
			if (MouseDownOnButton
			  && PositionWithinLocalBounds(mouseEvent.X, mouseEvent.Y))
			{
				ClickButton(mouseEvent);
			}

			MouseDownOnButton = false;

			base.OnMouseUp(mouseEvent);
		}
	}
}
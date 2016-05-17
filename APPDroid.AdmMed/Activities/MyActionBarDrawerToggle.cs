using System;
using Android.App;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V4.Widget;


namespace APPDroid.AdmMed.Activities
{
	#region Variables and Controls
	/// <summary>
	/// Action bar drawer event arguments.
	/// </summary>
	public class ActionBarDrawerEventArgs : EventArgs
	{
		public View DrawerView { get; set; }
		public float SlideOffset { get; set; }
		public int NewState { get; set; }
	}
	#endregion

	#region ActionBarDrawerChangedEventHandler
	/// <summary>
	/// Action bar drawer changed event handler.
	/// </summary>
	public delegate void ActionBarDrawerChangedEventHandler(object s, ActionBarDrawerEventArgs e);
	#endregion

	#region MyActionBarDrawerToggle
	/// <summary>
	/// My action bar drawer toggle.
	/// </summary>
	public class MyActionBarDrawerToggle : ActionBarDrawerToggle
	{
		public MyActionBarDrawerToggle(Activity activity,
			DrawerLayout drawerLayout,
			int drawerImageRes,
			int openDrawerContentDescRes,
			int closeDrawerContentDescRes)
			: base(activity,
				drawerLayout,
				drawerImageRes,
				openDrawerContentDescRes,
				closeDrawerContentDescRes)
		{

		}

		public event ActionBarDrawerChangedEventHandler DrawerClosed;
		public event ActionBarDrawerChangedEventHandler DrawerOpened;
		public event ActionBarDrawerChangedEventHandler DrawerSlide;
		public event ActionBarDrawerChangedEventHandler DrawerStateChanged;

		/// <Docs>Drawer view that is now closed</Docs>
		/// <summary>
		/// Raises the drawer closed event.
		/// </summary>
		/// <param name="drawerView">Drawer view.</param>
		public override void OnDrawerClosed(View drawerView)
		{
			if (null != this.DrawerClosed)
				this.DrawerClosed(this, new ActionBarDrawerEventArgs { DrawerView = drawerView });
			base.OnDrawerClosed(drawerView);
		}

		/// <Docs>Drawer view that is now open</Docs>
		/// <summary>
		/// Raises the drawer opened event.
		/// </summary>
		/// <param name="drawerView">Drawer view.</param>
		public override void OnDrawerOpened(View drawerView)
		{
			if (null != this.DrawerOpened)
				this.DrawerOpened(this, new ActionBarDrawerEventArgs { DrawerView = drawerView });
			base.OnDrawerOpened(drawerView);
		}
		/// <Docs>The child view that was moved</Docs>
		/// <summary>
		/// Raises the drawer slide event.
		/// </summary>
		/// <param name="drawerView">Drawer view.</param>
		/// <param name="slideOffset">Slide offset.</param>
		public override void OnDrawerSlide(View drawerView, float slideOffset)
		{
			if (null != this.DrawerSlide)
				this.DrawerSlide(this, new ActionBarDrawerEventArgs
					{
						DrawerView = drawerView,
						SlideOffset = slideOffset
					});
			base.OnDrawerSlide(drawerView, slideOffset);
		}

		/// <Docs>The new drawer motion state</Docs>
		/// <summary>
		/// Raises the drawer state changed event.
		/// </summary>
		/// <param name="newState">New state.</param>
		public override void OnDrawerStateChanged(int newState)
		{
			if (null != this.DrawerStateChanged)
				this.DrawerStateChanged(this, new ActionBarDrawerEventArgs
					{
						NewState = newState
					});
			base.OnDrawerStateChanged(newState);
		}
	}
	#endregion

}


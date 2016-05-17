
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using APPDroid.CarMed.Adapters;
using APPDroid.AdmMed.Activities;
using Android.Content.Res;
using Android.Support.V4.App;
using APPDroid.Framework.Context;

namespace APPDroid.CarMed.Activities
{
	[Activity (Label = "@string/app_carmed", Icon = "@drawable/ic_carmed")]		
	public class ActCarListPatients : FragmentActivity
	{
		private MyActionBarDrawerToggle  drawerToggle;
		private string drawerTitle;
		private string title;

		private DrawerLayout m_Drawer;
		private ListView _drawerList;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView(Resource.Layout.layout_car_patients);

			this.title = this.drawerTitle = this.Title;
			this.m_Drawer = this.FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			this._drawerList = this.FindViewById<ListView> (Resource.Id.left_drawer);

			this._drawerList.Adapter = new AdapCargarPatients(this);

			//Set click handler when item is selected
			this._drawerList.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
				ListItemClickeds (e.Position);
			};

			//Set Drawer Shadow
			this.m_Drawer.SetDrawerShadow (Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			//DrawerToggle is the animation that happens with the indicator next to the actionbar
			this.drawerToggle = new MyActionBarDrawerToggle (this, this.m_Drawer,
				Resource.Drawable.ic_drawer_light,
				Resource.String.drawer_open,
				Resource.String.drawer_close);

			//Display the current fragments title and update the options menu
			this.drawerToggle.DrawerClosed += (o, args) => {
				this.ActionBar.Title = this.title;
				this.InvalidateOptionsMenu ();
			};

			//Display the drawer title and update the options menu
			this.drawerToggle.DrawerOpened += (o, args) => {
				this.ActionBar.Title = this.drawerTitle;
				this.InvalidateOptionsMenu ();
			};

			//Set the drawer lister to be the toggle.
			this.m_Drawer.SetDrawerListener (this.drawerToggle);

			//if first time you will want to go ahead and click first item.
			if (savedInstanceState == null) {
				//ListItemClicked (0,items);
			}

			this.ActionBar.SetDisplayHomeAsUpEnabled (true);
			this.ActionBar.SetHomeButtonEnabled (true);

			//Abierto por defecto
			m_Drawer.OpenDrawer((int)GravityFlags.Left);
		}

        private void ListItemClickeds(int position) 
        {
            ActivitiesContext.Context.PositionPatient = position;
            FragCarMedicine obj = new FragCarMedicine();
            var fragmentTransation = FragmentManager.BeginTransaction();
            fragmentTransation.Add(Resource.Id.content_frame, obj);
            fragmentTransation.Commit();
            this._drawerList.SetItemChecked(position, true);

            this.m_Drawer.CloseDrawers();
        }

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			//MenuInflater.Inflate(Resource.Menu.main_menu, menu);
			var drawerOpen = this.m_Drawer.IsDrawerOpen((int)GravityFlags.Left);
			//when open don't show anything
			for (int i = 0; i < menu.Size (); i++)
				menu.GetItem (i).SetVisible (!drawerOpen);


			return base.OnPrepareOptionsMenu (menu);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			this.drawerToggle.SyncState ();
		}

		public override void OnConfigurationChanged (Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			this.drawerToggle.OnConfigurationChanged (newConfig);
		}

		// Pass the event to ActionBarDrawerToggle, if it returns
		// true, then it has handled the app icon touch event
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (this.drawerToggle.OnOptionsItemSelected (item))
				return true;

			return base.OnOptionsItemSelected (item);
		}
	}
}


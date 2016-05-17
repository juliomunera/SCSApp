using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using APPDroid.AdmMed.Fragments;

using Android.Support.V4.App;
using Android.Support.V4.Widget;
using SCSAPP.Services.Messages;
using System.Net.Http;
using APPDroid.Framework.Services;
using Newtonsoft.Json;
using SCSAPP.Framework.Context;
using APPDroid.Framework.Context;

namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_admed")]			
	public class FragActCamaCodigo : FragmentActivity
	{
		#region Variables and Controls
		Spinner spServicio;
		Spinner spUbicacion;
		Spinner spDesde;
		Spinner spHasta;
		List<MasterItem> DataService = null; 
		MasterItem ServiceSelected = null;
		MasterItem LocationSelected = null;
		MasterItem CamaInical;
		MasterItem CamaFinal;
		MyActionBarDrawerToggle drawerToggle;
		string drawerTitle;
		string title;
		DrawerLayout m_Drawer;
		#endregion

		#region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState){
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.app_list_camas_rango);

			title = drawerTitle = Title;
			m_Drawer = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);

			spServicio = FindViewById<Spinner> (Resource.Id.spinnerServicios);
			spServicio.ItemSelected += Event_servicio;

			spUbicacion = FindViewById<Spinner> (Resource.Id.spinnerUbicacion);
			spUbicacion.ItemSelected += Event_location;

			spDesde = FindViewById<Spinner> (Resource.Id.spinnerDesde);
			spDesde.ItemSelected += Event_bed_from;

			spHasta = FindViewById<Spinner> (Resource.Id.spinnerHasta);
			spHasta.ItemSelected += Event_bed_to;

			LoadServices ("ALL");

			Button BtnVer = FindViewById<Button> (Resource.Id.btnVer);
			BtnVer.Click += (sender, e) => ValidateUI ();

			//Set Drawer Shadow
			m_Drawer.SetDrawerShadow (Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			//DrawerToggle is the animation that happens with the indicator next to the actionbar
			drawerToggle = new MyActionBarDrawerToggle (this, m_Drawer,
				Resource.Drawable.ic_drawer_light,
				Resource.String.drawer_open,
				Resource.String.drawer_close);

			//Display the current fragments title and update the options menu
			drawerToggle.DrawerClosed += (o, args) => {
				ActionBar.Title = title;
				InvalidateOptionsMenu ();
			};

			//Display the drawer title and update the options menu
				drawerToggle.DrawerOpened += (o, args) => {
					ActionBar.Title = drawerTitle;
					InvalidateOptionsMenu ();
				};

			//Set the drawer lister to be the toggle.
			m_Drawer.SetDrawerListener (drawerToggle);

			//if first time you will want to go ahead and click first item.
			if (savedInstanceState == null) {
				global::Android.Support.V4.App.Fragment fragment = null;
				fragment = FragCodigoManilla.NewInstance (String.Empty, String.Empty);

				SupportFragmentManager.BeginTransaction ()
					.Replace (Resource.Id.content_frame, fragment)
					.Commit ();

				//this._drawerList.SetItemChecked (position, true);
				//ActionBar.Title = this.title = mSamples[position];
				m_Drawer.CloseDrawers();
			}

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			var prog = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
			var VCP = prog.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));

			if(VCP.Value.Equals("L")){
				//Abierto por defecto
				m_Drawer.OpenDrawer((int)GravityFlags.Left);
			}
		}
		#endregion

		#region Event_bed_to
		/// <summary>
		/// Events the bed to.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_bed_to (object sender, AdapterView.ItemSelectedEventArgs e){
			var Editor = sender as Spinner;
			if (Editor != null) {
				CamaFinal = LocationSelected.Children.FirstOrDefault (d => d.Value.Equals (Editor.SelectedItem.ToString ()));
			}
		}
		#endregion

		#region Event_bed_from
		/// <summary>
		/// Events the bed from.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_bed_from (object sender, AdapterView.ItemSelectedEventArgs e){
			var Editor = sender as Spinner;
			if (Editor != null) {

				CamaInical = LocationSelected.Children.FirstOrDefault (d => d.Value.Equals (Editor.SelectedItem.ToString ()));
				if (CamaInical != null)
					SetDataToFinalBed();
			}
		}
		#endregion

		#region Event_location
		/// <summary>
		/// Events the location.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_location (object sender, AdapterView.ItemSelectedEventArgs e){
			var Editor = sender as Spinner;
			if (Editor != null) {
				LocationSelected = ServiceSelected.Children.FirstOrDefault (d => d.Value.Equals (Editor.SelectedItem.ToString ()));
				if (LocationSelected != null) {
					SetDataToInitialBed();
				}
			}
		}
		#endregion

		#region Event_servicio
		/// <summary>
		/// Events the servicio.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Event_servicio (object sender, AdapterView.ItemSelectedEventArgs e){
			var editor = sender as Spinner;
			if (editor != null) {
				ServiceSelected = DataService.FirstOrDefault (d => d.Value.Equals (editor.SelectedItem.ToString ()));
				if (ServiceSelected != null) {
					SetDataToLocation();
				}
			}
		}
		#endregion

		#region LoadServices
		/// <summary>
		/// Get Data Of Service ServicesAndLocations
		/// </summary>
		public async void LoadServices(string tipo){
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)){
				var parameter = new {
					ParantEad = ContextApp.Instance.SelectedEAD.Code,
					ParanUser = ContextApp.Instance.User.Code,
					ParantTipo = tipo
				};
				var jsonRequest = JsonConvert.SerializeObject (parameter, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("ServicesAndLocations", new StringContent (jsonRequest, System.Text.Encoding.UTF8, GetString (APPDroid.Framework.Resource.String.txt_aplicacion_json)));
					await StartServicioAdm (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region StartServicioAdm
		/// <summary>
		/// Get Response of Service ServicesAndLocations
		/// </summary>
		/// <returns>The servicio adm.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartServicioAdm(HttpResponseMessage response){
			try {
				if(response.IsSuccessStatusCode){
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					DataService = JsonConvert.DeserializeObject<List<MasterItem>> (responseJsonText);
					if (DataService != null)
						SetDataToServices();
				}else{
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					Toast toast = Toast.MakeText(this, ExceptionMsg, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

		#region SetDataToServices
		/// <summary>
		/// Sets the data to services.
		/// </summary>
		void SetDataToServices ()
		{
			if (spServicio != null) {
				var dataS = (from i in DataService.AsEnumerable() select i.Value).ToArray();

				var DataServicio = new ArrayAdapter (this, Resource.Layout.item_spinner, dataS);
				DataServicio.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				spServicio.Adapter = DataServicio;
			}
		}
		#endregion

		#region SetDataToLocation
		/// <summary>
		/// Sets the data to location.
		/// </summary>
		void SetDataToLocation(){
			spUbicacion = FindViewById<Spinner> (Resource.Id.spinnerUbicacion);
			if (spUbicacion != null) {
				var dataL = (from i in ServiceSelected.Children.AsEnumerable() select i.Value).ToArray();

				var DataUbicacion = new ArrayAdapter(this, Resource.Layout.item_spinner, dataL);
				DataUbicacion.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				spUbicacion.Adapter = DataUbicacion;
			}
		}
		#endregion

		#region SetDataToInitialBed
		/// <summary>
		/// Sets the data to beds.
		/// </summary>
		void SetDataToInitialBed (){
			string[] dataB = {};
			if (spDesde != null) {
				if (LocationSelected.Children != null) {
					dataB = (from i in LocationSelected.Children.AsEnumerable ()
						select i.Value).ToArray ();
					var datosArrayDesde = new ArrayAdapter (this, Resource.Layout.item_spinner, dataB);
					datosArrayDesde.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
					spDesde.Adapter = datosArrayDesde;
				} else {
					var datosArrayDesde = new ArrayAdapter (this, Resource.Layout.item_spinner, dataB);
					datosArrayDesde.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
					spDesde.Adapter = datosArrayDesde;
					spHasta.Adapter = datosArrayDesde;
				}
			}
		}
		#endregion

		#region SetDataToFinalBed
		/// <summary>
		/// Sets the data to final bed.
		/// </summary>
		void SetDataToFinalBed (){
			string[] dataBF = {};
			if (spHasta != null) {
				dataBF = (from i in LocationSelected.Children.AsEnumerable ()
					where i.Order >= CamaInical.Order
					select i.Value).ToArray ();

				var datosArrayHasta = new ArrayAdapter (this, Resource.Layout.item_spinner, dataBF);
				datosArrayHasta.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				spHasta.Adapter = datosArrayHasta;
			}
		}
		#endregion

		#region SetDataToInitialBed
		/// <summary>
		/// Validates the U.
		/// </summary>
		void ValidateUI ()
		{
			if (ServiceSelected == null) {
				Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.txt_servicio, ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				return;
			} 

			if (LocationService == null) {
				Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.spnn_ubicacion, ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				return;
			}

			if (LocationSelected.Children != null && LocationSelected.Children.Count > 0) {

				if(CamaInical == null){
					Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.txt_cama_v_inicial, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					return;
				}

				if(CamaFinal == null){
					Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.txt_cama_v_final, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();

					if (spHasta != null) {
						spHasta.Focusable = true;
						spHasta.FocusableInTouchMode = true; 	
						spHasta.RequestFocus (FocusSearchDirection.Up);
					}

					return;
				}
			}

			var ventana = new Intent(this, typeof(ActPatientList));
			ActivitiesContext.Context.Parameters = new Dictionary<string, object> ();
			ActivitiesContext.Context.Parameters.Add ("locatioin", LocationSelected.Code);
			ActivitiesContext.Context.Parameters.Add ("initialBed", CamaInical.Order);
			ActivitiesContext.Context.Parameters.Add ("finalBed", CamaFinal.Order);
			StartActivity(ventana);
		}
		#endregion

		#region OnPrepareOptionsMenu
		/// <Docs>The options menu as last shown or first initialized by
		///  onCreateOptionsMenu().</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Prepare the Screen's standard options menu to be displayed. This is
		///  called right before the menu is shown, every time it is shown. You can
		///  use this method to efficiently enable/disable items or otherwise
		///  dynamically modify the contents.</para>
		/// <summary>
		/// Raises the prepare options menu event.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public override bool OnPrepareOptionsMenu (IMenu menu){

			//MenuInflater.Inflate(Resource.Menu.main_menu, menu);
			var drawerOpen = m_Drawer.IsDrawerOpen((int)GravityFlags.Left);
			//when open don't show anything
			for (int i = 0; i < menu.Size (); i++)
				menu.GetItem (i).SetVisible (!drawerOpen);


			return base.OnPrepareOptionsMenu (menu);
		}
		#endregion

		#region OnPostCreate
		/// <summary>
		/// Raises the post create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnPostCreate (Bundle savedInstanceState){
			base.OnPostCreate (savedInstanceState);
			drawerToggle.SyncState ();
		}
		#endregion

		#region OnConfigurationChanged
		/// <Docs>The new device configuration.</Docs>
		/// <remarks>Called by the system when the device configuration changes while your
		///  component is running. Note that, unlike activities, other components
		///  are never restarted when a configuration changes: they must always deal
		///  with the results of the change, such as by re-retrieving resources.</remarks>
		/// <summary>
		/// Raises the configuration changed event.
		/// </summary>
		/// <param name="newConfig">New config.</param>
		public override void OnConfigurationChanged (Configuration newConfig){
			base.OnConfigurationChanged (newConfig);
			drawerToggle.OnConfigurationChanged (newConfig);
		}
		#endregion

		#region OnOptionsItemSelected
		// Pass the event to ActionBarDrawerToggle, if it returns
		// true, then it has handled the app icon touch event
		public override bool OnOptionsItemSelected (IMenuItem item){
			return drawerToggle.OnOptionsItemSelected (item) || base.OnOptionsItemSelected (item);

		}
		#endregion

	}

}



using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using APPDroid.Framework.Context;
using APPDroid.Framework.Helpers;
using APPDroid.Framework.Services;
using SCSAPP.Framework.Context;
using SCSAPP.Services.Messages;

namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", Icon = "@drawable/ic_admed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]		
	public class ActMain : Activity
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
		DateTime initialDate;
		DateTime finalDate;
		bool _clickOnLoginBtn = false;
        int? camaI = null;
        int? camaF = null;
		ProgressDialog progressDialog;
		TextView TodoServicios;
		bool valueunicode;
		#endregion

		#region #region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name = "savedInstanceState"></param>
		/// <param name="savedInstanceState">Bundle.</param>
		protected override void OnCreate (Bundle savedInstanceState){
			base.OnCreate (savedInstanceState);

			SetContentView(Resource.Layout.layout_adm_main);

			spServicio = FindViewById<Spinner> (Resource.Id.spinnerServicios);
			spServicio.ItemSelected += Event_servicio;

			spUbicacion = FindViewById<Spinner> (Resource.Id.spinnerUbicacion);
			spUbicacion.ItemSelected += Event_location;

			spDesde = FindViewById<Spinner> (Resource.Id.spinnerDesde);
			spDesde.ItemSelected += Event_bed_from;

			spHasta = FindViewById<Spinner> (Resource.Id.spinnerHasta);
			spHasta.ItemSelected += Event_bed_to;

			TodoServicios = FindViewById<TextView> (Resource.Id.txtAllServices);
			TodoServicios.Click += TodoServicios_Click;

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			LoadServices ("USER");

			Button BtnVer = FindViewById<Button> (Resource.Id.btnVer);
			BtnVer.Click += (sender, e) => {
				if (_clickOnLoginBtn) return;

				_clickOnLoginBtn = true;
				ValidateUI();
			};
		}
		#endregion

		#region Clic Recover all services
		void TodoServicios_Click (object sender, EventArgs e)
		{
			LoadServices ("ALL");
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

                CamaInical = null;
                CamaFinal = null;

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

                CamaInical = null;
                CamaFinal = null;

				if (ServiceSelected != null) {
					SetDataToLocation();
				}
			}
		}
		#endregion

		#region Retrieve parameter List.
		/// <summary>
		/// Get Data Of Service ServicesAndLocations
		/// </summary>
		public async void LoadServices(string tipo){
			progressDialog.Show ();
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
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
				}
			}
		}
		#endregion

		#region answer parameter List.
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

					valueunicode = DataService[0].AllUserButton;

					if(!valueunicode){
						TodoServicios.Visibility = ViewStates.Gone;
					}

					var ServiceButton = DataService.FirstOrDefault (p => p.Code.Equals ("VALUEUNICODE"));
					if(ServiceButton != null){
						DataService.Remove(ServiceButton);
					}
						
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

			} finally {
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
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

				if(!String.IsNullOrEmpty(ContextApp.Instance.SelectedEAD.Homeservice)){
					var defaultService = DataService.FirstOrDefault (p => p.Code.Equals (ContextApp.Instance.SelectedEAD.Homeservice));
					if(defaultService != null){
						DataService.Remove(defaultService);
						DataService.Insert(0, defaultService);
					}
				}

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

					ArrayAdapter datosArrayHasta = new ArrayAdapter (this, Resource.Layout.item_spinner, dataBF);
					datosArrayHasta.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
					spHasta.Adapter = datosArrayHasta;
			}
		}
		#endregion

		#region Create menu event
		/// <Docs>The options menu in which you place your items.</Docs>
		/// <returns>To be added.</returns>
		/// <summary>
		/// Raises the create options menu event.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public override bool OnCreateOptionsMenu (IMenu menu){
			MenuInflater.Inflate(Resource.Menu.main_menu_adm, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		#endregion

		#region Event of the menu
		/// <Docs>The menu item that was selected.</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">This hook is called whenever an item in your options menu is selected.
		///  The default implementation simply returns false to have the normal
		///  processing happen (calling the item's Runnable or sending a message to
		///  its Handler as appropriate). You can use this method for any items
		///  for which you would like to do processing without those other
		///  facilities.</para>
		/// <summary>
		/// Raises the options item selected event.
		/// </summary>
		/// <param name = "item"></param>
		public override bool OnOptionsItemSelected (IMenuItem item){
			if (item.ItemId == Resource.Id.menu_salir_AdmMed) {
				var alerDialog = (new AlertDialog.Builder(this)).Create();
				alerDialog.SetTitle (APPDroid.Framework.Resource.String.txt_alertas);
				alerDialog.SetMessage (Resources.GetString (APPDroid.Framework.Resource.String.txt_cerrar_sesion));
				alerDialog.SetButton (Resources.GetString (APPDroid.Framework.Resource.String.btn_aceptar), delegate {
					DataBaseManager.DeleteContext(DataBaseManager.IDContextType.ContextApp);

					var loginWindow = new Intent(this, ActivitiesContext.Context.LoginType);
					StartActivity(loginWindow);
					Finish();
				});
				alerDialog.SetButton2 (Resources.GetString (APPDroid.Framework.Resource.String.btn_cancelar), delegate {
					alerDialog.Dismiss();
				});
				alerDialog.Show();
			}

			return base.OnOptionsItemSelected (item);
		}
		#endregion

		#region Validate required fields
		/// <summary>
		/// Validates the U.
		/// </summary>
		void ValidateUI ()
		{
			if (ServiceSelected == null) {

				Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.txt_servicio, ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();

				if (spServicio != null) {
					spServicio.Focusable = true;
					spServicio.FocusableInTouchMode = true; 	
					spServicio.RequestFocus (FocusSearchDirection.Up);
				}

				return;
			} 

			if (LocationService == null) {
				
				if (spUbicacion != null) {
					spUbicacion.Focusable = true;
					spUbicacion.FocusableInTouchMode = true; 	
					spUbicacion.RequestFocus (FocusSearchDirection.Up);
				}

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

					if (spDesde != null) {
						spDesde.Focusable = true;
						spDesde.FocusableInTouchMode = true; 	
						spDesde.RequestFocus (FocusSearchDirection.Up);
					}

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

			string Hour1 ;
			string Hour3;
			string iniDate;
			string finDate;

			var tiedefatr = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("TIEDEFATR"));
			var tiedefade = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (f => f.Code.ToUpper ().Equals ("TIEDEFADE"));

			Hour1 = tiedefatr.Value;

			if (ContextApp.Instance.NursingParametersS.Hour1 < Convert.ToInt32 (tiedefatr.Value)) 
			{
				Hour1 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour1);
			}

			iniDate = (tiedefatr == null || Hour1 == null) ? "1" : Hour1;

			Hour3 = tiedefade.Value;
			if (ContextApp.Instance.NursingParametersS.Hour3 < Convert.ToInt32 (tiedefade.Value)) 
			{
				Hour3 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour3);
			}

			finDate = (tiedefade == null || Hour3 == null) ? "1" : Hour3;

			int rango1Value = 0;
			int rango2Value = 0;

			if (int.TryParse (iniDate, out rango1Value)) {
				if(rango1Value > 12){
					iniDate = "12";
				}
			}

			if (int.TryParse (finDate, out rango2Value)) {
				if(rango2Value > 12){
					finDate = "12";
				}
			}

			initialDate = DateTime.Now;
			finalDate = DateTime.Now;

			if (!String.IsNullOrEmpty(iniDate)) {
				if (int.TryParse(iniDate, out rango1Value)) {
					initialDate = DateTime.Now.AddHours (rango1Value * -1);
				}
			}

			if (!String.IsNullOrEmpty(finDate)) {
				if (int.TryParse(finDate, out rango2Value)) {
					finalDate = DateTime.Now.AddHours (rango2Value);
				}
			}


            camaI = null;
            camaF = null;

            if (CamaInical != null || CamaFinal != null) 
            {
                camaI = CamaInical.Order;
                camaF = CamaFinal.Order;
            }

            LoadPacientes(LocationSelected.Code, camaI, camaF, initialDate, finalDate);

		}
		#endregion

		#region Load patients
		/// <summary>
		/// Loads the pacientes.
		/// </summary>
		/// <param name="location">Location.</param>
		/// <param name="initialbed">Initialbed.</param>
		/// <param name="finalbed">Finalbed.</param>
		/// <param name="initialdate">Initialdate.</param>
		/// <param name="finaldate">Finaldate.</param>
		public async void LoadPacientes(string location, int? initialbed, int? finalbed, DateTime initialdate, DateTime finaldate)
		{	
			progressDialog.Show ();
			ActivitiesContext.Context.PatientsLoadedInPatientList = new List<Patient> ();
			try {
				using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) 
				{
					var request = new
					{
						Location = LocationSelected.Code,
						InitialBed = initialbed,
						FinalBed = finalbed,
						FinalDate = finaldate,
						InitialDate = initialdate
					};

					var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
					try{
						var result = await httpClient.PostAsync ("PatientList", new StringContent (jsonRequest, System.Text.Encoding.UTF8, GetString(APPDroid.Framework.Resource.String.txt_aplicacion_json)));
						await StartPatienList (result);
					}catch (Exception ex) {
						Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						_clickOnLoginBtn = false;
						if (progressDialog.IsShowing) {
							progressDialog.Hide();
						}
					}
				}
			}catch (Exception ex) {
				Toast toast = Toast.MakeText(this, string.Format("Error: {0}",ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				_clickOnLoginBtn = false;
			}
		}
		#endregion

		#region Patients response
		/// <summary>
		/// Starts the patien list.
		/// </summary>
		/// <returns>The patien list.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatienList (HttpResponseMessage response)	{
			
			try {
				if (response.IsSuccessStatusCode) 
				{	
					//Validacion ventana de pacientes seleccion unica 
					Intent intent = ActivitiesContext.Context.ValVarCP.Value.Equals ("V") ? new Intent (this, typeof(ActSelectPatient)) : new Intent (this, typeof(ActPatientList));

					ActivitiesContext.Context.Parameters = new Dictionary<string, object> ();
					ActivitiesContext.Context.Parameters.Add ("locatioin", LocationSelected.Code);
					ActivitiesContext.Context.Parameters.Add("initialBed", camaI);
					ActivitiesContext.Context.Parameters.Add("finalBed", camaF);

					StartActivity(intent);	

					Finish();

				}else{
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					_clickOnLoginBtn = false;
				}

			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				_clickOnLoginBtn = false;
			}
			finally{
				_clickOnLoginBtn = false;
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region OnBackPressed
		public override void OnBackPressed ()
		{
			DataBaseManager.DeleteContext(DataBaseManager.IDContextType.ContextApp);
			var loginWindow = new Intent(this, ActivitiesContext.Context.MenuType);
			StartActivity(loginWindow);
			Finish();
		}
		#endregion

	}
}


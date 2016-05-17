
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using APPDroid.Framework.Context;
using APPDroid.Framework.Services;
using System.Net.Http;
using SCSAPP.Services.Messages.Entities;
using Newtonsoft.Json;
using SCSAPP.Framework.Context;
using SCSAPP.Services.Messages;

namespace APPDroid.CarMed.Activities
{
	[Activity (Label = "@string/app_carmed", Icon = "@drawable/ic_carmed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]				
	public class ActCarMain : Activity
	{
		#region Variables and Controls
		Spinner spEad;
		Spinner spServicios;
		Spinner spFuente;
		Spinner spAlmacen;
		Spinner spUbicacion;
		TextView tituloEad;
		DrugChargesInitialize responseInstance;
		PatientAdministrativeStructure sservices = null;
		AtentionService sUbicacion = null;
		ServiceLocation ubicacionSelect;
		Button btnVer;
		TextView periodo;
		string eadEnvio;
		ProgressDialog progressDialog;
		const string cargosMóvilSoloFuncionaParaOrdenesMédicas = "Cargos móvil solo funciona para Ordenes Médicas";
		#endregion

		#region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.layout_car_main);
			tituloEad = FindViewById<TextView> (Resource.Id.tituloEad);

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			spEad = FindViewById<Spinner> (Resource.Id.spead);
			spEad.ItemSelected += Event_ead;

			spServicios = FindViewById<Spinner> (Resource.Id.spServicios);
			spServicios.ItemSelected += Event_services;

			spAlmacen = FindViewById<Spinner> (Resource.Id.spAlmacenes);
			spAlmacen.ItemSelected += Event_almacen;

			spFuente = FindViewById<Spinner> (Resource.Id.spFuente);
			spFuente.ItemSelected += Event_fuente;

			btnVer = FindViewById<Button> (Resource.Id.btnVer);
			btnVer.Click += BtnVer_Click;
		          			
			spUbicacion = FindViewById<Spinner> (Resource.Id.spUbicacion);
			spUbicacion.ItemSelected += Event_ubicacion;


			periodo = FindViewById<TextView> (Resource.Id.periodo);

			var aplication = new List<SCSAPP.Services.Messages.Entities.Application> ();
			var modules = new List<Module> ();

			modules.Add (new Module { Code = "CUM-MOV" });

			aplication.Add (new SCSAPP.Services.Messages.Entities.Application {
				Code = "INVENT",
				Modules = modules
			});
			modules.Add (new Module { Code = "ORDENE" });

			aplication.Add (new SCSAPP.Services.Messages.Entities.Application {
				Code = ContextApp.Instance.Applications.FirstOrDefault (p => p.Code.ToUpper ().Equals ("HCECLI")).Code,
				Modules = modules
			});
			LoadServices (ContextApp.Instance.User.Code, ContextApp.Instance.SelectedEAD.Code, aplication);
		}
		#endregion

		#region Event_ubicacion
		/// <summary>
		/// Events the ubicacion.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_ubicacion (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var editor = sender as Spinner;
			if (editor != null) {
				ubicacionSelect = sUbicacion.ServiceLocation.FirstOrDefault (a => a.Name.Equals (editor.SelectedItem.ToString ()));
			}
		}
		#endregion

		#region Event_almacen
		/// <summary>
		/// Events the almacen.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_almacen (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var editor = sender as Spinner;
			if (editor != null) {
				var sAlmacen = responseInstance.Warehouses.FirstOrDefault (a => a.Name.Equals (editor.SelectedItem.ToString ()));
				ActivitiesContext.Context.Almacen = sAlmacen;

			}
		}
		#endregion

		#region Event_fuente
		/// <summary>
		/// Events the fuente.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_fuente (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var editor = sender as Spinner;
			if (editor != null) {
				var sFuente = sservices.PatientSources.FirstOrDefault (a => a.Name.Equals (editor.SelectedItem.ToString ()));
				ActivitiesContext.Context.AttachedConcept = sFuente.AttachedConcepto;
				ActivitiesContext.Context.PatientSelecte = sFuente;
			}
		}
		#endregion

		#region click to see
		/// <summary>
		/// Buttons the ver click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void BtnVer_Click (object sender, EventArgs e)
		{
			if (ValidateUI ()) {
				var programasGeneral = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmocarmed"));
				var MS = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("MS"));
				var VP = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("VP"));
				if((MS.Value == "N" || VP.Value == "S") && ActivitiesContext.Context.PatientSelecte.UpdateBalance == "S"){
					if(!ActivitiesContext.Context.Almacen.LastPeriodHasBalance){
						Toast.MakeText (this, "Bodega sin saldo en el periodo anterior", ToastLength.Long).Show ();
						return;
					}
				}

				var ConceptoWare = responseInstance.AuthorizedConceptsWarehouses.
					FirstOrDefault (a => a.WareHouse.Equals (ActivitiesContext.Context.Almacen.Code) && a.Concept.Equals (ActivitiesContext.Context.PatientSelecte.ConceptCode));

				if(ConceptoWare == null){
					Toast.MakeText (this, "Concepto-Bodega no autorizada", ToastLength.Long).Show ();
					return;
				}

				var Cmocarmed = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (v => v.Code.Equals ("cmocarmed"));
				var VarPi = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("PI"));
				var VarPC = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("PC"));
				LoadePatient (VarPi, VarPC);	
			}
		}
		#endregion

		#region Loade Patient
		/// <summary>
		/// Loades the patient.
		/// </summary>
		/// <param name="varPi">Variable pi.</param>
		/// <param name = "varPC"></param>
		public async void LoadePatient (MasterItem varPi, MasterItem varPC)
		{
			try {
				eadEnvio = string.Empty;
				progressDialog.Show ();

				eadEnvio = responseInstance.PatientsEad.Count <= 1 ? responseInstance.PatientsEad [0].Code : sservices.Code;

				ActivitiesContext.Context.ubicacionCar = ubicacionSelect.Code;
				ActivitiesContext.Context.NumberUbic = ubicacionSelect.Name;
				ActivitiesContext.Context.NumeroOrdenCar = responseInstance.NumberOfHoursToRetrieveOrder;

				using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
					var request = new
					{
						Location = ubicacionSelect.Code,
						AdminEad = eadEnvio,
						PatientStatus = varPi.Value,
						AdmissionCharges = varPC.Value,
						UserSession = ContextApp.Instance.SelectedEAD.UserGroup,
						NumberOrder = responseInstance.NumberOfHoursToRetrieveOrder
					};

					var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings  { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
					try{
						var result = await httpClient.PostAsync ("PatientsRecover", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
						await StartPatients (result);
					}catch (Exception ex) {
						Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						if (progressDialog.IsShowing) {
							progressDialog.Hide();
						}
					}
				}	
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();	
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region Start Patients
		/// <summary>
		/// Starts the patients.
		/// </summary>
		/// <returns>The patients.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatients (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {

					string responseJsonText = await response.Content.ReadAsStringAsync ();
					var listPatients = JsonConvert.DeserializeObject<List<ListOfPatiens>> (responseJsonText);

					int cantidadR = listPatients.Count;

					for (int i = 0; i < cantidadR; i++) {
						var liscero = listPatients.FirstOrDefault (p => p.ordersQuantity == 0);
						listPatients.Remove (liscero);
					}

					listPatients.OrderBy (k => k.CodeRoom);

					if (listPatients.Count > 0) {

						ActivitiesContext.Context.listPatients = listPatients;

						ActivitiesContext.Context.EadPatient = eadEnvio;
						ActivitiesContext.Context.year = responseInstance.DefaultWarehouse.Year;
						ActivitiesContext.Context.month = responseInstance.DefaultWarehouse.Month;

						var listaPaciente = new Intent (this, typeof(ActCarList));
						StartActivity (listaPaciente);
					}

				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower ().Contains ("html")) {
						Toast.MakeText (this, ExceptionMsg, ToastLength.Long).Show ();	
					} else {
						string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
						Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();	
					}
				}
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();	
			} finally {
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region Load Services
		/// <summary>
		/// Loads the services.
		/// </summary>
		/// <param name="CodeUser">Code user.</param>
		/// <param name="CodeEad">Code ead.</param>
		/// <param name = "aplication"></param>
		public async void LoadServices (string CodeUser, string CodeEad, List<SCSAPP.Services.Messages.Entities.Application> aplication)
		{
			progressDialog.Show ();
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
				var request = new
				{
					UserCode = CodeUser,
					EadCode = CodeEad,
					Applications = aplication
				};

				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("InitialDrugChargesData", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await StartServices (result);
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

		#region Start Services
		/// <summary>
		/// Starts the services.
		/// </summary>
		/// <returns>The services.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartServices (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					responseInstance = JsonConvert.DeserializeObject<DrugChargesInitialize> (responseJsonText);

					var moduloRes = responseInstance.Applications.FirstOrDefault (a => a.Code.Equals ("HCECLI"));
					var existe = moduloRes.Modules.FirstOrDefault (b => b.Code.Equals ("ORDENE"));

					var moduloCum = responseInstance.Applications.FirstOrDefault (f => f.Code.Equals ("INVENT"));
					var existeCum = moduloCum.Modules.FirstOrDefault (c => c.Code.Equals ("CUM-MOV"));

					ActivitiesContext.Context.indicaCum = existeCum.ExistModule;

					if (existe.ExistModule) {
						Toast.MakeText (this, cargosMóvilSoloFuncionaParaOrdenesMédicas, ToastLength.Long).Show ();
						Finish ();
					} else {
						ActivitiesContext.Context.year = responseInstance.DefaultWarehouse.Year;
						ActivitiesContext.Context.month = responseInstance.DefaultWarehouse.Month;
						ActivitiesContext.Context.NumberHours = responseInstance.NumberOfHoursOfExpirationWarning;

						periodo.Text = String.Format ("Periodo: {0}/{1}", responseInstance.DefaultWarehouse.Year, responseInstance.DefaultWarehouse.Month);

						if (responseInstance != null) {
							if (responseInstance.PatientsEad.Count > 1) {
								spEad.Visibility = ViewStates.Visible;
								tituloEad.Visibility = ViewStates.Visible;
								SetDatatoEad ();
								SetDatatoAlmacenes ();
							} else {
								SetDatatoEad ();
								SetDatatoAlmacenes ();
								EventOneEad ();
							}

						} else {
							Toast.MakeText (this, APPDroid.Framework.Resource.String.txt_des_serializada, ToastLength.Long).Show ();
							Finish ();
						}
					}
				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower ().Contains ("html")) {
						Toast.MakeText (this, ExceptionMsg, ToastLength.Long).Show ();	
						Finish ();
					} else {
						string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
						Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();
						Finish ();
					}
				}
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();	
				Finish ();
			} finally {
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region validate fields
		/// <summary>
		/// Validates the U.
		/// </summary>
		bool ValidateUI ()
		{
			if(sservices == null){
				Toast.MakeText (this, APPDroid.Framework.Resource.String.txt_servicio, ToastLength.Short).Show ();
				if (spServicios == null) {
					spServicios.Focusable = true;
					spServicios.FocusableInTouchMode = true; 	
					spServicios.RequestFocus (FocusSearchDirection.Up);
				}
				return false;	
			}

			if(sUbicacion == null){
				Toast.MakeText (this, APPDroid.Framework.Resource.String.txt_servicio, ToastLength.Short).Show ();
				if (spUbicacion == null) {
					spUbicacion.Focusable = true;
					spUbicacion.FocusableInTouchMode = true; 	
					spUbicacion.RequestFocus (FocusSearchDirection.Up);
				}
				return false;	
			}

			return true;
		}
		#endregion

		#region Event One Ead
		/// <summary>
		/// Events the one ead.
		/// </summary>
		void EventOneEad(){
			sservices = responseInstance.PatientsEad[0];
			SetDataToFuente ();
			SetDataToServices ();
		}
		#endregion

		#region Set Data Ead
		/// <summary>
		/// Sets the datato ead.
		/// </summary>
		void SetDatatoEad()
		{
			var dataS = (from i in responseInstance.PatientsEad select i.Name).ToArray ();

			var DataServicio = new ArrayAdapter (this, Resource.Layout.item_spinner, dataS);
			DataServicio.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spEad.Adapter = DataServicio;
		}
		#endregion

		#region Set Data Shop
		/// <summary>
		/// Sets the datato almacenes.
		/// </summary>
		void SetDatatoAlmacenes()
		{
			if(spAlmacen != null)
			{
				if (!String.IsNullOrEmpty (responseInstance.DefaultWarehouse.Code)) 
				{
					var defaultAlmacen = responseInstance.Warehouses.FirstOrDefault (p => p.Code.Equals (responseInstance.DefaultWarehouse.Code));
					if (defaultAlmacen != null)
					{
						responseInstance.Warehouses.Remove (defaultAlmacen);
						responseInstance.Warehouses.Insert (0, defaultAlmacen);
					}
				}

				var dataAlm = (from i in responseInstance.Warehouses select i.Name).ToArray ();
				var DataAlmacen = new ArrayAdapter (this, Resource.Layout.item_spinner, dataAlm);
				DataAlmacen.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				spAlmacen.Adapter = DataAlmacen;	

			}
		}
		#endregion

		#region Event_ead
		/// <summary>
		/// Events the ead.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_ead (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var editor = sender as Spinner;
			if (editor != null) 
			{
				sservices = responseInstance.PatientsEad.FirstOrDefault (d => d.Name.Equals (editor.SelectedItem.ToString ()));
				
                SetDataToFuente ();
				SetDataToServices ();
			}
		}
		#endregion

		#region Event_services
		/// <summary>
		/// Events the services.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_services (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var editor = sender as Spinner;
			if (editor != null) {

				ActivitiesContext.Context.servicesCar = editor.SelectedItem.ToString ();

				sUbicacion = sservices.AtentionServices.FirstOrDefault (d => d.Name.Equals (editor.SelectedItem.ToString ()));
				SetDataToUbication ();
			} else {
				SetDataToUbication ();
			}
		}
		#endregion

		#region SetDataToUbication
		/// <summary>
		/// Sets the data to ubication.
		/// </summary>
		void SetDataToUbication ()
		{
			var dataP = (from i in sUbicacion.ServiceLocation select i.Name).ToArray();
			var DataUbicacion = new ArrayAdapter(this, Resource.Layout.item_spinner, dataP);
			DataUbicacion.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spUbicacion.Adapter = DataUbicacion;
		}
		#endregion

		#region SetDataToFuente
		/// <summary>
		/// Sets the data to fuente.
		/// </summary>
		void SetDataToFuente()
		{
			if (!String.IsNullOrEmpty (sservices.DefaultSource.Code)) 
			{
				var defaultService = sservices.PatientSources.FirstOrDefault (p => p.Code.Equals (sservices.DefaultSource.Code));
				if (defaultService != null)
				{
					sservices.PatientSources.Remove (defaultService);
					sservices.PatientSources.Insert (0, defaultService);
				}
			}

			var dataP = (from i in sservices.PatientSources select i.Name).ToArray();
			var Datafuente = new ArrayAdapter(this, Resource.Layout.item_spinner, dataP);
			Datafuente.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spFuente.Adapter = Datafuente;
		}
		#endregion

		#region SetDataToServices
		/// <summary>
		/// Sets the data to services.
		/// </summary>
		void SetDataToServices()
		{
			if (!String.IsNullOrEmpty (sservices.DefaultSource.Code)) 
			{
				var defaultService = sservices.PatientSources.FirstOrDefault (p => p.Code.Equals (sservices.DefaultSource.Code));
				if (defaultService != null)
				{
					sservices.PatientSources.Remove (defaultService);
					sservices.PatientSources.Insert (0, defaultService);
				}
			}
			var dataL = (from i in sservices.AtentionServices select i.Name).ToArray();
			var DataUbicacion = new ArrayAdapter(this, Resource.Layout.item_spinner, dataL);
			DataUbicacion.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spServicios.Adapter = DataUbicacion;
		}
		#endregion

		#region OnCreateOptionsMenu
		/// <Docs>The options menu in which you place your items.</Docs>
		/// <returns>To be added.</returns>
		/// <summary>
		/// Raises the create options menu event.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public override bool OnCreateOptionsMenu (IMenu menu){
			MenuInflater.Inflate(APPDroid.Framework.Resource.Menu.menu_framework, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		#endregion

		#region OnOptionsItemSelected
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
		/// <param name="item">Item.</param>
		public override bool OnOptionsItemSelected (IMenuItem item){
			if (item.ItemId == Resource.Id.menu_incosistencias_framework) {
				var inten = new Intent (this, typeof(ActInconsistencia));
				StartActivity (inten);
			}
			return base.OnOptionsItemSelected (item);

		}
		#endregion

	}
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Views;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SCSAPP.Services.Messages.Entities;
using SCSAPP.Framework.Context;
using System.Net.Http;
using Newtonsoft.Json;
using APPDroid.Framework.Services;
using APPDroid.Framework.Context;

namespace APPDroid.DevMed.Activities
{
	[Activity (Label = "@string/txt_devoluciones", Icon = "@drawable/ic_devmed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]  		
	public class ActDevmMain : Activity
	{
		#region Variables and Controls
		TextView periodo;
		Spinner almacenSp, fuenteSp;
		ImageButton searchPatient;
		InitialRefundData listPatients;
		EditText editCodMe;
		ProgressDialog progressDialog;
		Button Verbtn;
		RefundPatient refundPatient;
		bool codeRead;
		string Code;
		#endregion

		#region constructor method
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.layout_dev_main);

			periodo = FindViewById<TextView> (Resource.Id.txtPeriodo);

			almacenSp = FindViewById<Spinner> (Resource.Id.spAlmacen);
			almacenSp.ItemSelected += Event_almacen;

			searchPatient = FindViewById<ImageButton> (Resource.Id.buscarMedicamento);
			searchPatient.Click += SearchPatient_Click;

			fuenteSp = FindViewById<Spinner> (Resource.Id.spFuente);
			fuenteSp.ItemSelected += Event_fuente;

			Verbtn = FindViewById<Button> (Resource.Id.btnVer);
			Verbtn.Click += Verbtn_Click;

			editCodMe = FindViewById<EditText> (Resource.Id.editCodMe);

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			LoadParameters ();

			editCodMe.AfterTextChanged += (sender, e) => {
				if (codeRead)
					return;	

				var Editor = sender as EditText;
				if (Editor != null && !String.IsNullOrEmpty (Editor.Text)) {
					if (Editor.Text.Trim ().Length > 1 && Editor.Text.Substring (Editor.Text.Length - 1, 1).Equals ("\n")) {
						codeRead = false;

						Code = editCodMe.Text;
						Code = Code.Replace ("\n", String.Empty);

						if (Code.Substring (Code.Length - 1, 1).Equals ("\n"))
							Code = Code.Substring (0, Code.Length - 1);

						if (string.IsNullOrEmpty (editCodMe.Text)) {
							Toast.MakeText (this, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long).Show ();
						} else {

							var HistoryText = Code;
							long HistoryLong = -1;

							if (!long.TryParse(HistoryText, out HistoryLong)) 
							{
								Toast toast = Toast.MakeText(this, Resources.GetString(APPDroid.Framework.Resource.String.txt_numbre_historia), ToastLength.Long);
								toast.SetGravity(GravityFlags.Center, 0, 0);
								toast.Show();
								editCodMe.Text = "";
							}else{
								validatePatientma (Code);	
							}
						}
					}
				}
			};

		}
		#endregion

		#region Verbtn_Click
		/// <summary>
		/// Verbtns the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Verbtn_Click (object sender, EventArgs e)
		{
			if (refundPatient == null || String.IsNullOrEmpty (editCodMe.Text)) {
				Toast.MakeText (this, GetString(APPDroid.Framework.Resource.String.txt_obligatorio_texto), ToastLength.Long).Show ();
				return;
			}else if(refundPatient.Sources == null){
				Toast.MakeText (this, GetString(APPDroid.Framework.Resource.String.txt_obligatorio_texto_fuente), ToastLength.Long).Show ();
				return;
			}

			var ConceptoWare = refundPatient.AuthorizedConceptsWarehouses.
											FirstOrDefault (a => a.WareHouse.Equals(ActivitiesContext.Context.Almacen.Code) && a.Concept.Equals(ActivitiesContext.Context.PatientSelecte.ConceptCode));

			if(ConceptoWare == null){
				Toast.MakeText (this, "Concepto-Bodega no autorizada", ToastLength.Long).Show ();
				return;
			}

			ActivitiesContext.Context.listmedicament = null;
			var ventanaPrincipal = new Intent (this, typeof(ActDevMed));	
			StartActivity (ventanaPrincipal);
		}
		#endregion

		#region Load Parameters
		/// <summary>
		/// Loads the parameters.
		/// </summary>
		public async void LoadParameters ()
		{
			var aplication = new List<SCSAPP.Services.Messages.Entities.Application> ();
			var modules = new List<Module> ();
			modules.Add (new Module { Code = "CUM-MOV" });

			aplication.Add (new SCSAPP.Services.Messages.Entities.Application {
				Code = "INVENT",
				Modules = modules
			});

			var Cmocarmed = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (v => v.Code.Equals ("cmodevmed"));
			var MSv = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("MS"));
			var VPv = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("VP"));

			progressDialog.Show ();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Refunds)) {
				var request = new
				{
					UserCode = ContextApp.Instance.User.Code,
					EadCode = ContextApp.Instance.SelectedEAD.Code,
					Applications = aplication,
					MS = MSv.Value,
					VP = VPv.Value
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("InitialRefundData", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
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
					listPatients = JsonConvert.DeserializeObject<InitialRefundData> (responseJsonText);

					var moduloCum = listPatients.ValidatedApplictaions.FirstOrDefault (f => f.Code.Equals ("INVENT"));
					var existeCum = moduloCum.Modules.FirstOrDefault (c => c.Code.Equals ("CUM-MOV"));
					ActivitiesContext.Context.indicaCum = existeCum.ExistModule;

					setViewPeriodo (listPatients);
					setViewAlamacen (listPatients);

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

		#region SearchPatient_Click
		/// <summary>
		/// Searchs the patient click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void SearchPatient_Click (object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty (editCodMe.Text)) {
				Toast.MakeText (this, GetString(APPDroid.Framework.Resource.String.txt_historia_requerida), ToastLength.Long).Show ();
				return;
			}

			var HistoryText = editCodMe.Text;
			long HistoryLong = -1;

			if (!long.TryParse(HistoryText, out HistoryLong)) 
			{
				Toast toast = Toast.MakeText(this, Resources.GetString(APPDroid.Framework.Resource.String.txt_numbre_historia), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				editCodMe.Text = "";
				return;
			}

			validatePatientma (editCodMe.Text);
		}
		#endregion

		#region validatePatientma
		/// <summary>
		/// Validates the patientma.
		/// </summary>
		/// <param name="codeHistory">Code history.</param>
		async void validatePatientma (string codeHistory)
		{
			var Cmocarmed = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (v => v.Code.Equals ("cmodevmed"));
			var VarPi = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("PI"));
			progressDialog.Show ();
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Refunds)) {
				var request = new
				{
					AdministrativeStructure = ContextApp.Instance.SelectedEAD.Code,
					UserCode = ContextApp.Instance.User.Code,
					PI = VarPi.Value,
					listPatients.DefaultWarehouse.Year,
					listPatients.DefaultWarehouse.Month, 
					History = codeHistory,
					Warehouses = listPatients.Warehouses
				};

				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("PatientInfo", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResponseDataCode (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText (this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity (GravityFlags.Center, 0, 0);
					toast.Show ();
					if (progressDialog.IsShowing) {
						progressDialog.Hide ();
					}
				}
			} 
		}
		#endregion

		#region ResponseDataCode
		/// <summary>
		/// Responses the data code.
		/// </summary>
		/// <returns>The data code.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task ResponseDataCode (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					refundPatient = JsonConvert.DeserializeObject<RefundPatient> (responseJsonText);

					ActivitiesContext.Context.devPatients = refundPatient;
					ActivitiesContext.Context.EadPatient = refundPatient.PatientEad;
					setViewFuente (refundPatient);
					ActivitiesContext.Context.patienFund = refundPatient;

				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower ().Contains ("html")) {
						Toast.MakeText (this, ExceptionMsg, ToastLength.Long).Show ();
						refundPatient = new RefundPatient ();
						setViewFuente (refundPatient);
					} else {
						string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
						Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();
						refundPatient = new RefundPatient ();
						setViewFuente (refundPatient);
					}
					editCodMe.Text = "";
				}
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();    
				editCodMe.Text = "";
			} finally {	
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}	
		}
		#endregion

		#region setViewFuente
		/// <summary>
		/// Sets the view fuente.
		/// </summary>
		/// <param name="refundPatient">refundPatient.</param>
		void setViewFuente (RefundPatient refundPatient)
		{
			if (refundPatient.DefaultSource != null) {
				if (!String.IsNullOrEmpty (refundPatient.DefaultSource.Code)) {
					var defaultFuente = refundPatient.Sources.FirstOrDefault (p => p.Code.Equals (refundPatient.DefaultSource.Code));
					if (defaultFuente != null) {
						refundPatient.Sources.Remove (defaultFuente);
						refundPatient.Sources.Insert (0, defaultFuente);
					}
				}

				var dataP = (from i in refundPatient.Sources
				             select i.Name).ToArray ();
				var Datafuente = new ArrayAdapter (this, Resource.Layout.item_spinner, dataP);
				Datafuente.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				fuenteSp.Adapter = Datafuente;
			} else {
				var dataP = new string[0];

				var Datafuente = new ArrayAdapter (this, Resource.Layout.item_spinner, dataP);
				Datafuente.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				fuenteSp.Adapter = Datafuente;	
			}
		}
		#endregion

		#region setViewAlamacen
		/// <summary>
		/// Sets the view alamacen.
		/// </summary>
		/// <param name="listPatients">List patients.</param>
		void setViewAlamacen (InitialRefundData listPatients)
		{
			if (listPatients == null)
				throw new ArgumentNullException ("listPatients");
			if (!String.IsNullOrEmpty (listPatients.DefaultWarehouse.Code)) {
				var defaultService = listPatients.Warehouses.FirstOrDefault (p => p.Code.Equals (listPatients.DefaultWarehouse.Code));
				if (defaultService != null) {
					listPatients.Warehouses.Remove (defaultService);
					listPatients.Warehouses.Insert (0, defaultService);
				}
			}
			var dataP = (from i in listPatients.Warehouses
			             select i.Name).ToArray ();
			var Datafuente = new ArrayAdapter (this, Resource.Layout.item_spinner, dataP);
			Datafuente.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			almacenSp.Adapter = Datafuente;
		}
		#endregion

		#region setViewPeriodo
		/// <summary>
		/// Sets the view periodo.
		/// </summary>
		/// <param name="data">Data.</param>
		void setViewPeriodo (InitialRefundData data)
		{
			ActivitiesContext.Context.year = data.DefaultWarehouse.Year;
			ActivitiesContext.Context.month = data.DefaultWarehouse.Month;
			ActivitiesContext.Context.NumberHours = data.NumberOfHoursOfExpirationWarning;
			periodo.Text = String.Format ("Periodo: {0}/{1}", data.DefaultWarehouse.Year, data.DefaultWarehouse.Month);
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
				//var sFuente = refundPatient.Sources.FirstOrDefault (a => a.Name.Equals (editor.SelectedItem.ToString ()));
				ActivitiesContext.Context.PatientSelecte = refundPatient.Sources[e.Position];
				/*var ConceptoWare = refundPatient.AuthorizedConceptsWarehouses.
					FirstOrDefault (a => a.WareHouse.Equals(ActivitiesContext.Context.Almacen.Code) && a.Concept.Equals(ActivitiesContext.Context.PatientSelecte.ConceptCode));

				if(ConceptoWare == null){
					Toast.MakeText (this, "Concepto-Bodega no autorizada", ToastLength.Long).Show ();
				}*/
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
				//var sAlmacen = listPatients.Warehouses.FirstOrDefault (a => a.Name.Equals (editor.SelectedItem.ToString ()));
				ActivitiesContext.Context.Almacen = listPatients.Warehouses[e.Position];
			}
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
				//var inten = new Intent (this, typeof(ActInconsistencia));
				//StartActivity (inten);
			}
			return base.OnOptionsItemSelected (item);

		}
		#endregion

	}

}


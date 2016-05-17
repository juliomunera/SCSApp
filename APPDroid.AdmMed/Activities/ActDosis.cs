using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using APPDroid.Framework.Context;
using APPDroid.Framework.Services;
using SCSAPP.Services.Messages;
using System.Net.Http;
using Newtonsoft.Json;
using SCSAPP.Framework.Context;
using Android.Views.InputMethods;
using Android.Views;

namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", NoHistory = true, Icon = "@drawable/ic_admed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]		
	public class ActDosis : Activity
	{
		#region Variables and Controls
		TextView NombreMedicament;
		TextView Concentracion;
		TextView cantidadAdmin;
		TextView resta;
		TextView formato1;
		TextView formato2;
		EditText BarCode;
		Button Cancelar, Aceptar;
		ImageButton buscarMedicamento;
		Decimal rango1Value;
		Decimal rango2Value;
		Decimal valor = 0;
		Decimal valor2 = 0;
		bool codeRead = false;
		string Code;
		ProgressDialog progressDialog;
		#endregion

		#region #region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Bundle.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.app_docis);

			NombreMedicament = FindViewById<TextView> (Resource.Id.txtNombreMedicamento);
			Concentracion = FindViewById<TextView> (Resource.Id.QualityUnidad);
			cantidadAdmin = FindViewById<TextView> (Resource.Id.cantidadAdmin);
			resta = FindViewById<TextView> (Resource.Id.resta);
			BarCode = FindViewById<EditText> (Resource.Id.editCodMeDocis);

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			InputMethodManager imms = (InputMethodManager)GetSystemService (Context.InputMethodService);
			imms.HideSoftInputFromInputMethod (BarCode.WindowToken, 0);
			Window.SetSoftInputMode (SoftInput.StateHidden);

			var programa = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
			var DC = programa.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));

			BarCode.Click += delegate {
				if(DC.Value.Equals ("N")){
					imms.HideSoftInputFromWindow(BarCode.WindowToken, 0);
				}
			};

			formato1 = FindViewById<TextView> (Resource.Id.formato1);
			formato1.Text = ActivitiesContext.Context.medicamentDose.Unit;
			formato2 = FindViewById<TextView> (Resource.Id.formato2);
			formato2.Text = ActivitiesContext.Context.medicamentDose.Unit;
			Cancelar = FindViewById<Button> (Resource.Id.btnCancelar);
			Aceptar = FindViewById<Button> (Resource.Id.btnAceptar);
			Aceptar.Enabled = false;

			buscarMedicamento = FindViewById<ImageButton> (Resource.Id.buscarMedicamento);

			NombreMedicament.Text = ActivitiesContext.Context.medicamentDose.Description;
			Concentracion.Text = String.Format("{0} {1}", Math.Round(ActivitiesContext.Context.medicamentDose.Dose.Quantity, 2), ActivitiesContext.Context.medicamentDose.Unit);
			cantidadAdmin.Text = String.Format ("{0}",  Math.Round(ActivitiesContext.Context.medicamentDose.ConcentrationFactor, 2));

			resta.Text = String.Format("{0}", Math.Round(ActivitiesContext.Context.medicamentDose.Dose.Quantity - ActivitiesContext.Context.medicamentDose.ConcentrationFactor, 2));

			rango2Value = ActivitiesContext.Context.medicamentDose.ConcentrationFactor;

			buscarMedicamento.Click += BuscarMedicamento_Click;

			BarCode.AfterTextChanged += (sender, e) => {
				if (codeRead)
					return;

				EditText Editor = sender as EditText;
				if (Editor != null && !String.IsNullOrEmpty (Editor.Text)) {
					if (Editor.Text.Trim ().Length > 1 && Editor.Text.Substring (Editor.Text.Length - 1, 1).Equals ("\n")) {
						codeRead = false;

						Code = Editor.Text;
						Code = Code.Replace ("\n", String.Empty);

						if (Code.Substring (Code.Length - 1, 1).Equals ("\n"))
							Code = Code.Substring (0, Code.Length - 1);

						if (string.IsNullOrEmpty (Editor.Text)) {
							Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();
						} else {
							var medicament = new List<Medicament> ();
							medicament.Add (ActivitiesContext.Context.medicamentDose);
							ValidateBarCode (Code, medicament);
						}
					}
				}
			};

			Aceptar.Click += Aceptar_Click;

			Cancelar.Click += Cancelar_Click;
		}
		#endregion

		#region Click cancel
		/// <summary>
		/// Determines whether this instance cancelar click the specified sender e.
		/// </summary>
		/// <returns><c>true</c> if this instance cancelar click the specified sender e; otherwise, <c>false</c>.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Cancelar_Click (object sender, EventArgs e)
		{
			var alerDialog = (new AlertDialog.Builder (this)).Create ();
			alerDialog.SetTitle (Resource.String.txt_alertas);
			alerDialog.SetMessage (GetString(APPDroid.Framework.Resource.String.txt_desea_cancelar));
			alerDialog.SetButton ("Si", delegate {
				var intent = new Intent(this, typeof(ActPacienteExmpan));
				StartActivity(intent);
				Finish();
			});
			alerDialog.SetButton2 ("No", (s, ev) =>  {
				var dialog = s as AlertDialog;
				if (dialog != null) {
					dialog.Dismiss ();	
				}
			});
			alerDialog.Show ();
		}
		#endregion

		#region Click accept
		/// <summary>
		/// Aceptars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Aceptar_Click (object sender, EventArgs e)
		{
			//Validar si requiere diluyente
			if(ActivitiesContext.Context.medicamentDose.RequireDiluent){
				ActivitiesContext.Context.IsScanning = true;
				ActivitiesContext.Context.AdministarMedicament = true;
				var windDiluent = new Intent(this, typeof(ActDiluyente));
				StartActivity(windDiluent);	
				return;
			}
			var solicitaFirma = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault ().Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("SF"));

			ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;

			if (solicitaFirma.Value.Equals ("S")) {
				//Siempre pide firma
				ActivitiesContext.Context.IsScanning = true;
				ActivitiesContext.Context.AdministarMedicament = true;
				var windFirma = new Intent (this, typeof(ActConfirmar));
				StartActivity (windFirma);
				Finish ();
			} else if (solicitaFirma.Value.Equals ("N") || solicitaFirma.Value.Equals (String.Empty)) {
				//Solo si es por primera ves
				if (ActivitiesContext.Context.FirstPrimary) {
					ActivitiesContext.Context.IsScanning = true;
					ActivitiesContext.Context.AdministarMedicament = true;
					var windFirma = new Intent (this, typeof(ActConfirmar));
					StartActivity (windFirma);
					Finish ();
				} else {
					AdministerHandler ();	
				}
			} else if (solicitaFirma.Value.Equals ("J")) {
				//Nunca pide firma
				AdministerHandler ();	
			}

		}
		#endregion

		#region Click seek medication
		/// <summary>
		/// Buscars the medicamento click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void BuscarMedicamento_Click (object sender, EventArgs e)
		{
			progressDialog.Show();

			if(String.IsNullOrEmpty(BarCode.Text)){
				Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();

				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}

				return;	
			}
			var medicament = new List<Medicament> ();
			medicament.Add (ActivitiesContext.Context.medicamentDose);
			ValidateBarCode (BarCode.Text, medicament);
		}
		#endregion

		#region AdministerHandler
		/// <summary>
		/// Administers the handler.
		/// </summary>
		public async void AdministerHandler (){

			progressDialog.Show();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var parameter = new {
					IsScanning = ActivitiesContext.Context.IsScanning,
					Medicament = ActivitiesContext.Context.medicamentDose,
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode, Location = new {Code = ActivitiesContext.Context.Patient.Location.Code}},
					User = new { Code = ContextApp.Instance.User.Code, Password = ContextApp.Instance.User.Password, AdministrativeStructure = ContextApp.Instance.SelectedEAD.Code, MainSpeciality = new { Code = ContextApp.Instance.SelectedSpeciality.Code } },
					Diluent = new {}
				};

				var jsonRequest = JsonConvert.SerializeObject (parameter, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync (GetString (APPDroid.Framework.Resource.String.txt_administrar), new StringContent (jsonRequest, System.Text.Encoding.UTF8, GetString (APPDroid.Framework.Resource.String.txt_aplicacion_json)));
					await AdministrarResponse (result);
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

		#region AdministrarResponse
		/// <summary>
		/// Administrars the response.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task AdministrarResponse (HttpResponseMessage result){
			try{
				if (result.IsSuccessStatusCode) {

					ActivitiesContext.Context.AdministerData[ActivitiesContext.Context.PositionOnAdminister] = (int)AdministerType.AdminBarCode;
					var intent = new Intent(this, typeof(ActPacienteExmpan));
					StartActivity(intent);
					Finish();
				} else {
					
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, string.Format (GetString (APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
			finally
			{
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}

		}
		#endregion

		#region Validate Barcode
		/// <summary>
		/// Validates the bar code.
		/// </summary>
		/// <param name="barCode">Bar code.</param>
		/// <param name="medicaments">Medicaments.</param>
		public async void ValidateBarCode (string barCode, List<Medicament> medicaments) {

			progressDialog.Show();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new
				{
					Code = barCode,
					Medicaments = medicaments,
					Startdate = ActivitiesContext.Context.InitialDate,
					Finaldate = ActivitiesContext.Context.FinalDate

				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("ValidateArticle", new StringContent (jsonRequest, System.Text.Encoding.UTF8, "application/json"));
					await ResultValidate (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show ();
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
				}
			}
		}
		#endregion

		#region Response barcode
		/// <summary>
		/// Results the validate.
		/// </summary>
		/// <returns>The validate.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResultValidate (HttpResponseMessage result){
			try {
				if (result.IsSuccessStatusCode) {
					
					ActivitiesContext.Context.medicamentDose.BarCode.Add (BarCode.Text);

					if (Decimal.TryParse (resta.Text, out rango1Value)){
						valor = rango1Value - ActivitiesContext.Context.medicamentDose.ConcentrationFactor;
						if(valor <= 0){
							valor = 0;
							BarCode.Enabled = false;
							buscarMedicamento.Enabled = false;
							Aceptar.Enabled = true;
						}
					}
					resta.Text = valor.ToString ("F");

					if (Decimal.TryParse (cantidadAdmin.Text, out rango2Value)){
						if(valor > 0){
							valor2 = rango2Value + ActivitiesContext.Context.medicamentDose.ConcentrationFactor;
						}else{
							valor2 = ActivitiesContext.Context.medicamentDose.Dose.Quantity;
						}
					}
					cantidadAdmin.Text = valor2.ToString ("F");
				}else{
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, string.Format (GetString (APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			} finally {
				BarCode.Text = String.Empty;
				BarCode.RequestFocus ();
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}
				
		}
		#endregion

		#region OnBackPressed
		/// <Docs>Called when the activity has detected the user's press of the back
		///  key.</Docs>
		/// <para tool="javadoc-to-mdoc">Called when the activity has detected the user's press of the back
		///  key. The default implementation simply finishes the current activity,
		///  but you can override this to do whatever you want.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 5"></since>
		/// <summary>
		/// Raises the back pressed event.
		/// </summary>
		public override void OnBackPressed ()
		{
			var WindPatint = new Intent (this, typeof(ActPacienteExmpan));	
			StartActivity (WindPatint);	
			Finish ();
		}
		#endregion

	}
}


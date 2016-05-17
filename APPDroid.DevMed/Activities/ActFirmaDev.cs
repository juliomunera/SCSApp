
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages;
using SCSAPP.Framework.Context;
using APPDroid.Framework.Context;
using Android.OS;
using APPDroid.Framework.Services;
using System.Net.Http;
using Newtonsoft.Json;
using SCSAPP.Services.Messages.Entities;
using APPDroid.Framework.Helpers;
using System.Text;

namespace APPDroid.DevMed.Activities
{
	[Activity (Label = "@string/txt_devoluciones", Icon = "@drawable/ic_devmed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]  			
	public class ActFirmaDev : Activity
	{
		#region Variables and Controls
		Button btnconfirmar;
		Button btnCancelar;
		ProgressDialog progressDialog;
		bool _clickOnLoginBtn = false;
		EditText usuairo, password;
		#endregion

		#region constructor method
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.layout_dev_firma);

			btnconfirmar = FindViewById<Button> (Resource.Id.btnConfirar);
			btnconfirmar.Click += Btnconfirmar_Click;
			btnCancelar = FindViewById<Button> (Resource.Id.brnCancelarConfirmarG);
			btnCancelar.Click += BtnCancelar_Click;

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			usuairo = FindViewById<EditText> (Resource.Id.editUser);
			password = FindViewById<EditText> (Resource.Id.editPassword);

			usuairo.Text = ContextApp.Instance.User.Code;


		}
		#endregion

		#region click cancel
		/// <summary>
		/// Buttons the cancelar click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void BtnCancelar_Click (object sender, EventArgs e)
		{
			var alerDialog = (new AlertDialog.Builder (this)).Create ();
			alerDialog.SetTitle (Resource.String.txt_alertas);
			alerDialog.SetMessage (GetString(APPDroid.Framework.Resource.String.txt_desea_cancelar)); 
			alerDialog.SetButton ("Si", delegate {
				Finish ();
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

		#region click confirm
		/// <summary>
		/// Btnconfirmars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void Btnconfirmar_Click (object sender, EventArgs e)
		{
			if (_clickOnLoginBtn) return;
				_clickOnLoginBtn = true;

			progressDialog.Show ();

			try {
				using (var httpClient = WebServices.GetBaseHttpClient (URIType.Authentication)) {
					var request = new
					{
						User = usuairo.Text,
						Password = Convert.ToBase64String(Encoding.ASCII.GetBytes(password.Text))
					};

					var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
					try{
						var result = await httpClient.PostAsync (GetString(APPDroid.Framework.Resource.String.txt_login_app), new StringContent (jsonRequest, Encoding.UTF8, GetString(APPDroid.Framework.Resource.String.txt_aplicacion_json)));
						await ValidateLoginResponse (result);
					}catch (Exception ex) {
						_clickOnLoginBtn = false;
						Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}
				}
			}
			catch (Exception ex) {
				Toast.MakeText (this, string.Format(GetString (APPDroid.Framework.Resource.String.txt_error),ex.Message), ToastLength.Long).Show ();
				_clickOnLoginBtn = false;
			}
		}
		#endregion

		#region Validate Login Response
		/// <summary>
		/// Validates the login response.
		/// </summary>
		/// <returns>The login response.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task ValidateLoginResponse (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					LoginResponse responseInstance = JsonConvert.DeserializeObject<LoginResponse> (responseJsonText);
					if (responseInstance != null) {
						//Guardar Devolucion en Sibuzopl
						guardarSibuzpl ();
					} else {
						Toast.MakeText (this, APPDroid.Framework.Resource.String.txt_des_serializada, ToastLength.Long).Show ();
					}
				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
					Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();
				}
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();	
			} finally {
				_clickOnLoginBtn = false;
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region GuardarSibuzpl
		/// <summary>
		/// Guardars the sibuzpl.
		/// </summary>
		async void guardarSibuzpl ()
		{
			progressDialog.Show();
			
			//var telephonyManager = (TelephonyManager)GetSystemService (Context.TelephonyService);
			//string deviceId = telephonyManager.DeviceId;

			string ContextImei = DataBaseManager.GetContexts (DataBaseManager.IDContextType.imei);

			var detailSave = new List<DetailSaveOrders> ();

			var progCn = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmodevmed"));
			var CD = progCn.Variables.FirstOrDefault (v => v.Code.Equals ("CD"));
			var IT = progCn.Variables.FirstOrDefault (v => v.Code.Equals ("IT"));

			for (int i = 0; i < ActivitiesContext.Context.listmedicament.Count; i++) {
				var det = new List<DetLotCumReg> ();

				if (CD.Value.Equals ("O"))
				{
					if (string.IsNullOrEmpty (ActivitiesContext.Context.listmedicament [i].ResponseCauses)) 
					{
						Toast.MakeText (this, string.Format("El medicamento {0} no tiene causa de no despacho", ActivitiesContext.Context.listmedicament [i].Code ), ToastLength.Long).Show ();
						break;
					}
				}

				if (ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM != null) {
					for (int e = 0; e < ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM.Count; e++) {

						det.Add (new DetLotCumReg {
							NumberLote = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].NumberLote,
							CodeMedic = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].CodeMedic,
							Invima = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].Invima,
							lotNumber = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].lotNumber,
							DocumentSourceEnter = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].DocumentSourceEnter,
							DocumentEnter = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].DocumentEnter,
							ServicesEnter = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].ServicesEnter
						});                    
					}
				}

				detailSave.Add (new DetailSaveOrders {
					SourceOrder = ActivitiesContext.Context.listmedicament [i].OrderSource,
					DocumentOrder = ActivitiesContext.Context.listmedicament [i].OrderDocument,
					LineOrder = ActivitiesContext.Context.listmedicament [i].OrderLine,
					LineOrderCode = ActivitiesContext.Context.listmedicament [i].Code,
					TypeArticle = "C",
					QualityArticle = ActivitiesContext.Context.listmedicament [i].AmountAlistada,
					NotBecauseOffice = ActivitiesContext.Context.listmedicament [i].ResponseCauses ?? String.Empty,
					QuantityPlayed = ActivitiesContext.Context.listmedicament [i].PendingAmount,
					ListItemDetailCum = det

				});
			}
			//David Ciro
			//Soluciòn al control 401726, se lee el usuario de la caja de texto y no del que se logueo
			//Parametro 	CodeUserDataBase=usuairo.Text,
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
		
				var request = new
				{
					CodeEad = ContextApp.Instance.SelectedEAD.Code,
					UserCode = ContextApp.Instance.User.Code,
					CodeUserDataBase=usuairo.Text,
					CodeProgram = "cmodevmed",
					ValueParameter = IT.Value,
					PeriodYear = ActivitiesContext.Context.year,
					PeriodMonth = ActivitiesContext.Context.month,
					SourceCar = ActivitiesContext.Context.PatientSelecte.Code,
					WarehouseCode = ActivitiesContext.Context.Almacen.Code,
					EadPatient = ActivitiesContext.Context.EadPatient,
					PatientEpi = ActivitiesContext.Context.devPatients.Episode,
					TypeEpisode = "I",
					NumberHistory = ActivitiesContext.Context.devPatients.History,
					NombrePatient = string.Format("{0} {1} {2}",ActivitiesContext.Context.devPatients.FirstName, 
						ActivitiesContext.Context.devPatients.MiddleName,
						ActivitiesContext.Context.devPatients.LastName),
					NumberIngress = ActivitiesContext.Context.devPatients.EntryNumber,
					SourceIngress = "",
					DocumentIngress = "",
					DataSystem = DateTime.Now,
					CodeMobile = ContextImei,
					ListMedicament = detailSave
				};    


				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
			
				try{
					var result = await httpClient.PostAsync ("SaveOrdersXml", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResulGuardar (result);
				}catch (Exception ex) {
					_clickOnLoginBtn = false;
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

		#region Resul save
		/// <summary>
		/// Resuls the guardar.
		/// </summary>
		/// <returns>The guardar.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResulGuardar (HttpResponseMessage result){
			try{
				if (result.IsSuccessStatusCode) {
					string responseJsonText = await result.Content.ReadAsStringAsync ();
					bool respuestaGuardar = JsonConvert.DeserializeObject<bool>(responseJsonText);

					System.Threading.Thread.Sleep (10000);

					if(respuestaGuardar)
					{
						Toast.MakeText (this, "El proceso de guardado se realizó correctamente.", ToastLength.Long).Show ();
						var intent = new Intent(this, typeof(ActDevmMain));
						StartActivity(intent);
						Finish();
					}
				}else{
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
					Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();
				}
			} 
			catch (Exception ex) {
				Toast.MakeText (this, String.Format (GetString(APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long).Show ();
			} 
			finally{
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}
		}
		#endregion

	}

}


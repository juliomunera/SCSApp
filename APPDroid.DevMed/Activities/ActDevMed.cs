
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages;
using APPDroid.DevMed.Adapters;
using SCSAPP.Framework.Context;
using APPDroid.Framework.Services;
using APPDroid.Framework.Context;
using System.Net.Http;
using Newtonsoft.Json;
using SCSAPP.Services.Messages.Entities;
using Android.Views.InputMethods;

namespace APPDroid.DevMed.Activities
{
	[Activity (Label = "@string/txt_devoluciones", Icon = "@drawable/ic_devmed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]  				
	public class ActDevMed : Activity
	{
		#region Variables and Controls
		ExpandableListView lisView;
		Button buscar;
		Button guardar;
		EditText barcodeDev;
		ListView listaMedicamento;
		List<Medicament> listMedicament;
		Medicament medicament;
		bool codeRead = false;
		string Code;
		Program progCn;
		MasterItem CD;
		ProgressDialog progressDialog;
		#endregion

		#region constructor method
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.layout_dev_medicament);

			barcodeDev = FindViewById<EditText> (Resource.Id.editCodMe);

			lisView = FindViewById<ExpandableListView> (Resource.Id.myExpandableListview);
			lisView.SetAdapter(new AdapterPatients(this));

			listaMedicamento = FindViewById<ListView> (Resource.Id.listViewMedicine);

			buscar = FindViewById<Button> (Resource.Id.buscarMedicamento);
			buscar.Click += Buscar_Click;
			guardar = FindViewById<Button> (Resource.Id.imageViewGuardar);
			guardar.Click += Guardar_Click;

			InputMethodManager imms = (InputMethodManager)GetSystemService (Context.InputMethodService);
			imms.HideSoftInputFromInputMethod (barcodeDev.WindowToken, 0);
			// try hide the keyboard 
			Window.SetSoftInputMode (SoftInput.StateHidden);

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			progCn = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmodevmed"));
			CD = progCn.Variables.FirstOrDefault (v => v.Code.Equals ("CD"));
			var DC = progCn.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));

			barcodeDev.Click += delegate {
				if(DC.Value.Equals ("N")){
					imms.HideSoftInputFromWindow(barcodeDev.WindowToken, 0);
				}
			};

			listMedicament = new List<Medicament> ();

			barcodeDev.AfterTextChanged += (sender, e) => {
				if (codeRead)
					return;	

				var Editor = sender as EditText;
				if (Editor != null && !String.IsNullOrEmpty (Editor.Text)) {
					if (Editor.Text.Trim ().Length > 1 && Editor.Text.Substring (Editor.Text.Length - 1, 1).Equals ("\n")) {
						codeRead = false;

						Code = barcodeDev.Text;
						Code = Code.Replace ("\n", String.Empty);

						if (Code.Substring (Code.Length - 1, 1).Equals ("\n"))
							Code = Code.Substring (0, Code.Length - 1);

						if (string.IsNullOrEmpty (barcodeDev.Text)) {
							Toast.MakeText (this, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long).Show ();
						} else {
							ValidateBarCode (Code, "escaneado");
						}
					}
				}
			};
		}
		#endregion

		#region Click save
		/// <summary>
		/// Guardars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Guardar_Click (object sender, EventArgs e)
		{
			bool valI = false;
			if (ActivitiesContext.Context.listmedicament == null || ActivitiesContext.Context.listmedicament.Count <= 0) 
			{
				Toast.MakeText (this, "No se tiene medicamentos a devolver", ToastLength.Long).Show ();
				return;
			} 
			else 
			{
				for (int i = 0; i < ActivitiesContext.Context.listmedicament.Count; i++) 
				{
					if (CD.Value.Equals ("O")) 
					{
						if (string.IsNullOrEmpty (ActivitiesContext.Context.listmedicament [i].ResponseCauses))
						{
							Toast.MakeText (this, string.Format("El medicamento {0} no tiene causa de devolición", ActivitiesContext.Context.listmedicament [i].Code ), ToastLength.Long).Show ();
							valI = true;
							break;
						}
					}
				}
					
				if (!valI) 
				{
					var ventanaPrincipal = new Intent (this, typeof(ActFirmaDev));	
					StartActivity (ventanaPrincipal);
				}
			}
		}
		#endregion

		#region Click search
		/// <summary>
		/// Buscars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Buscar_Click (object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty (barcodeDev.Text)) {
				Toast.MakeText (this, "El código de barra es un campo requerido", ToastLength.Long).Show ();
				return;
			}
			ValidateBarCode (barcodeDev.Text, "Digitado");
		}
		#endregion

		#region ValidateBarCode
		/// <summary>
		/// Validates the bar code.
		/// </summary>
		/// <param name="barCode">Bar code.</param>
		/// <param name="tipo">Tipo.</param>
		public async void ValidateBarCode (string barCode, string tipo)
		{
			progressDialog.Show ();

			var prog = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmodevmed"));

			var LB = prog.Variables.FirstOrDefault (v => v.Code.Equals ("LB"));
			var CN = prog.Variables.FirstOrDefault (v => v.Code.Equals ("CN"));
			var CS = prog.Variables.FirstOrDefault (v => v.Code.Equals ("CS"));

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new
				{
					Code = barCode,
					Type = tipo,
					AttachedConcept = ActivitiesContext.Context.PatientSelecte.AttachedConcepto,
					ConceptRequireBalance = ActivitiesContext.Context.PatientSelecte.UpdateBalance,
					ConceptUseBatch = ActivitiesContext.Context.PatientSelecte.UseBatch,
					ConceptUseCum = ActivitiesContext.Context.PatientSelecte.UseCumCode,
					ConceptRequireInvima = ActivitiesContext.Context.PatientSelecte.UseInvima,
					CN = CN.Value,
					Warehouse = ActivitiesContext.Context.Almacen.Code,
					CanUseCum = ActivitiesContext.Context.indicaCum,
					Year = ActivitiesContext.Context.year,
					Month = ActivitiesContext.Context.month,
					ExpiringHours = ActivitiesContext.Context.NumberHours ?? 0,
					IsRefund = "S", 
					ActivitiesContext.Context.patienFund.History,
					ActivitiesContext.Context.patienFund.Episode,
					ActivitiesContext.Context.patienFund.EntryNumber,
					ActivitiesContext.Context.patienFund.PatientEad,
					LB = LB.Value,
					Clinic = "N",
					CS = CS.Value
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("ValidateArticle", new StringContent (jsonRequest, System.Text.Encoding.UTF8, "application/json"));
					await ResultValidate (result);
				} catch (Exception ex) {
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

		#region ResultValidate
		/// <summary>
		/// Results the validate.
		/// </summary>
		/// <returns>The validate.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResultValidate (HttpResponseMessage result)
		{
			try {
				if (result.IsSuccessStatusCode) {
					string responseJsonText = await result.Content.ReadAsStringAsync ();
					medicament = JsonConvert.DeserializeObject<Medicament> (responseJsonText);

					bool indicadorIncremento = true;

					if (ActivitiesContext.Context.listmedicament == null) {
						listMedicament.Add (medicament);
						ActivitiesContext.Context.listmedicament = listMedicament;
					} else {

						if (!ActivitiesContext.Context.listmedicament.Exists (k => k.Code.Equals (medicament.Code))) {
							ActivitiesContext.Context.listmedicament.Add (medicament);
						} else {
							incrementar ();
							indicadorIncremento = false;
						}
					} 

					if(indicadorIncremento){
						//Pregunto si esta en la lista.
						for (int i = 0; i < ActivitiesContext.Context.listmedicament.Count; i++) {
							if(ActivitiesContext.Context.listmedicament [i].Code == medicament.Code)
							{
								if (string.IsNullOrEmpty (ActivitiesContext.Context.listmedicament [i].ResponseCauses)) 
								{
									if(CD.Value.Equals ("N"))
									{
										incrementar ();
									}
									else if(CD.Value.Equals ("S") || CD.Value.Equals ("O"))
									{
										var alertDialogCausaNo = (new AlertDialog.Builder (this)).Create ();
										var inflaterNoAdmi = LayoutInflater.Inflate (APPDroid.Framework.Resource.Layout.app_select_master, null);
										ListView ListView = inflaterNoAdmi.FindViewById<ListView> (APPDroid.Framework.Resource.Id.listViewEad);

										ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM ?? new List<DetLotCumReg> ();

										ActivitiesContext.Context.listmedicament [i].AmountAlistada++;

										ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM.Add (new DetLotCumReg {
											NumberLote = medicament.BatchNumber ?? String.Empty,  
											CodeMedic = medicament.CumCode ?? String.Empty,
											Invima = medicament.InvimaCode ?? String.Empty,
											lotNumber = 1,
											DocumentSourceEnter = medicament.OrderSource ?? String.Empty,
											DocumentEnter = medicament.OrderDocument,
											ServicesEnter = medicament.Warehouse ?? String.Empty,
											Maturing = medicament.ExpiringSoon,
											ExpirationDate = medicament.ExpirationDate
										});

										listaMedicamento.Adapter = new AdapterMedDev (this, ActivitiesContext.Context.listmedicament, listaMedicamento);
										var listViewCausa = new AdapterCausaDev (this, ActivitiesContext.Context.patienFund.NonDispatchCauses, alertDialogCausaNo, i, listaMedicamento, medicament, false);
										alertDialogCausaNo.SetTitle ("Seleccionar Causa de Devolución");
										ListView.Adapter = listViewCausa;
										alertDialogCausaNo.SetView (inflaterNoAdmi);
										alertDialogCausaNo.Show ();
									}

									break;
								}
							}
						}
					}

				} else {
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
					Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();
				}
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (GetString (APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long).Show ();
			} finally {
				barcodeDev.Text = string.Empty;
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region increase
		/// <summary>
		/// Incrementar this instance.
		/// </summary>
		void incrementar ()
		{
			for (int i = 0; i < ActivitiesContext.Context.listmedicament.Count; i++) {

				if (ActivitiesContext.Context.listmedicament [i].Code.Equals (medicament.Code)) {
					if (ActivitiesContext.Context.listmedicament [i].AmountAlistada == ActivitiesContext.Context.listmedicament [i].NumberOfUnits) {
						Toast.MakeText (this, "La cantidad alistada no puede superar la cantidad solicitada", ToastLength.Long).Show ();
						break;
					} else {

						if (!ActivitiesContext.Context.listmedicament [i].ExpiringSoon)
							ActivitiesContext.Context.listmedicament [i].ExpiringSoon = medicament.ExpiringSoon;

						ActivitiesContext.Context.listmedicament [i].AmountAlistada++;

						if (ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM == null)
							ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM = new List<DetLotCumReg> ();

						ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM.Add (new DetLotCumReg {
							NumberLote = medicament.BatchNumber ?? String.Empty,  
							CodeMedic = medicament.CumCode ?? String.Empty,
							Invima = medicament.InvimaCode ?? String.Empty,
							lotNumber = 1,
							DocumentSourceEnter = medicament.OrderSource ?? String.Empty,
							DocumentEnter = medicament.OrderDocument,
							ServicesEnter = medicament.Warehouse ?? String.Empty,
							Maturing = medicament.ExpiringSoon
						});

						break;
					}
				}
			}

			listaMedicamento.Adapter = new AdapterMedDev (this, ActivitiesContext.Context.listmedicament, listaMedicamento);
		}
		#endregion

	}

}


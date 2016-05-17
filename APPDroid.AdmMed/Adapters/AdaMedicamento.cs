
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using APPDroid.AdmMed.Activities;
using SCSAPP.Services.Messages;
using SCSAPP.Framework.Context;
using Android.Graphics;
using System.Net.Http;
using Newtonsoft.Json;
using APPDroid.Framework.Services;
using APPDroid.Framework.Context;

namespace APPDroid.AdmMed.Adapters
{
	public class AdaMedicamento : BaseAdapter{

		#region Variables and Controls
		readonly Activity context;
		TextView MedNombre;
		TextView MedFecha;
		TextView BriefDosage;
		ImageButton overflowButtonEdit;
		DateTime initialDateValidate;
		DateTime finalDateValidate;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.AdmMed.Adapters.AdaMedicamento"/> class.
		/// </summary>
		/// <param name="c">C.</param>
		public AdaMedicamento(Activity c){
			context = c;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count{
			get { 
				return ActivitiesContext.Context.Patient.Medicaments == null ? 0 : ActivitiesContext.Context.Patient.Medicaments.Count;
			}
		}
		#endregion

		#region GetItem
		/// <Docs>Position of the item whose data we want within the adapter's 
		///  data set.</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Get the data item associated with the specified position in the data set.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <param name="position">Position.</param>
		public override Java.Lang.Object GetItem(int position){return null;}
		#endregion

		#region GetItemId
		/// <Docs>The position of the item within the adapter's data set whose row id we want.</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Get the row id associated with the specified position in the list.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Gets the item identifier.
		/// </summary>
		/// <param name="position">Position.</param>
		public override long GetItemId(int position){return position;}
		#endregion

		#region GetView 
		/// <Docs>The position of the item within the adapter's data set of the item whose view
		///  we want.</Docs>
		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <returns>The view.</returns>
		/// <param name="position">Position.</param>
		/// <param name="convertView">Convert view.</param>
		/// <param name="parent">Parent.</param>
		public override View GetView(int position, View convertView, ViewGroup parent){
			convertView = context.LayoutInflater.Inflate (Resource.Layout.item_medicamentos, parent, false);
			var colletpenFecha = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETPEN"));
			var colletprePred = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETPRE"));

			//Desarrollo pendiente..... VARIABLE REPROGRAMADA.... 
			var colletrep = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETREP"));

			//Desarrollo pendiente..... VARIABLE REFORMULADO.... 
			var colletref = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETREF"));

			MasterItem varCA = null;

			var ProgChemedica = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.Equals ("cmoadmmed"));
			if (ProgChemedica != null) {
				varCA = ProgChemedica.Variables.FirstOrDefault (v => v.Code.ToUpper ().Equals ("CA")); 
				if (varCA == null) {
					Toast toast = Toast.MakeText(context, "La variable CA es Null", ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} else {
				Toast toast = Toast.MakeText(context, "El programa CHEMEDICA es Null", ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();

				return convertView;
			}

			MedNombre = convertView.FindViewById<TextView> (Resource.Id.projectName);
			MedFecha = convertView.FindViewById<TextView> (Resource.Id.companyName);
			BriefDosage = convertView.FindViewById<TextView> (Resource.Id.txtPosologia);

			ImageButton overflowButton = convertView.FindViewById<ImageButton> (Resource.Id.overflowButton);
			overflowButtonEdit = convertView.FindViewById<ImageButton> (Resource.Id.overflowButtonEdit);
			overflowButtonEdit.Visibility = ViewStates.Gone;

			MedNombre.Text = ActivitiesContext.Context.Patient.Medicaments[position].Description;
			if (ActivitiesContext.Context.Patient.Medicaments [position].Dose.IsLast){
				MedNombre.Text = String.Format ("{0} - {1}", ActivitiesContext.Context.Patient.Medicaments[position].Description, "Última dosis");
			}
			
			MedFecha.Text = string.Format ("{0:dd/MM/yyyy HH:mm}", ActivitiesContext.Context.Patient.Medicaments [position].Dose.ScheduledDate);

			decimal?  redondear = ActivitiesContext.Context.Patient.Medicaments [position].OrderDuration == null ? ActivitiesContext.Context.Patient.Medicaments [position].OrderDuration : Math.Round (ActivitiesContext.Context.Patient.Medicaments [position].OrderDuration ?? 0);
			
			BriefDosage.Text = string.Format ("{0}{1} {2} {3} Por {4}H", Math.Round (ActivitiesContext.Context.Patient.Medicaments [position].Dose.Quantity, 2),
																	 	 ActivitiesContext.Context.Patient.Medicaments [position].Unit, 
																	 	 ActivitiesContext.Context.Patient.Medicaments [position].WayDescription,
																	 	 ActivitiesContext.Context.Patient.Medicaments [position].OrdeFrequency, 
																	  	 redondear);
			//Administrar sobre el boton manual 
			overflowButton.Click += (sender, e) => {

				var ActVali = context as ActPacienteExmpan;

				initialDateValidate = DateTime.Now;
				finalDateValidate = DateTime.Now;

				string iniDateValidate1 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour1);
				int rango1Value1 = 0;

				string finDateValidate2 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour2);
				int rango1Value2 = 0;

				string finDateValidate3 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour3);

				if(ActivitiesContext.Context.Patient.OrderOutputIndicator.Equals("S"))
				{
					//True hora1 hora2 orden de salida confirmada
					if (!String.IsNullOrEmpty(iniDateValidate1)) {
						if (int.TryParse(iniDateValidate1, out rango1Value1)) {
							initialDateValidate = DateTime.Now.AddHours (rango1Value1 * -1);
						}
					}

					if (!String.IsNullOrEmpty(finDateValidate2)) {
						if (int.TryParse(finDateValidate2, out rango1Value2)) {
							finalDateValidate = DateTime.Now.AddHours (rango1Value2);
						}
					}

					if(ActVali.Between(ActivitiesContext.Context.Patient.Medicaments [position].Dose.ScheduledDate,initialDateValidate,finalDateValidate))
						ValidateStateMedicamentAdap (ActivitiesContext.Context.Patient.Medicaments [position], position);
					else
					{
						Toast toast = Toast.MakeText(context, "La hora de administración no corresponde para este medicamento", ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						//Actulizar
						if (ActVali != null) {
							ActVali.LoadPatientHistory();
						}
					}
				}
				else
				{
					//False hora1 hora3 sin orden de salida 
					if (!String.IsNullOrEmpty(iniDateValidate1)) {
						if (int.TryParse(iniDateValidate1, out rango1Value1)) {
							initialDateValidate = DateTime.Now.AddHours (rango1Value1 * -1);
						}
					}

					if (!String.IsNullOrEmpty(finDateValidate3)) {
						if (int.TryParse(finDateValidate3, out rango1Value2)) {
							finalDateValidate = DateTime.Now.AddHours (rango1Value2);
						}
					}

					if(ActVali.Between(ActivitiesContext.Context.Patient.Medicaments [position].Dose.ScheduledDate,initialDateValidate,finalDateValidate))
						ValidateStateMedicamentAdap (ActivitiesContext.Context.Patient.Medicaments [position], position);
					else
					{
						Toast toast = Toast.MakeText(context, "La hora de administración no corresponde para este medicamento", ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						//Actulizar
						if (ActVali != null) {
							ActVali.LoadPatientHistory();
						}
					}
				}

			};

			var indicador = 0;

			if (varCA.Value.ToUpper ().Equals ("S")) {
				//PreDespacho mostrar botón 
				if (!ActivitiesContext.Context.Patient.Medicaments [position].IsPreDispatched) {
					overflowButton.Visibility = ViewStates.Gone;
				}
			} 

			//REFORMULADO
			if(ActivitiesContext.Context.Patient.Medicaments[position].IsReformulated){
				if (!String.IsNullOrEmpty (colletref.Value)) {
					MedNombre.SetTextColor (Color.ParseColor (colletref.Value));
					MedFecha.SetTextColor (Color.ParseColor (colletref.Value));
					BriefDosage.SetTextColor (Color.ParseColor (colletref.Value));
					indicador = 1;
				}
			}

			//FECHA COLOR 
			if (ActivitiesContext.Context.Patient.Medicaments [position].Dose.ScheduledDate < DateTime.Now && indicador != 1) 
			{
				if (!String.IsNullOrEmpty (colletpenFecha.Value)) 
				{
					MedNombre.SetTextColor (Color.ParseColor (colletpenFecha.Value));
					MedFecha.SetTextColor (Color.ParseColor (colletpenFecha.Value));
					BriefDosage.SetTextColor (Color.ParseColor (colletpenFecha.Value));
					indicador = 1;
				}						
			}

			//REPROGRAMADO
			if(ActivitiesContext.Context.Patient.Medicaments[position].Reprogrammed && indicador != 1)
			{
				if (!String.IsNullOrEmpty (colletrep.Value)) {
					MedNombre.SetTextColor (Color.ParseColor (colletrep.Value));
					MedFecha.SetTextColor (Color.ParseColor (colletrep.Value));
					BriefDosage.SetTextColor (Color.ParseColor (colletrep.Value));
					indicador = 1;
				}
			}

			if (varCA.Value.ToUpper ().Equals ("S"))
			{
				//COLOR DEL PREDESPACHO
				if (!ActivitiesContext.Context.Patient.Medicaments [position].IsPreDispatched && indicador != 1) 
				{
					if (!String.IsNullOrEmpty (colletprePred.Value)) {
						MedNombre.SetTextColor (Color.ParseColor (colletprePred.Value));
						MedFecha.SetTextColor (Color.ParseColor (colletprePred.Value));
						BriefDosage.SetTextColor (Color.ParseColor (colletprePred.Value));
						indicador = 1;
					}
				}	
			}

			SetImageToButton (overflowButton, position);

			convertView.Click += (sender, e) => {
				
				if (ActivitiesContext.Context.Patient.Medicaments [position].IsReformulated) {
					(context as ActPacienteExmpan).UpdateReformulado (position);
				}

				var alertDialog = (new AlertDialog.Builder (context)).Create ();
				var inflater = context.LayoutInflater.Inflate (Resource.Layout.app_posologia, null);
				alertDialog.SetTitle (ActivitiesContext.Context.Patient.Medicaments [position].Description);

				inflater.FindViewById<TextView> (Resource.Id.txtFecha).Text = string.Format ("{0:dd/MM/yyyy HH:mm}", ActivitiesContext.Context.Patient.Medicaments [position].Dose.ScheduledDate);
				inflater.FindViewById<TextView> (Resource.Id.txtPosologia).Text = string.Format ("{0} Observaciones: {1}", ActivitiesContext.Context.Patient.Medicaments [position].Posology, ActivitiesContext.Context.Patient.Medicaments [position].Observation);

				alertDialog.SetView (inflater);
				alertDialog.Show ();

			};

			return convertView;
		}
		#endregion

		#region SetImageToButton
		/// <summary>
		/// Sets the image to button.
		/// </summary>
		/// <param name="btn">Button.</param>
		/// <param name="Pos">Position.</param>
		void SetImageToButton (ImageButton btn, int Pos){
			btn.SetImageResource (Resource.Drawable.ic_action_record);	

			var Value = ActivitiesContext.Context.AdministerData [Pos];

			if (Value != (int) AdministerType.None) {
				if (Value == (int)AdministerType.NoAdmin) {
					btn.SetImageResource (Resource.Drawable.ic_action_cancel);
					btn.Enabled = false;
				} else if (Value == (int)AdministerType.AdminManual || Value == (int)AdministerType.AdminBarCode) {
					btn.SetImageResource (Resource.Drawable.ic_action_tick);	
					btn.Enabled = false;
				}
			}
		}
		#endregion

		#region Main dialogue
		/// <summary>
		/// Dialogs the principal.
		/// </summary>
		/// <param name="indicador">If set to <c>true</c> indicador.</param>
		/// <param name="Position">Position.</param>
		void DialogPrincipal(bool indicador, int Position){
			var alertDialog = (new AlertDialog.Builder (context)).Create ();
			var inflaterAdministrar = context.LayoutInflater.Inflate (Resource.Layout.app_accion_medicamentos, null);
			if (indicador) {
				//No puede administrar manual 
				inflaterAdministrar.FindViewById<Button>(Resource.Id.btnAdministrar).Visibility = ViewStates.Gone;
			} 
			alertDialog.SetView(inflaterAdministrar);
			alertDialog.Show();

			Button btnNoAdministrar = inflaterAdministrar.FindViewById<Button> (Resource.Id.btnNoAdministrar);
			btnNoAdministrar.Click += (sender, e) => {
				alertDialog.Dismiss ();
				ActivitiesContext.Context.PositionOnAdminister = Position;
				LoadCausas ();
			};

			Button btnAdministrar = inflaterAdministrar.FindViewById<Button> (Resource.Id.btnAdministrar);
			btnAdministrar.Click += (sender, e) => {
				alertDialog.Dismiss ();
				ActivitiesContext.Context.PositionOnAdminister = Position;

				//Valida diluyente
				if (ActivitiesContext.Context.Patient.Medicaments [Position].RequireDiluent) {
					ActivitiesContext.Context.IsScanning = false;
					ActivitiesContext.Context.AdministarMedicament = true;
					var windDiluent = new Intent (context, typeof(ActDiluyente));
					context.StartActivity (windDiluent);	
				} else {
					var solicitaFirma = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault ().Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("SF"));
					if (solicitaFirma.Value.Equals ("S")) {
						//Siempre pide firma
						ActivitiesContext.Context.IsScanning = false;
						ActivitiesContext.Context.AdministarMedicament = true;
						ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
						var windFirma = new Intent (context, typeof(ActConfirmar));
						context.StartActivity (windFirma);

					} else if (solicitaFirma.Value.Equals ("N") || solicitaFirma.Value.Equals (String.Empty)) {
						//Solo si es por primera ves
						if (ActivitiesContext.Context.FirstPrimary) {
							ActivitiesContext.Context.IsScanning = false;
							ActivitiesContext.Context.AdministarMedicament = true;
							ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
							var windFirma = new Intent (context, typeof(ActConfirmar));
							context.StartActivity (windFirma);
						} else {
							ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
							AdministerHandler (Position);	
						}
					} else if (solicitaFirma.Value.Equals ("J")) {
						//Nunca pide firma
						ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
						AdministerHandler (Position);
					}
				}
			};
		}
		#endregion

		#region state validate drug
		/// <summary>
		/// Validates the state medicament.
		/// </summary>
		/// <returns><c>true</c>, if state medicament was validated, <c>false</c> otherwise.</returns>
		/// <param name="responseInstance">Response instance.</param>
		/// <param name = "MeditPot"></param>
		public async void ValidateStateMedicamentAdap (Medicament responseInstance, int MeditPot)
		{
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
				var request = new
				{
					Medicament = responseInstance,
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode}
				};

				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("ValidateMedicamentAdminister", new StringContent (jsonRequest, System.Text.Encoding.UTF8, "application/json"));
					await ResulValidateMedicament (result, MeditPot);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(context, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region drug validation result
		/// <summary>
		/// Resuls the validate medicament.
		/// </summary>
		/// <returns>The validate medicament.</returns>
		/// <param name="result">Result.</param>
		/// <param name="position">Position.</param>
		public async System.Threading.Tasks.Task ResulValidateMedicament (HttpResponseMessage result, int position){
			try{
				if (result.IsSuccessStatusCode) 
				{
					var AdministarPrograms = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault ().Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("AM"));
					if (AdministarPrograms == null) {
						DialogPrincipal (false, position);
						return;
					}
					if (AdministarPrograms.Value.Equals ("S")) {
						//Puede administrar manual 
						DialogPrincipal (false, position);
					} else {
						//No puede administrar manual
						DialogPrincipal (true, position);
					}
				}
				else
				{
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(context, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();

					var ActV = context as ActPacienteExmpan;
					if (ActV != null) {
						ActV.LoadPatientHistory();
					}
				}
					
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(context, String.Format (context.GetString(APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

		#region administer medication
		/// <summary>
		/// Administers the handler.
		/// </summary>
		/// <param name="Position">Position.</param>
		public async void AdministerHandler (int Position){
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {

				var request = new
				{
					Medicament = ActivitiesContext.Context.Patient.Medicaments[Position],
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode, Location = new {Code = ActivitiesContext.Context.Patient.Location.Code}},
					User = new {Code = ContextApp.Instance.User.Code, Password = ContextApp.Instance.User.Password, AdministrativeStructure = ContextApp.Instance.SelectedEAD.Code, MainSpeciality = new { Code = ContextApp.Instance.SelectedSpeciality.Code }},
					Diluent = new {}
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("Administer", new StringContent (jsonRequest, System.Text.Encoding.UTF8, "application/json"));
					await AdministerResponse (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(context, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}


			}
		}
		#endregion

		#region response management
		/// <summary>
		/// Administers the response.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task AdministerResponse (HttpResponseMessage result){
			try{
 				if (result.IsSuccessStatusCode) {

					ActivitiesContext.Context.AdministerData[ActivitiesContext.Context.PositionOnAdminister] = (int)AdministerType.AdminManual;
					var intent = new Intent(context, typeof(ActPacienteExmpan));
					context.StartActivity(intent);

				} else {

					var Act = context as ActPacienteExmpan;
					if (Act != null) {
						Act.LoadPatientHistory();
					}

					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(context, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();

				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(context, String.Format ("Error: {0}", ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

		#region LoadCausas
		/// <summary>
		/// Loads the causas.
		/// </summary>
		public async void LoadCausas(){
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)){
				var result = await httpClient.GetAsync ("CausesForNoAdministration");
				await StartResponse (result);
			}
		}
		#endregion

		#region StartResponse
		/// <summary>
		/// Starts the response.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartResponse(HttpResponseMessage response){
			try {
				if(response.IsSuccessStatusCode){
					
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					List<MasterItem> DataService = JsonConvert.DeserializeObject<List<MasterItem>> (responseJsonText);

					if (DataService != null){
						var alertDialogCausaNo = (new AlertDialog.Builder (context)).Create ();
						var inflaterNoAdmi = context.LayoutInflater.Inflate (APPDroid.Framework.Resource.Layout.app_select_master, null);
						ListView ListView = inflaterNoAdmi.FindViewById<ListView>(APPDroid.Framework.Resource.Id.listViewEad);
						var listViewCausa = new AdapCausa(context, DataService, alertDialogCausaNo);
						alertDialogCausaNo.SetTitle ("Seleccione una Causa de no administración");
						ListView.Adapter = listViewCausa;
						alertDialogCausaNo.SetView(inflaterNoAdmi);
						alertDialogCausaNo.Show();
					}
				}else{
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					Toast toast = Toast.MakeText(context, ExceptionMsg, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();

				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(context, String.Format("Error: {0}", ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

	}

}
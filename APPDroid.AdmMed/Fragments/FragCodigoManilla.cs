
using System;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;

using APPDroid.AdmMed.Activities;
using APPDroid.Framework.Services;
using System.Net.Http;
using SCSAPP.Services.Messages;
using Newtonsoft.Json;
using Android.OS;
using SCSAPP.Framework.Context;
using APPDroid.Framework.Context;

namespace APPDroid.AdmMed.Fragments
{
	public class FragCodigoManilla : Android.Support.V4.App.Fragment
	{	
		#region Variables and Controls
		EditText EditCodigo;
		DateTime initialDate;
		DateTime finalDate;
		bool codeRead = false;
		string Code;
		#endregion

		#region NewInstance
		/// <summary>
		/// News the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="initialDate">Initial date.</param>
		/// <param name="finalDate">Final date.</param>
		public static FragCodigoManilla NewInstance(string initialDate, string finalDate){
			var fragmentCodigo = new FragCodigoManilla{ Arguments = new Bundle()};
			fragmentCodigo.Arguments.PutString("initialDate", initialDate);
			fragmentCodigo.Arguments.PutString("finalDate", finalDate);
			return fragmentCodigo;
		}
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.AdmMed.Fragments.FragCodigoManilla"/> class.
		/// </summary>
		public FragCodigoManilla()
		{
			RetainInstance = true;
		} 
		#endregion

		#region OnCreateView
		/// <Docs>The LayoutInflater object that can be used to inflate
		///  any views in the fragment,</Docs>
		/// <param name="savedInstanceState">If non-null, this fragment is being re-constructed
		///  from a previous saved state as given here.</param>
		/// <returns>To be added.</returns>
		/// <summary>
		/// Raises the create view event.
		/// </summary>
		/// <param name="inflater">Inflater.</param>
		/// <param name="container">Container.</param>
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{	
			var view = inflater.Inflate(Resource.Layout.fragment_codigo_b, null);
			EditCodigo = view.FindViewById<EditText> (Resource.Id.editCodigo);

			EditCodigo.AfterTextChanged += (sender, e) => {
				if (codeRead)
					return;

				var Editor = sender as EditText;
				if (Editor != null && !String.IsNullOrEmpty (Editor.Text)) {
					if (Editor.Text.Trim ().Length > 1 && Editor.Text.Substring (Editor.Text.Length - 1, 1).Equals ("\n")) {
						codeRead = false;

						Code = Editor.Text;
						Code = Code.Replace ("\n", String.Empty);

						if (Code.Substring (Code.Length - 1, 1).Equals ("\n"))
							Code = Code.Substring (0, Code.Length - 1);

						if (string.IsNullOrEmpty (Editor.Text)) {
							
							Toast toast = Toast.MakeText(Activity, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();

							Editor.Focusable = true;
							Editor.FocusableInTouchMode = true;
						} else {
							ActionBuscar ();
						}
					}
				}
			};

			var prog = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
			var VCP = prog.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));

			if(VCP.Value.Equals("L")){
				var iniDate = Arguments.GetString ("initialDate");
				var finDate = Arguments.GetString ("finalDate");

				int intIniDate = -1;
				int intFinDate = -1;

				initialDate = DateTime.Now;
				finalDate = DateTime.Now;

				if (!String.IsNullOrEmpty(iniDate)) {
					if (int.TryParse(iniDate, out intIniDate)) {
						initialDate = DateTime.Now.AddHours (intIniDate * -1);
					}
				}

				if (!String.IsNullOrEmpty(finDate)) {
					if (int.TryParse(finDate, out intFinDate)) {
						finalDate = DateTime.Now.AddHours (intFinDate);
					}
				}
			}else{
				var tiedefatr = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("TIEDEFATR"));
				var tiedefade = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (f => f.Code.ToUpper ().Equals ("TIEDEFADE"));

				var iniDate = (tiedefatr == null || tiedefatr.Value == null) ? "1" : tiedefatr.Value;
				var finDate = (tiedefade == null || tiedefade.Value == null) ? "1" : tiedefade.Value;

				int rango1Value = 0;
				int rango2Value = 0;

				if (int.TryParse (iniDate, out rango1Value)) {
					if(rango1Value > 12){
						iniDate = "12";
					}
				} else {
					iniDate = "1";
				}

				if (int.TryParse (finDate, out rango2Value)) {
					if(rango2Value > 12){
						finDate = "12";
					}
				}
				else {
					finDate = "1";
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
			}

			Button BtnCodigoPaciente = view.FindViewById<Button>(Resource.Id.btnBuscarPaciente);
			BtnCodigoPaciente.Click += (sender, e) => {
				Code = EditCodigo.Text;

				ActionBuscar ();
			};

			return view;
		}
		#endregion

		#region action search
		/// <summary>
		/// Actions the buscar.
		/// </summary>
		public void ActionBuscar ()
		{
			if (String.IsNullOrEmpty (Code)) 
			{
				Toast toast = Toast.MakeText(Activity, Resources.GetString(APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();

				EditCodigo.Focusable = true;
				EditCodigo.FocusableInTouchMode = true;
			} 
			else 
			{
				var HistoryText = Code;
				long HistoryLong = -1;

				if (!long.TryParse(HistoryText, out HistoryLong)) 
				{
					Toast toast = Toast.MakeText(Activity, Resources.GetString(APPDroid.Framework.Resource.String.txt_numbre_historia), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();

					return;
				}
				else 
				{
					if (ActivitiesContext.Context != null && ActivitiesContext.Context.ValVarCP != null && ActivitiesContext.Context.ValVarCP.Value.ToUpper().Equals("L")) 
					{
						if (ActivitiesContext.Context.PatientsLoadedInPatientList.FirstOrDefault (P => P.History.Equals (HistoryLong)) == null) {
							Toast toast = Toast.MakeText(Activity, Resources.GetString(APPDroid.Framework.Resource.String.txt_patient_no_list), ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();

							return;	
						}
					}

					LoadPatientHistory();
				}
			}
		}
		#endregion

		#region Load Patient History
		/// <summary>
		/// Loads the patient history.
		/// </summary>
		public async void LoadPatientHistory (){

			ActivitiesContext.Context.InitialDate = initialDate;
			ActivitiesContext.Context.FinalDate = finalDate;

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new { 
					History = Code,
					InitialDate = initialDate,
					FinalDate = finalDate
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("Patient", new StringContent (jsonRequest, System.Text.Encoding.UTF8, GetString(APPDroid.Framework.Resource.String.txt_aplicacion_json)));
					await StartPatient (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(Activity, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region StartPatient
		/// <summary>
		/// Starts the patien list.
		/// </summary>
		/// <returns>The patien list.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatient (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {
					
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					Patient responseInstance = JsonConvert.DeserializeObject<Patient> (responseJsonText);

					if (responseInstance != null) {

						//var objectReferent = responseInstance.Medicaments;

						ActivitiesContext.Context.PatientFull = responseInstance.MedicamentslistFull;

						ActivitiesContext.Context.Patient = responseInstance;

						ActivitiesContext.Context.AdministerData = new int[ActivitiesContext.Context.Patient.Medicaments.Count];

						for (int i = 0; i < ActivitiesContext.Context.AdministerData.Length; i++)
							ActivitiesContext.Context.AdministerData [i] = (int)AdministerType.None;	

						ActivitiesContext.Context.PositionOnAdminister = -1;

						var ventana = new Intent (Activity, typeof(ActPacienteExmpan));
						StartActivity (ventana);
						Activity.Finish ();

					}else{
						Toast toast = Toast.MakeText(Activity, responseInstance.ToString(), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}
				}else{
					
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(Activity, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}	

			} catch (Exception ex) {
				Toast toast = Toast.MakeText(Activity, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
			finally
			{
				EditCodigo.Text = "";
			}
		}
		#endregion

	}

}


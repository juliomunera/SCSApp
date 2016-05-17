using System;
using System.Collections.Generic;
using System.Net.Http;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using APPDroid.Framework.Context;
using APPDroid.Framework.Helpers;
using APPDroid.Framework.Services;
using Newtonsoft.Json;

using SCSAPP.Android.Adapters;
using SCSAPP.Framework.Context;

using SCSAPP.Services.Messages;

namespace AppDroid.Main.Activities
{
	[Activity (Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/ic_servinte", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		#region Variables. 
		Button buttonIngresar;
		EditText ediTxtusr, editTxtPwd;
		TextView config;
		AdapEad listViewEad;
		ImageButton imgSetting;
		bool _clickOnLoginBtn = false;
		ProgressDialog progressDialog;
		#endregion

		#region class principal 
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name = "savedInstanceState"></param>
		protected override void OnCreate (Bundle savedInstanceState){
			try {
				
				base.OnCreate (savedInstanceState);
				SetContentView (Resource.Layout.app_login);

				buttonIngresar = FindViewById<Button> (Resource.Id.button1);
				config = FindViewById<TextView> (Resource.Id.txtConfiguracion);
				ediTxtusr = FindViewById<EditText> (Resource.Id.editUser);
				editTxtPwd = FindViewById<EditText> (Resource.Id.editPassword);
				progressDialog = new ProgressDialog (this);
				progressDialog.SetMessage ("Cargando...");
				progressDialog.SetCancelable (false);
				imgSetting = FindViewById<ImageButton> (Resource.Id.imgBtnsettings);

				string ContextLicence = DataBaseManager.GetContexts (DataBaseManager.IDContextType.Lincence);
				string ContextWcf = DataBaseManager.GetContexts (DataBaseManager.IDContextType.Wcf);
				string ContextToRestore = DataBaseManager.GetContexts (DataBaseManager.IDContextType.ContextApp);

				ediTxtusr.Enabled = false;
				editTxtPwd.Enabled = false;

				if (!String.IsNullOrEmpty (ContextLicence) && !String.IsNullOrEmpty (ContextWcf)) {
					MobileLicenseContext RestoredlicContext = LicenseContextApp.GetContextFromSerializedString (ContextLicence);
					LicenseContextApp.Init (RestoredlicContext);

					MobileServicesContext RestoredWcfContext = WebServicesContextApp.GetContextFromSerializedString (ContextWcf);
					WebServicesContextApp.Init (RestoredWcfContext);

					buttonIngresar.Visibility = ViewStates.Visible;
					config.Visibility = ViewStates.Gone;
					ediTxtusr.Enabled = true;
					editTxtPwd.Enabled = true;
				}
				
				var newTypes = new ActivitiesTypes ();
				newTypes.LoginType = typeof(MainActivity);
				newTypes.HomeType = typeof(AppHomeList);
				newTypes.Patient = null;
				newTypes.Parameters = new Dictionary<string, object>();
				newTypes.AdministerData = new int[0];
				newTypes.FirstPrimary = true;
				newTypes.PositionOnAdminister = -1;
				ActivitiesContext.Init (newTypes);

				if (!String.IsNullOrEmpty (ContextToRestore)) {
					MobileContext RestoredContext = ContextApp.GetContextFromSerializedString (ContextToRestore);
					ContextApp.Init (RestoredContext);

					var ventanaHome = new Intent (this, typeof(AppHomeList));
					StartActivity (ventanaHome);
				} else {
					//No login
					ActionBar.Hide();

					buttonIngresar.Click += async (sender, e) => {
						
						if (_clickOnLoginBtn) return;
							_clickOnLoginBtn = true;

						progressDialog.Show();

						var progs = new List<ProgramRequest>();

						if (LicenseContextApp.Instance.HaveAdmMed) {
							progs.Add(new ProgramRequest {
								Code = "cmoadmmed", 
								Variables = new List<string> { "CP", "SF", "AM", "CA", "DC" }
							});

							progs.Add(new ProgramRequest {
									Code = "cmocarmed",
									Variables = new List<string> { "PI", "LB", "IT", "CS", "CN", "PC", "VP", "MS", "DC" }
							});

							progs.Add(new ProgramRequest {
									Code = "cmodevmed",
									Variables = new List<string> { "LB", "CD", "PI", "CN", "CS", "IT", "MS", "VP", "DC" }
							});
						}

						using (var httpClient = WebServices.GetBaseHttpClient (URIType.Authentication)) {
							var request = new
							{
								User = ediTxtusr.Text,
								Password = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(editTxtPwd.Text)),
								Configuration = new {
									Applications = new List<string> { "SCSAPP", "ORDENG", "FACTUR", "INFGEN", "ORDENE", "PREORD", "SUMINI", "ITLINK", "HCECLI"},
									ConfigurationVariables = new List<string> { "COLLETPEN", "COLLETPRE", "COLLETREP", "TIEDEFATR", "TIEMACTLIS", 
										"TIEDEFADE", "COLLETPIN", "COLLETPIN", "COLLETMEZ", "COLLETMPV", 
										"COLLETREF", "COLORLETRAANTEM", "COLORFONDOANTEM", "COLORLETRAALERM"},
									Programs = progs.ToArray()
								}
							};

							var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
							try{
								var result = await httpClient.PostAsync (GetString(APPDroid.Framework.Resource.String.txt_login_app), new StringContent (jsonRequest, System.Text.Encoding.UTF8, GetString(APPDroid.Framework.Resource.String.txt_aplicacion_json)));
								await StartMenuActivity (result);
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
					};

					imgSetting.Click += (sender, e) => DialogValidateApp ();

				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format ("Error: {0}", ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				_clickOnLoginBtn = false;
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}
		}
		#endregion

		#region DialogValidar.
		/// <summary>
		/// Dialogs the validate app.
		/// </summary>
		void DialogValidateApp()
		{
			var alerDialogV = (new AlertDialog.Builder (this)).Create ();
			var inflaterV = LayoutInflater.Inflate(Resource.Layout.layout_dialog_validate, null);
			EditText CajaValida = inflaterV.FindViewById<EditText>(Resource.Id.editCodeValidate);
			alerDialogV.SetTitle (APPDroid.Framework.Resource.String.txt_titulo_validacion);
			alerDialogV.SetView(inflaterV);

			alerDialogV.SetButton ("Aceptar", delegate {
				if(string.IsNullOrEmpty(CajaValida.Text))
				{
					Toast toast = Toast.MakeText(this, "La contraseña es requerida", ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					DialogValidateApp();
				}
				else
				{
					if(CajaValida.Text.Equals(GetSystemPwd())){
						var settings = new Intent (this, typeof(AppSetting));
						StartActivity (settings);
					}else{
						Toast toast = Toast.MakeText(this, "La contraseña no es correcta", ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						DialogValidateApp();
					}
				}
			});

			alerDialogV.SetButton2 ("Cancelar", (s, ev) =>  {
				var dialog = s as AlertDialog;
				if (dialog != null) {
					dialog.Dismiss ();	
				}
			});

			alerDialogV.Show ();
		}
		#endregion

		#region GetSystemPwd. 
		/// <summary>
		/// Gets the system pwd.
		/// </summary>
		/// <returns>The system pwd.</returns>
		static string GetSystemPwd()
		{
			return String.Format ("{0}-{1}", DateTime.Now.Month.ToString ("00"), DateTime.Now.Month + DateTime.Now.Day + 28);
		}
		#endregion

		#region Metodo de respuesta del servicio.
		/// <summary>
		/// Starts the menu activity.
		/// </summary>
		/// <returns>The menu activity.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartMenuActivity(HttpResponseMessage response){
			try {
				if (response.IsSuccessStatusCode) {
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					LoginResponse responseInstance = JsonConvert.DeserializeObject<LoginResponse> (responseJsonText);

					if (responseInstance != null) {
						var newContext = new MobileContext();
						newContext.User = responseInstance.User;
						newContext.AdministrativeStructures = responseInstance.AdministrativeStructures;
						newContext.Applications = responseInstance.Applications;
						newContext.NursingParametersS = responseInstance.NursingParameter;
						ContextApp.Init (newContext);
						ContextApp.Instance.DeleteAllDeniedProgramsOnEAD();

						if (ContextApp.Instance.AdministrativeStructures.Count > 0) {
							if (ContextApp.Instance.RequiredEad) {

								var alerDialog = (new AlertDialog.Builder (this)).Create ();
								var inflater = LayoutInflater.Inflate(Resource.Layout.app_select_master, null);
								ListView ListView = inflater.FindViewById<ListView>(Resource.Id.listViewEad);
								listViewEad = new AdapEad(this, ContextApp.Instance.AdministrativeStructures);
								ListView.Adapter = listViewEad;
								alerDialog.SetTitle (APPDroid.Framework.Resource.String.txt_estructura_admin);
								alerDialog.SetView(inflater);
								alerDialog.Show ();

							}
							else {
								ContextApp.Instance.SelectedEAD = ContextApp.Instance.AdministrativeStructures[0];
								var ventanaHome = new Intent (this, typeof(AppHomeList));
								StartActivity (ventanaHome);
							}

							DataBaseManager.InsertContext (DataBaseManager.IDContextType.ContextApp, ContextApp.GetContextSerialized ());	
						}
						else {
							Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.txt_usuario_ead, ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();
						}
					} else {
						Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.txt_des_serializada, ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}
				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower().Contains("html")) {
						Toast toast = Toast.MakeText(this, "El servicio no se encuentra disponible.", ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}
					else {
						string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
						Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();	
					}
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
			finally {
				_clickOnLoginBtn = false;
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}
		}
		#endregion

		#region Metodo de regresar en la activity
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
			var alerDialog = (new AlertDialog.Builder (this)).Create ();
			alerDialog.SetTitle (Resource.String.txt_alertas);
			alerDialog.SetMessage (Resources.GetString (Resource.String.txt_salirApp));
			alerDialog.SetButton (Resources.GetString (Resource.String.btn_aceptar), delegate {
				FinishAffinity ();
			});
			alerDialog.SetButton2 (Resources.GetString (Resource.String.btn_cancelar), delegate {
				alerDialog.Dismiss ();
			});
			alerDialog.Show ();
		}
		#endregion

	}
}
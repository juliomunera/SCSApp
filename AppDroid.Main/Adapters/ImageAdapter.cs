
using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

using SCSAPP.Framework.Context;
using APPDroid.CarMed.Activities;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Context;
using APPDroid.AdmMed.Activities;


namespace SCSAPP.Android.Adapters
{
	public class ImageAdapter : BaseAdapter{

		#region Variables
		readonly Context context;
		int imagen;
		TextView nombreApp;
		AdapSpeciality listViewSpeciality;
		#endregion

		#region Constructor Method
		/// <summary>
		/// Initializes a new instance of the <see cref="SCSAPP.Android.Adapters.ImageAdapter"/> class.
		/// </summary>
		/// <param name="c">C.</param>
		public ImageAdapter(Context c){ context = c; }
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count{
			get {
				if (ContextApp.Instance != null && ContextApp.Instance.SelectedEAD != null)
					return ContextApp.Instance.SelectedEAD.Programs.Count;

				return 0;
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
		public override Java.Lang.Object GetItem(int position){
			return null;
		}
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
		public override long GetItemId(int position){
			return position;
		}
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
			
			var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
			convertView = inflater.Inflate(AppDroid.Main.Resource.Layout.items_grids, parent, false);

			ImageView imgIconApp = convertView.FindViewById<ImageView> (AppDroid.Main.Resource.Id.imageView1);

			if (ContextApp.Instance != null && ContextApp.Instance.SelectedEAD != null) {
				int index = 0;
				string TagName = string.Empty;
				string Name = string.Empty;

				foreach (var Prog in ContextApp.Instance.SelectedEAD.Programs) {
					if (index == position) {
						TagName = Prog.Code;
						break;
					}
					index ++;
				}
				switch (TagName.ToLower()) {
					case "cmoadmmed":
						imagen = APPDroid.Framework.Resource.Drawable.ic_admed;
						Name = context.Resources.GetString(APPDroid.Framework.Resource.String.txt_administracion_med); 
						convertView.Tag = context.Resources.GetString(APPDroid.Framework.Resource.String.txt_cmoadmmed);
						break;
					case "cmocarmed":
						imagen = APPDroid.Framework.Resource.Drawable.ic_carmed;
						Name = context.Resources.GetString(APPDroid.Framework.Resource.String.txt_cargos);
						convertView.Tag = context.Resources.GetString(APPDroid.Framework.Resource.String.txt_cmocarmed);
						break;
					case "cmodevmed":
						Name = context.Resources.GetString(APPDroid.Framework.Resource.String.txt_devoluciones);
						imagen = APPDroid.Framework.Resource.Drawable.ic_devmed;
						convertView.Tag = context.Resources.GetString(APPDroid.Framework.Resource.String.txt_cmodevmed);
						break;
				}
                imgIconApp.SetImageResource(imagen);
                nombreApp = convertView.FindViewById<TextView>(AppDroid.Main.Resource.Id.txtApp);
                nombreApp.Text = Name;
                
				convertView.Click += (sender, e) => {
					var item = sender as View;
					if (item != null && item.Tag != null) {
						switch (item.Tag.ToString ()) {
						case "cmoadmmed":

							var pInfo = ContextApp.Instance.GetProgramInfo (context.Resources.GetString (APPDroid.Framework.Resource.String.txt_cmoadmmed));
							if (pInfo != null && pInfo.IsClinic) {

								if (pInfo.MainSpeciality == null && (pInfo.Specialities == null || pInfo.Specialities.Count == 0)) {
									Toast.MakeText (context, context.Resources.GetString (APPDroid.Framework.Resource.String.txt_no_especiales), ToastLength.Long).Show ();
									return;
								}

								if (pInfo.MainSpeciality != null && !String.IsNullOrEmpty (pInfo.MainSpeciality.Code))
									ContextApp.Instance.SelectedSpeciality = pInfo.MainSpeciality;

								if (pInfo.Specialities != null || pInfo.Specialities.Count > 0) {
									
									var alerDialog = (new AlertDialog.Builder (context)).Create ();

									if (pInfo.MainSpeciality != null) {
										var Find = pInfo.Specialities.Any (i => i.Code.Equals (pInfo.MainSpeciality.Code));
										if (!Find)
											pInfo.Specialities.Add (pInfo.MainSpeciality);
									}

									if ((pInfo.Specialities != null && pInfo.Specialities.Count > 0) && (pInfo.MainSpeciality != null && !String.IsNullOrEmpty (pInfo.MainSpeciality.Code))) {
										var newInflater = inflater.Inflate (AppDroid.Main.Resource.Layout.app_select_master, null);
										ListView ListView = newInflater.FindViewById<ListView> (AppDroid.Main.Resource.Id.listViewEad);

										var prog = pInfo.Specialities.FirstOrDefault (p => p.Code.Equals (pInfo.MainSpeciality.Code));

										pInfo.Specialities.Remove (prog);
										pInfo.Specialities.Insert (0, prog);

										if(pInfo.Specialities.Count == 1)
										{
											var programasGeneralw = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
											programasGeneralw = ParameterProgramer(programasGeneralw);

											//Validation of the variable CP different views
											var VCP = programasGeneralw.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));
											ActivitiesContext.Context.ValVarCP = VCP;

											Intent ventanaPrincipal = null;

											if (String.IsNullOrEmpty(VCP.Value) || VCP.Value.Equals("L") || VCP.Value.Equals("V")){
												ventanaPrincipal = new Intent(context, typeof(ActMain));    
											}else if(VCP.Value.Equals("H")){
												ventanaPrincipal = new Intent(context, typeof(FragActCamaCodigo)); 
											}
												
											context.StartActivity(ventanaPrincipal);
			
										}
										else
										{
											
											listViewSpeciality = new AdapSpeciality (context, pInfo.Specialities, pInfo.MainSpeciality, alerDialog, context.Resources.GetString (APPDroid.Framework.Resource.String.txt_cmoadmmed));
											ListView.Adapter = listViewSpeciality;
											alerDialog.SetTitle (context.Resources.GetString (APPDroid.Framework.Resource.String.txt_select_especialidad));
											alerDialog.SetView (newInflater);
											alerDialog.SetCancelable (true);
											alerDialog.Show ();
										}


									} else {
										Toast.MakeText (context, context.Resources.GetString (APPDroid.Framework.Resource.String.txt_no_especiality), ToastLength.Long).Show ();		
									}
								} else {
									Toast.MakeText (context, context.Resources.GetString (APPDroid.Framework.Resource.String.txt_no_especiality), ToastLength.Long).Show ();
								}

							} else {
								ContextApp.Instance.SelectedSpeciality = null;
							}

							break;

						case "cmocarmed":
							var programasGeneral = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmocarmed"));
							if(programasGeneral.Variables != null){
								var CN = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("CN"));
								if (CN == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "CN", Value = "N" });
								}
								var CS = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("CS"));
								if (CS == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "CS", Value = "D" });
								}
								var PC = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("PC"));
								if (PC == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "PC", Value = "N" });
								}
								var VP = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("VP"));
								if (VP == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "VP", Value = "S" });
								}
								var IT = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("IT"));
								if (IT == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "IT", Value = "S" });
								}
								var LB = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("LB"));
								if (LB == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "LB", Value = "S" });
								}
								var PI = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("PI"));
								if (PI == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "PI", Value = "S" });
								}
								var MS = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("MS"));
								if (MS == null) {
									programasGeneral.Variables.Add(new MasterItem { Code = "MS", Value = "N" });
								}
								var DC = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));
								if (DC == null) 
								{
									programasGeneral.Variables.Add(new MasterItem { Code = "DC", Value = "S" });
								}
							} else {
								programasGeneral.Variables.Add(new MasterItem {	Code = "CN", Value = "N" });
								programasGeneral.Variables.Add(new MasterItem { Code = "CS", Value = "D" });
								programasGeneral.Variables.Add(new MasterItem { Code = "PC", Value = "N" });
								programasGeneral.Variables.Add(new MasterItem { Code = "VP", Value = "S" });
								programasGeneral.Variables.Add(new MasterItem { Code = "IT", Value = "S" });
								programasGeneral.Variables.Add(new MasterItem { Code = "LB", Value = "S" });
								programasGeneral.Variables.Add(new MasterItem { Code = "PI", Value = "S" });
								programasGeneral.Variables.Add(new MasterItem { Code = "MS", Value = "N" });
								programasGeneral.Variables.Add(new MasterItem { Code = "DC", Value = "S" });
							}	

							var Ordeng = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("ORDENG"));
							var Preord = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("PREORD"));
							var Infgen = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("INFGEN"));
							var Factur = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("FACTUR"));
							var Sumini = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("SUMINI"));
							var Itlink = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("ITLINK"));

							if (Ordeng == null) {
								Toast.MakeText (context, "No hay aplicación Ordenes Médicas", ToastLength.Long).Show ();
								return;
							} else if (Preord == null) {
								Toast.MakeText (context, "No hay aplicación Predespacho", ToastLength.Long).Show ();
								return;
							} else if (Infgen == null) {
								Toast.MakeText (context, "No hay aplicación Admisiones", ToastLength.Long).Show ();
								return;
							} else if (Factur == null) {
								Toast.MakeText (context, "No se tiene el módulo de Facturación", ToastLength.Long).Show ();
								return;
							} else if (Sumini == null) {
								Toast.MakeText (context, "No se tiene módulo de Suministros", ToastLength.Long).Show ();
								return;
							} else if (Itlink == null) {
								Toast.MakeText (context, "No se tiene módulo de ITLINK", ToastLength.Long).Show ();
								return;
							} else {	
								if (ContextApp.Instance.SelectedEAD.LastLevel) {
									var ventanaPrincipal = new Intent (context, typeof(ActCarMain));	
									context.StartActivity (ventanaPrincipal);	
								} else {
									Toast.MakeText (context, "EAD en sesión no es de último Nivel", ToastLength.Long).Show ();
									return;
								}
							}
							break;

						case "cmodevmed":
							var programasGeneral2 = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmodevmed"));
							if(programasGeneral2 != null){
								var LB2 = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("LB"));
								if (LB2 == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "LB", Value = "S" });
								}
								var CS2 = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("CS"));
								if (CS2 == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "CS", Value = "D" });
								}
								var CD = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("CD"));
								if (CD == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "CD", Value = "S" });
								}
								var PI2 = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("PI"));
								if (PI2 == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "PI", Value = "S" });
								}
								var CN2 = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("CN"));
								if (CN2 == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "CN", Value = "N" });
								}	
								var MS2 = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("MS"));
								if (MS2 == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "MS", Value = "N" });
								}
								var VP2 = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("VP"));
								if (VP2 == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "VP", Value = "S" });
								}
								var IT2 = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("IT"));
								if (IT2 == null) {
									programasGeneral2.Variables.Add(new MasterItem { Code = "IT", Value = "S" });
								}
								var DC = programasGeneral2.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));
								if (DC == null) 
								{
									programasGeneral2.Variables.Add(new MasterItem { Code = "DC", Value = "S" });
								}
							} else {
								programasGeneral2.Variables.Add(new MasterItem { Code = "LB", Value = "S" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "CS", Value = "D" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "CD", Value = "S" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "PI", Value = "S" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "CN", Value = "N" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "MS", Value = "N" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "VP", Value = "S" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "IT", Value = "S" });
								programasGeneral2.Variables.Add(new MasterItem { Code = "DC", Value = "S" });

							}

							var InfgenDev = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("INFGEN"));
							var FacturDev = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("FACTUR"));
							var SuminiDev = ContextApp.Instance.Applications.FirstOrDefault (v => v.Code.ToUpper ().Equals ("SUMINI"));

							if (InfgenDev == null) {
								Toast.MakeText (context, "No hay aplicación Admisiones", ToastLength.Long).Show ();
								return;
							} else if (FacturDev == null) {
								Toast.MakeText (context, "No se tiene el módulo de Facturación", ToastLength.Long).Show ();
								return;
							} else if (SuminiDev == null) {
								Toast.MakeText (context, "No se tiene módulo de Suministros", ToastLength.Long).Show ();
								return;
							} else {
								if (ContextApp.Instance.SelectedEAD.LastLevel) {
									var ventanaPrincipal12 = new Intent (context, typeof(APPDroid.DevMed.Activities.ActDevmMain));	
									context.StartActivity (ventanaPrincipal12);
								} else {
									Toast.MakeText (context, "EAD en sesión no es de último Nivel", ToastLength.Long).Show ();
									return;
								}
							}

							break;
						}

					}
				};
			}

			return convertView;
		}
		#endregion

		#region ParameterProgramer
		/// <summary>
		/// Parameters the programer.
		/// </summary>
		/// <returns>The programer.</returns>
		/// <param name="programasGeneral">Programas general.</param>
		public Program ParameterProgramer(Program programasGeneral){
			if(programasGeneral.Variables != null){
				var CP = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));
				if (CP == null) 
				{
					programasGeneral.Variables.Add(new MasterItem {	Code = "CP", Value = "L" });
				}
				var SF = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("SF"));
				if (SF == null) 
				{
					programasGeneral.Variables.Add(new MasterItem { Code = "SF", Value = "N" });
				}
				var AM = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("AM"));
				if (AM == null) 
				{
					programasGeneral.Variables.Add(new MasterItem { Code = "AM", Value = "S" });
				}
				var CA = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("CA"));
				if (CA == null) 
				{
					programasGeneral.Variables.Add(new MasterItem { Code = "CA", Value = "N" });
				}
				var DC = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));
				if (DC == null) 
				{
					programasGeneral.Variables.Add(new MasterItem { Code = "DC", Value = "S" });
				}
			} else {
				programasGeneral.Variables.Add(new MasterItem {	Code = "CP", Value = "L" });
				programasGeneral.Variables.Add(new MasterItem { Code = "SF", Value = "N" });
				programasGeneral.Variables.Add(new MasterItem { Code = "AM", Value = "S" });
				programasGeneral.Variables.Add(new MasterItem { Code = "CA", Value = "N" });
				programasGeneral.Variables.Add(new MasterItem { Code = "DC", Value = "S" });
			}

			return programasGeneral;
		}
		#endregion

	}

}


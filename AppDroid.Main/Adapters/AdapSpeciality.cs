
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

using SCSAPP.Framework.Context;
using SCSAPP.Services.Messages;
using Android.Graphics;
using APPDroid.Framework.Context;
using APPDroid.AdmMed.Activities;

namespace SCSAPP.Android.Adapters
{
	public class AdapSpeciality : BaseAdapter{

		#region Variables
		readonly Context context;
		readonly List<MasterItem> specialities;
		readonly MasterItem speciality;
		readonly AlertDialog alert;
		readonly string pCode;
		#endregion

		#region Constructor Method
		/// <summary>
		/// Initializes a new instance of the <see cref="SCSAPP.Android.Adapters.AdapEad"/> class.
		/// </summary>
		/// <param name="c">C.</param>
		/// <param name = "specialities"></param>
		/// <param name = "speciality"></param>
		/// <param name = "alert"></param>
		/// <param name = "ProgCode"></param>
		public AdapSpeciality(Context c, List<MasterItem> specialities, MasterItem speciality, AlertDialog alert, string ProgCode){
			context = c;
			this.specialities = specialities;
			this.speciality = speciality;
			this.alert = alert;
			pCode = ProgCode;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count{
			get { return specialities.Count;}
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
			convertView = inflater.Inflate(AppDroid.Main.Resource.Layout.items_master, parent, false);

			TextView nombre = convertView.FindViewById<TextView> (AppDroid.Main.Resource.Id.EadNombre);

			if (speciality != null && specialities[position].Code.Equals(speciality.Code))
				nombre.SetTextColor(Color.Gray);
			
			nombre.Text = specialities[position].Value.Trim();

			convertView.Click += (sender, e) => {
				
				ContextApp.Instance.SelectedSpeciality = specialities [position];

				alert.Dismiss ();

				switch (pCode) {
				case "cmoadmmed":
					var programasGeneral = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
					if (programasGeneral.Variables != null) {
						var CP = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));
						if (CP == null) {
							programasGeneral.Variables.Add (new MasterItem {	Code = "CP", Value = "L" });
						}
						var SF = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("SF"));
						if (SF == null) {
							programasGeneral.Variables.Add (new MasterItem { Code = "SF", Value = "N" });
						}
						var AM = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("AM"));
						if (AM == null) {
							programasGeneral.Variables.Add (new MasterItem { Code = "AM", Value = "S" });
						}
						var CA = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("CA"));
						if (CA == null) {
							programasGeneral.Variables.Add (new MasterItem { Code = "CA", Value = "N" });
						}
						var DC = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));
						if (DC == null) {
							programasGeneral.Variables.Add (new MasterItem { Code = "DC", Value = "S" });
						}
					} else {
						programasGeneral.Variables.Add (new MasterItem { Code = "CP", Value = "L" });
						programasGeneral.Variables.Add (new MasterItem { Code = "SF", Value = "N" });
						programasGeneral.Variables.Add (new MasterItem { Code = "AM", Value = "S" });
						programasGeneral.Variables.Add (new MasterItem { Code = "CA", Value = "N" });
						programasGeneral.Variables.Add (new MasterItem { Code = "DC", Value = "S" });
					}

					//Validation of the variable CP different views
					var VCP = programasGeneral.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));
					ActivitiesContext.Context.ValVarCP = VCP;
					Intent ventanaPrincipal = null;

					if (String.IsNullOrEmpty(VCP.Value) || VCP.Value.Equals("L") || VCP.Value.Equals("V")){
						ventanaPrincipal = new Intent(context, typeof(ActMain));    
					}else if(VCP.Value.Equals("H")){
						ventanaPrincipal = new Intent(context, typeof(FragActCamaCodigo)); 
					}
						
					context.StartActivity (ventanaPrincipal);

					break;

				}
			};

			return convertView;
		}
		#endregion

	}

}


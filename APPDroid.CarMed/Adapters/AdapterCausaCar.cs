using Android.Widget;
using Android.App;
using SCSAPP.Services.Messages.Entities;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using System;
using APPDroid.Framework.Context;

namespace APPDroid.CarMed.Adapters
{
	public class AdapterCausaCar : BaseAdapter
	{
		#region Variables and Controls
		readonly Activity context;
		readonly List<NonDispatchCause> listCauses;
		readonly AlertDialog alertDialog;
		readonly ListView listMedicine;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.CarMed.Adapters.AdapterCausaCar"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="listCauses">List causes.</param>
		/// <param name="alertDialog">Alert dialog.</param>
		/// <param name="listMedicine">List medicine.</param>
		public AdapterCausaCar(Activity context, List<NonDispatchCause> listCauses, AlertDialog alertDialog, ListView listMedicine){
			this.context = context;
			this.listCauses = listCauses;
			this.alertDialog = alertDialog;
			this.listMedicine = listMedicine;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count{
			get { return listCauses.Count;}
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
		public override View GetView(int position, View convertView, ViewGroup parent)
		{

			var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
			convertView = inflater.Inflate(APPDroid.Framework.Resource.Layout.items_master, parent, false);
			TextView nombre = convertView.FindViewById<TextView> (APPDroid.Framework.Resource.Id.EadNombre);

			string indicadorPendiente = "";

			if(listCauses[position].GeneratePendings.Equals("S")){
				indicadorPendiente = "(Pendiente)";
			}

			nombre.Text = string.Format("{0}  {1}", listCauses[position].Description, indicadorPendiente);

			convertView.Click += (sender, e) => {
				ActivitiesContext.Context.listmedicament [ActivitiesContext.Context.PositionOnAdminister].ResponseCauses = listCauses [position].Code;
				//ActivitiesContext.Context.listmedicament [ActivitiesContext.Context.PositionOnAdminister].AmountAlistada = 0;
				//ActivitiesContext.Context.listmedicament [ActivitiesContext.Context.PositionOnAdminister].ListItemDetailCumM = null;
				ActivitiesContext.Context.AdministerData [ActivitiesContext.Context.PositionOnAdminister] = (int)AdministerType.NoAdmin;	
				listMedicine.Adapter = new AdapMedicines (context, ActivitiesContext.Context.listmedicament, listMedicine);
				alertDialog.Dismiss ();
			};

			return convertView;
		}
		#endregion
			
	}
}


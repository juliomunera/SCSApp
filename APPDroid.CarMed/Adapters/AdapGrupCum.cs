using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using APPDroid.Framework.Context;
using System;

namespace APPDroid.CarMed.Adapters
{
	public class AdapGrupCum : BaseAdapter
	{
		#region Variables and Controls
		readonly Activity context;
		readonly List<GroupResult> detLotCumReg;
		readonly int positionMedi;
		readonly ListView listMedicine;
		readonly AlertDialog alertDialog;
		DateTime fecha;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.CarMed.Adapters.AdapGrupCum"/> class.
		/// </summary>
		/// <param name="actividad">Actividad.</param>
		/// <param name="detLotCumReg">Det lot cum reg.</param>
		/// <param name="positionMedi">Position medi.</param>
		/// <param name="listMedicine">List medicine.</param>
		/// <param name="alertDialog">Alert dialog.</param>
		public AdapGrupCum(Activity actividad, List<GroupResult> detLotCumReg, int positionMedi, ListView listMedicine, AlertDialog alertDialog)
		{
			context = actividad;
			this.detLotCumReg = detLotCumReg;
			this.positionMedi = positionMedi;
			this.listMedicine = listMedicine;
			this.alertDialog = alertDialog;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count
		{
			get { return detLotCumReg.Count; }
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
		public override Java.Lang.Object GetItem(int position)
		{
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
		public override long GetItemId(int position)
		{
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
			var inflater = (LayoutInflater) context.GetSystemService(Context.LayoutInflaterService);
			convertView = inflater.Inflate(Resource.Layout.item_grup_medicament, parent, false);

			TextView cantidadGrup = convertView.FindViewById<TextView> (Resource.Id.txtCantidadGru);
			cantidadGrup.Text = string.Format ("{0}", detLotCumReg[position].Count);

			TextView lote = convertView.FindViewById<TextView> (Resource.Id.txtLoteGru);
			if (string.IsNullOrEmpty (detLotCumReg [position].NumberLote)) {
				lote.Visibility = ViewStates.Gone;
			} else {
				lote.Text = string.Format ("LT. {0}", detLotCumReg[position].NumberLote);
			}

			TextView cum = convertView.FindViewById<TextView> (Resource.Id.txtCumGru);
			if (string.IsNullOrEmpty (detLotCumReg [position].CodeMedic)) {
				cum.Visibility = ViewStates.Gone;
			} else {
				cum.Text = string.Format ("CM: {0}", detLotCumReg[position].CodeMedic);
			}

			TextView invima = convertView.FindViewById<TextView> (Resource.Id.txtInvimaGru);
			if (string.IsNullOrEmpty (detLotCumReg [position].Invima)) {
				invima.Visibility = ViewStates.Gone;
			} else {
				invima.Text = string.Format ("RI: {0}", detLotCumReg[position].Invima);
			}

			TextView txtFechaVen = convertView.FindViewById<TextView> (Resource.Id.txtFechaVen);
			if (!DateTime.TryParse(detLotCumReg [position].FechaVencimiento.ToString(),out fecha)){
				txtFechaVen.Visibility = ViewStates.Gone;
			} else {
				txtFechaVen.Text = string.Format ("FV: {0:yyyy/MM/dd}", detLotCumReg[position].FechaVencimiento);
			}


			Button EliminarRegistro = convertView.FindViewById<Button> (Resource.Id.btnIEliminarGru);
			EliminarRegistro.Click += delegate {
				
				int Count = ActivitiesContext.Context.listmedicament[positionMedi].ListItemDetailCumM.RemoveAll(X => X.Invima.Equals(detLotCumReg[position].Invima) && X.CodeMedic.Equals(detLotCumReg[position].CodeMedic) && X.NumberLote.Equals(detLotCumReg[position].NumberLote));
				ActivitiesContext.Context.listmedicament[positionMedi].AmountAlistada = ActivitiesContext.Context.listmedicament[positionMedi].AmountAlistada - Count;
				alertDialog.Dismiss();

				if(ActivitiesContext.Context.listmedicament[positionMedi].ListItemDetailCumM.Count == 0)
					ActivitiesContext.Context.AdministerData [positionMedi] = (int)AdministerType.None;

				listMedicine.Adapter = new AdapMedicines(context, ActivitiesContext.Context.listmedicament, listMedicine);
			};
				
			return convertView;
		}
		#endregion

	}

}


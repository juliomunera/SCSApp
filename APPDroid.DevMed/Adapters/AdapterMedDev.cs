
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages;
using SCSAPP.Framework.Context;
using Android.Graphics;
using APPDroid.Framework.Context;

namespace APPDroid.DevMed.Adapters
{
	public class AdapterMedDev : BaseAdapter
	{
		#region Variables and Controls
		readonly Activity context;
		readonly List<Medicament> data;
		readonly MasterItem colletmpv;
		readonly MasterItem colletmez;
		readonly ListView listaMedicamento;
		const string cantDevuelta = "Cant. Devuelta: {0}";
		const string cantNoAdministrada = "Cant. No Administrada: {0}";
		const string cantidadInterpretada = "Cantidad interpretada {0}";
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.DevMed.Adapters.AdapterMedDev"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		/// <param name="listaMedicamento">Lista medicamento.</param>
		public AdapterMedDev(Activity context, List<Medicament> data, ListView listaMedicamento) 
		{
			this.context = context;
			this.data = data;	
			colletmpv = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETMPV"));
			colletmez = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETMEZ"));
			this.listaMedicamento = listaMedicamento;

		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count
		{
			get { return data.Count; }
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

			var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
			convertView = inflater.Inflate(Resource.Layout.items_dev_medicament, parent, false);


			TextView nombre = convertView.FindViewById<TextView>(Resource.Id.nombreMedicamento);
			nombre.Text = data[position].Description;

			TextView codeArticulo = convertView.FindViewById<TextView>(Resource.Id.codeMedDev);
			codeArticulo.Text = String.Format ("Código: {0}",data [position].Code);
		
			TextView cantidadNoAdmin = convertView.FindViewById<TextView>(Resource.Id.cantidadNoAdministrada);
			cantidadNoAdmin.Text = String.Format (cantNoAdministrada, data [position].NumberOfUnits);  

			TextView unidadMedida = convertView.FindViewById<TextView>(Resource.Id.unidaMedida);
			unidadMedida.Text = String.Format ("{0}", data[position].UnitsOfMeasurement); 

			TextView cantidadDevuelta = convertView.FindViewById<TextView>(Resource.Id.cantidadDevuelta);
			cantidadDevuelta.Text = String.Format (cantDevuelta, data [position].AmountAlistada.ToString ("N1"));  

			//Indicador mexclas
			if(data [position].IsMixture)
			{
				if (colletmez != null) 
				{
					nombre.SetTextColor (Color.ParseColor (colletmez.Value));
					codeArticulo.SetTextColor (Color.ParseColor (colletmez.Value));
					cantidadNoAdmin.SetTextColor (Color.ParseColor (colletmez.Value));
					cantidadDevuelta.SetTextColor (Color.ParseColor (colletmez.Value));
					unidadMedida.SetTextColor (Color.ParseColor (colletmez.Value));	
				} 
				else 
				{
					nombre.SetTextColor (Color.ParseColor ("#008000"));
					codeArticulo.SetTextColor (Color.ParseColor ("#008000"));
					cantidadNoAdmin.SetTextColor (Color.ParseColor ("#008000"));
					cantidadDevuelta.SetTextColor (Color.ParseColor ("#008000"));
					unidadMedida.SetTextColor (Color.ParseColor ("#008000"));
				}
			}

			//Indicador proximo..
			if (data [position].ExpiringSoon) 
			{
				if (colletmpv != null) 
				{
					nombre.SetTextColor (Color.ParseColor (colletmpv.Value));
					codeArticulo.SetTextColor (Color.ParseColor (colletmpv.Value));
					cantidadNoAdmin.SetTextColor (Color.ParseColor (colletmpv.Value));
					cantidadDevuelta.SetTextColor (Color.ParseColor (colletmpv.Value));
					unidadMedida.SetTextColor (Color.ParseColor (colletmpv.Value));
				} 
				else 
				{
					nombre.SetTextColor (Color.ParseColor ("#FFA500"));
					codeArticulo.SetTextColor (Color.ParseColor ("#FFA500"));
					cantidadNoAdmin.SetTextColor (Color.ParseColor ("#FFA500"));
					cantidadDevuelta.SetTextColor (Color.ParseColor ("#FFA500"));
					unidadMedida.SetTextColor (Color.ParseColor ("#FFA500"));
				}
			}				

			convertView.Click += (sender, e) => {
				
				var alertDialog = (new AlertDialog.Builder (context)).Create ();
				var inflaterDes = context.LayoutInflater.Inflate (Resource.Layout.dialog_detalle_medic_dev, null);

				inflaterDes.FindViewById<TextView> (Resource.Id.txtNombreMedicament).Text = ActivitiesContext.Context.listmedicament [position].Description;
				inflaterDes.FindViewById<TextView> (Resource.Id.txtUnidadMedida).Text = ActivitiesContext.Context.listmedicament [position].UnitsOfMeasurement;
				inflaterDes.FindViewById<TextView> (Resource.Id.txtCantidad).Text =  string.Format(cantDevuelta, data [position].AmountAlistada.ToString ("N1"));

				ListView listViewGrup = inflaterDes.FindViewById<ListView> (Resource.Id.listView);

				if (ActivitiesContext.Context.listmedicament [position].ListItemDetailCumM != null) {

					var GroupByResult = ActivitiesContext.Context.listmedicament [position].ListItemDetailCumM.GroupBy (n => new { n.NumberLote, n.CodeMedic, n.Invima, n.Maturing, n.ExpirationDate })
						.Select (n => new { TotalItems = n.Count (), Data = n.Key }).OrderBy (n => n.TotalItems).ToList ();

					if (GroupByResult != null) {
						var data = new List<GroupResult> ();
						GroupByResult.ForEach (r => data.Add (new GroupResult {
							Count = r.TotalItems,
							NumberLote = r.Data.NumberLote,
							Invima = r.Data.Invima,
							CodeMedic = r.Data.CodeMedic,
							indicatorV = r.Data.Maturing,
							FechaVencimiento = r.Data.ExpirationDate
						}));
						
						var grupCum = new AdapGrupCumDev (context, data, position, listaMedicamento, alertDialog);
						listViewGrup.Adapter = grupCum;	
					}

				}
				alertDialog.SetView (inflaterDes);
				alertDialog.Show ();
			};
				
			return convertView;
		}
		#endregion

	}

	#region GroupResult
	/// <summary>
	/// Group result.
	/// </summary>
	public class GroupResult
	{

		public int Count {
			get;
			set;
		}

		public String NumberLote {
			get;
			set;
		}

		public String CodeMedic {
			get;
			set;
		}

		public String Invima {
			get;
			set;
		}

		public bool indicatorV {
			get;
			set;
		}

		public DateTime? FechaVencimiento {
			get;
			set;
		}
			
	}
	#endregion

}


using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SCSAPP.Framework.Context;
using Android.Graphics;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Context;


namespace APPDroid.CarMed.Adapters
{
    public class AdapMedicines : BaseAdapter
    {
		#region Variables and Controls
		Activity context;
		MasterItem colletmez;
		List<Medicament> data;
		ListView listMedicine;
		readonly MasterItem colletmpv;
		string titleCauseDialog;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.CarMed.Adapters.AdapMedicines"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		/// <param name = "listMedicine"></param>
		public AdapMedicines(Activity context, List<Medicament> data, ListView listMedicine) 
        {
            this.context = context;
            this.data = data;
			colletmpv = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETMPV"));
			colletmez = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETMEZ"));
			this.listMedicine = listMedicine;
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
            convertView = inflater.Inflate(Resource.Layout.item_car_medicine, parent, false);

            TextView nombre = convertView.FindViewById<TextView>(Resource.Id.nombreMedicamento);
			nombre.Text = data[position].TradingName;

			TextView codeArticulo = convertView.FindViewById<TextView>(Resource.Id.codigoMedicamento);
			codeArticulo.Text = String.Format ("Código: {0}", data [position].Code);

			TextView cantidadInter = convertView.FindViewById<TextView>(Resource.Id.cantidadInterpretada);
			cantidadInter.Text = String.Format ("Cant. Interpretada: {0}", data[position].PendingAmount);  

			TextView fechaOrden = convertView.FindViewById<TextView>(Resource.Id.fechaOrde);
			fechaOrden.Text = string.Format ("{0:g}", data[position].OrderDate); 

			TextView unidad = convertView.FindViewById<TextView>(Resource.Id.unidaMedida);
			unidad.Text = String.Format ("{0}", data[position].UnitsOfMeasurement);

			TextView cantAlistada = convertView.FindViewById<TextView> (Resource.Id.cantidadAlistada);
			cantAlistada.Text = String.Format ("Cant. Alistada: {0}", data[position].AmountAlistada.ToString("N2"));

			ImageView StatusIndicator = convertView.FindViewById<ImageView> (Resource.Id.imgStatus);

			ImageView StatusNostatuLoad = convertView.FindViewById<ImageView> (Resource.Id.imgEditCar);

			StatusNostatuLoad.Click += (sender, e) => {

				var alerDialog = (new AlertDialog.Builder (context)).Create ();
				alerDialog.SetTitle (Resource.String.txt_alertas);

				titleCauseDialog = "¿Está seguro de descartar el medicamento?";
				if(!string.IsNullOrEmpty(ActivitiesContext.Context.listmedicament [position].ResponseCauses)){
					titleCauseDialog = "¿Está seguro de Editar la causa?";	
				}
				alerDialog.SetMessage (titleCauseDialog); 

				alerDialog.SetButton ("Si", delegate {
					ActivitiesContext.Context.PositionOnAdminister = position;
					var alertDialogCausaNo = (new AlertDialog.Builder (context)).Create ();
					var inflaterNoAdmi = context.LayoutInflater.Inflate (APPDroid.Framework.Resource.Layout.app_select_master, null);
					ListView ListView = inflaterNoAdmi.FindViewById<ListView> (APPDroid.Framework.Resource.Id.listViewEad);

					if (data [position].NonDispatchCauses.Count <= 0) {
						Toast.MakeText (context, "Medicamento no tiene causa", ToastLength.Long).Show ();
					} else {
						var listViewCausa = new AdapterCausaCar (context, data [position].NonDispatchCauses, alertDialogCausaNo, listMedicine);
						alertDialogCausaNo.SetTitle ("Seleccione Causa de No Despacho");
						ListView.Adapter = listViewCausa;
						alertDialogCausaNo.SetView (inflaterNoAdmi);
						alertDialogCausaNo.Show ();
					}
				});
				alerDialog.SetButton2 ("No", (s, ev) =>  {
					var dialog = s as AlertDialog;
					if (dialog != null) {
						dialog.Dismiss ();	
					}
				});

				alerDialog.Show ();

			};

			//Determinar si es mescla
			if(data[position].IsMixture){
				if (colletmez.Value != null && !String.IsNullOrEmpty (colletmez.Value)) {
					nombre.SetTextColor (Color.ParseColor (colletmez.Value));
					codeArticulo.SetTextColor (Color.ParseColor (colletmez.Value));
					cantidadInter.SetTextColor (Color.ParseColor (colletmez.Value));
					cantAlistada.SetTextColor (Color.ParseColor (colletmez.Value));
					fechaOrden.SetTextColor (Color.ParseColor (colletmez.Value));
					unidad.SetTextColor (Color.ParseColor (colletmez.Value));
				}
			}

			//Determinar si esta proximo vencerse.
			if(data[position].ExpiringSoon){
				if (colletmpv.Value != null && !String.IsNullOrEmpty (colletmpv.Value)) {
					nombre.SetTextColor (Color.ParseColor (colletmpv.Value));
					codeArticulo.SetTextColor (Color.ParseColor (colletmpv.Value));
					cantidadInter.SetTextColor (Color.ParseColor (colletmpv.Value));
					cantAlistada.SetTextColor (Color.ParseColor (colletmpv.Value));
					fechaOrden.SetTextColor (Color.ParseColor (colletmpv.Value));
					unidad.SetTextColor (Color.ParseColor (colletmpv.Value));
				}
			}

			if(ActivitiesContext.Context.listmedicament [position].PendingAmount == ActivitiesContext.Context.listmedicament [position].AmountAlistada)
				ActivitiesContext.Context.AdministerData [position] = (int)AdministerType.AdminManual;	

			SetImageToButton (StatusIndicator, StatusNostatuLoad, position);

			convertView.Click += (sender, e) => {
				
				var alertDialog = (new AlertDialog.Builder (context)).Create ();
				var inflaterDes = context.LayoutInflater.Inflate (Resource.Layout.dialog_detalle_medic, null);

				inflaterDes.FindViewById<TextView> (Resource.Id.txtNombreMedicament).Text = ActivitiesContext.Context.listmedicament [position].TradingName;
				inflaterDes.FindViewById<TextView> (Resource.Id.txtUnidadMedida).Text = ActivitiesContext.Context.listmedicament [position].UnitsOfMeasurement;
				inflaterDes.FindViewById<TextView> (Resource.Id.txtCantidad).Text = string.Format ("Cantidad interpretada {0}", ActivitiesContext.Context.listmedicament [position].PendingAmount);

				ListView listViewGrup = inflaterDes.FindViewById<ListView> (Resource.Id.listView);

				if (ActivitiesContext.Context.listmedicament [position].ListItemDetailCumM != null) {

					var GroupByResult = ActivitiesContext.Context.listmedicament [position].ListItemDetailCumM.GroupBy (n => new { n.NumberLote, n.CodeMedic, n.Invima, n.ExpirationDate})
						.Select (n => new { TotalItems = n.Count (), Data = n.Key }).OrderBy (n => n.TotalItems).ToList ();

					if (GroupByResult != null) {
						var data = new List<GroupResult> ();
						GroupByResult.ForEach (r => data.Add (new GroupResult {
							Count = r.TotalItems,
							NumberLote = r.Data.NumberLote,
							Invima = r.Data.Invima,
							CodeMedic = r.Data.CodeMedic,
							FechaVencimiento = r.Data.ExpirationDate
						}));

						var grupCum = new AdapGrupCum (context, data, position, listMedicine, alertDialog);
						listViewGrup.Adapter = grupCum;	
					}
				}
				alertDialog.SetView (inflaterDes);
				alertDialog.Show ();
			};

            return convertView;
		}
		#endregion

		#region SetImageToButton
		/// <summary>
		/// Sets the image to button.
		/// </summary>
		/// <param name = "btnStatus"></param>
		/// <param name = "btnEdit"></param>
		/// <param name="Pos">Position.</param>
		void SetImageToButton (ImageView btnStatus, ImageView btnEdit, int Pos){
			btnStatus.SetImageResource (Resource.Drawable.ic_action_record);	
			var Value = ActivitiesContext.Context.AdministerData [Pos];
	
			if (Value != (int)AdministerType.None) {
				if (Value == (int)AdministerType.NoAdmin) {
					btnStatus.SetImageResource (Resource.Drawable.no_cargar_d);
					btnStatus.Enabled = false;
				} else if (Value == (int)AdministerType.AdminManual || Value == (int)AdministerType.AdminBarCode) {
					btnStatus.SetImageResource (Resource.Drawable.completado_d);	
					btnStatus.Enabled = false;
					btnEdit.Enabled = false;
				}
			} else {
				btnStatus.SetImageResource (Resource.Drawable.ic_action_record);
			}
		}
		#endregion

    }

	#region GroupResult
	/// <summary>
	/// Group result.
	/// </summary>
	public class GroupResult{

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

		public DateTime? FechaVencimiento {
			get;
			set;
		}

	}
	#endregion

}
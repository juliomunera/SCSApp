
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages.Entities;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Context;

namespace APPDroid.DevMed.Adapters
{		
	public class AdapterCausaDev : BaseAdapter
	{
		#region Variables and Controls
		readonly Activity context;
		readonly List<NonDispatchCause> listCauses;
		readonly AlertDialog alertDialog;
		readonly int medicamento;
		readonly ListView listaMedicamento;
		readonly Medicament medicament;
		readonly bool incrementaNcausa;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.DevMed.Adapters.AdapterCausaDev"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="listCauses">List causes.</param>
		/// <param name="alertDialog">Alert dialog.</param>
		/// <param name="medicamento">Medicamento.</param>
		/// <param name="listaMedicamento">Lista medicamento.</param>
		/// <param name="medicament">Medicament.</param>
		/// <param name = "incrementaNcausa"></param>
		public AdapterCausaDev(Activity context, List<NonDispatchCause> listCauses, AlertDialog alertDialog, int medicamento, ListView listaMedicamento, Medicament medicament, bool incrementaNcausa){
			this.context = context;
			this.listCauses = listCauses;
			this.alertDialog = alertDialog;
			this.medicamento = medicamento;
			this.listaMedicamento = listaMedicamento;
			this.medicament = medicament;
			this.incrementaNcausa = incrementaNcausa;
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
			var inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			convertView = inflater.Inflate (APPDroid.Framework.Resource.Layout.items_master, parent, false);
			TextView nombre = convertView.FindViewById<TextView> (APPDroid.Framework.Resource.Id.EadNombre);
			nombre.Text = listCauses [position].Description;

			convertView.Click += (sender, e) => {
				
				ActivitiesContext.Context.listmedicament [medicamento].ResponseCauses = listCauses [position].Code;

				if(incrementaNcausa){
					ActivitiesContext.Context.listmedicament [medicamento].AmountAlistada++;

					if (ActivitiesContext.Context.listmedicament [medicamento].ListItemDetailCumM == null)
						ActivitiesContext.Context.listmedicament [medicamento].ListItemDetailCumM = new List<DetLotCumReg> ();

					ActivitiesContext.Context.listmedicament [medicamento].ListItemDetailCumM.Add (new DetLotCumReg {
						NumberLote = medicament.BatchNumber ?? String.Empty,  
						CodeMedic = medicament.CumCode ?? String.Empty,
						Invima = medicament.InvimaCode ?? String.Empty,
						lotNumber = 1,
						DocumentSourceEnter = medicament.OrderSource ?? String.Empty,
						DocumentEnter = medicament.OrderDocument,
						ServicesEnter = medicament.Warehouse ?? String.Empty,
					});
				}
				alertDialog.Dismiss ();
				listaMedicamento.Adapter = new AdapterMedDev (context, ActivitiesContext.Context.listmedicament, listaMedicamento);					
			};

			return convertView;
		}
		#endregion

	}

}


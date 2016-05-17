
using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

using AppDroid.Main.Activities;
using SCSAPP.Framework.Context;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Helpers;

namespace SCSAPP.Android.Adapters
{
	public class AdapEad : BaseAdapter{

		#region Variables
		readonly Context context;
		List<AdministrativeStructure> Eads;
		#endregion

		#region Constructor Method
		/// <summary>
		/// Initializes a new instance of the <see cref="SCSAPP.Android.Adapters.AdapEad"/> class.
		/// </summary>
		/// <param name="c">C.</param>
		/// <param name="Eads">Eads.</param>
		public AdapEad(Context c, List<AdministrativeStructure> Eads){
			this.context = c;
			this.Eads = Eads;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count{
			get { return Eads.Count;}
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
			//return (Java.Lang.Object)ContextApp.Instance.MenuOptions[position];
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
			//return ContextApp.Instance.MenuOptions[position].TagName;
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
			nombre.Text = Eads[position].Name;

			convertView.Click += (object sender, EventArgs e) => {
				
				ContextApp.Instance.SelectedEAD = Eads[position];
				DataBaseManager.InsertContext (DataBaseManager.IDContextType.ContextApp, ContextApp.GetContextSerialized ());

				Intent ventanaHome = new Intent (this.context, typeof(AppHomeList));
				this.context.StartActivity(ventanaHome);
			};

			return convertView;
		}
		#endregion

	}

}


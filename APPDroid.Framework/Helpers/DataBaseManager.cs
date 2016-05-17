
using System;
using System.IO;
using Mono.Data.Sqlite;

namespace APPDroid.Framework.Helpers
{		

	#region Main database
	public class DataBaseManager
	{
		
		#region MyVariables
		const string db_file = "servintebd.db3";
		#endregion

		#region Constantes
		public const string QUERY_CREATE_DB = "CREATE TABLE CONTEXTS (idContext INTEGER, context ntext); ";
		public const string QUERY_SELECT_TABLET = "SELECT * FROM CONTEXTS WHERE idContext = {0}";
		public const string QUERY_DELETE_FILE = "DELETE FROM CONTEXTS WHERE idContext = {0}";
		public const string QUERY_INSERT_TABLE = "INSERT INTO CONTEXTS (idContext, context) VALUES (@idContext, @context)";
		#endregion


		/// <summary>
		/// Connect to the database
		/// </summary>
		/// <returns>The connection.</returns>
		static SqliteConnection GetConnection ()
		{
			var dbPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), db_file);
			bool exists = File.Exists (dbPath);
			if (!exists)
				SqliteConnection.CreateFile (dbPath);

			var conn = new SqliteConnection ("Data Source=" + dbPath);

			if (!exists)
				CreateDatabase (conn);

			return conn;
		}


		/// <summary>
		/// Creates the database.
		/// </summary>
		/// <param name="connection">Connection.</param>
		static void CreateDatabase (System.Data.IDbConnection connection)
		{
			const string sql = QUERY_CREATE_DB;
			connection.Open ();
			using (var cmd = connection.CreateCommand ()) {
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery ();
			}
			connection.Close ();
		}

		/// <summary>
		/// Gets the contexts.
		/// </summary>
		/// <returns>The contexts.</returns>
		/// <param name="id">Identifier.</param>
		public static string GetContexts (IDContextType id)	{
			var sql = string.Format (QUERY_SELECT_TABLET, (int)id);
			using (var conn = GetConnection ()) {
				conn.Open ();
				using (var cmd = conn.CreateCommand ()) {
					cmd.CommandText = sql;
					using (var reader = cmd.ExecuteReader ()) {
						return reader.Read () ? reader.GetString (1) : string.Empty;
					}
				}
			}	
		}

		/// <summary>
		/// Deletes the context.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public static void DeleteContext (IDContextType id)
		{
			var sql = string.Format (QUERY_DELETE_FILE, (int)id);  

			using (var conn = GetConnection ()) {
				conn.Open ();

				using (var cmd = conn.CreateCommand ()) {
					cmd.CommandText = sql;
					cmd.ExecuteNonQuery ();
				}
			}
		}

		/// <summary>
		/// Inserts the context.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="valor">Valor.</param>
		public static void InsertContext (IDContextType id, string valor)
		{
			DeleteContext(id);

			const string sql = QUERY_INSERT_TABLE; 			
			using (var conn = GetConnection ()) {
				conn.Open ();

				using (var cmd = conn.CreateCommand ()) {
					cmd.CommandText = sql;
					cmd.Parameters.AddWithValue ("@idContext", (int)id);
					cmd.Parameters.AddWithValue ("@context", valor);
					cmd.ExecuteNonQuery ();
				}
			}
		}


		/// <summary>
		/// Identifier context type.
		/// </summary>
		public enum IDContextType
		{
			Lincence = 1,
			ContextApp = 2,
			Wcf = 3,
			imei = 4
		}


	}
	#endregion

}


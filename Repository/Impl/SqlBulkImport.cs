using System;
using System.IO;
using System.Data;
using PruebaAA.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace PruebaAA.Repository.Impl
{
  /// <summary>
  /// Clase que contiene los metodos que realiza la carga masiva a la tabla.
  /// </summary>
  public class SqlBulkImport: ISqlBulkImport
   {
      private readonly PruebaAAContext _db;
      public SqlBulkImport(PruebaAAContext db)
      {
      _db = db;
      }

      /// <summary>
      /// metodo que realiza la carga masiva de los registro leidos del archivo CSV.
      ///
      /// se ejeuta el procedimiento almacenado que recibe como parametro una colección de
      /// registros tipo DataTable.
      /// </summary>
      /// <param name="data2Bulk">(DataTable) colección de registro extraidos para cargar</param>
      public void BulkImport(DataTable data2Bulk)
      {
         DataTable data4sp = data2Bulk.Copy();
         var parm = new SqlParameter
         {
            ParameterName = "tblStockTableType",
            SqlDbType = SqlDbType.Structured,
            Direction = ParameterDirection.Input,
            TypeName = "dbo.tblTypeStock",
            Value = data4sp
         };
         string sqlSp = "[dbo].[spBulkImportStock] @tblStockTableType";
          _db.Database.ExecuteSqlRaw(sqlSp, parm);
    }

   }
}
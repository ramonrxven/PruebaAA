using System.Data;
using System;
using System.Collections.Generic;

namespace PruebaAA.Repository
{

    /// <summary>
    /// Interfaz que expone los metodos de la clase que realiza la carga masiva a la tabla.
    /// </summary>
    interface ISqlBulkImport
    {

      /// <summary>
      /// metodo que realiza la carga masiva de los registro leidos del archivo CSV.
      /// </summary>
      /// <param name="data2Bulk">(DataTable) colección de registro extraidos para cargar</param>
      void BulkImport(DataTable data2Bulk);
    }
}
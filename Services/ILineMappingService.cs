using System.Data;
using PruebaAA.Models;

namespace PruebaAA.Services
{

   /// <summary>
   /// Interface para la Clase de Mapping de los registro a la tabla.
   /// </summary>
   interface ILineMappingService
   {

      /// <summary>
      /// Metodo que mapea la linea leida del archivo CSV.
      ///
      /// este metodo toma la línea leída del archivo en string,
      /// separa el string según el separador de campos configurado en la aplicación,
      ///  convierte el string en un array y mapea el array en una instanacia del modelo StockModel.
      /// </summary>
      /// <param name="stockLine">string línea leída del archivo.</param>
      /// <returns>retorna una instancia de StockModel</returns>
      StockModel MappStockLine(string stockLine);


      object[] MappStockLine4Bulk(string stockLine);
   }

}
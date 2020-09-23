using System.Data;
using System;
using System.Reflection;
using PruebaAA.Models;
using Microsoft.Extensions.Options;

namespace PruebaAA.Services.Impl
{
  /// <summary>
  /// Implementación de la clase ILineMappingService.
  ///
  /// Esta Clase contiene los metodos para realizar el mapeo de los datos,
  /// recibidos en la linea leida del archivo CSV.
  /// </summary>
  class LineMappingService: ILineMappingService
    {
      private readonly Config _config;
      private DataTable stock4bulk = new DataTable();

      /// <summary>
      /// /// Constructor de la clase para inyectar la clase de configuración de la app.
      /// </summary>
      /// <param name="config">instancia de la Configuración de la app inyectada en esta clase.</param>
      public LineMappingService(IOptions<Config> config)
      {
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));

        stock4bulk.Columns.Add(new DataColumn("PointOfSale", typeof(string)));
        stock4bulk.Columns.Add(new DataColumn("Product", typeof(string)));
        stock4bulk.Columns.Add(new DataColumn("Date", typeof(string)));
        stock4bulk.Columns.Add(new DataColumn("Stock", typeof(float)));
      }


      /// <summary>
      /// Mapea la linea leida del archivo en una instancia del modelo Stock.
      ///
      ///este proceso toma stockLine(string), lo parsea a un arreglo y luego pasa los valores
      /// del arreglo y los asigna a las propiedas correspondientes en la instancia de stock,
      /// segun la configuración de correspondencia entre campos y columnas del archivo csv.
      /// </summary>
      /// <param name="stockLine">linea leida del archivo</param>
      /// <returns>una instancia de StockModel</returns>
      public StockModel MappStockLine(string stockLine)
      {
        String[] stockData = stockLine.Split(_config.FieldSeparator);
        StockModel stock = new StockModel();
        Type stockType = stock.GetType();
        foreach (var fc in _config.FileColumns)
        {
            stockType.GetProperty(fc.Name).SetValue(stock, new Castering(new CastData(fc.Type, stockData[fc.Position])).GetValueCast());
        }
        return stock;
      }

      /// <summary>
      /// Clase modelo para recibir los datos en el metodo de Castering.
      /// </summary>
      class CastData
      {
        public string typecast;
        public string datacast;

        /// <summary>
        /// Constructor de la clase.
        ///
        /// en este constructor se asignan los valores para convertir.
        /// </summary>
        /// <param name="typeCast">(string)tipo de dato deseado para convertir</param>
        /// <param name="dataCast">(string) dato para convertir</param>
        public CastData(string typeCast, string dataCast)
        {
          typecast = typeCast;
          datacast = dataCast;
        }
      }

      /// <summary>
      /// Clase que convierte los tipos de datos recibidos la linea del archivo CSV.
      ///
      /// esta clase solo convierte el tipo String a Int32, ya que este conversión,
      /// no es implicita, como string a Date.
      /// </summary>
      class Castering
      {
        private CastData _obj;
        public Castering(CastData obj)
        {
          _obj = obj;
        }

        /// <summary>
        /// devuelve el valor convertido.
        ///
        /// devuelve el valor convertido al tipo solicitado
        /// solo afecta al tipo Int32.
        ///  </summary>
        /// <returns>(object) valor convertido.</returns>
        public object GetValueCast()
        {
          object objr = new {};
          switch (_obj.typecast)
          {
            case "Int32":
             objr = Int16.Parse(_obj.datacast);
             break;
            default:
             objr = _obj.datacast;
             break;
          }
          return objr;
        }
      }

    public object[] MappStockLine4Bulk(string stockLine)
    {

      String[] stockData = stockLine.Split(_config.FieldSeparator);
      StockModel stock = new StockModel();
      Type stockType = stock.GetType();
      DataRow stockRow = stock4bulk.NewRow();
      foreach (var fc in _config.FileColumns)
      {
        stockRow[fc.Name] = new Castering(new CastData(fc.Type, stockData[fc.Position])).GetValueCast();
      }
      return stockRow.ItemArray;
    }
  }
}
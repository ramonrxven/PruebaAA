using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using PruebaAA.Models;
using Microsoft.Extensions.Options;


namespace PruebaAA.Services.Impl
{
  /// <summary>
  /// Implementacion de la clase ReadFileService.
  ///
  /// Los metodos provistos en esta clase permiten abrir y validar el archivo CSV,
  /// se revisa que la estructura cumpla con lo requerido por la tabla de la base
  /// de datos para poder ser importado.
  ///
  /// proporcionada por la configuracion la linea que contiene el header del archivo,
  /// se asume que apartir de la siguiente linea comienzan los datos.
  /// </summary>
  class ReadFileService : IReadFileService
  {
    /// <summary>
    /// Instancia de la configuración de la aplicación
    /// </summary>
    private readonly Config _config;
    // private readonly IImportService _import;

    /// <summary>
    /// stream del recurso
    /// </summary>
    private StreamReader file = null;

    /// <summary>
    /// constructor de la implementación para la inyección de dependencias
    /// </summary>
    /// <param name="config">instancia de la clase configuracion que seinyecta en esta implementación</param>
    public ReadFileService(IOptions<Config> config)
    {
      _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
    }


    /// <summary>
    /// Abre el recurso a leer
    /// </summary>
    /// <returns>devuelve una instancia WebRequest o null si no puede abrir el recurso/returns>
    private WebRequest GetFile(){
      try
      {
        return  WebRequest.Create(@_config.RemoteSource);
      }
      catch (System.Exception)
      {
          Console.WriteLine($"ERROR: Ocurrrió un Error al tratar de acceder a la fuente: {_config.RemoteSource}");
          return null;
      }
    }


    /// <summary>
    /// abre el archivo CSV dependiendo de la configuración establecida.
    /// 
    /// el archivo puede ser abierto desde una ubicación local o una Remota.
    /// dependiendo del setting establecido para [IsRemoteSource](true/false) en appsetting.json
    /// </summary>
    /// <returns>instancia de StreamReder para acceder linea a linea</returns>
    private StreamReader OpenFile()
    {
      if (_config.IsRemoteSource)
      {
        return OpenRemoteFile();
      }else{
        return OpenLocalFile();
      }
    }


    /// <summary>
    /// Abre el archivo y devuelve un HttpBaseStream para la lectura del Archivo directo desde su ubicación remota.
    ///
    ///
    /// al no poder mover el apuntador del archivo con los metodos Seek o Position, por ser un
    /// archivo HttpBaseStream, la estrategia para poder recorrer el archivo desde el comienzo
    /// es cerrarlo y abrirlo nuevamente.
    /// </summary>
    private StreamReader OpenRemoteFile()
    {
      Console.WriteLine($"El Archivo en proceso: {_config.RemoteSource}");
      var webRequest = GetFile();
      if ( webRequest != null ){
        var response = webRequest.GetResponse();
        var content = response.GetResponseStream();
        file = new StreamReader(content);
        return  file;
      }else{
        return null;
      }
    }


    /// <summary>
    /// Abre el archivo de forma local y devuelve un stream para la lectura del archivo CSV.
    /// 
    /// </summary>
    private StreamReader OpenLocalFile()
    {
      try
      {
        Console.WriteLine($"El Archivo en proceso: {_config.LocalSource}");
          file = new StreamReader(_config.LocalSource);
          return  file;
      }
      catch (System.Exception)
      {
        Console.WriteLine($"ERROR: Ocurrrió un Error al tratar de abrir el Archivo: {_config.LocalSource}");
          return null;
      }
    }


    /// <summary>
    /// Validar que la estructura del archivo CSV a procesar es la esperada.
    ///
    /// se utilizan los valores establecidos en la configuración para establecer los
    /// parametros de verificación.
    ///
    /// valida que el recurso:
    /// - contenga un encabezado
    /// - que el encabezado tenga la cantidad de campos especificada en la configuración
    /// </summary>
    /// <returns>retorna true/false resultado de la validación</returns>
    public Boolean ValidateFile()
    {
      Console.WriteLine("Validando estructura del archivo...");
       return ValidateHeader();
    }


    /// <summary>
    /// valida que el encabezado del archivo sea correcto
    /// </summary>
    /// <returns>devuelve un valor boolean, resultado de evaluar si es valido el header del archivo</returns>
    private bool ValidateHeader(){
      Console.WriteLine("Validando Encabezado");
      String[] header = GetHeader();
      return ( header.Length >= _config.FileColumns.Count);
      // return false;
    }


    /// <summary>
    /// devuelve el header del archivo CSV donde debe estar el nombre de los campos.
    /// </summary>
    private  String[] GetHeader()
    {
        Console.WriteLine("Mostrando el encabezado del archivo");
        String ln = GetLine(_config.RowContainFieldLabel);
        Console.WriteLine($"Header: {ln}");
        return  SplitColumn(ln);
    }


    /// <summary>
    /// devuelve una linea especifica del archivo
    /// </summary>
    /// <param name="line4Return">número de linea que devolverá (int)</param>
    /// <returns>devuelve string de la linea solicitada</returns>
    private String GetLine(int line4Return)
    {
      string ln = null;

      // OpenFile();
      foreach (var line in LineRunner(line4Return))
      {
        ln = line;
      }
      // file.Close();
      return ln;
    }


    /// <summary>
    /// recorre todo el archivo abierto línea por línea de forma iterativa.
    ///
    /// el metodo acepta parametros para delimitar el recorrido hasta una linea especifica.
    /// </summary>
    /// <param name="max">valor int, que indica el número de la línea hasta donde recorrerá.</param>
    /// <returns>devuelve un string con la linea especificada en position o null, si el recorrido del archivo es total.</returns>
    public  IEnumerable<String> LineRunner(int? max=null){
      int counter = 0;
      string ln = null;
      OpenFile();
      if (file != null)
      {
        while ((ln = file.ReadLine()) != null)
        {
          yield return ln;
          counter++;
          if ( max.HasValue && counter >= max.Value ) {
            Console.WriteLine($"Breaking in : {counter}");
            break;
          }
        }
        file.Close();
      }
    }


    /// <summary>
    /// separa la linea del archivo en columnas segun el separador
    /// </summary>
    /// <param name="ln">(String)Línea del recurso</param>
    /// <returns></returns>
    private String[] SplitColumn(string ln)
    {
      return ln.Split(_config.FieldSeparator);
    }

  }
}
using System;
using System.Collections.Generic;

namespace PruebaAA.Services
{
  /// <summary>
  /// Clase que Abre el archivo CSV.
  ///
  /// Abre el archivo y valida que se pueda procesar
  /// </summary>
  interface IReadFileService
  {


    /// <summary>
    /// Procesa el archivo CSV registro a registro y lo almacena en la tabla designada
    /// </summary>
    // void ProcessFile();


    /// <summary>
    /// recorre todo el archivo abierto línea por línea de forma iterativa.
    ///
    /// el metodo acepta parametros para delimitar el recorrido hasta una linea especifica.
    /// </summary>
    /// <param name="max">valor int, que indica el número de la línea hasta donde recorrerá.</param>
    /// <returns>devuelve un string con la linea especificada en position o null, si el recorrido del archivo es total.</returns>
    IEnumerable<String> LineRunner(int? max = null);



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
    public Boolean ValidateFile();
  }
}
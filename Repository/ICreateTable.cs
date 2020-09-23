
namespace PruebaAA.Repository
{
    /// <summary>
    /// Interfaz para la publicación de los clase que contiene el metodo que comprueba 
    /// si existe la tabla y el Stored Procedure requeridos para cargar el archivo CSV.
    /// </summary>
    interface ICreateTable
    {
        /// <summary>
        /// verifica que exista la tabla y el SP necesario para cargar los datos del archivo CSV.
        /// 
        /// si no existe la tabla y/o el SP, el metodo lo creará
        /// </summary>
        void PrepareDatabase();
        
    }
    
}
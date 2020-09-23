using Microsoft.EntityFrameworkCore;

namespace PruebaAA.Models
{
  /// <summary>
  /// Clase Context para establecer el contexto de datos para el Modelo StockModel.
  /// </summary>
  public class PruebaAAContext : DbContext
  {
    public PruebaAAContext(DbContextOptions<PruebaAAContext> options) : base(options)
    {
    }

    public DbSet<StockModel> Stocks { get; set; }

  }
}
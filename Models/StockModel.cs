using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PruebaAA.Models
{

  /// <summary>
  /// Entity para la tabla Stock
  /// </summary>
  [Table("StockTmp")]
  public class StockModel
  {
    public long Id { get; set; }

    [Required]
    public string PointOfSale { get; set; }

    [Required]
    public string Product { get; set; }

    public string Date { get; set; }

    public int Stock { get; set; }
  }

}
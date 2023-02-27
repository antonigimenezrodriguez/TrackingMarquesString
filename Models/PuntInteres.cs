using SQLite;

namespace TrackingMarques.Models
{
    [Table("PuntsInteres")]
    public class PuntInteres : PuntBase
    {
        public string Nom { get; set; }
    }
}

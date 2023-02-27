using SQLite;

namespace TrackingMarques.Models
{
    [Table("Rutes")]
    public class Ruta
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }
        public string Nom { get; set; }
        public List<PuntRuta> PuntsRuta { get; set; }
        public List<PuntInteres> PuntsInteres { get; set; }
    }
}

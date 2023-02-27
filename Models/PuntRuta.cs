using SQLite;

namespace TrackingMarques.Models
{
    [Table("PuntsRuta")]
    public class PuntRuta : PuntBase
    {
        public PuntRuta(double latitud, double longitud, double? elevacio, DateTime dataHora, int rutaId)
            : base(latitud, longitud, elevacio, dataHora, rutaId)
        { }

        public PuntRuta() { }
    }
}

using SQLite;

namespace TrackingMarques.Models
{
    [Table("PuntsInteres")]
    public class PuntInteres : PuntBase
    {
        public string Nom { get; set; }

        public PuntInteres(double latitud, double longitud, double? elevacio, DateTime dataHora, int rutaId, string nom)
            : base(latitud, longitud, elevacio, dataHora, rutaId)
        {
            Nom = nom;
        }
        public PuntInteres() : base() { }
    }
}

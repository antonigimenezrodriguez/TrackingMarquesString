using SQLite;

namespace TrackingMarques.Models
{
    [Table("PuntsBase")]
    public class PuntBase
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double? Elevacio { get; set; }
        public DateTime DataHora { get; set; }

        
        public int RutaId { get; set; }

        public PuntBase(double latitud, double longitud, double? elevacio, DateTime dataHora, int rutaId)
        {
            Latitud = latitud;
            Longitud = longitud;
            Elevacio = elevacio;
            DataHora = dataHora;
            RutaId = rutaId;
        }
        public PuntBase() { }
    }
}

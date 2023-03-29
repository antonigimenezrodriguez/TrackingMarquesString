using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingMarques.Models;

namespace TrackingMarques.Domini
{
    public interface IDominiRuta
    {
        public void CrearTaules();
        public Task<int> InsertarNovaRuta();
        public Task InsertarPuntRuta(int rutaId);
        public Task InsertarPuntInteres(int rutaId, string nom);
        public Task<Ruta> ObtenirRutaNoFinalitzada();
        public Task<List<PuntInteres>> ObtenirPuntsInteresRuta(int rutaId);
        public Task<List<PuntRuta>> ObtenirPuntsRutaRuta(int rutaId);
        public Task<bool> FinalitzarRuta(int rutaId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingMarques.Models;

namespace TrackingMarques.Dades.Repositori.RutaRepository
{
    public interface IRutaRepository
    {
        public Task<Ruta> RecuperarRutaNoFinalitzada();
        public Task<Ruta> ObtenirRuta(int rutaId);
        public Task<List<Ruta>> ObtenirRutes();
        public Task ActualitzarRuta(Ruta ruta);
        public Task CrearTaulaRuta();
        public Task<int> BorrarRuta(Ruta ruta);
        public Task<int> InsertarRuta(Ruta ruta);
    }
}

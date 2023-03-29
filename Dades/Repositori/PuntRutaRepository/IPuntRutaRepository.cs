using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingMarques.Models;

namespace TrackingMarques.Dades.Repositori.PuntRutaRepository
{
    public interface IPuntRutaRepository
    {
        public Task<List<PuntRuta>> ObtenirPuntsRutaRuta(int rutaId);
        public Task<int> GuardarPuntRuta(PuntRuta puntRuta);
        public Task CrearTaulaPuntRuta();
        public Task<int> BorrarPuntRuta(PuntRuta puntRuta);
    }
}

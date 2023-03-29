using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingMarques.Models;

namespace TrackingMarques.Dades.Repositori.PuntInteresRepository
{
    public interface IPuntInteresRepository
    {
        public Task<List<PuntInteres>> ObtenirPuntsInteresRuta(int rutaId);
        public Task<int> GuardarPuntInteres(PuntInteres puntInteres);
        public Task CrearTaulaPuntInteres();
        public Task<int> BorrarPuntInteres(PuntInteres puntInteres);
    }
}

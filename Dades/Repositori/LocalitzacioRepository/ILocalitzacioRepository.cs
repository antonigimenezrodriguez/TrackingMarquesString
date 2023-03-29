using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingMarques.Dades.Repositori.LocalitzacioRepository
{
    public interface ILocalitzacioRepository
    {
        public Task<Location> ObtenirLocalitzacio();
    }
}

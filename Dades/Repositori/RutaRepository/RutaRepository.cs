using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingMarques.Models;

namespace TrackingMarques.Dades.Repositori.RutaRepository
{
    public class RutaRepository : IRutaRepository
    {
        private SQLiteAsyncConnection sqliteConnection;
        public RutaRepository()
        {
            this.sqliteConnection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }
        public async Task<int> BorrarRuta(Ruta ruta)
        {
            return await sqliteConnection.DeleteAsync(ruta);
        }

        public async Task CrearTaulaRuta()
        {
            await sqliteConnection.CreateTableAsync<Ruta>();
        }

        public async Task ActualitzarRuta(Ruta ruta)
        {
            await sqliteConnection.UpdateAsync(ruta);
        }

        public async Task<int> InsertarRuta(Ruta ruta)
        {
            return await sqliteConnection.InsertAsync(ruta);
        }

        public async Task<Ruta> ObtenirRuta(int rutaId)
        {
            return await sqliteConnection.Table<Ruta>().Where(w => w.Id == rutaId).FirstOrDefaultAsync();
        }

        public async Task<List<Ruta>> ObtenirRutes()
        {
            return await sqliteConnection.Table<Ruta>().ToListAsync();
        }

        public async Task<Ruta> RecuperarRutaNoFinalitzada()
        {
            return await sqliteConnection.Table<Ruta>().Where(w => !w.Finalitzada).FirstOrDefaultAsync();
        }
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingMarques.Models;

namespace TrackingMarques.Dades.Repositori.PuntInteresRepository
{
    public class PuntInteresRepository : IPuntInteresRepository
    {
        private SQLiteAsyncConnection sqliteConnection;

        public PuntInteresRepository()
        {
            this.sqliteConnection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }
        public async Task<int> BorrarPuntInteres(PuntInteres puntInteres)
        {
            return await sqliteConnection.DeleteAsync(puntInteres); 
        }

        public async Task CrearTaulaPuntInteres()
        {
            await sqliteConnection.CreateTableAsync<PuntInteres>();
        }

        public async Task<int> GuardarPuntInteres(PuntInteres puntInteres)
        {
            return await sqliteConnection.InsertAsync(puntInteres);
        }

        public async Task<List<PuntInteres>> ObtenirPuntsInteresRuta(int rutaId)
        {
            return await sqliteConnection.Table<PuntInteres>().Where(w => w.RutaId == rutaId).ToListAsync();
        }

    }
}

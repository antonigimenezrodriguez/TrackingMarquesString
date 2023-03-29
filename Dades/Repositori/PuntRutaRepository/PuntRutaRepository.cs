using SQLite;
using TrackingMarques.Models;

namespace TrackingMarques.Dades.Repositori.PuntRutaRepository
{
    public class PuntRutaRepository : IPuntRutaRepository
    {
        private SQLiteAsyncConnection sqliteConnection;

        public PuntRutaRepository()
        {
            this.sqliteConnection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public async Task<int> BorrarPuntRuta(PuntRuta puntRuta)
        {
            return await sqliteConnection.DeleteAsync(puntRuta);
        }

        public async Task CrearTaulaPuntRuta()
        {
            await sqliteConnection.CreateTableAsync<PuntRuta>();
        }

        public async Task<int> GuardarPuntRuta(PuntRuta puntRuta)
        {
            return await sqliteConnection.InsertAsync(puntRuta);
        }

        public async Task<List<PuntRuta>> ObtenirPuntsRutaRuta(int rutaId)
        {
            return await sqliteConnection.Table<PuntRuta>().Where(w => w.RutaId == rutaId).ToListAsync();
        }
    }
}

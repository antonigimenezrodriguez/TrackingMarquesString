using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingMarques.Dades.Repositori.LocalitzacioRepository
{
    public class LocalitzacioRepository : ILocalitzacioRepository
    {
        GeolocationRequest request;
        CancellationTokenSource cancelTokenSource;

        public LocalitzacioRepository()
        {
            request = new GeolocationRequest(GeolocationAccuracy.Best, Constants.TimeOutLocation);
            cancelTokenSource = new CancellationTokenSource();
        }
        public async Task<Location> ObtenirLocalitzacio()
        {
            return await Geolocation.Default.GetLocationAsync(request, cancelTokenSource.Token);
        }
    }
}

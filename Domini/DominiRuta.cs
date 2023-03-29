using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingMarques.Dades.Repositori.LocalitzacioRepository;
using TrackingMarques.Dades.Repositori.PuntInteresRepository;
using TrackingMarques.Dades.Repositori.PuntRutaRepository;
using TrackingMarques.Dades.Repositori.RutaRepository;
using TrackingMarques.Models;

namespace TrackingMarques.Domini
{
    public class DominiRuta : IDominiRuta
    {
        public IRutaRepository RutaRepository { get; set; }
        public IPuntRutaRepository PuntRutaRepository { get; set; }
        public IPuntInteresRepository PuntInteresRepository { get; set; }
        public ILocalitzacioRepository LocalitzacioRepository { get; set; }
        NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };
        public DominiRuta()
        {
            this.RutaRepository = new RutaRepository();
            this.PuntRutaRepository = new PuntRutaRepository();
            this.PuntInteresRepository = new PuntInteresRepository();
            this.LocalitzacioRepository = new LocalitzacioRepository();
        }

        public async void CrearTaules()
        {
            await RutaRepository.CrearTaulaRuta();
            await PuntRutaRepository.CrearTaulaPuntRuta();
            await PuntInteresRepository.CrearTaulaPuntInteres();
        }

        public async Task<int> InsertarNovaRuta()
        {
            List<Ruta> rutes = await RutaRepository.ObtenirRutes();

            List<Ruta> rutesAEsborrar = rutes.Where(w => !w.Finalitzada).ToList();

            foreach (Ruta rutaAEsborrar in rutesAEsborrar)
            {
                int res = await RutaRepository.BorrarRuta(rutaAEsborrar);

                List<PuntInteres> puntsInteresAEsborrar = await PuntInteresRepository.ObtenirPuntsInteresRuta(rutaAEsborrar.Id);

                foreach (PuntInteres puntInteres in puntsInteresAEsborrar)
                {
                    res = await PuntInteresRepository.BorrarPuntInteres(puntInteres);
                }

                List<PuntRuta> puntsRutaAEsborrar = await PuntRutaRepository.ObtenirPuntsRutaRuta(rutaAEsborrar.Id);

                foreach (PuntRuta puntRuta in puntsRutaAEsborrar)
                {
                    res = await PuntRutaRepository.BorrarPuntRuta(puntRuta);
                }
            }

            Location location = await LocalitzacioRepository.ObtenirLocalitzacio();

            DateTime ara = DateTime.Now;
            Ruta ruta = new Ruta();
            ruta.Nom = "Ruta";
            ruta.DataHora = ara;
            ruta.Finalitzada = false;

            int result = await RutaRepository.InsertarRuta(ruta);

            rutes = await RutaRepository.ObtenirRutes();

            Ruta rutaDB = rutes.Where(w => w.DataHora == ara).FirstOrDefault();

            return rutaDB.Id;
        }

        public async Task InsertarPuntRuta(int rutaId)
        {
            Location location = await LocalitzacioRepository.ObtenirLocalitzacio();
            PuntRuta puntRuta = new PuntRuta(location.Latitude, location.Longitude, location.Altitude, DateTime.Now, rutaId);
            int result = await PuntRutaRepository.GuardarPuntRuta(puntRuta);
        }

        public async Task InsertarPuntInteres(int rutaId, string nom)
        {
            Location location = await LocalitzacioRepository.ObtenirLocalitzacio();
            PuntInteres puntInteres = new PuntInteres(location.Latitude, location.Longitude, location.Altitude, DateTime.Now, rutaId, nom);
            int result = await PuntInteresRepository.GuardarPuntInteres(puntInteres);
        }

        public async Task<Ruta> ObtenirRutaNoFinalitzada()
        {
            return await RutaRepository.RecuperarRutaNoFinalitzada();
        }

        public async Task<List<PuntInteres>> ObtenirPuntsInteresRuta(int rutaId)
        {
            return await PuntInteresRepository.ObtenirPuntsInteresRuta(rutaId);
        }

        public async Task<List<PuntRuta>> ObtenirPuntsRutaRuta(int rutaId)
        {
            return await PuntRutaRepository.ObtenirPuntsRutaRuta(rutaId);
        }

        public async Task<bool> FinalitzarRuta(int rutaId)
        {
            Ruta ruta = await RutaRepository.ObtenirRuta(rutaId);
            if (ruta != null)
            {
                ruta.Finalitzada = true;

                List<PuntInteres> puntsInteres = await PuntInteresRepository.ObtenirPuntsInteresRuta(rutaId);
                List<PuntRuta> puntsRuta = await PuntRutaRepository.ObtenirPuntsRutaRuta(rutaId);

                string xml = string.Empty;
                xml = "<root>\r\n";
                xml = xml + "<puntsRuta>\r\n";
                foreach (PuntRuta puntRuta in puntsRuta)
                {
                    xml = xml + "<ruta>" + "\r\n";
                    xml = xml + "<latitud>" + puntRuta.Latitud.ToString(nfi) + "</latitud>" + "\r\n";
                    xml = xml + "<longitud>" + puntRuta.Longitud.ToString(nfi) + "</longitud>" + "\r\n";
                    xml = xml + "<elevacio>" + puntRuta.Elevacio?.ToString(nfi) + "</elevacio>" + "\r\n";
                    xml = xml + "<dataHora>" + puntRuta.DataHora.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
                    xml = xml + "</ruta>" + "\r\n";
                }
                xml = xml + "</puntsRuta>\r\n";

                xml = xml + "<puntsInteres>\r\n";
                foreach (PuntInteres puntInteres in puntsInteres)
                {
                    xml = xml + "<punt>" + "\r\n";
                    xml = xml + "<nom>" + puntInteres.Nom + "</nom>" + "\r\n";
                    xml = xml + "<latitud>" + puntInteres.Latitud.ToString(nfi) + "</latitud>" + "\r\n";
                    xml = xml + "<longitud>" + puntInteres.Longitud.ToString(nfi) + "</longitud>" + "\r\n";
                    xml = xml + "<elevacio>" + puntInteres.Elevacio?.ToString(nfi) + "</elevacio>" + "\r\n";
                    xml = xml + "<dataHora>" + puntInteres.DataHora.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
                    xml = xml + "</punt>" + "\r\n";
                }
                xml = xml + "</puntsInteres>\r\n";
                xml = xml + "</root>\r\n";

                string fitxer = $"ruta_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
                using var stream = new MemoryStream(Encoding.Default.GetBytes(xml));

                try
                {
                    if (!OperatingSystem.IsAndroidVersionAtLeast(33))
                    {
                        var fileLocation = await FileSaver.Default.SaveAsync($"{fitxer}.{Constants.ExtensioFitxer}", stream, new CancellationTokenSource().Token);
                    }
                    else
                    {
                        File.WriteAllText($"{Constants.RutaFitxer}{fitxer}.{Constants.ExtensioFitxer}", xml);
                    }
                    await RutaRepository.ActualitzarRuta(ruta);
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(false);
                }

                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
    }
}

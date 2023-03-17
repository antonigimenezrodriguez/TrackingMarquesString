using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using SQLite;
using System.Globalization;
using System.Text;
using TrackingMarques.Models;

namespace TrackingMarques;

public partial class MainPage : ContentPage
{

    int numeroDePuntsInteres = 0;
    int numeroDePuntsRuta = 0;
    private CancellationTokenSource _cancelTokenSource;
    int rutaId = 0;
    SQLiteAsyncConnection conn = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
    NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

    public MainPage()
    {
        numeroDePuntsInteres = 0;
        numeroDePuntsRuta = 0;
        InitializeComponent();
        CrearTaules();
        ActualitzarLabelsContadors(null, null);
        VisibilitatBotoRecuperarRuta();
    }

    private async void IniciBtn_Clicked(object sender, EventArgs e)
    {
        bool resposta = await DisplayAlert("Iniciar nova ruta?", "Vols iniciar una nova ruta? Qualsevol progrés es perdrà", "Sí", "No");
        if (resposta)
        {
            IniciBtn.IsEnabled = false;
            CrearTaules();
            ActualitzarLabelsContadors(0, 0);
            await InsertarNovaRutaBD();
            await AnyadirPuntRuta();
            PuntInteresBtn.IsEnabled = true;
            PuntRutaBtn.IsEnabled = true;
            RecuperarBtn.IsEnabled = false;
            IniciBtn.IsEnabled = true;
        }
    }

    private async void PuntInteresBtn_Clicked(object sender, EventArgs e)
    {
        DesactivarTotsElsBotons();
        await AnyadirPuntInteres(PuntInteresEntry.Text);
        VisibilitatBotonsDespresAfegirPunts();
    }
    private async void PuntRutaBtn_Clicked(object sender, EventArgs e)
    {
        DesactivarTotsElsBotons();
        await AnyadirPuntRuta();
        VisibilitatBotonsDespresAfegirPunts();
    }
    private async void FinalBtn_Clicked(object sender, EventArgs e)
    {
        bool resposta = await DisplayAlert("Iniciar nova ruta?", "Vols finalitzar la ruta? Es guardarà el fitxer XML i es començarà de nou", "Sí", "No");
        if (resposta)
        {
            DesactivarTotsElsBotons();
            await AnyadirPuntRuta();
            await FinalitzarRuta();
            IniciBtn.IsEnabled = true;
        }
    }

    private async void RecuperarBtn_Clicked(object sender, EventArgs e)
    {
        CrearTaules();
        Ruta ruta = await conn.Table<Ruta>().Where(w => !w.Finalitzada).FirstOrDefaultAsync();
        if (ruta != null)
        {
            rutaId = ruta.Id;
            List<PuntInteres> puntsInteres = await conn.Table<PuntInteres>().Where(w => w.RutaId == ruta.Id).ToListAsync();
            List<PuntRuta> puntsRuta = await conn.Table<PuntRuta>().Where(w => w.RutaId == ruta.Id).ToListAsync();
            int numeroPuntsInteres = puntsInteres.Count;
            int numeroPuntsRuta = puntsRuta.Count;
            ActualitzarLabelsContadors(numeroPuntsInteres, numeroPuntsRuta);
            VisibilitatBotonsDespresAfegirPunts();
            RecuperarBtn.IsEnabled = false;
            await Toast.Make($"S'ha recuperat la ruta amb data d'inici: {ruta.DataHora}").Show();
        }
        else
        {
            await Toast.Make($"No s'ha pogut recuperar cap ruta").Show();
            RecuperarBtn.IsEnabled = false;
        }
    }

    private async Task FinalitzarRuta()
    {
        Ruta ruta = await conn.Table<Ruta>().Where(w => w.Id == rutaId).FirstOrDefaultAsync();
        if (ruta != null)
        {
            ruta.Finalitzada = true;

            List<PuntInteres> puntsInteres = await conn.Table<PuntInteres>().Where(w => w.RutaId == ruta.Id).ToListAsync();
            List<PuntRuta> puntsRuta = await conn.Table<PuntRuta>().Where(w => w.RutaId == ruta.Id).ToListAsync();

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
            _cancelTokenSource = new CancellationTokenSource();
            try
            {
                //File.WriteAllText($"{Constants.RutaFitxer}{fitxer}.{Constants.ExtensioFitxer}" , xml);
                var fileLocation = await FileSaver.Default.SaveAsync($"{fitxer}.{Constants.ExtensioFitxer}", stream, _cancelTokenSource.Token);
                await Toast.Make($"Fitxer guardat correctament", CommunityToolkit.Maui.Core.ToastDuration.Long).Show(_cancelTokenSource.Token);
                await conn.UpdateAsync(ruta);
            }
            catch (Exception ex)
            {
                await Toast.Make($"El fitxer no s'ha pogut guardar. Recupera la ruta i prova un altre directori", CommunityToolkit.Maui.Core.ToastDuration.Long).Show(_cancelTokenSource.Token);
            }
            VisibilitatBotoRecuperarRuta();
            ActualitzarLabelsContadors(0, 0);
        }
    }

    private void VisibilitatBotoRecuperarRuta()
    {
        CrearTaules();
        Ruta rutaNoFinalitzada = conn.Table<Ruta>().Where(w => !w.Finalitzada).FirstOrDefaultAsync().Result;
        if (rutaNoFinalitzada != null)
            RecuperarBtn.IsEnabled = true;
        else
            RecuperarBtn.IsEnabled = false;
    }

    private async Task AnyadirPuntInteres(string nom)
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        await AnyadirPuntInteresDB(location, nom);

        ActualitzarLabelsContadors(++numeroDePuntsInteres, null);
    }

    private async Task AnyadirPuntRuta()
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        await AnyadirPuntRutaDB(location);

        ActualitzarLabelsContadors(null, ++numeroDePuntsRuta);
    }

    private async void CrearTaules()
    {
        await conn.CreateTableAsync<Ruta>();

        await conn.CreateTableAsync<PuntInteres>();

        await conn.CreateTableAsync<PuntRuta>();
    }

    private async Task InsertarNovaRutaBD()
    {
        List<Ruta> rutes = await conn.Table<Ruta>().ToListAsync();

        List<Ruta> rutesAEsborrar = rutes.Where(w => !w.Finalitzada).ToList();

        foreach (Ruta rutaAEsborrar in rutesAEsborrar)
        {
            int res = await conn.DeleteAsync(rutaAEsborrar);

            List<PuntInteres> puntsInteresAEsborrar = await conn.Table<PuntInteres>().Where(w => w.RutaId == rutaAEsborrar.Id).ToListAsync();

            foreach (PuntInteres puntInteres in puntsInteresAEsborrar)
            {
                res = await conn.DeleteAsync(puntInteres);
            }

            List<PuntRuta> puntsRutaAEsborrar = await conn.Table<PuntRuta>().Where(w => w.RutaId == rutaAEsborrar.Id).ToListAsync();

            foreach (PuntRuta puntRuta in puntsRutaAEsborrar)
            {
                res = await conn.DeleteAsync(puntRuta);
            }
        }

        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
        DateTime ara = DateTime.Now;
        Ruta ruta = new Ruta();
        ruta.Nom = "Ruta";
        ruta.DataHora = ara;
        ruta.Finalitzada = false;

        int result = await conn.InsertAsync(ruta);

        rutes = await conn.Table<Ruta>().ToListAsync();

        Ruta rutaDB = rutes.Where(w => w.DataHora == ara).FirstOrDefault();

        rutaId = rutaDB.Id;
    }
    private async Task AnyadirPuntRutaDB(Location location)
    {
        PuntRuta puntRuta = new PuntRuta(location.Latitude, location.Longitude, location.Altitude, DateTime.Now, rutaId);
        int result = await conn.InsertAsync(puntRuta);
    }
    private async Task AnyadirPuntInteresDB(Location location, string nom)
    {
        PuntInteres puntInteres = new PuntInteres(location.Latitude, location.Longitude, location.Altitude, DateTime.Now, rutaId, nom);
        int result = await conn.InsertAsync(puntInteres);
    }

    private void VisibilitatBotonsDespresAfegirPunts()
    {
        PuntInteresEntry.Text = null;
        IniciBtn.IsEnabled = true;
        PuntInteresBtn.IsEnabled = true;
        PuntRutaBtn.IsEnabled = true;
        if (!FinalBtn.IsEnabled)
            if (numeroDePuntsInteres > 0 && numeroDePuntsRuta > 0)
                FinalBtn.IsEnabled = true;
    }

    private void DesactivarTotsElsBotons()
    {
        IniciBtn.IsEnabled = false;
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        PuntRutaBtn.IsEnabled = false;
        RecuperarBtn.IsEnabled = false;
    }

    private void ActualitzarLabelsContadors(int? numeroPuntsInteres, int? numeroPuntsRuta)
    {
        if (numeroPuntsInteres.HasValue)
            numeroDePuntsInteres = numeroPuntsInteres.Value;
        if (numeroPuntsRuta.HasValue)
            numeroDePuntsRuta = numeroPuntsRuta.Value;
        LabelPuntsInteres.Text = $"{Constants.TextPuntsInteres}: {numeroDePuntsInteres}";
        LabelPuntsRuta.Text = $"{Constants.TextPuntsRuta}: {numeroDePuntsRuta}";
    }
}
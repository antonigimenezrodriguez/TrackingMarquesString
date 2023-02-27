using SQLite;
using TrackingMarques.Models;

namespace TrackingMarques;

public partial class MainPage : ContentPage
{

    int numeroDePuntsInteres = 0;
    int numeroDePuntsRuta = 0;
    private CancellationTokenSource _cancelTokenSource;
    //string xmlPuntsInteres = string.Empty;
    //string xmlPuntsRuta = string.Empty;
    string textPuntsInteres = "Punts interés introduïts";
    string textPuntsRuta = "Punts ruta introduïts";
    int rutaId = 0;
    SQLiteAsyncConnection conn = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

    public MainPage()
    {
        numeroDePuntsInteres = 0;
        numeroDePuntsRuta = 0;
        InitializeComponent();
        LabelPuntsInteres.Text = $"{textPuntsInteres}: 0";
        LabelPuntsRuta.Text = $"{textPuntsRuta}: 0";
        var asd = conn.Table<Ruta>().Where(w => w.Finalitzada).FirstOrDefaultAsync().Result;
    }

    private async void IniciBtn_Clicked(object sender, EventArgs e)
    {
        await CrearTaules();
        numeroDePuntsInteres = 0;
        LabelPuntsInteres.Text = $"{textPuntsInteres}: {numeroDePuntsInteres}";
        numeroDePuntsRuta = 0;
        LabelPuntsRuta.Text = $"{textPuntsRuta}: {numeroDePuntsRuta}";
        await InsertarNovaRutaBD();
        await AnyadirPuntRuta();
        PuntInteresBtn.IsEnabled = true;
        PuntRutaBtn.IsEnabled = true;
        RecuperarBtn.IsEnabled = false;
    }

    private async void PuntInteresBtn_Clicked(object sender, EventArgs e)
    {
        IniciBtn.IsEnabled = false;
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        PuntRutaBtn.IsEnabled = false;
        await AnyadirPuntInteres(PuntInteresEntry.Text);
        PuntInteresEntry.Text = null;
        IniciBtn.IsEnabled = true;
        PuntInteresBtn.IsEnabled = true;
        FinalBtn.IsEnabled = true;
        PuntRutaBtn.IsEnabled = true;
    }
    private async void PuntRutaBtn_Clicked(object sender, EventArgs e)
    {
        IniciBtn.IsEnabled = false;
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        PuntRutaBtn.IsEnabled = false;
        await AnyadirPuntRuta();
        PuntInteresEntry.Text = null;
        IniciBtn.IsEnabled = true;
        PuntInteresBtn.IsEnabled = true;
        FinalBtn.IsEnabled = true;
        PuntRutaBtn.IsEnabled = true;
    }
    private async void FinalBtn_Clicked(object sender, EventArgs e)
    {
        await AnyadirPuntRuta();
        await FinalitzarRuta();
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        PuntRutaBtn.IsEnabled = false;
        /* string ruta = "/storage/emulated/0/Documents/";
         string fichero = $"ruta_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
         string extension = "xml";
         string xmlFinal = string.Empty;
         xmlFinal = "<root>\r\n";
         xmlFinal = xmlFinal + "<puntsRuta>\r\n" + xmlPuntsRuta + "</puntsRuta>\r\n";
         xmlFinal = xmlFinal + "<puntsInteres>\r\n" + xmlPuntsInteres + "</puntsInteres>\r\n";
         xmlFinal = xmlFinal + "</root>";
         File.WriteAllText($"{ruta}{fichero}.{extension}", xmlFinal);*/
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
                xml = xml + "<latitut>" + puntRuta.Latitud + "</latitut>" + "\r\n";
                xml = xml + "<longitut>" + puntRuta.Longitud + "</longitut>" + "\r\n";
                xml = xml + "<elevacio>" + puntRuta.Elevacio + "</elevacio>" + "\r\n";
                xml = xml + "<dataHora>" + puntRuta.DataHora.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
                xml = xml + "</ruta>" + "\r\n";
            }
            xml = xml + "</puntsRuta>\r\n";

            xml = xml + "<puntsInteres>\r\n";
            foreach (PuntInteres puntInteres in puntsInteres)
            {
                xml = xml + "<punt>" + "\r\n";
                xml = xml + "<nom>" + puntInteres.Nom + "</nom>" + "\r\n";
                xml = xml + "<latitut>" + puntInteres.Latitud + "</latitut>" + "\r\n";
                xml = xml + "<longitut>" + puntInteres.Longitud + "</longitut>" + "\r\n";
                xml = xml + "<elevacio>" + puntInteres.Elevacio + "</elevacio>" + "\r\n";
                xml = xml + "<dataHora>" + puntInteres.DataHora.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
                xml = xml + "</punt>" + "\r\n";
            }
            xml = xml + "</puntsInteres>\r\n";
            xml = xml + "</root>\r\n";

            string rutaFitxer = "/storage/emulated/0/Documents/";
            string fitxer = $"ruta_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
            string extension = "xml";
            File.WriteAllText($"{rutaFitxer}{fitxer}.{extension}", xml);
            await conn.UpdateAsync(ruta);
        }
    }

    private async Task AnyadirPuntInteres(string nom)
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        /*  xmlPuntsInteres = xmlPuntsInteres + "<punt>" + "\r\n";
          xmlPuntsInteres = xmlPuntsInteres + "<nom>" + nom + "</nom>" + "\r\n";
          xmlPuntsInteres = xmlPuntsInteres + "<latitut>" + location.Latitude + "</latitut>" + "\r\n";
          xmlPuntsInteres = xmlPuntsInteres + "<longitut>" + location.Longitude + "</longitut>" + "\r\n";
          xmlPuntsInteres = xmlPuntsInteres + "<elevacio>" + location.Altitude + "</elevacio>" + "\r\n";
          xmlPuntsInteres = xmlPuntsInteres + "<dataHora>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
          xmlPuntsInteres = xmlPuntsInteres + "</punt>" + "\r\n";*/

        await AnyadirPuntInteresDB(location, nom);

        numeroDePuntsInteres++;
        LabelPuntsInteres.Text = $"{textPuntsInteres}: {numeroDePuntsInteres}";
    }

    private async Task AnyadirPuntRuta()
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        /* xmlPuntsRuta = xmlPuntsRuta + "<ruta>" + "\r\n";
         xmlPuntsRuta = xmlPuntsRuta + "<latitut>" + location.Latitude + "</latitut>" + "\r\n";
         xmlPuntsRuta = xmlPuntsRuta + "<longitut>" + location.Longitude + "</longitut>" + "\r\n";
         xmlPuntsRuta = xmlPuntsRuta + "<elevacio>" + location.Altitude + "</elevacio>" + "\r\n";
         xmlPuntsRuta = xmlPuntsRuta + "<dataHora>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
         xmlPuntsRuta = xmlPuntsRuta + "</ruta>" + "\r\n";*/

        await AnyadirPuntRutaDB(location);

        numeroDePuntsRuta++;
        LabelPuntsRuta.Text = $"{textPuntsRuta}: {numeroDePuntsRuta}";
    }

    private async Task CrearTaules()
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
        ruta.Nom = "Test";
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

    private async void RecuperarBtn_Clicked(object sender, EventArgs e)
    {
        Ruta ruta = await conn.Table<Ruta>().Where(w => !w.Finalitzada).FirstOrDefaultAsync();
        if (ruta != null)
        {
            rutaId = ruta.Id;
            List<PuntInteres> puntsInteres = await conn.Table<PuntInteres>().Where(w => w.RutaId == ruta.Id).ToListAsync();
            List<PuntRuta> puntsRuta = await conn.Table<PuntRuta>().Where(w => w.RutaId == ruta.Id).ToListAsync();
            int numeroPuntsInteres = puntsInteres.Count;
            int numeroPuntsRuta = puntsRuta.Count;
            numeroDePuntsInteres = numeroPuntsInteres;
            numeroDePuntsRuta = numeroPuntsRuta;
            LabelPuntsInteres.Text = $"{textPuntsInteres}: {numeroPuntsInteres}";
            LabelPuntsRuta.Text = $"{textPuntsRuta}: {numeroPuntsRuta}";
        }
    }
}


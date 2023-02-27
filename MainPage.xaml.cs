using SQLite;
using TrackingMarques.Models;

namespace TrackingMarques;

public partial class MainPage : ContentPage
{

    int numeroDePuntsInteres = 0;
    int numeroDePuntsRuta = 0;
    private CancellationTokenSource _cancelTokenSource;
    string xmlPuntsInteres = string.Empty;
    string xmlPuntsRuta = string.Empty;
    string textPuntsInteres = "Punts interés introduïts";
    string textPuntsRuta = "Punts ruta introduïts";
    SQLiteAsyncConnection conn = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

    public MainPage()
    {
        

        numeroDePuntsInteres = 0;
        numeroDePuntsRuta = 0;
        InitializeComponent();
        LabelPuntsInteres.Text = $"{textPuntsInteres}: 0";
        LabelPuntsRuta.Text = $"{textPuntsRuta}: 0";
    }

    private async void IniciBtn_Clicked(object sender, EventArgs e)
    {
        await CrearTaules();
        numeroDePuntsInteres = 0;
        LabelPuntsInteres.Text = $"{textPuntsInteres}: {numeroDePuntsInteres}";
        numeroDePuntsRuta = 0;
        LabelPuntsRuta.Text = $"{textPuntsRuta}: {numeroDePuntsRuta}";
        await AnyadirRuta();
        PuntInteresBtn.IsEnabled = true;
        PuntRutaBtn.IsEnabled = true;
        await InsertarNovaRutaBD();
    }

    private async void PuntInteresBtn_Clicked(object sender, EventArgs e)
    {
        IniciBtn.IsEnabled = false;
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        PuntRutaBtn.IsEnabled = false;
        await AnyadirWPT(PuntInteresEntry.Text);
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
        await AnyadirRuta();
        PuntInteresEntry.Text = null;
        IniciBtn.IsEnabled = true;
        PuntInteresBtn.IsEnabled = true;
        FinalBtn.IsEnabled = true;
        PuntRutaBtn.IsEnabled = true;
    }
    private async void FinalBtn_Clicked(object sender, EventArgs e)
    {
        await AnyadirRuta();
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        PuntRutaBtn.IsEnabled = false;
        string ruta = "/storage/emulated/0/Documents/";
        string fichero = $"ruta_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
        string extension = "xml";
        string xmlFinal = string.Empty;
        xmlFinal = "<root>\r\n";
        xmlFinal = xmlFinal + "<puntsRuta>\r\n" + xmlPuntsRuta + "</puntsRuta>\r\n";
        xmlFinal = xmlFinal + "<puntsInteres>\r\n" + xmlPuntsInteres + "</puntsInteres>\r\n";
        xmlFinal = xmlFinal + "</root>";
        File.WriteAllText($"{ruta}{fichero}.{extension}", xmlFinal);
    }

    private async Task AnyadirWPT(string nombre)
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        xmlPuntsInteres = xmlPuntsInteres + "<punt>" + "\r\n";
        xmlPuntsInteres = xmlPuntsInteres + "<nom>" + nombre + "</nom>" + "\r\n";
        xmlPuntsInteres = xmlPuntsInteres + "<latitut>" + location.Latitude + "</latitut>" + "\r\n";
        xmlPuntsInteres = xmlPuntsInteres + "<longitut>" + location.Longitude + "</longitut>" + "\r\n";
        xmlPuntsInteres = xmlPuntsInteres + "<elevacio>" + location.Altitude + "</elevacio>" + "\r\n";
        xmlPuntsInteres = xmlPuntsInteres + "<dataHora>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
        xmlPuntsInteres = xmlPuntsInteres + "</punt>" + "\r\n";

        numeroDePuntsInteres++;
        LabelPuntsInteres.Text = $"{textPuntsInteres}: {numeroDePuntsInteres}";
    }

    private async Task AnyadirRuta()
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        xmlPuntsRuta = xmlPuntsRuta + "<ruta>" + "\r\n";
        xmlPuntsRuta = xmlPuntsRuta + "<latitut>" + location.Latitude + "</latitut>" + "\r\n";
        xmlPuntsRuta = xmlPuntsRuta + "<longitut>" + location.Longitude + "</longitut>" + "\r\n";
        xmlPuntsRuta = xmlPuntsRuta + "<elevacio>" + location.Altitude + "</elevacio>" + "\r\n";
        xmlPuntsRuta = xmlPuntsRuta + "<dataHora>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dataHora>" + "\r\n";
        xmlPuntsRuta = xmlPuntsRuta + "</ruta>" + "\r\n";

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
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
        DateTime ara = DateTime.Now;
        Ruta ruta = new Ruta();
        ruta.Nom = "Test";
        ruta.DataHora = ara;
        ruta.Finalitzada = false;

        int result = await conn.InsertAsync(ruta);

        var rutes = await conn.Table<Ruta>().ToListAsync();
        Ruta rutaDB = rutes.Where(w => w.DataHora == ara).FirstOrDefault();

        var asd = "";
    }
}


using System.Xml;

namespace TrackingMarques;

public partial class MainPage : ContentPage
{

    int numeroDePunts = 0;
    private CancellationTokenSource _cancelTokenSource;
    string xml = string.Empty;

    public MainPage()
    {
        numeroDePunts = 0;
        InitializeComponent();
        LabelPunts.Text = $"Punts introduïts: 0";
    }
    private async void IniciBtn_Clicked(object sender, EventArgs e)
    {
        xml = "<punts>" + "\r\n";
        numeroDePunts = 0;
        LabelPunts.Text = $"Punts introduïts: {numeroDePunts}";
        await anyadirWPT("Punt Inici de la ruta");
        PuntInteresBtn.IsEnabled = true;
    }

    private async void PuntInteresBtn_Clicked(object sender, EventArgs e)
    {
        IniciBtn.IsEnabled = false;
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        await anyadirWPT(PuntInteresEntry.Text);
        PuntInteresEntry.Text = null;
        IniciBtn.IsEnabled = true;
        PuntInteresBtn.IsEnabled = true;
        FinalBtn.IsEnabled = true;
    }

    private async void FinalBtn_Clicked(object sender, EventArgs e)
    {
        await anyadirWPT("Final de la ruta");
        xml = xml + "</punts>";
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        string ruta = "/storage/emulated/0/Documents/";
        string fichero = "ruta";
        string extension = "xml";
        int numeroFichero = 0;
        if (File.Exists($"{ruta}{fichero}.{extension}"))
        {
            numeroFichero++;
            while (File.Exists($"{ruta}{fichero} ({numeroFichero}).{extension}"))
            {
                numeroFichero++;
            }
            File.WriteAllText($"{ruta}{fichero} ({numeroFichero}).{extension}", xml);
            //doc.Save($"{ruta}{fichero} ({numeroFichero}).{extension}");
        }
        else
        {
            //  doc.Save($"{ruta}{fichero}.{extension}");
            File.WriteAllText($"{ruta}{fichero}.{extension}", xml);

        }
    }

    private async Task anyadirWPT(string nombre)
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        xml = xml + "<punt>" + "\r\n";
        xml = xml + "<nom>" + PuntInteresEntry.Text + "</nom>" + "\r\n";
        xml = xml + "<latitut>" + location.Latitude + "</latitut>" + "\r\n";
        xml = xml + "<longitut>" + location.Longitude + "</longitut>" + "\r\n";
        xml = xml + "<elevacio>" + location.Altitude + "</elevacio>" + "\r\n";
        xml = xml + "<dataHora>" + DateTime.Now + "</dataHora>" + "\r\n";
        xml = xml + "</punt>" + "\r\n";

        numeroDePunts++;
        LabelPunts.Text = $"Punts introduïts: {numeroDePunts}";
    }
}


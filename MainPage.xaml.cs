using CommunityToolkit.Maui.Alerts;
using TrackingMarques.Domini;
using TrackingMarques.Models;

namespace TrackingMarques;

public partial class MainPage : ContentPage
{

    int numeroDePuntsInteres = 0;
    int numeroDePuntsRuta = 0;
    int rutaId = 0;
    public IDominiRuta DominiRuta { get; set; }

    public MainPage()
    {
        this.DominiRuta = new DominiRuta();
        numeroDePuntsInteres = 0;
        numeroDePuntsRuta = 0;
        InitializeComponent();
        DominiRuta.CrearTaules();
        ActualitzarLabelsContadors(null, null);
        VisibilitatBotoRecuperarRuta();
    }

    private async void IniciBtn_Clicked(object sender, EventArgs e)
    {
        bool resposta = await DisplayAlert("Iniciar nova ruta?", "Vols iniciar una nova ruta? Qualsevol progrés es perdrà", "Sí", "No");
        if (resposta)
        {
            IniciBtn.IsEnabled = false;
            DominiRuta.CrearTaules();
            ActualitzarLabelsContadors(0, 0);
            rutaId = await DominiRuta.InsertarNovaRuta();
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
        DominiRuta.CrearTaules();
        Ruta ruta = await DominiRuta.ObtenirRutaNoFinalitzada();
        if (ruta != null)
        {
            rutaId = ruta.Id;
            List<PuntInteres> puntsInteres = await DominiRuta.ObtenirPuntsInteresRuta(rutaId);
            List<PuntRuta> puntsRuta = await DominiRuta.ObtenirPuntsRutaRuta(rutaId);
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
        bool result = await DominiRuta.FinalitzarRuta(rutaId);
        CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        if (result)
        {
            await Toast.Make($"Fitxer guardat correctament", CommunityToolkit.Maui.Core.ToastDuration.Long).Show(_cancelTokenSource.Token);

        }
        else
        {
            await Toast.Make($"El fitxer no s'ha pogut guardar.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show(_cancelTokenSource.Token);

        }

        VisibilitatBotoRecuperarRuta();
        ActualitzarLabelsContadors(0, 0);
    }
    private async void VisibilitatBotoRecuperarRuta()
    {
        DominiRuta.CrearTaules();
        Ruta rutaNoFinalitzada = await DominiRuta.ObtenirRutaNoFinalitzada();
        if (rutaNoFinalitzada != null)
            RecuperarBtn.IsEnabled = true;
        else
            RecuperarBtn.IsEnabled = false;
    }

    private async Task AnyadirPuntInteres(string nom)
    {
        await DominiRuta.InsertarPuntInteres(rutaId, nom);

        ActualitzarLabelsContadors(++numeroDePuntsInteres, null);
    }
    private async Task AnyadirPuntRuta()
    {
        await DominiRuta.InsertarPuntRuta(rutaId);

        ActualitzarLabelsContadors(null, ++numeroDePuntsRuta);
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
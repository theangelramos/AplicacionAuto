using AplicacionAuto.Clases;
using Microsoft.Maui.Controls;
using System;

namespace AplicacionAuto
{
    public partial class AjenaCarroceria : ContentPage
    {
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;

        public AjenaCarroceria(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;

            // Ocultar la barra de navegación programáticamente
            Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            siBtn.IsEnabled = true;
            noBtn.IsEnabled = true;
        }

        private async void ButtonSi(object sender, EventArgs e)
        {
            tipoGolpe1 = "Fuerte";
            siBtn.IsEnabled = false;
            await Navigation.PushAsync(new NotificacionCarroceria(datosAuto1, prioridad1, servicio1, tipoGolpe1));
        }

        private async void ButtonNo(object sender, EventArgs e)
        {
            noBtn.IsEnabled = false;
            await Navigation.PushAsync(new GolpeLeveMedio(datosAuto1, prioridad1, servicio1, tipoGolpe1));
        }
    }
}
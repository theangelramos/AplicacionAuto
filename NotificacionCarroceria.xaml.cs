using AplicacionAuto.Clases;
using System;
using Microsoft.Maui.Controls;

namespace AplicacionAuto
{
    public partial class NotificacionCarroceria : ContentPage
    {
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;

        public NotificacionCarroceria(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe)
        {
            InitializeComponent();

            // Ocultar barra de navegación
            NavigationPage.SetHasNavigationBar(this, false);

            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            continuarBtn.IsEnabled = true;
        }

        private async void ButtonContinuar(object sender, EventArgs e)
        {
            continuarBtn.IsEnabled = false;
            await Navigation.PushAsync(new GolpeFuerte(datosAuto1, prioridad1, servicio1, tipoGolpe1));
        }
    }
}
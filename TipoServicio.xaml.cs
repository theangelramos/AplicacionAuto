using AplicacionAuto.Clases;
using Microsoft.Maui.Controls;
using System;

namespace AplicacionAuto
{
    public partial class TipoServicio : ContentPage
    {
        String servicio;
        DatosAuto datosAuto1;
        String prioridad1;

        public TipoServicio(DatosAuto datosAuto, String prioridad)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;

            // Ocultar la barra de navegación programáticamente
            Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            hojalateriaPinturaBtn.IsEnabled = true;
            pinturaBtn.IsEnabled = true;
            pulidoEnseradoBtn.IsEnabled = true;
        }

        private async void ButtonPintura(object sender, EventArgs e)
        {
            servicio = "Pintura";
            pinturaBtn.IsEnabled = false;
            await Navigation.PushAsync(new PiezaoCompleto(datosAuto1, prioridad1, servicio));
        }

        private async void ButtonHojalateriayPintura(object sender, EventArgs e)
        {
            servicio = "Hojalatería y Pintura";
            hojalateriaPinturaBtn.IsEnabled = false;
            await Navigation.PushAsync(new TipoGolpe(datosAuto1, prioridad1, servicio));
        }

        private async void ButtonPulido(object sender, EventArgs e)
        {
            servicio = "Pulido y encerado";
            pulidoEnseradoBtn.IsEnabled = false;
            await Navigation.PushAsync(new PulirParteCompleto(datosAuto1, prioridad1, servicio));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Seleccione uno de los servicios\r\n\r\n1. Hojalatería y Pintura:\r\nLa hojalatería y pintura es un servicio completo para la reparación y renovación de la apariencia exterior de un vehículo.\r\n\r\n" +
                "2. Pintura:\r\nEl servicio de pintura se enfoca exclusivamente en mejorar o cambiar el color de la carrocería de un vehículo. Puede tratarse de una renovación completa de la pintura o de pequeños retoques en áreas específicas.\r\n\r\n" +
                "3. Pulido y Encerado:\r\nEl pulido y encerado es un servicio de mantenimiento que se centra en realzar el brillo y la protección de la pintura existente en un vehículo. ";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}
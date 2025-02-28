using AplicacionAuto.Clases;
using Microsoft.Maui.Controls;
using System;

namespace AplicacionAuto
{
    public partial class TipoPrioridad : ContentPage
    {
        public String prioridad;
        DatosAuto datosAuto1;

        public TipoPrioridad(DatosAuto datosAuto)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            btnNormal.IsEnabled = true;
            btnExpress.IsEnabled = true;
        }

        private async void ButtonNormal_Clicked(object sender, EventArgs e)
        {
            btnNormal.IsEnabled = false;
            prioridad = "Normal";
            await Navigation.PushAsync(new TipoServicio(datosAuto1, prioridad));
        }

        private async void ButtonExpress_Clicked(object sender, EventArgs e)
        {
            btnExpress.IsEnabled = false;
            prioridad = "Exprés";
            await Navigation.PushAsync(new TipoServicio(datosAuto1, prioridad));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Seleccione la prioridad del servicio\r\n\r\n1. Servicio Normal:\r\nUn servicio normal, también conocido como servicio completo o estándar, abarca una gama completa de tareas y procesos detallados para mantener o reparar un producto o vehículo. Este tipo de servicio está diseñado para una renovación completa, inspección detallada y atención exhaustiva a todas las áreas relevantes.\r\n\r\n" +
                "2. Servicio Express:\r\nUn servicio express es una opción rápida diseñada para satisfacer necesidades urgentes o limitadas de los clientes. Este tipo de servicio se caracteriza por su velocidad y conveniencia.\r\n";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}
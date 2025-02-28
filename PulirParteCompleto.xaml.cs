using AplicacionAuto.Clases;
using Microsoft.Maui.Controls;
using System;

namespace AplicacionAuto
{
    public partial class PulirParteCompleto : ContentPage
    {
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String opcionTodo;

        public PulirParteCompleto(DatosAuto datosAuto, String prioridad, String servicio)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;

            // Ocultar la barra de navegación programáticamente
            Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            todoVehiculoBtn.IsEnabled = true;
            piesasBtn.IsEnabled = true;
        }

        private async void ButtonPulirPieza(object sender, EventArgs e)
        {
            piesasBtn.IsEnabled = false;
            await Navigation.PushAsync(new PulirPiezas(datosAuto1, prioridad1, servicio1));
        }

        private async void ButtonPulirCompleto(object sender, EventArgs e)
        {
            todoVehiculoBtn.IsEnabled = false;
            opcionTodo = "PulidoEncerado";
            await Navigation.PushAsync(new PulirTodoVehiculo(datosAuto1, prioridad1, servicio1, opcionTodo));
        }
    }
}
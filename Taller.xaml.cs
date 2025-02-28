using AplicacionAuto.Clases;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AplicacionAuto
{
    public partial class Taller : ContentPage
    {
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        List<GolpeMedida> datosGolpes1;
        String paquete1;
        TallerDTO taller1;
        String opcionTodo1;
        private List<string> partesSeleccionadasPE1;
        List<Imagen> imagenesGolpeFuerte1;
        List<Imagen> imagenesTodoElVehiculo1;

        public Taller(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe, List<GolpeMedida> datosGolpes, String paquete, TallerDTO taller, List<string> partesSeleccionadasPE, String opcionTodo, List<Imagen> imagenesGolpeFuerte, List<Imagen> imagenesTodoElVehiculo)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;
            datosGolpes1 = datosGolpes;
            paquete1 = paquete;
            taller1 = taller;
            opcionTodo1 = opcionTodo;
            titleLabel.Text = taller.Nombre;
            imagenesTodoElVehiculo1 = imagenesTodoElVehiculo;
            var datosTaller = $"Nombre: {taller.Nombre}\n\nDirección: {taller.Direccion}\n\nCorreo electrónico: {taller.CorreoElectronico}\n\nEncargado: {taller.Encargado}\n\nHorario de servicio: {taller.HorarioServicio}\n\n";
            addressLabel.Text = datosTaller;
            partesSeleccionadasPE1 = partesSeleccionadasPE;
            imagenesGolpeFuerte1 = imagenesGolpeFuerte;
        }

        private void fechaPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            DateTime fechaSeleccionada = e.NewDate;
        }

        public String hora;
        public String fecha;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            siguienteBtn.IsEnabled = true;
        }

        private async void Button_Siguiente(object sender, EventArgs e)
        {
            siguienteBtn.IsEnabled = false;
            DateTime fechaSeleccionada = fechaPicker.Date;

            fecha = fechaSeleccionada.ToString("dd/MMM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            await Navigation.PushAsync(new InfoDetalles(datosAuto1, prioridad1, servicio1, tipoGolpe1, datosGolpes1, paquete1, taller1, fecha, hora, partesSeleccionadasPE1, opcionTodo1, imagenesGolpeFuerte1, imagenesTodoElVehiculo1));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Ingresa la fecha y hora en la que deseas agendar una cita en el taller. Asegúrate de seleccionar una fecha y hora disponibles y convenientes para ti y el taller. " +
                "¡Estamos aquí para ayudarte!";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}
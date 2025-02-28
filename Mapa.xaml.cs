using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Maps;

namespace AplicacionAuto
{
    public partial class Mapa : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        public List<TallerDTO> talleres;
        static bool alerta = false;

        DatosAuto datosAuto1;
        string prioridad1;
        string servicio1;
        string tipoGolpe1;
        List<GolpeMedida> datosGolpes1;
        string paquete1;
        string opcionTodo1;
        List<Imagen> imagenesGolpeFuerte1;
        private List<string> partesSeleccionadasPE1;
        List<Imagen> imagenesTodoElVehiculo1;

        public Mapa(DatosAuto datosAuto, string prioridad, string servicio, string opcionTodo, string tipoGolpe, List<GolpeMedida> datosGolpes, string paquete, List<Imagen> imagenesGolpeFuerte, List<string> partesSeleccionadasPE, List<Imagen> imagenesTodoElVehiculo)
        {
            InitializeComponent();

            // Asignar valores a los campos
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;
            datosGolpes1 = datosGolpes;
            paquete1 = paquete;
            imagenesGolpeFuerte1 = imagenesGolpeFuerte;
            partesSeleccionadasPE1 = partesSeleccionadasPE;
            opcionTodo1 = opcionTodo;
            imagenesTodoElVehiculo1 = imagenesTodoElVehiculo;

            // Inicializar ubicación y cargar talleres  
            UbicacionDeInicio();
            ObtenerUbicacion();

            // Verificar conexión a internet
            var tieneConexionInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
            }
            else
            {
                try
                {
                    peticion.PedirComunicacion("Taller/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                    var jsonRecibir = peticion.ObtenerJson();
                    talleres = JsonConvertidor.Json_ListaObjeto<TallerDTO>(jsonRecibir);
                }
                catch (Exception)
                {
                    CerrarApp("Error en la petición al servidor", "\n\nAsegúrate de tener una conexión estable a internet.");
                }
            }

            // Agregar pines de talleres
            if (talleres != null)
            {
                foreach (var taller in talleres)
                {
                    var latitudLongitud = new Location(double.Parse(taller.Latitud), double.Parse(taller.Longitud));

                    var datosTaller = $"Nombre: {taller.Nombre}\n\nDirección: {taller.Direccion}\n\nCorreo electrónico: {taller.CorreoElectronico}\n\nEncargado: {taller.Encargado}\n\nHorario de servicio: {taller.HorarioServicio}\n\n";

                    var tallerPin = new Pin
                    {
                        Label = taller.Nombre,
                        Location = latitudLongitud,
                        Type = PinType.Place
                    };

                    tallerPin.MarkerClicked += async (s, args) =>
                    {
                        var result = await DisplayAlert("Detalles del Taller:", datosTaller, "Seleccionar", "Cancelar");

                        if (result)
                        {
                            await Navigation.PushAsync(new Taller(datosAuto1, prioridad1, servicio1, tipoGolpe1, datosGolpes1, paquete1, taller, partesSeleccionadasPE1, opcionTodo1, imagenesGolpeFuerte1, imagenesTodoElVehiculo1));
                        }
                    };

                    map.Pins.Add(tallerPin);
                }

                labelseleccionar.IsVisible = true;
            }
        }

        private void UbicacionDeInicio()
        {
            Location initialLocation = new Location(19.713029682070907, -98.96875946108221);
            var mapSpan = MapSpan.FromCenterAndRadius(initialLocation, Distance.FromMiles(10));

            map.MoveToRegion(mapSpan);
        }

        public async void CerrarApp(string titulo, string mensaje)
        {
            if (!alerta)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                alerta = true;
            }
        }

        private async void ObtenerUbicacion()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    var currentLocationPin = new Pin
                    {
                        Label = "Tu ubicación actual",
                        Location = new Location(location.Latitude, location.Longitude),
                        Type = PinType.Place
                    };

                    map.Pins.Add(currentLocationPin);
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Location(location.Latitude, location.Longitude),
                        Distance.FromKilometers(10)
                    ));
                }
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Error", "La ubicación no está habilitada en este dispositivo.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Permiso requerido", "Es necesario permitir el acceso a la ubicación para utilizar esta aplicación.", "OK");
            }
            catch (Exception)
            {
                await DisplayAlert("Ubicación desactivada", "Activa tu ubicación para una mejor experiencia y vuelve a cargar esta página", "ACEPTAR");
            }
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "¡Importante! Asegúrate de permitir el acceso a tu ubicación para obtener resultados precisos. Selecciona 'Obtener Ubicación' para mostrar talleres cercanos en el mapa. Recuerda que la ubicación y los talleres mostrados pueden variar según la disponibilidad de datos. Selecciona en el icono del taller y después haz click en seleccionar para escoger el taller";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}
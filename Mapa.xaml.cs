using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace AplicacionAuto
{
    public partial class Mapa : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        public List<TallerDTO> talleres;
        static bool alerta = false;
        private Microsoft.Maui.Devices.Sensors.Location currentLocation;
        private bool isMapLoaded = false;
        private bool isRequestingPermission = false;

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

        // Constructor
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

            // Configurar WebView
            mapWebView.Navigated += MapWebView_Navigated;

            // Suscribirse a eventos de JavaScript
            mapWebView.Navigating += MapWebView_Navigating;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Ocultar la barra de navegación
            NavigationPage.SetHasNavigationBar(this, false);

            // Solicitar permisos y comenzar la inicialización
            await InicializarMapa();
        }

        private async Task InicializarMapa()
        {
            // Verificar y solicitar permisos primero
            var status = await CheckAndRequestLocationPermission();
            if (status == PermissionStatus.Granted)
            {
                // Después de tener permisos, obtenemos ubicación
                await ObtenerUbicacion();
                // Cargar los talleres después de tener la ubicación
                await CargarTalleres();
            }
            else
            {
                // Si el permiso fue denegado, mostrar la lista alternativa sin mapa
                talleresContainer.IsVisible = true;
                await DisplayAlert("Permiso requerido", "Es necesario permitir el acceso a la ubicación para utilizar el mapa. Se mostrará una lista alternativa.", "OK");
                await CargarTalleres();
            }
        }

        private async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            if (isRequestingPermission)
                return PermissionStatus.Unknown;

            isRequestingPermission = true;

            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                return status;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error de permisos", $"No se pudieron solicitar los permisos: {ex.Message}", "OK");
                return PermissionStatus.Denied;
            }
            finally
            {
                isRequestingPermission = false;
            }
        }

        private void MapWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success)
            {
                isMapLoaded = true;
                ActualizarPinesEnMapa();
            }
            else
            {
                // Si falla la carga del mapa, mostrar la lista alternativa
                talleresContainer.IsVisible = true;
                DisplayAlert("Error", "No se pudo cargar el mapa. Se muestra una lista alternativa.", "OK");
            }
        }

        private void MapWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            // Interceptar eventos de navegación que puedan ser mensajes desde JavaScript
            if (e.Url.StartsWith("https://talleres-app/"))
            {
                e.Cancel = true; // Evitar la navegación real

                string action = e.Url.Replace("https://talleres-app/", "");
                if (action.StartsWith("selectTaller:"))
                {
                    string tallerNombre = Uri.UnescapeDataString(action.Replace("selectTaller:", ""));
                    // Encontrar el taller por nombre
                    var taller = talleres.FirstOrDefault(t => t.Nombre == tallerNombre);
                    if (taller != null)
                    {
                        // Invocar la selección del taller
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await SeleccionarTaller(taller);
                        });
                    }
                }
            }
        }

        private async Task CargarTalleres()
        {
            // Verificar conexión a internet
            var tieneConexionInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
                return;
            }

            try
            {
                // Mostrar indicador de carga
                IsBusy = true;

                // Obtener datos de talleres del servidor
                peticion.PedirComunicacion("Taller/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                var jsonRecibir = await peticion.ObtenerJson();
                talleres = JsonConvertidor.Json_ListaObjeto<TallerDTO>(jsonRecibir);

                // Mostrar talleres en la interfaz
                if (talleres != null && talleres.Count > 0)
                {
                    MostrarTalleres();
                    labelseleccionar.IsVisible = true;

                    // Iniciar carga del mapa solo si tenemos permisos
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (status == PermissionStatus.Granted)
                    {
                        await CargarMapaOpenStreetMap();
                    }
                }
                else
                {
                    await DisplayAlert("Información", "No se encontraron talleres disponibles", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                CerrarApp("Error en la petición al servidor", $"\n\nAsegúrate de tener una conexión estable a internet. Detalles: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CargarMapaOpenStreetMap()
        {
            try
            {
                // Coordenadas iniciales (centrar en México si no hay ubicación)
                double initialLat = 19.713029682070907;
                double initialLng = -98.96875946108221;
                int initialZoom = 10;

                // Si tenemos ubicación actual, usar esas coordenadas
                if (currentLocation != null)
                {
                    initialLat = currentLocation.Latitude;
                    initialLng = currentLocation.Longitude;
                    initialZoom = 12;
                }

                // Construir HTML para el mapa con OpenStreetMap y Leaflet
                string htmlContent = GenerarHTMLMapaOpenStreetMap(initialLat, initialLng, initialZoom);

                // Cargar el HTML en el WebView - usamos BaseUrl vacío para garantizar que funcione
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // Debug para verificar si la fuente se está configurando
                    Console.WriteLine("Configurando HTML en WebView...");

                    mapWebView.Source = new HtmlWebViewSource
                    {
                        Html = htmlContent,
                        BaseUrl = ""
                    };
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo cargar el mapa: {ex.Message}", "OK");
                talleresContainer.IsVisible = true;
            }
        }

        private string GenerarHTMLMapaOpenStreetMap(double lat, double lng, int zoom)
        {
            // HTML con OpenStreetMap usando Leaflet.js - versión simplificada
            var html = new StringBuilder();
            html.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
    <!-- Incluir Leaflet directamente sin integridad, para asegurar que cargue -->
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css'>
    <style>
        html, body {
            height: 100%;
            width: 100%;
            margin: 0;
            padding: 0;
        }
        #map {
            height: 100%;
            width: 100%;
            z-index: 1;
        }
        .custom-popup .leaflet-popup-content-wrapper {
            background: #fff;
            color: #333;
            font-size: 14px;
            border-radius: 10px;
        }
        .custom-popup .leaflet-popup-content {
            margin: 12px;
        }
        .custom-popup .popup-content {
            padding: 5px;
        }
        .custom-popup .select-btn {
            background: #e74c3c;
            color: white;
            border: none;
            padding: 8px 12px;
            margin-top: 10px;
            border-radius: 5px;
            cursor: pointer;
            display: block;
            width: 100%;
            text-align: center;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <div id='map'></div>
    
    <!-- Incluir Leaflet directamente sin integridad, para asegurar que cargue -->
    <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
    
    <script>
        // Comprobar explícitamente si Leaflet está cargado
        if (typeof L === 'undefined') {
            console.error('¡Error! Leaflet no está cargado.');
            // Mostrar un mensaje en el mapa
            document.getElementById('map').innerHTML = '<div style=""padding:20px;text-align:center;background:white;"">' +
                '<h3 style=""color:red"">Error al cargar el mapa</h3>' +
                '<p>No se pudo cargar la librería Leaflet. Por favor, actualiza la página.</p>' +
                '</div>';
        } else {
            console.log('Leaflet está cargado correctamente. Inicializando mapa...');
            
            // Configurar opciones del mapa
            var mapOptions = {
                zoomControl: true,
                attributionControl: false,
                scrollWheelZoom: true,
                touchZoom: true,
                dragging: true
            };
            
            // Inicializar el mapa
            var map = L.map('map', mapOptions).setView([" + lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " + lng.ToString(System.Globalization.CultureInfo.InvariantCulture) + "], " + zoom + @");
            
            // Capa base de OpenStreetMap
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19
            }).addTo(map);
        }
        
        // Marcadores para almacenar las referencias
        var markers = [];
        
        // Icono personalizado para ubicación actual
        var blueIcon = L.icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        
        // Icono personalizado para talleres
        var redIcon = L.icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        
        // Marcador para la ubicación actual
        var currentLocationMarker = L.marker([" + lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " + lng.ToString(System.Globalization.CultureInfo.InvariantCulture) + @"], {icon: blueIcon})
            .addTo(map)
            .bindPopup('<b>Tu ubicación actual</b>')
            .openPopup();
        
        markers.push(currentLocationMarker);
");

            // Añadir marcadores para cada taller
            if (talleres != null)
            {
                foreach (var taller in talleres)
                {
                    try
                    {
                        // Verificar y limpiar datos de coordenadas
                        string latitudStr = taller.Latitud?.Trim() ?? "";
                        string longitudStr = taller.Longitud?.Trim() ?? "";

                        // Convertir a formato invariable para evitar problemas regionales
                        latitudStr = latitudStr.Replace(',', '.');
                        longitudStr = longitudStr.Replace(',', '.');

                        if (!double.TryParse(latitudStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double tallerLat) ||
                            !double.TryParse(longitudStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double tallerLng))
                        {
                            // Coordenadas inválidas, usar coordenadas predeterminadas (centradas en México)
                            tallerLat = 19.432608;
                            tallerLng = -99.133209;
                            Console.WriteLine($"Coordenadas inválidas para taller {taller.Nombre}. Usando ubicación predeterminada.");
                        }

                        string tallerNombreEscapado = EscaparJS(taller.Nombre);
                        string tallerDireccionEscapada = EscaparJS(taller.Direccion);
                        string tallerHorarioEscapado = EscaparJS(taller.HorarioServicio);

                        html.Append($@"
        // Marcador para {tallerNombreEscapado}
        var tallerMarker = L.marker([{tallerLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {tallerLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}], {{icon: redIcon}})
            .addTo(map)
            .bindPopup(
                '<div class=""popup-content"">' +
                '<b>{tallerNombreEscapado}</b><br>' +
                'Dirección: {tallerDireccionEscapada}<br>' +
                'Horario: {tallerHorarioEscapado}<br>' +
                '<button class=""select-btn"" onclick=""selectTaller(\'{tallerNombreEscapado}\')"">Seleccionar Taller</button>' +
                '</div>',
                {{ className: 'custom-popup' }}
            );
        
        markers.push(tallerMarker);
");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al añadir marcador para taller {taller.Nombre}: {ex.Message}");
                    }
                }
            }

            html.Append(@"
        // Función para seleccionar un taller
        function selectTaller(tallerNombre) {
            window.location.href = 'https://talleres-app/selectTaller:' + encodeURIComponent(tallerNombre);
        }
        
        // Función para actualizar el mapa y centrarlo en la ubicación actual
        function actualizarMapa(lat, lng) {
            // Actualizar la posición del marcador de ubicación actual
            if (markers.length > 0) {
                markers[0].setLatLng([lat, lng]);
            }
            // Centrar el mapa en la nueva ubicación
            map.setView([lat, lng], 13);
        }
        
        // Asegurarnos de que los gestos táctiles funcionan correctamente
        document.addEventListener('DOMContentLoaded', function() {
            setTimeout(function() {
                map.invalidateSize();
            }, 500);
        });
        
        // Notificar a la aplicación cuando el mapa esté completamente cargado
        window.addEventListener('load', function() {
            try {
                if (window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.mapLoaded) {
                    window.webkit.messageHandlers.mapLoaded.postMessage('loaded');
                }
                // Equivalente para Android
                if (window.mapLoaded) {
                    window.mapLoaded.postMessage('loaded');
                }
            } catch (e) {
                console.error('Error notificando carga del mapa', e);
            }
        });
    </script>
</body>
</html>");

            return html.ToString();
        }

        private string EscaparJS(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            return texto.Replace("'", "\\'")
                       .Replace("\"", "\\\"")
                       .Replace("\n", "\\n")
                       .Replace("\r", "\\r");
        }

        private void ActualizarPinesEnMapa()
        {
            if (!isMapLoaded || mapWebView == null || currentLocation == null)
                return;

            try
            {
                // Actualizar la posición del marcador de ubicación actual usando la función JavaScript
                string script = $"actualizarMapa({currentLocation.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                               $"{currentLocation.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)});";

                // Ejecutar script en WebView
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    mapWebView.Eval(script);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar pines en el mapa: {ex.Message}");
            }
        }

        private void MostrarTalleres()
        {
            // Limpiar el contenedor de talleres (vista alternativa)
            talleresContainer.Clear();

            if (talleres == null || talleres.Count == 0)
                return;

            // Ordenar talleres por distancia si tenemos la ubicación actual
            if (currentLocation != null)
            {
                talleres = talleres.OrderBy(t =>
                {
                    try
                    {
                        return CalcularDistancia(
                            currentLocation.Latitude,
                            currentLocation.Longitude,
                            double.Parse(t.Latitud?.Replace(',', '.') ?? "0", System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(t.Longitud?.Replace(',', '.') ?? "0", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    catch
                    {
                        return double.MaxValue; // Si hay error, poner al final
                    }
                }).ToList();
            }

            // Crear un ScrollView para contener los talleres
            var scrollView = new ScrollView
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Always,
                Orientation = ScrollOrientation.Vertical
            };

            var stackLayout = new VerticalStackLayout { Spacing = 10 };
            scrollView.Content = stackLayout;

            // Mostrar cada taller como un elemento en la lista (vista alternativa)
            foreach (var taller in talleres)
            {
                var frame = new Frame
                {
                    BorderColor = Colors.LightGray,
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = new Thickness(15),
                    Margin = new Thickness(10, 0, 10, 10)
                };

                var tallerStack = new VerticalStackLayout { Spacing = 8 };

                // Nombre del taller
                tallerStack.Add(new Label
                {
                    Text = taller.Nombre,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black,
                    FontSize = 18
                });

                // Dirección
                tallerStack.Add(new Label
                {
                    Text = $"Dirección: {taller.Direccion}",
                    TextColor = Colors.DarkGray,
                    FontSize = 14
                });

                // Distancia (si tenemos ubicación actual)
                if (currentLocation != null)
                {
                    try
                    {
                        double distancia = CalcularDistancia(
                            currentLocation.Latitude,
                            currentLocation.Longitude,
                            double.Parse(taller.Latitud?.Replace(',', '.') ?? "0", System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(taller.Longitud?.Replace(',', '.') ?? "0", System.Globalization.CultureInfo.InvariantCulture));

                        tallerStack.Add(new Label
                        {
                            Text = $"Distancia aproximada: {distancia:F1} km",
                            TextColor = Colors.DarkGray,
                            FontSize = 14
                        });
                    }
                    catch
                    {
                        // Si hay error en las coordenadas, no mostrar distancia
                    }
                }

                // Botón para seleccionar este taller
                var seleccionarButton = new Button
                {
                    Text = "Seleccionar",
                    BackgroundColor = Color.Parse("OrangeRed"),
                    TextColor = Colors.White,
                    CornerRadius = 20,
                    Margin = new Thickness(0, 10, 0, 0),
                    HorizontalOptions = LayoutOptions.Fill
                };

                // Guardar una referencia al taller actual para el manejador de eventos
                var currentTaller = taller;
                seleccionarButton.Clicked += async (sender, e) =>
                {
                    await SeleccionarTaller(currentTaller);
                };

                tallerStack.Add(seleccionarButton);
                frame.Content = tallerStack;
                stackLayout.Add(frame);
            }

            // Agregar el ScrollView al contenedor
            talleresContainer.Add(scrollView);
        }

        private async Task SeleccionarTaller(TallerDTO taller)
        {
            // Mostrar detalles del taller y confirmar selección
            var datosTaller = $"Nombre: {taller.Nombre}\n\nDirección: {taller.Direccion}\n\nCorreo electrónico: {taller.CorreoElectronico}\n\nEncargado: {taller.Encargado}\n\nHorario de servicio: {taller.HorarioServicio}\n\n";

            var result = await DisplayAlert("Detalles del Taller:", datosTaller, "Confirmar selección", "Cancelar");

            if (result)
            {
                await Navigation.PushAsync(new Taller(datosAuto1, prioridad1, servicio1, tipoGolpe1, datosGolpes1, paquete1, taller, partesSeleccionadasPE1, opcionTodo1, imagenesGolpeFuerte1, imagenesTodoElVehiculo1));
            }
        }

        private async Task ObtenerUbicacion()
        {
            try
            {
                // Verificar primero si tenemos permiso
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        locationLabel.Text = "Se requiere permiso de ubicación";
                    });
                    return;
                }

                // Configurar solicitud de ubicación con un timeout más largo
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(15));

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    locationLabel.Text = "Obteniendo ubicación...";
                });

                // Obtener ubicación con timeout extendido
                var cancelTokenSource = new System.Threading.CancellationTokenSource();
                cancelTokenSource.CancelAfter(TimeSpan.FromSeconds(20)); // 20 segundos de timeout total

                var location = await Geolocation.GetLocationAsync(request, cancelTokenSource.Token);

                if (location != null)
                {
                    currentLocation = new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        locationLabel.Text = $"Lat: {location.Latitude:F6}, Long: {location.Longitude:F6}";
                    });

                    // Si ya tenemos talleres cargados, actualizamos el mapa
                    if (talleres != null && talleres.Count > 0 && isMapLoaded)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            ActualizarPinesEnMapa();
                        });
                    }

                    // Actualizar lista de talleres con nuevas distancias
                    if (talleres != null && talleres.Count > 0)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            MostrarTalleres();
                        });
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        locationLabel.Text = "No se pudo obtener la ubicación actual";
                    });
                }
            }
            catch (FeatureNotEnabledException)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    locationLabel.Text = "Ubicación desactivada. Actívala en la configuración del dispositivo.";
                    await DisplayAlert("Error", "La ubicación no está habilitada en este dispositivo.", "OK");
                });
            }
            catch (PermissionException)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    locationLabel.Text = "Permiso de ubicación denegado";
                    await DisplayAlert("Permiso requerido", "Es necesario permitir el acceso a la ubicación para utilizar esta aplicación.", "OK");
                });
            }
            catch (TaskCanceledException)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    locationLabel.Text = "Timeout al obtener ubicación. Intenta de nuevo.";
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    locationLabel.Text = "Error al obtener ubicación";
                    await DisplayAlert("Ubicación desactivada", $"Activa tu ubicación para una mejor experiencia. Error: {ex.Message}", "ACEPTAR");
                });
            }
        }

        private async void RefreshMapClicked(object sender, EventArgs e)
        {
            refreshLocationButton.IsEnabled = false;
            refreshLocationButton.Text = "Actualizando...";

            try
            {
                // Verificar y solicitar permisos nuevamente
                var status = await CheckAndRequestLocationPermission();

                if (status == PermissionStatus.Granted)
                {
                    await ObtenerUbicacion();

                    // Recargar el mapa con la nueva ubicación si ha fallado
                    if (!isMapLoaded)
                    {
                        await CargarMapaOpenStreetMap();
                    }
                    else
                    {
                        // Si el mapa ya está cargado, actualizar los pines
                        ActualizarPinesEnMapa();
                    }
                }
                else
                {
                    // Si el permiso fue denegado nuevamente, mostrar mensaje
                    await DisplayAlert("Permiso denegado", "No se puede actualizar el mapa sin permiso de ubicación.", "OK");

                    // Asegurarse de que la lista alternativa sea visible
                    talleresContainer.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo actualizar el mapa: {ex.Message}", "OK");
            }
            finally
            {
                refreshLocationButton.IsEnabled = true;
                refreshLocationButton.Text = "Actualizar Mapa";
            }
        }

        public async void CerrarApp(string titulo, string mensaje)
        {
            if (!alerta)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                alerta = true;
            }
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "¡Importante! Asegúrate de permitir el acceso a tu ubicación para obtener resultados precisos. " +
                "Usa el botón 'Actualizar Mapa' si necesitas refrescar tu posición actual. " +
                "Los marcadores azules indican tu ubicación actual, mientras que los rojos muestran la ubicación de los talleres. " +
                "Pulsa sobre un marcador para ver los detalles del taller y seleccionarlo.";

            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }

        // Método para calcular la distancia entre dos puntos geográficos usando la fórmula de Haversine
        private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            const double radioTierra = 6371; // Radio de la Tierra en km

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distancia = radioTierra * c;

            return distancia;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}

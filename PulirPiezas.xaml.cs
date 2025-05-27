using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class PulirPiezas : ContentPage
    {
        private readonly PeticionHTTP _peticion = new PeticionHTTP(Urls.UrlServer());
        private static Boolean _alerta = false;
        private readonly List<string> _partesSeleccionadasPE;

        private readonly DatosAuto _datosAuto;
        private readonly String _prioridad;
        private readonly String _servicio;
        private String _tipoGolpe;
        private String _opcionTodo;
        private List<Imagen> _imagenesTodoElVehiculo;

        // Indicador de carga
        private ActivityIndicator _activityIndicator;

        public PulirPiezas(DatosAuto datosAuto, String prioridad, String servicio)
        {
            InitializeComponent();

            // Asignar valores a propiedades
            _datosAuto = datosAuto;
            _prioridad = prioridad;
            _servicio = servicio;
            _partesSeleccionadasPE = new List<string>();

            // Inicializar el ListView con mensaje vacío y agregar evento para actualizar la visibilidad
            ActualizarListView();

            // Cargar piezas con un pequeño retraso para permitir que la UI se renderice
            MainThread.BeginInvokeOnMainThread(async () => {
                MostrarCargando(true);
                await Task.Delay(300); // Breve retraso para UI
                await CargarPiezas();
                MostrarCargando(false);
            });
        }

        private void MostrarCargando(bool mostrar)
        {
            if (mostrar)
            {
                // Crear indicador de carga si no existe
                if (_activityIndicator == null)
                {
                    _activityIndicator = new ActivityIndicator
                    {
                        IsRunning = true,
                        Color = Colors.Red,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HeightRequest = 40,
                        WidthRequest = 40
                    };

                    // Agregar al layout
                    piezasContainer.Children.Add(_activityIndicator);
                }
                else
                {
                    _activityIndicator.IsRunning = true;
                    _activityIndicator.IsVisible = true;
                }
            }
            else if (_activityIndicator != null)
            {
                _activityIndicator.IsRunning = false;
                _activityIndicator.IsVisible = false;
            }
        }

        private async Task CargarPiezas()
        {
            // Verificar conexión a internet
            bool tieneConexionInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
                return;
            }

            try
            {
                // Realizar petición al servidor
                _peticion.PedirComunicacion(
                    "Pieza/Obtener",
                    MetodoHTTP.GET,
                    TipoContenido.JSON,
                    Preferences.Default.Get("token", "")
                );

                string jsonRecibir = await _peticion.ObtenerJson();
                List<PiezaDTO> piezas = JsonConvertidor.Json_ListaObjeto<PiezaDTO>(jsonRecibir);

                // Filtrar piezas para "Pulido y encerado"
                var piezasFiltradas = piezas.FindAll(x =>
                    x.TipoPiezaNombre.ToLower() == "Pulido y encerado".ToLower());

                piezas = piezasFiltradas;

                // Remover "Todo el vehículo" de las opciones
                piezas.RemoveAll(p => p.Nombre == "Todo el vehículo");

                // Asignar como contexto de datos
                BindingContext = new { piezas };

                // Generar controles dinámicamente para cada pieza
                piezasContainer.Children.Clear();

                // Organizar las piezas alfabéticamente para mejor experiencia
                piezas = piezas.OrderBy(p => p.Nombre).ToList();

                foreach (var pieza in piezas)
                {
                    var frame = new Frame
                    {
                        BorderColor = Colors.Transparent,
                        BackgroundColor = Colors.Transparent,
                        Padding = new Thickness(3),
                        Margin = new Thickness(0, 2)
                    };

                    var horizontalStackLayout = new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Margin = new Thickness(0, 2)
                    };

                    var label = new Label
                    {
                        Text = pieza.Nombre,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 16,
                        MaxLines = 2,
                        LineBreakMode = LineBreakMode.WordWrap,
                        WidthRequest = 230
                    };

                    var checkBox = new CheckBox();
                    checkBox.CheckedChanged += CheckBox_CheckedChanged;

                    // Para diferenciación visual
                    if (piezas.IndexOf(pieza) % 2 == 0)
                    {
                        frame.BackgroundColor = new Color(0.98f, 0.98f, 0.98f);
                    }

                    horizontalStackLayout.Children.Add(label);
                    horizontalStackLayout.Children.Add(checkBox);

                    frame.Content = horizontalStackLayout;
                    piezasContainer.Children.Add(frame);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar piezas: {ex.Message}");
                CerrarApp(
                    "Error en la petición al servidor",
                    "\n\nAsegúrate de tener una conexión estable a internet."
                );
            }
        }

        public async void CerrarApp(String titulo, String mensaje)
        {
            if (_alerta == false)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                _alerta = true;
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is not CheckBox checkBox) return;

            string parteCarro = GetCheckBoxText(checkBox);
            if (string.IsNullOrEmpty(parteCarro)) return;

            if (e.Value)
            {
                if (!_partesSeleccionadasPE.Contains(parteCarro))
                {
                    _partesSeleccionadasPE.Add(parteCarro);
                }
            }
            else
            {
                if (_partesSeleccionadasPE.Contains(parteCarro))
                {
                    _partesSeleccionadasPE.Remove(parteCarro);
                }
            }
        }

        private string GetCheckBoxText(CheckBox checkBox)
        {
            try
            {
                // Obtener el Frame padre (si existe)
                var parent = checkBox.Parent;
                while (parent != null && !(parent is Frame))
                {
                    if (parent is HorizontalStackLayout horizontalStackLayout)
                    {
                        // El Label es el primer elemento del HorizontalStackLayout
                        if (horizontalStackLayout.Children[0] is Label label)
                        {
                            return label.Text;
                        }
                    }
                    parent = parent.Parent;
                }

                // Si no tiene Frame padre, obtener el HorizontalStackLayout directamente
                if (checkBox.Parent is HorizontalStackLayout stackLayout)
                {
                    if (stackLayout.Children[0] is Label labelDirecto)
                    {
                        return labelDirecto.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener texto del checkbox: {ex.Message}");
            }

            return string.Empty;
        }

        private void ActualizarListView()
        {
            try
            {
                // Actualizar la fuente de datos del ListView
                lstPartesSeleccionadasListView.ItemsSource = null;
                lstPartesSeleccionadasListView.ItemsSource = _partesSeleccionadasPE;

                // Actualizar visibilidad del mensaje de "no selección"
                lblNoSeleccion.IsVisible = _partesSeleccionadasPE == null || _partesSeleccionadasPE.Count == 0;

                // Mostrar botón siguiente si hay elementos seleccionados
                btnSiguiente.IsVisible = _partesSeleccionadasPE.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar ListView: {ex.Message}");
            }
        }

        private void ButtonGuardar_Clicked(object sender, EventArgs e)
        {
            ActualizarListView();

            // Mostrar confirmación al usuario
            if (_partesSeleccionadasPE.Count > 0)
            {
                MainThread.BeginInvokeOnMainThread(async () => {
                    await DisplayAlert("Selección guardada", $"Se han seleccionado {_partesSeleccionadasPE.Count} partes para pulir y encerar.", "Aceptar");
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () => {
                    await DisplayAlert("Sin selección", "No ha seleccionado ninguna parte para pulir y encerar.", "Aceptar");
                });
            }
        }

        private async void ButtonSiguiente_Clicked(object sender, EventArgs e)
        {
            btnSiguiente.IsEnabled = false;

            try
            {
                await Navigation.PushAsync(new PaquetePulidoEncerado(
                    _datosAuto,
                    _prioridad,
                    _servicio,
                    _tipoGolpe,
                    _partesSeleccionadasPE,
                    _opcionTodo,
                    _imagenesTodoElVehiculo
                ));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema al navegar: {ex.Message}", "Aceptar");
                btnSiguiente.IsEnabled = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            btnSiguiente.IsEnabled = true;

            // Actualizar lista cada vez que la página aparezca
            ActualizarListView();
        }
    }
}
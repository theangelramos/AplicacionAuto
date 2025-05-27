using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AplicacionAuto
{
    public partial class GolpeLeveMedio : ContentPage, INotifyPropertyChanged
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        static Boolean alerta = false;
        private ObservableCollection<GolpeMedida> medidasList;

        // Para manejo de interfaz responsiva
        private double listHeight;
        public double ListHeight
        {
            get => listHeight;
            set
            {
                if (listHeight != value)
                {
                    listHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        private int golpeCounter = 1;
        private string selectedImagePath;
        private bool isLoading = false;

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;

        public byte[] imageData;

        // Constructor
        public GolpeLeveMedio(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe)
        {
            InitializeComponent();
            BindingContext = this;

            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;

            // Verificar conexión a internet
            bool tieneConexionInternet = NetworkAccess.Internet == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
            }
            else
            {
                CargarDatosServidor();
            }

            // Inicializar la lista de medidas
            medidasList = new ObservableCollection<GolpeMedida>();
            medidasList.CollectionChanged += (sender, e) => {
                ActualizarAlturaLista();

                // Actualizar visibilidad del botón Guardar
                siguienteButton.IsVisible = medidasList.Count > 0;
            };

            // Configurar CollectionView
            collectionView.ItemsSource = medidasList;
            CustomizeCollectionView();

            // Asegurarse que el botón Guardar se inicia oculto
            siguienteButton.IsVisible = false;
        }

        // Implementar INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método para cargar datos del servidor
        private async void CargarDatosServidor()
        {
            try
            {
                // Cargar piezas
                await CargarPiezas();

                // Cargar medidas horizontales
                await CargarMedidas("Diámetro horizontal hojalatería", pickerMedidaHorizontal);

                // Cargar medidas verticales
                await CargarMedidas("Diámetro vertical hojalatería", pickerMedidaVertical);

                // Cargar medidas de profundidad
                await CargarMedidas("Profundidad", pickerMedidaProfundidad);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
                CerrarApp("Error en la petición al servidor", "\n\nAsegúrate de tener una conexión estable a internet.");
            }
        }

        // Cargar piezas desde el servidor
        private async Task CargarPiezas()
        {
            peticion.PedirComunicacion("Pieza/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
            string jsonRecibir = await peticion.ObtenerJson();
            List<PiezaDTO> piezas = JsonConvertidor.Json_ListaObjeto<PiezaDTO>(jsonRecibir);

            var piezasFiltradas = piezas.FindAll(x => x.TipoPiezaNombre.ToLower() == "Hojalatería y pintura".ToLower());

            foreach (var pieza in piezasFiltradas)
            {
                pickerPartes.Items.Add(pieza.Nombre);
            }
        }

        // Cargar medidas desde el servidor
        private async Task CargarMedidas(string tipoMedidaNombre, Picker picker)
        {
            peticion.PedirComunicacion("Medida/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
            string jsonRecibir = await peticion.ObtenerJson();
            List<MedidaDTO> medidas = JsonConvertidor.Json_ListaObjeto<MedidaDTO>(jsonRecibir);

            var medidasFiltradas = medidas.FindAll(x => x.TipoMedidaNombre.ToLower() == tipoMedidaNombre.ToLower());

            foreach (var medida in medidasFiltradas)
            {
                picker.Items.Add(medida.Descripcion);
            }
        }

        // Mostrar alerta y cerrar
        public async void CerrarApp(String titulo, String mensaje)
        {
            if (alerta == false)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                alerta = true;
            }
        }

        // Seleccionar imagen
        private async void selectedImage_Clicked(object sender, EventArgs e)
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                var options = new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Seleccionar imagen"
                };

                var result = await FilePicker.PickAsync(options);

                if (result != null)
                {
                    selectedImagePath = result.FullPath;
                    selectedImage.Source = ImageSource.FromFile(result.FullPath);

                    using (FileStream stream = new FileStream(selectedImagePath, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            imageData = reader.ReadBytes((int)stream.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo cargar la imagen: {ex.Message}", "OK");
            }
            finally
            {
                isLoading = false;
            }
        }

        // Agregar golpe a la lista
        private async void AgregarGolpe_Clicked(object sender, EventArgs e)
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                // Verificar que todos los campos estén completos
                string opcionSeleccionada = pickerPartes.SelectedItem as string;
                string opcionSeleccionada2 = pickerMedidaHorizontal.SelectedItem as string;
                string opcionSeleccionada3 = pickerMedidaVertical.SelectedItem as string;
                string opcionSeleccionada4 = pickerMedidaProfundidad.SelectedItem as string;

                // Verificar que todos los campos estén completos antes de agregar
                if (string.IsNullOrEmpty(opcionSeleccionada))
                {
                    await DisplayAlert("Campo faltante", "Debes seleccionar una parte del carro", "OK");
                    isLoading = false;
                    return;
                }

                if (string.IsNullOrEmpty(selectedImagePath))
                {
                    await DisplayAlert("Campo faltante", "Debes seleccionar una imagen del golpe", "OK");
                    isLoading = false;
                    return;
                }

                if (string.IsNullOrEmpty(opcionSeleccionada2))
                {
                    await DisplayAlert("Campo faltante", "Debes seleccionar el diámetro horizontal", "OK");
                    isLoading = false;
                    return;
                }

                if (string.IsNullOrEmpty(opcionSeleccionada3))
                {
                    await DisplayAlert("Campo faltante", "Debes seleccionar el diámetro vertical", "OK");
                    isLoading = false;
                    return;
                }

                if (string.IsNullOrEmpty(opcionSeleccionada4))
                {
                    await DisplayAlert("Campo faltante", "Debes seleccionar la profundidad", "OK");
                    isLoading = false;
                    return;
                }

                // Si todos los campos están completos, crear el objeto GolpeMedida
                string medida = $"Diámetro horizontal: {opcionSeleccionada2}  \nDiámetro vertical: {opcionSeleccionada3} \nProfundidad: {opcionSeleccionada4}\n";

                GolpeMedida golpeMedida = new GolpeMedida
                {
                    ImagenPath = selectedImagePath,
                    DatoImagen = imageData,
                    PiezaNombre = opcionSeleccionada,
                    EtiquetaPiezaNombre = $"Pieza: {opcionSeleccionada}",
                    DiametroH = opcionSeleccionada2,
                    DiametroV = opcionSeleccionada3,
                    ProfundidadGolpe = opcionSeleccionada4,
                    EtiquetaDatos = medida
                };

                // Agregar a la lista
                medidasList.Add(golpeMedida);

                // Confirmar que se agregó el golpe
                await DisplayAlert("Éxito", "El golpe se ha agregado correctamente", "OK");

                // Limpiar selecciones
                pickerPartes.SelectedItem = null;
                pickerMedidaHorizontal.SelectedItem = null;
                pickerMedidaVertical.SelectedItem = null;
                pickerMedidaProfundidad.SelectedItem = null;
                selectedImage.Source = null;
                selectedImagePath = null;
                imageData = null;

                // Forzar actualización del layout y scroll
                this.ForceLayout();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo agregar el golpe: {ex.Message}", "OK");
            }
            finally
            {
                isLoading = false;
            }
        }

        // Mostrar ayuda
        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "¡Importante! Para un registro preciso, asegúrate de tener fotos claras del golpe en tu dispositivo. Esto nos ayudará a evaluar mejor el daño. Además, proporciona las medidas en centímetros del golpe, tanto horizontal como verticalmente. ¡Tu colaboración nos permite brindarte un servicio más preciso y eficiente en la hojalatería y pintura de tu vehículo!";

            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }

        // Al aparecer la página
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ocultar barra de navegación
            if (Navigation.NavigationStack.Count > 0)
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }

            // Habilitar botón
            siguienteButton.IsEnabled = true;

            // Verificar si debe mostrar el botón Guardar
            siguienteButton.IsVisible = medidasList.Count > 0;

            // Forzar ajuste de layout
            Device.BeginInvokeOnMainThread(() => {
                this.ForceLayout();
                ActualizarAlturaLista();
            });
        }

        // Ajustar altura del CollectionView basado en contenido
        private void ActualizarAlturaLista()
        {
            if (medidasList.Count > 0)
            {
                // Altura aproximada por cada elemento (ajusta según tu diseño)
                ListHeight = Math.Min(medidasList.Count * 250, 500);
            }
            else
            {
                ListHeight = 0;
            }
        }

        // Configurar el CollectionView
        private void CustomizeCollectionView()
        {
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var frame = new Frame
                {
                    BorderColor = Colors.LightGray,
                    CornerRadius = 10,
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 15),
                    HasShadow = true
                };

                var stack = new VerticalStackLayout
                {
                    Spacing = 10
                };

                var image = new Image
                {
                    HeightRequest = 150,
                    Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.Center
                };
                image.SetBinding(Image.SourceProperty, "ImagenPath");

                var labelPieza = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.Red
                };
                labelPieza.SetBinding(Label.TextProperty, "EtiquetaPiezaNombre");

                var labelDatos = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.Black
                };
                labelDatos.SetBinding(Label.TextProperty, "EtiquetaDatos");

                stack.Add(image);
                stack.Add(labelPieza);
                stack.Add(labelDatos);

                frame.Content = stack;
                return frame;
            });
        }

        // Botón para guardar y navegar a la siguiente página
        private async void Button_Guardar(object sender, EventArgs e)
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                siguienteButton.IsEnabled = false;

                // Verificar que haya al menos un golpe agregado
                if (medidasList.Count == 0)
                {
                    await DisplayAlert("Error", "Debes agregar al menos un golpe antes de continuar", "OK");
                    siguienteButton.IsEnabled = true;
                    isLoading = false;
                    return;
                }

                // Confirmar antes de continuar
                bool respuesta = await DisplayAlert("Confirmar", "¿Estás seguro de guardar y continuar?", "Sí", "No");

                if (respuesta)
                {
                    // Navegar a la siguiente página pasando los datos
                    await Navigation.PushAsync(new PaqueteHojalateriaPintura(datosAuto1, prioridad1, servicio1, tipoGolpe1, medidasList));
                }
                else
                {
                    siguienteButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo continuar: {ex.Message}", "OK");
                siguienteButton.IsEnabled = true;
            }
            finally
            {
                isLoading = false;
            }
        }

        // Al cambiar tamaño de la pantalla
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // Forzar actualización del layout al cambiar orientación o tamaño
            Device.BeginInvokeOnMainThread(() => {
                ActualizarAlturaLista();
            });
        }
    }
}
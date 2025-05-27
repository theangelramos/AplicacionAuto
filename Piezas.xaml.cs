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

namespace AplicacionAuto
{
    public partial class Piezas : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        static Boolean alerta = false;

        private ObservableCollection<GolpeMedida> medidasList;

        private int golpeCounter = 1;
        private string selectedImagePath;
        private bool isLoading = false;

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String opcionTodo1;

        public byte[] imageData;

        List<Imagen> imagenesGolpeFuerte = new List<Imagen>();

        public Piezas(DatosAuto datosAuto, String prioridad, String servicio)
        {
            InitializeComponent();

            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;

            // Verificar conexión a internet
            bool tieneConexionInternet = NetworkAccess.Internet == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
            }
            else
            {
                _ = CargarDatosPiezas();
            }

            // Inicializar la lista de medidas
            medidasList = new ObservableCollection<GolpeMedida>();
            medidasList.CollectionChanged += (sender, e) => {
                // Actualizar visibilidad del botón Guardar
                siguienteBtn.IsVisible = medidasList.Count > 0;
                siguienteBtn.IsEnabled = medidasList.Count > 0;

                // Actualizar visibilidad del contenedor de lista
                listadoBorder.IsVisible = medidasList.Count > 0;

                // Actualizar altura del CollectionView si es necesario
                if (medidasList.Count > 0)
                {
                    // Ajustar altura en función del número de elementos (máx 500)
                    collectionView.HeightRequest = Math.Min(medidasList.Count * 250, 500);
                }

                // Forzar actualización del layout y scroll
                Device.BeginInvokeOnMainThread(() => {
                    ForceLayout();
                });
            };

            // Configurar CollectionView
            collectionView.ItemsSource = medidasList;

            // Asegurar que los elementos inicien ocultos
            siguienteBtn.IsVisible = false;
            listadoBorder.IsVisible = false;
        }

        // Cargar las listas de piezas y medidas desde el servidor
        private async Task CargarDatosPiezas()
        {
            try
            {
                // Cargar piezas
                peticion.PedirComunicacion("Pieza/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
                string jsonRecibir = await peticion.ObtenerJson();
                List<PiezaDTO> piezas = JsonConvertidor.Json_ListaObjeto<PiezaDTO>(jsonRecibir);

                var piezasFiltradas = piezas.FindAll(x => x.TipoPiezaNombre.ToLower() == "Pintura".ToLower());
                piezasFiltradas.RemoveAll(p => p.Nombre == "Todo el vehículo");

                foreach (var pieza in piezasFiltradas)
                {
                    pickerPiezas.Items.Add(pieza.Nombre);
                }

                // Cargar medidas horizontales
                await CargarMedidas("Diámetro horizontal pintura", pickerMedidaHorizontal);

                // Cargar medidas verticales
                await CargarMedidas("Diámetro vertical pintura", pickerMedidaVertical);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
                CerrarApp("Error en la petición al servidor", "\n\nAsegúrate de tener una conexión estable a internet.");
            }
        }

        // Método para cargar medidas desde el servidor
        private async Task CargarMedidas(string tipoMedidaNombre, Picker picker)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando medidas {tipoMedidaNombre}: {ex.Message}");
                throw;
            }
        }

        public async void CerrarApp(String titulo, String mensaje)
        {
            if (alerta == false)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                alerta = true;
            }
        }

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

        private async void AgregarGolpe_Clicked(object sender, EventArgs e)
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                string opcionSeleccionada = pickerPiezas.SelectedItem as string;
                string opcionSeleccionada2 = pickerMedidaHorizontal.SelectedItem as string;
                string opcionSeleccionada3 = pickerMedidaVertical.SelectedItem as string;

                // Verificar que todos los campos estén completos
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

                // Crear el objeto GolpeMedida
                string medida = $"Diámetro horizontal: {opcionSeleccionada2} cm \r\nDiametro vertical: {opcionSeleccionada3} cm\n";

                GolpeMedida golpeMedida = new GolpeMedida
                {
                    ImagenPath = selectedImagePath,
                    DatoImagen = imageData,
                    PiezaNombre = opcionSeleccionada,
                    EtiquetaPiezaNombre = $"Pieza: {opcionSeleccionada}",
                    DiametroH = opcionSeleccionada2,
                    DiametroV = opcionSeleccionada3,
                    ProfundidadGolpe = null,
                    EtiquetaDatos = medida
                };

                // Agregar a la lista
                medidasList.Add(golpeMedida);

                // Mostrar botón guardar y activarlo
                siguienteBtn.IsVisible = true;
                siguienteBtn.IsEnabled = true;

                // Mostrar el contenedor del listado
                listadoBorder.IsVisible = true;

                // Confirmar que se agregó el golpe
                await DisplayAlert("Éxito", "El golpe se ha agregado correctamente", "OK");

                // Limpiar los campos
                pickerPiezas.SelectedItem = null;
                pickerMedidaHorizontal.SelectedItem = null;
                pickerMedidaVertical.SelectedItem = null;
                selectedImage.Source = null;
                selectedImagePath = null;
                imageData = null;

                // Forzar actualización del layout y scroll
                Device.BeginInvokeOnMainThread(() => {
                    ForceLayout();

                    // Hacer scroll para mostrar el listado agregado
                    if (medidasList.Count > 0)
                    {
                        // Dar tiempo al layout para actualizarse
                        Device.StartTimer(TimeSpan.FromMilliseconds(300), () => {
                            // Intentar hacer scroll hacia el listado
                            listadoBorder.Focus();
                            return false; // run once
                        });
                    }
                });
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

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "¡Importante! Para un registro preciso, asegúrate de tener fotos claras del golpe en tu dispositivo. Esto nos ayudará a evaluar mejor el daño. Además, proporciona las medidas en centímetros del golpe, tanto horizontal como verticalmente. ¡Tu colaboración nos permite brindarte un servicio más preciso y eficiente en la  pintura de tu vehículo!";

            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ocultar la barra de navegación programáticamente
            if (Navigation.NavigationStack.Count > 0)
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }

            // Asegurar que los estados sean correctos
            siguienteBtn.IsEnabled = medidasList.Count > 0;
            siguienteBtn.IsVisible = medidasList.Count > 0;
            listadoBorder.IsVisible = medidasList.Count > 0;

            // Configurar el CollectionView programáticamente
            CustomizeCollectionView();

            // Forzar actualización del layout
            Device.BeginInvokeOnMainThread(() => {
                ForceLayout();
            });
        }

        private void CustomizeCollectionView()
        {
            // Crear un DataTemplate programáticamente para los elementos del CollectionView
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var frame = new Frame
                {
                    BorderColor = Colors.LightGray,
                    CornerRadius = 10,
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 15),
                    HasShadow = true,
                    BackgroundColor = Colors.White
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

        private async void Button_Guardar(object sender, EventArgs e)
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                siguienteBtn.IsEnabled = false;

                // No es necesaria esta verificación adicional ya que el botón solo está visible si hay elementos,
                // pero lo mantenemos por seguridad
                if (medidasList.Count == 0)
                {
                    await DisplayAlert("Error", "Debes agregar al menos un golpe antes de continuar", "OK");
                    siguienteBtn.IsEnabled = true;
                    isLoading = false;
                    return;
                }

                await Navigation.PushAsync(new PaquetePintura(datosAuto1, prioridad1, servicio1, medidasList, opcionTodo1, imagenesGolpeFuerte));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo continuar: {ex.Message}", "OK");
                siguienteBtn.IsEnabled = true;
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
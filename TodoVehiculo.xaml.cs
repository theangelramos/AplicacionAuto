using AplicacionAuto.Clases;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AplicacionAuto
{
    public partial class TodoVehiculo : ContentPage
    {
        private List<Image> selectedImages;

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        ObservableCollection<GolpeMedida> datosGolpes;
        String opcionTodo1;

        List<Imagen> imagenesTodoElVehiculo = new List<Imagen>();

        public TodoVehiculo(DatosAuto datosAuto, String prioridad, String servicio, String opcionTodo)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            opcionTodo1 = opcionTodo;
            selectedImages = new List<Image>();

            // Ocultar la barra de navegación programáticamente
            Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void selectedImage_Clicked(object sender, EventArgs e)
        {
            imagenesTodoElVehiculo.Clear();
            var options = new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Seleccionar imágenes"
            };

            var result = await FilePicker.PickMultipleAsync(options);
            byte[] imageData;
            String sourceImagen;

            if (result != null)
            {
                selectedImage1.Source = null;
                selectedImage2.Source = null;
                selectedImage3.Source = null;
                selectedImages.Clear();

                int imagesCount = result.Count() > 3 ? 3 : result.Count();

                for (int i = 0; i < imagesCount; i++)
                {
                    var image = new Image
                    {
                        Source = ImageSource.FromFile(result.ElementAt(i).FullPath),
                        WidthRequest = 100,
                        HeightRequest = 100
                    };

                    sourceImagen = result.ElementAt(i).FullPath;
                    using (FileStream stream = new FileStream(sourceImagen, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            imageData = reader.ReadBytes((int)stream.Length);

                            String imageDataString = imageData.ToString();
                            String imageDataString2 = Convert.ToBase64String(imageData);
                            Console.WriteLine(imageDataString);
                            Console.WriteLine(imageDataString2);
                        }
                    }

                    var imagen = new Imagen
                    {
                        DatoImagen = imageData
                    };

                    imagenesTodoElVehiculo.Add(imagen);
                    selectedImages.Add(image);

                    if (i == 0)
                        selectedImage1.Source = image.Source;
                    else if (i == 1)
                        selectedImage2.Source = image.Source;
                    else if (i == 2)
                        selectedImage3.Source = image.Source;
                }

                continuarBtn.IsEnabled = imagesCount > 0;

                // Ya no necesitamos ajustar la posición del botón porque estamos usando StackLayout
                // en lugar de Grid
            }
        }

        private async void ButtonContinuar(object sender, EventArgs e)
        {
            continuarBtn.IsEnabled = false;
            await Navigation.PushAsync(new PaquetePintura(datosAuto1, prioridad1, servicio1, datosGolpes, opcionTodo1, imagenesTodoElVehiculo));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            continuarBtn.IsEnabled = true;
        }
    }
}
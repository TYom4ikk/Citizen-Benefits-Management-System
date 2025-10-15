using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для CitizenSelectWindow.xaml
    /// </summary>
    public partial class CitizenSelectWindow : Window
    {
        private readonly CitizensController _citizensController;
        private List<Citizens> _allCitizens;

        public int SelectedCitizenId { get; private set; }
        public string SelectedCitizenName { get; private set; }

        public CitizenSelectWindow()
        {
            InitializeComponent();
            _citizensController = new CitizensController();
        }

        /// <summary>
        /// Обработчик загрузки окна
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCitizens();
        }

        /// <summary>
        /// Загружает список граждан
        /// </summary>
        private void LoadCitizens()
        {
            _allCitizens = _citizensController.GetAllCitizens();
            DgCitizens.ItemsSource = _allCitizens;
        }

        /// <summary>
        /// Обработчик изменения текста поиска
        /// </summary>
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCitizens();
        }

        /// <summary>
        /// Фильтрует граждан по поисковому запросу
        /// </summary>
        private void FilterCitizens()
        {
            if (_allCitizens == null)
                return;

            string searchText = TxtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                DgCitizens.ItemsSource = _allCitizens;
            }
            else
            {
                var filtered = _allCitizens.Where(c =>
                    c.LastName.ToLower().Contains(searchText) ||
                    c.FirstName.ToLower().Contains(searchText) ||
                    (c.MiddleName != null && c.MiddleName.ToLower().Contains(searchText)) ||
                    c.SNILS.Contains(searchText)
                ).ToList();

                DgCitizens.ItemsSource = filtered;
            }
        }

        /// <summary>
        /// Обработчик двойного клика по строке
        /// </summary>
        private void DgCitizens_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectCitizen();
        }

        /// <summary>
        /// Обработчик кнопки "Выбрать"
        /// </summary>
        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectCitizen();
        }

        /// <summary>
        /// Выбирает гражданина
        /// </summary>
        private void SelectCitizen()
        {
            if (DgCitizens.SelectedItem is Citizens citizen)
            {
                SelectedCitizenId = citizen.CitizenID;
                SelectedCitizenName = $"{citizen.LastName} {citizen.FirstName} {citizen.MiddleName}";
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Выберите гражданина из списка", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Отмена"
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

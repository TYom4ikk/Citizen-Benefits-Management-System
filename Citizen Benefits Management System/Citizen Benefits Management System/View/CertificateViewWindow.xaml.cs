using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Model;
using System.Windows;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для CertificateViewWindow.xaml
    /// </summary>
    public partial class CertificateViewWindow : Window
    {
        public CertificateViewWindow(Certificates certificate)
        {
            InitializeComponent();
            LoadCertificateData(certificate);
        }

        /// <summary>
        /// Загружает данные справки
        /// </summary>
        private void LoadCertificateData(Certificates certificate)
        {
            TxtCertificateNumber.Text = certificate.CertificateID.ToString();
            TxtCitizenName.Text = $"{certificate.Citizens.LastName} {certificate.Citizens.FirstName} {certificate.Citizens.MiddleName}";
            TxtSNILS.Text = ValidationHelper.FormatSNILS(certificate.Citizens.SNILS);
            TxtCertificateType.Text = certificate.CertificateType;
            TxtIssueDate.Text = certificate.IssueDate.ToString("dd.MM.yyyy");
            TxtIssuedBy.Text = $"{certificate.Users.LastName} {certificate.Users.FirstName} {certificate.Users.MiddleName}";
            TxtNotes.Text = string.IsNullOrEmpty(certificate.Notes) ? "Нет примечаний" : certificate.Notes;
        }

        /// <summary>
        /// Обработчик кнопки "Закрыть"
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

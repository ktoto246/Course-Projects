using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore; // Обязательно для AsNoTracking
using WpfApp1.Data;
using WpfApp1.Helpers;

namespace WpfApp1
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                // Фикс 1: Добавили AsNoTracking(), чтобы отвязать юзера от закрывающегося контекста БД
                var user = db.Employees
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        MessageBox.Show("Этот аккаунт заблокирован. Обратитесь к администратору.", "Доступ закрыт", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Фикс 2: Используем метод Login вместо прямого присваивания
                    CurrentSession.Login(user);

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
using System;
using WpfApp1.Models;

namespace WpfApp1.Helpers
{
    public static class CurrentSession
    {
        public static Employee CurrentUser { get; private set; }

        public static void Login(Employee user)
        {
            CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
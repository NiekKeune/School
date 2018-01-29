using System;
using System.Windows;

namespace School
{
    /// <summary>
    /// Interaction logic for StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        #region Predefined code

        public StudentForm()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(this.firstName.Text))
            {
                var Notification = MessageBox.Show("The student must have a first name.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (String.IsNullOrEmpty(this.lastName.Text))
            {
                var Notification = MessageBox.Show("The student must have a last name.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime result; 
            if (!DateTime.TryParse(this.dateOfBirth.Text, out result))
            {
                var Notification = MessageBox.Show("The student must have a date of birth.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
           // result = (DateTime.TryParse(this.dateOfBirth.Text, out result));
            TimeSpan age = DateTime.Now.Subtract(result); 
            if (age.Days / 365.25 < 5)
            {
                MessageBox.Show("The student must be at least 5 years old", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.DialogResult = true;
        }

        #endregion
    }
}

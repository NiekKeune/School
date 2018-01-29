using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using School.Data;


namespace School
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Connection to the School database
        private SchoolDBEntities schoolContext = null;

        // Field for tracking the currently selected teacher
        private Teacher teacher = null;

        // List for tracking the students assigned to the teacher's class
        private IList studentsInfo = null;

        #region Predefined code

        public MainWindow()
        {
            InitializeComponent();
        }

        // Connect to the database and display the list of teachers when the window appears
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.schoolContext = new SchoolDBEntities();
            teachersList.DataContext = this.schoolContext.Teachers;
        }

        // When the user selects a different teacher, fetch and display the students for that teacher
        private void teachersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Find the teacher that has been selected
            this.teacher = teachersList.SelectedItem as Teacher;
            this.schoolContext.LoadProperty<Teacher>(this.teacher, s => s.Students);

            // Find the students for this teacher
            this.studentsInfo = ((IListSource)teacher.Students).GetList();

            // Use databinding to display these students
            studentsList.DataContext = this.studentsInfo;
        }

        #endregion

        // When the user presses a key, determine whether to add a new student to a class, remove a student from a class, or modify the details of a student
        private void studentsList_KeyDown(object sender, KeyEventArgs e)
        {
            var Student = studentsList.SelectedItem as Student;                                  //tracks the selected student

            switch (e.Key)                                                          
            {
                case Key.Enter:                                                                  //when enter is pressed, it goes to the form to edit a students info
                    EditStudent(Student);
                    break;

                case Key.Insert:                                                                 //when insert is pressed, it goes to the form to add a student. 
                    AddNewStudent();
                    break;

                case Key.Delete:                                                                 //when delete is pressed, it goes to the messagebox to delete a student
                    RemoveStudent(Student);
                    break;
            }

        }

        private void EditStudent(Student Student)
        {
            StudentForm sf = new StudentForm();
            sf.Title = "Edit Student Details";
            sf.firstName.Text = Student.FirstName;                                           //converts the values from a student to string in the textboxes
            sf.lastName.Text = Student.LastName;                                             //
            sf.dateOfBirth.Text = Student.DateOfBirth.ToString("d");                         //converts the datetime of their birthdate to string

            sf.ShowDialog();

            bool checkEdit = sf.DialogResult ?? false;                                           //sf. gets the method for the new form and not the general method, ?? makes sure that the boolean can't return null.

            if (checkEdit)
            {
                Student.FirstName = sf.firstName.Text;                                       //saves the changed values in the textbox to the student
                Student.LastName = sf.lastName.Text;                                         //
                Student.DateOfBirth = DateTime.Parse(sf.dateOfBirth.Text);                   //converts the string of birthdate back to datetime

                saveChanges.IsEnabled = true;
            }
        }

        private void AddNewStudent()
        {
            StudentForm addStudent = new StudentForm();                                  //however, you'll have to select a student first before you can press insert, whilst it'll be better if you can press insert before that
            addStudent.Title = "New Student for Class" + teacher.Class;

            addStudent.ShowDialog();

            bool checkAdd = addStudent.DialogResult ?? false;                            //sf. gets the method for the new form and not the general method, ?? makes sure that the boolean can't return null.

            if (checkAdd)
            {
                Student addedStudent = new Student();

                addedStudent.FirstName = addStudent.firstName.Text;                           //saves the inserted values in the textbox to the new student
                addedStudent.LastName = addStudent.lastName.Text;                             //
                addedStudent.DateOfBirth = DateTime.Parse(addStudent.dateOfBirth.Text);       //converts the string of birthdate back to datetime

                this.teacher.Students.Add(addedStudent);

                saveChanges.IsEnabled = true;
            }
        }

        private void RemoveStudent(Student Student)
        {
            var Notification = MessageBox.Show("Remove " + Student.FirstName + " " +     //creates the messagebox, sets the standard button to no incase the user presses enter too fast so he won't accidentally delete a student
                        Student.LastName + "?", "Confirm", MessageBoxButton.YesNo,
                        MessageBoxImage.Question, MessageBoxResult.No);
            if (Notification.Equals(MessageBoxResult.Yes))
            {
                this.teacher.Students.Remove(Student);

                saveChanges.IsEnabled = true;
            }
        }
            #region Predefined code

        private void studentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var Student = studentsList.SelectedItem as Student;                                  //tracks the selected student

            EditStudent(Student);
        }

        // Save changes back to the database and make them permanent
        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                schoolContext.SaveChanges();

                saveChanges.IsEnabled = false;
            }
            catch(OptimisticConcurrencyException)
            {
                // If the user has changed the same students earlier, then overwrite their changes with the new data.
                schoolContext.Refresh(RefreshMode.ClientWins, schoolContext.Students);
                schoolContext.SaveChanges();
            }
            catch(UpdateException uEx)
            {
                // If some sort of database exception has occured, then display the reason for the exception and rollback.
                MessageBox.Show(uEx.InnerException.Message, "Error saving changes");
                this.schoolContext.Refresh(RefreshMode.StoreWins, schoolContext.Students);
            }
            catch(Exception ex)
            {
                // If some other exception occurs, report it to the user.
                MessageBox.Show(ex.Message, "Error saving changes");
                this.schoolContext.Refresh(RefreshMode.ClientWins, schoolContext.Students);
            }
        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(Decimal))]
    class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            else
            {
                DateTime studentDate = (DateTime)value;                                          //not sure how this works

                TimeSpan studentYearDifference = DateTime.Now.Subtract(studentDate);

                double studentAge = (studentYearDifference.Days / 365.25);

                int studentAge2 = (int)studentAge;
                string studentAge3 = (studentAge2.ToString());
                return studentAge3;
            }
            #region Predefined code
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

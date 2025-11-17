using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using StudentManagementApp.Data;
using StudentManagementApp.Models;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Data;

namespace StudentManagementApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public List<string> Departments { get; }
        private readonly ObservableCollection<Student> _studentsCollection;
        public ICollectionView StudentsView { get; set; }

        // Add this new property for the search text
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                StudentsView.Refresh(); // This re-runs the filter
            }
        }
        // Database Context

        private readonly AppDbContext _dbContext;

        // --- Properties for Data Binding ---

        // This list is bound to the DataGrid

        // This property is bound to the "Student Details" editing form
        private Student? _editingStudent;
        public Student? EditingStudent
        {
            get => _editingStudent;
            set { _editingStudent = value; OnPropertyChanged(); }
        }

        // This property is bound to the selected item in the DataGrid
        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();

                // When a student is selected, copy their details to the edit form
                if (_selectedStudent != null)
                {
                    EditingStudent = new Student
                    {
                        Id = _selectedStudent.Id,
                        FirstName = _selectedStudent.FirstName,
                        LastName = _selectedStudent.LastName,
                        Email = _selectedStudent.Email,
                        DateOfBirth = _selectedStudent.DateOfBirth,
                        GPA = _selectedStudent.GPA
                    };
                }
            }
        }

        // --- Commands for Buttons ---
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ResetEditCommand { get; }

        // --- Constructor ---
        public MainViewModel()
        {
            Departments = new List<string>
            {
                "Computer Science",
                "Engineering",
                "Arts & Humanities",
                "Business",
                "Science"
            };
            // 1. Initialize Database
            _dbContext = new AppDbContext();
            _dbContext.Database.EnsureCreated();

            // 2. Load Data into the *private* collection
            _studentsCollection = new ObservableCollection<Student>(
                _dbContext.Students.ToList()
            );

            // 3. Create the *public* view from the collection
            StudentsView = CollectionViewSource.GetDefaultView(_studentsCollection);
            StudentsView.Filter = FilterStudents; // Link the filter logic
            // 3. Initialize Commands
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            SaveCommand = new RelayCommand(OnSave);
            ResetEditCommand = new RelayCommand(ResetEditForm);

            // 4. Set up the edit form
            ResetEditForm();
        }

        // --- Command Logic Methods ---
        private bool FilterStudents(object item)
        {
            if (item is Student student)
            {
                // If search text is empty, show everything
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;

                // Otherwise, check for matches (case-insensitive)
                return student.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       student.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        private void OnAdd()
        {
            // Set up the form to add a new, blank student
            EditingStudent = new Student
            {
                Id = 0, // 0 indicates a new student
                DateOfBirth = DateTime.Now.AddYears(-18) // Set a default DOB
            };
        }

        private void OnDelete()
        {
            if (SelectedStudent == null) return;

            // Remove from Database
            _dbContext.Students.Remove(SelectedStudent);
            _dbContext.SaveChanges();

            // Remove from the *private* collection
            _studentsCollection.Remove(SelectedStudent);
        }

        private bool CanDelete()
        {
            // Can only delete if a student is selected
            return SelectedStudent != null;
        }

        private void OnSave()
        {
            if (EditingStudent == null) return;

            // --- START VALIDATION ---
            if (string.IsNullOrWhiteSpace(EditingStudent.FirstName))
            {
                MessageBox.Show("First Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Stop the save
            }

            if (EditingStudent.GPA < 0.0 || EditingStudent.GPA > 4.0)
            {
                MessageBox.Show("GPA must be between 0.0 and 4.0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Stop the save
            }
            // --- END VALIDATION ---


            // If validation passes, the rest of your save logic runs
            if (EditingStudent.Id == 0)
            {
                // This is a NEW student
                _dbContext.Students.Add(EditingStudent);
                _dbContext.SaveChanges(); // Save to get the new ID from the database

                // Add to the UI list
                _studentsCollection.Add(EditingStudent);
            }
            else
            {
                // This is an EXISTING student
                var existingStudentInDb = _dbContext.Students.Find(EditingStudent.Id);
                if (existingStudentInDb != null)
                {
                    // Update database
                    existingStudentInDb.FirstName = EditingStudent.FirstName;
                    existingStudentInDb.LastName = EditingStudent.LastName;
                    existingStudentInDb.Email = EditingStudent.Email;
                    existingStudentInDb.DateOfBirth = EditingStudent.DateOfBirth;
                    existingStudentInDb.GPA = EditingStudent.GPA;
                    existingStudentInDb.Department = EditingStudent.Department;
                    _dbContext.SaveChanges();

                    // Update the UI list
                    var existingStudentInView = _studentsCollection.FirstOrDefault(s => s.Id == EditingStudent.Id);
                    if (existingStudentInView != null)
                    {
                        existingStudentInView.FirstName = EditingStudent.FirstName;
                        existingStudentInView.LastName = EditingStudent.LastName;
                        existingStudentInView.Email = EditingStudent.Email;
                        existingStudentInView.DateOfBirth = EditingStudent.DateOfBirth;
                        existingStudentInView.GPA = EditingStudent.GPA;
                        existingStudentInView.Department = EditingStudent.Department;
                    }
                }
                StudentsView.Refresh(); // Refresh the view to show updates
            }
            // Clear the form
            ResetEditForm();
        }

        private void ResetEditForm()
        {
            EditingStudent = new Student { DateOfBirth = DateTime.Now.AddYears(-18) };
        }


        // --- Boilerplate INotifyPropertyChanged ---
        // (This code tells the UI when a property changes)
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // --- Helper RelayCommand Class ---
    // (This class makes the ICommand bindings work)
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();
    }
}
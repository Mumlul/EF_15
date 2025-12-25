using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EF_Core_15.Models;
using EF_Core_15.Service;

namespace EF_Core_15.Pages;

public partial class CategoryPage : Page,INotifyPropertyChanged
{
    #region fields

        private ObservableCollection<Category> _categories = new ();
        private ICollectionView _productsView;
        private Category _selectedCategory=new ()!;
        private bool addbuttonenabled=true;
        private bool changebuttonenabled=false;
        private string _newcategoryname = "";

    #endregion
    
    #region properties

        public Ef15Context db = DbService.Instance.Context;
        public ObservableCollection<Category> Categories { get => _categories;
            set
            {
                /*if(SetField(ref _categories,value)) _categories = value;*/
                SetField(ref _categories, value);
            }
        }
        public Category SelectedCategory {get  => _selectedCategory;
            set
            {
                if (SetField(ref _selectedCategory, value))
                {
                    if (_selectedCategory != null)
                    {
                        Addbuttonenabled = false;
                        Changebuttonenabled=true;
                        Newcategoryname=_selectedCategory.Name;
                    }
                    else
                    {
                        Addbuttonenabled = true;
                        Changebuttonenabled=false;
                    }
                }
            }}
        public bool Addbuttonenabled {get=> addbuttonenabled;set => SetField(ref addbuttonenabled, value);}
        public bool Changebuttonenabled {get=> changebuttonenabled;set => SetField(ref changebuttonenabled, value);}
        public string  Newcategoryname {get => _newcategoryname;set => SetField(ref _newcategoryname, value);}

    #endregion
    
    #region INotifyPropertyChangedImplementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    #endregion
    
    #region methods
    #endregion
    
    #region events
        private void RefreshChange(object sender, RoutedEventArgs e)
        {
            SelectedCategory = null;
        }
        public CategoryPage(ObservableCollection<Category> categories)
        {
            InitializeComponent();
            foreach(var c in categories)
                Categories.Add(c);
            _productsView=CollectionViewSource.GetDefaultView(Categories);
            Application.Current.MainWindow.Title = "Страница с категориями";
            DataContext = this;
        }
        private void ChangeCategory(object sender, RoutedEventArgs e)
        {
            SelectedCategory.Name = Newcategoryname;
            db.Categories.Update(SelectedCategory);
            db.SaveChanges();
            _productsView.Refresh();
            SelectedCategory = null;
            Newcategoryname = "";
        }
        private void AddCategory(object sender, RoutedEventArgs e)
        {
            var category = new Category()
            {
                Id = Categories.Count + 1,
                Name = Newcategoryname,
            };
            db.Categories.Add(category);
            db.SaveChanges();
            Categories.Add(category);
            _productsView.Refresh();
            SelectedCategory = null;
            Newcategoryname = "";
        }
        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
        var result = MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            db.Categories.Remove(SelectedCategory);
            db.SaveChanges();
            _productsView.Refresh();
            Categories.Remove(SelectedCategory);
            SelectedCategory = null;
            Newcategoryname = "";
        }

           
        }
    #endregion
    
    #region navigations
        private void GoBack(object sender, RoutedEventArgs e) =>NavigationService.Navigate(new Main(true));
    #endregion
}
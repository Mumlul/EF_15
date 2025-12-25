using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EF_Core_15.Models;
using EF_Core_15.Service;

namespace EF_Core_15.Pages;

public partial class BrandPage : Page,INotifyPropertyChanged
{
    #region fields
        private ObservableCollection<Brand> _brands = new ();
        private ICollectionView _productsView;
        private Brand _selectedbrand=new ()!;
        private bool addbuttonenabled=true;
        private bool changebuttonenabled=false;
        private string _newbrandname="";
    #endregion
    
    #region properties
        public Ef15Context db = DbService.Instance.Context;
        public ObservableCollection<Brand> Brands { get => _brands;
            set
            {
                SetField(ref _brands, value);
            }
        }
        public Brand SelectedBrand {get  => _selectedbrand; set
            {
                if (SetField(ref _selectedbrand, value))
                {
                    if (_selectedbrand != null)
                    {
                        Addbuttonenabled = false;
                        Changebuttonenabled=true;
                        Newbrandname=_selectedbrand.Name;
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
        public string Newbrandname { get => _newbrandname; set => SetField(ref _newbrandname,value); }

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
        public BrandPage(ObservableCollection<Brand> brands)
        {
            InitializeComponent();
            foreach (var b in brands)
                Brands.Add(b);
            _productsView=CollectionViewSource.GetDefaultView(Brands);
            Application.Current.MainWindow.Title = "Страница с брендами";
            DataContext = this;
        }
        private void RefreshChange(object sender, RoutedEventArgs e)
        {
            SelectedBrand = null;
        }
        private void ChangeBrand(object sender, RoutedEventArgs e)
        {
            SelectedBrand.Name = Newbrandname;
            db.Brands.Update(SelectedBrand);
            db.SaveChanges();
            _productsView.Refresh();
            SelectedBrand = null;
            Newbrandname = "";
        }
        private void AddBrand(object sender, RoutedEventArgs e)
        {
            var brand = new Brand()
            {
                Id = Brands.Count + 1,
                Name = Newbrandname,
            };
            db.Brands.Add(brand);
            db.SaveChanges();
            Brands.Add(brand);
            _productsView.Refresh();
            SelectedBrand = null;
            Newbrandname = "";
        }
        private void DeleteBrand(object sender, RoutedEventArgs e)
        {
            db.Brands.Remove(SelectedBrand);
            db.SaveChanges();
            _productsView.Refresh();
            Brands.Remove(SelectedBrand);
            SelectedBrand = null;
            Newbrandname = "";
        }
    #endregion

    #region navigations
    private void GoBack(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new Main(true));
    }
    #endregion
    
    
    
    

   
}
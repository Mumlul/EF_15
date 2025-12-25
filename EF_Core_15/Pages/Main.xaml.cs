using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using EF_Core_15.Models;
using EF_Core_15.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EF_Core_15.Pages;

public partial class Main : Page,INotifyPropertyChanged
{
    #region Private fields
        private ICollectionView _productsView;
        private string _searchText=null!;
        private bool _filtercategory=false;
        private bool _filterbrand=false;
        private Brand? _selectedBrand;
        private Category? _selectedCategory;
        private bool _pricefilter = false;
        private bool _managerbuttons = false;
        private int _allcount = 0;
        private int _currentcount = 0;
    #endregion
    
    #region Properties
        public Ef15Context db = DbService.Instance.Context;
        public ObservableCollection<Product> Products { get;} = new();
        public ObservableCollection<Brand> BrandsName { get; } = new();
        public ObservableCollection<Category> CategoriesName { get; } = new();
        public string MinPrice { get; set; } = null!;
        public string MaxPrice { get; set; } = null!;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                {
                    RefreshView();
                }
            }
        }
        public bool ManagerButtons { get => _managerbuttons; set => SetField(ref _managerbuttons, value); }
        public bool FilterCategory {get => _filtercategory; set=>SetField(ref _filtercategory, value);}
        public bool FilterBrand {get => _filterbrand; set=>SetField(ref _filterbrand, value);}
        public Brand? SelectedBrand { get => _selectedBrand; set { if (SetField(ref _selectedBrand, value)) _productsView.Refresh(); } }
        public Category? SelectedCategory { get => _selectedCategory; set { if (SetField(ref _selectedCategory, value)) _productsView.Refresh(); } }
        public ObservableCollection<Tag> TagsName { get; } = new();
        public bool PriceFilter {get => _pricefilter; set=>SetField(ref _pricefilter, value);}
        public int AllCount { get => _allcount; set=>SetField(ref _allcount, value); }
        public int CurrentCount { get => _currentcount; set=>SetField(ref _currentcount, value); }
    #endregion
    
    #region Events
        public Main(bool _ismanager)
        {   Products.Clear();
            var products = db.Products
                .Include(p => p.Tags)
                .Include(p => p.Category)
                .ToList();
            foreach (var product in products)
            {
                Products.Add(product);
            }
            
            foreach (var brand in db.Brands)
                BrandsName.Add(brand);

            foreach (var category in db.Categories)
                CategoriesName.Add(category);

            foreach (var t in db.Tags)
                TagsName.Add(t);
            
            _productsView = CollectionViewSource.GetDefaultView(Products);
            _productsView.Filter = FilterProduct;
            InitializeComponent();
            ManagerButtons = _ismanager;
            Application.Current.MainWindow.Title = "Главная страница";
            AllCount = Products.Count;
            RefreshView();
            DataContext = this;
        }
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            SortPopup.IsOpen = !SortPopup.IsOpen;
        }
    
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterPopup.IsOpen = !FilterPopup.IsOpen;
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            _productsView.SortDescriptions.Clear();
        }
        private void FilterPrice(object sender, RoutedEventArgs e)
        {
            _productsView.Filter = FilterProduct;
            RefreshView();
        }

        private void RefreshFilter(object sender, RoutedEventArgs e)
        {
            RefreshView();
            SelectedCategory = null;
            SelectedBrand = null;
            MinPrice = "";
            MaxPrice = "";
            FilterCategory = false;
            FilterBrand = false;
            PriceFilter = false;
        }
        private void SortRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton rb) return;
            _productsView.SortDescriptions.Clear();
            switch (rb.Content.ToString())
            {
                case "По имени (а-я)":
                    _productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Name), ListSortDirection.Ascending));
                    break;
                case "По имени (я-а)":
                    _productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Name), ListSortDirection.Descending));
                    break;
                case "По цене (убывание)":
                    _productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Descending));
                    break;
                case "По цене (возрастание)":
                    _productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Ascending));
                    break;
                case "По кол-ву (убывание)":
                    _productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Stock), ListSortDirection.Descending));
                    break;
                case "По кол-ву (возрастание)":
                    _productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Stock), ListSortDirection.Ascending));
                    break;
            }
            RefreshView();
            SortPopup.IsOpen = false; 
        }

        private void Test(object sender, RoutedEventArgs E)
        {
            if (sender is Border border &&
                border.DataContext is Product product &&
                product.Stock < 10)
            {
                border.BorderBrush = (Brush)new BrushConverter()
                    .ConvertFromString("#ffd129");
                border.BorderThickness = new Thickness(2);
            }
        }
    #endregion
    
    #region INotifyPropertyChanged Implementation
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

    #region Methods
        private bool FilterProduct(object obj)
        {
            if(obj is not Product)
                return false;
            
            var product = (Product)obj;
            
            if (FilterCategory && SelectedCategory != null)
            {
                if (product.CategoryId != SelectedCategory.Id)
                    return false;
            }
            
            if (FilterBrand && SelectedBrand != null)
            {
                if (product.BrandId != SelectedBrand.Id)
                    return false;
            }
            
            if (SearchText != null && !product.Name.Contains(SearchText,
                    StringComparison.CurrentCultureIgnoreCase))
                return false;
            
            if (!MinPrice.IsNullOrEmpty() && Convert.ToDouble(MinPrice) > product.Price)
                return false;
            if (!MaxPrice.IsNullOrEmpty() && Convert.ToDouble(MaxPrice) < product.Price)
                return false;
            
            return true;
        }
        private void RefreshView()
        {
            _productsView.Refresh();
            CurrentCount = _productsView.Cast<object>().Count();
        }

    #endregion

    #region Navigation
        private void GotoProducts(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductPage(Products));
        }
        private void GotoBrands(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BrandPage(BrandsName));
        }
        private void GotoTags(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TagPage(TagsName));
        }
        private void GotoCategory(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CategoryPage(CategoriesName));
        }
        

    #endregion
    

    

    

    

   
}
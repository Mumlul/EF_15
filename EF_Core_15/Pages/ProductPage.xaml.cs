using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EF_Core_15.Models;
using EF_Core_15.Service;

namespace EF_Core_15.Pages;

public partial class ProductPage : Page,INotifyPropertyChanged
{
    #region fields
        private Product changeproduct=new Product()!;
        private bool _addbutton = true;
        private bool _changebutton = false;
        private ICollectionView _productsView;
        private Brand _brand =null!;
        private Category _category = null!;
        private ObservableCollection<Tag> _selectedTags = new();
        private string _searchtext = null!;
        private Product _newProduct=new (){ Rating = 0};
    #endregion
    
    #region properties
        public Ef15Context db = DbService.Instance.Context;
        public ObservableCollection<Product> Products { get;} = new();
        public bool AddButton { get => _addbutton; set => SetField(ref _addbutton, value); }
        public bool ChangeButton { get => _changebutton; set =>SetField(ref _changebutton, value); }
        public Product ChangeProduct 
        { 
            get => changeproduct; 
            set
            {
                if (SetField(ref changeproduct, value))
                {
                    SelectedTags = (changeproduct?.Tags != null) 
                        ? new ObservableCollection<Tag>(changeproduct.Tags) 
                        : new ObservableCollection<Tag>();
                    
                    if (changeproduct != null && changeproduct.Tags != null)
                    {
                        SelectedTags = new ObservableCollection<Tag>(changeproduct.Tags);
                    }
                    else
                    {
                        SelectedTags = new ObservableCollection<Tag>();
                    }

                    if (changeproduct!=null)
                    {
                        AddButton = false;
                        ChangeButton = true;
                    }
                    else
                    {
                        AddButton = true;
                        ChangeButton = false;
                    }
                    
                    if (value != null)
                    {
                        SelectedBrand = value.Brand;
                        SelectedCategory = value.Category;
                        NewProduct = changeproduct;
                    }
                    else
                    {
                        SelectedBrand = null;
                        SelectedCategory = null;
                        NewProduct=new ();
                    }
                }
            } 
        }
        public ObservableCollection<Tag> SelectedTags { get => _selectedTags; set => SetField(ref _selectedTags, value); }
        public ObservableCollection<Brand> Brands { get; } = new();
        public ObservableCollection<Category> Categories { get; } = new();
        public ObservableCollection<Tag> TagsName { get; } = new();
        public Brand SelectedBrand { get => _brand; set { if (SetField(ref _brand, value)&& NewProduct!=null) NewProduct.Brand = value; } }
        public Category SelectedCategory { get => _category; set { if (SetField(ref _category, value) && NewProduct!=null) NewProduct.Category = value; } }
        public string Searchtext { get => _searchtext; set { if (SetField(ref _searchtext, value)) {_searchtext = value; _productsView.Refresh(); } } }

        public Product NewProduct
        {
            get => _newProduct;
            set
            {
                SetField(ref _newProduct, value);
            }
        }
        #endregion
    
    #region events
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            slider.Value = Math.Round(slider.Value, 1);
        }
        private void ResetSelecProduct(object sender, RoutedEventArgs e)
        {
            ChangeProduct = null;
        }
        public ProductPage(ObservableCollection<Product>? products)
        {
            InitializeComponent();
            foreach (var product in products)
            {
                Products.Add(product);
            }
            
            foreach (var brand in db.Brands)
                Brands.Add(brand);

            foreach (var category in db.Categories)
                Categories.Add(category);
            
            foreach (var tag in db.Tags)
                TagsName.Add(tag);
            _productsView = CollectionViewSource.GetDefaultView(Products);
            _productsView.Filter = Filter;
            DataContext = this;
            Application.Current.MainWindow.Title = "Страница с продуктами";
        }
        
        private void AddProduct(object sender, RoutedEventArgs e)
        {

        MessageBox.Show(NewProduct.Category.Id.ToString());
            var product = new Product()
            {
                Id = Products.Count + 1,
                Name = NewProduct.Name,
                Description = NewProduct.Description,
                Price = NewProduct.Price,
                Category = NewProduct.Category,
                Brand = NewProduct.Brand,
                Stock = NewProduct.Stock,
                Tags = SelectedTags.ToList(),
                Rating = NewProduct.Rating,
                CreatedAt = DateOnly.FromDateTime(DateTime.Today),
                CategoryId = NewProduct.Category.Id,
                BrandId = NewProduct.Brand.Id,
            };
            db.Products.Add(product);
            db.SaveChanges();
            Products.Add(product);
            _productsView.Refresh();
            NewProduct = new();
        }
        private void ChangeProductData(object sender, RoutedEventArgs e)
        {
            NewProduct.Tags = SelectedTags.ToList();
            db.Products.Update(NewProduct);
            db.SaveChanges();
            _productsView.Refresh();
            ChangeProduct = null;
            NewProduct = new();
            MessageBox.Show("Product change");
        }
        
        private void TagsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChangeProduct != null)
            {
                SelectedTags.Clear();
                foreach (Tag tag in TagsListBox.SelectedItems)
                    SelectedTags.Add(tag);
            }
        }
        private void DeleteProduct(object sender, RoutedEventArgs e)
        {

            var result = MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                db.Products.Remove(ChangeProduct);
                db.SaveChanges();
                Products.Remove(ChangeProduct);
                _productsView.Refresh();
                ChangeProduct = null;
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
        private void UpdateSelectedTagsInListBox()
        {
            if (ChangeProduct?.Tags != null)
            {
                TagsListBox.SelectedItems.Clear();
                foreach (var tag in ChangeProduct.Tags)
                {
                    TagsListBox.SelectedItems.Add(tag);
                }
            }
        }

        private bool Filter(object obj)
        {
            if(obj is not Product)
                return false;
        
            var product = (Product)obj;
            
            if (Searchtext != null && !product.Name.Contains(Searchtext,
                    StringComparison.CurrentCultureIgnoreCase))
                return false;
            
            return true;
        }
    #endregion

    #region Navigation

    private void GoBack(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
    #endregion
    
    
    
   
    

    
    
   

    

   

    
    
   

   
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EF_Core_15.Models;
using EF_Core_15.Service;

namespace EF_Core_15.Pages;

public partial class TagPage : Page, INotifyPropertyChanged
{
    #region fields
        private ObservableCollection<Tag> _tags = new ();
        private ICollectionView _tagsView;
        private Tag _selecttag=new ()!;
        private bool addbuttonenabled=true;
        private bool changebuttonenabled=false;
        private string _newtagname = "";
    #endregion
    
    #region properties
        public Ef15Context db = DbService.Instance.Context;
        public ObservableCollection<Tag> Tags { get => _tags;
            set
            {
                SetField(ref _tags, value);
            }
        }
        public Tag Selecttag {get  => _selecttag;
            set
            {
                if (SetField(ref _selecttag, value))
                {
                    if (_selecttag != null)
                    {
                        Addbuttonenabled = false;
                        Changebuttonenabled=true;
                        Newtagname=_selecttag.Name;
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
        public string  Newtagname {get => _newtagname;set => SetField(ref _newtagname, value);}
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
        public TagPage(ObservableCollection<Tag>  tags)
        {
            InitializeComponent();
            foreach (var t in tags)
                Tags.Add(t);
            _tagsView = CollectionViewSource.GetDefaultView(Tags);
            DataContext = this;
            Application.Current.MainWindow.Title = "Страница с тегами";
        }
        private void RefreshChange(object sender, RoutedEventArgs e)
        {
            Selecttag = null;
        }
        private void ChangeTag(object sender, RoutedEventArgs e)
        {
            Selecttag.Name = Newtagname;
            db.Tags.Update(Selecttag);
            db.SaveChanges();
            _tagsView.Refresh();
            Selecttag = null;
            Newtagname = "";
        }
        private void AddTag(object sender, RoutedEventArgs e)
        {
            var tag = new Tag()
            {
                Id = Tags.Count + 1,
                Name = Newtagname,
            };
            db.Tags.Add(tag);
            db.SaveChanges();
            Tags.Add(tag);
            _tagsView.Refresh();
            Selecttag = null;
            Newtagname = "";
        }
        private void DeleteTag(object sender, RoutedEventArgs e)
        {
            db.Tags.Remove(Selecttag);
            db.SaveChanges();
            Tags.Remove(Selecttag);
            _tagsView.Refresh();
            Selecttag = null;
            Newtagname = "";
        }
    #endregion
    
    #region navigations

    private void GoBack(object sender, RoutedEventArgs e) => NavigationService.Navigate(new Main(true));

    #endregion
}
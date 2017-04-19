using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

namespace CabineParty.Core.Mvvm
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public ViewModel()
        {
            CloseCommand = new RelayCommand(OnRequestClose);
        }

        public ICommand CloseCommand { get; set; }

        public event Action<object> RequestClose;

        protected virtual void OnRequestClose(object obj)
        {
            RequestClose?.Invoke(obj);
        }

        protected void Set<T>(Expression<Func<T>> propertyExpression, ref T field, T value)
        {
            if (Equals(field, value)) return;
            field = value;

            var propertyName = GetPropertyName(propertyExpression);
            if (propertyName != null) RaisedPropertyChanged(propertyName);
        }

        public bool ThrowOnInvalidPropertyName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisedPropertyChanged(GetPropertyName(propertyExpression));
        }

        protected void RaisedPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] != null) return;

            var msg = $"Invalid property name. propertyName:{propertyName}.";

            if (ThrowOnInvalidPropertyName) throw new Exception(msg);
            Debug.Fail(msg);
        }

        protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }
            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }
            var property = body.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }
            return property.Name;
        }

        public void Dispose()
        {

        }
    }
}
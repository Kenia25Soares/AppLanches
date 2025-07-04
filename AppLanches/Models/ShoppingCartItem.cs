﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Models
{
    public class ShoppingCartItem : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total { get; set; }


        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? UrlImage { get; set; }

        public string? ImagePath => AppConfig.BaseUrl + UrlImage;


        public event PropertyChangedEventHandler? PropertyChanged;

        // Metodo padrao para notificar a mudana de propriedade, implementar a interface, e atualizar a interface INotifyPropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)  
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }
    }
}

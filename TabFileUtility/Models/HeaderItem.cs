using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabFileUtility.Models
{
    public class HeaderItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public HeaderItem()
        {

        }

        public string Name { get; set; } = "";
        public bool Output { get; set; } = true;
    }
}

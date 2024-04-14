using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafe_Managment.Controls;
using Cafe_Managment.Utilities;

namespace Cafe_Managment.ViewModel
{
    internal class OrderVM : ViewModelBase
    {

        List<Category> _categoryList;
        public List<Category> CategoryList
        {
            get { return _categoryList; }
            set { _categoryList = value; 
                OnPropertyChanged(nameof(CategoryList));
            }
        }
        public OrderVM() 
        {
            CategoryList = new List<Category>();
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
        }
    }
}

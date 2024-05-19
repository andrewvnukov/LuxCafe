using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Cafe_Managment.Model
{
    //КЛАСС ДЛЯ ТЕКУЩЕГО ПОЛЬЗОВАТЕЛЯ

    public abstract class UserData
    {
        public static int Id { get; set; }
        public static int RoleId { get; set; }
        public static int BranchId { get; set; }
        public static string CreatedAt { get; set; }
        public static DateTime DeletedAt { get; set; }
        public static string Status { get; set; }
        public static string Name { get; set; }
        public static string Surname { get; set; }
        public static string Patronomic { get; set; }
        public static string PhoneNumber { get; set; }
        public static string Email { get; set; }
        public static string BirthDay { get; set; }
        public static string Address {  get; set; }
        public static string Title { get; set; }
        public static BitmapImage ProfileImage { get; set; }
        
    }
}


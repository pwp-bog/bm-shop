using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm_shop
{
    public class User
    {
        public int id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public bool admin { get; set; }

        public User() { }

        public User(int id, string login, string password, string surname, string name, string patronymic, bool admin)
        {
            this.id = id;
            this.login = login;
            this.password = password;
            this.surname = surname;
            this.name = name;
            this.patronymic = patronymic;
            this.admin = admin;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPR
{
    public class User //обязательно из таблицы БД также и создаем поля класса
    //ЭТО КЛАСС-МОДЕЛЬ (описывает табличку USERS с БД)
    {
        public int id { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string pass { get; set; }
        public User() { }
        public User(string name, string role, string pass)
        {
            this.name = name;
            this.role = role;
            this.pass = pass;
        }
    }
}
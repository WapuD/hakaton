﻿using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public int RoleId { get; set; }

        public string? Login { get; set; }
        public string? Password { get; set; }

        public Role? Role { get; set; }
    }
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public int RoleId { get; set; }
    }
}

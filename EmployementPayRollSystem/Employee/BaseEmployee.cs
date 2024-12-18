using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployementPayRollSystem.Employee
{


 

    public abstract class BaseEmployee
    {
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public double BasicPay { get; set; }
        public double Allowances { get; set; }
        public double Deductions { get; set; }

        public BaseEmployee(string name, int employeeId, double basicPay, double allowances, double deductions)
        {
            Name = name;
            EmployeeId = employeeId;
            BasicPay = basicPay;
            Allowances = allowances;
            Deductions = deductions;
        }

        // Abstract method to calculate salary
        public abstract double CalculateSalary();
    }

    public class Manager : BaseEmployee
    {
        public Manager(string name, int employeeId, double basicPay, double allowances, double deductions)
            : base(name, employeeId, basicPay, allowances, deductions) { }

        public override double CalculateSalary()
        {
            return BasicPay + Allowances - Deductions;
        }
    }

    public class Developer : BaseEmployee
    {
        public Developer(string name, int employeeId, double basicPay, double allowances, double deductions)
            : base(name, employeeId, basicPay, allowances, deductions) { }

        public override double CalculateSalary()
        {
            return BasicPay + Allowances - Deductions;
        }
    }

    public class Intern : BaseEmployee
    {
        public Intern(string name, int employeeId, double basicPay, double allowances, double deductions)
            : base(name, employeeId, basicPay, allowances, deductions) { }

        public override double CalculateSalary()
        {
            return BasicPay + Allowances - Deductions;
        }
    }

}




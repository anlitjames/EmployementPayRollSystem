
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using EmployementPayRollSystem.Employee;
using EmployementPayRollSystem;

namespace EmployeePayrollSystem
{
    class Program
    {
        static void Main(string[] args)

        {
            List<InventoryItem> inventoryItems = new List<InventoryItem>
{
    new InventoryItem { ItemId = 1, CurrentStock = 50, ForecastedDemand = 100, ReorderCostPerUnit = 5.00, ReorderBatchSize = 20 },
    new InventoryItem { ItemId = 2, CurrentStock = 30, ForecastedDemand = 60, ReorderCostPerUnit = 4.00, ReorderBatchSize = 10 },
    new InventoryItem { ItemId = 3, CurrentStock = 100, ForecastedDemand = 120, ReorderCostPerUnit = 3.50, ReorderBatchSize = 30 }
};
            // Instantiate InventoryReorderingSystem
            InventoryReorderingSystem reorderingSystem = new InventoryReorderingSystem(inventoryItems);

            // Generate reorder plan
            var reorderPlan = reorderingSystem.GenerateReorderingPlan();

            // Display reorder plan
            Console.WriteLine("Reordering Plan:");
            foreach (var order in reorderPlan)
            {
                Console.WriteLine($"Item ID: {order.ItemId}, Units to Order: {order.UnitsToOrder}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Retrieve connection string from appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");
           
            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Add a new employee");
                Console.WriteLine("2. Display employee details");
                Console.WriteLine("3. Calculate and display individual salaries");
                Console.WriteLine("4. Display total payroll");
                Console.WriteLine("5. Update employee salary");
                Console.WriteLine("6. Exit");
                Console.Write("Please select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddEmployee(connectionString);
                        break;
                    case "2":
                        DisplayEmployeeDetails(connectionString);
                        break;
                    case "3":
                        CalculateAndDisplaySalaries(connectionString);
                        break;
                    case "4":
                        DisplayTotalPayroll(connectionString);
                        break;
                    case "5":
                        UpdateEmployeeSalary(connectionString);
                        break;
                    case "6":
                        exitProgram = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please select again.");
                        break;
                }
            }
        }

        static void AddEmployee(string connectionString)
        {
            Console.Write("Enter employee name: ");
            string name = Console.ReadLine();

            Console.Write("Enter employee role (Manager/Developer/Intern): ");
            string role = Console.ReadLine();

            Console.Write("Enter department name: ");
            string departmentName = Console.ReadLine();

            Console.Write("Enter hiring date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime hiringDate))
            {
                Console.WriteLine("Invalid date format. Employee not added.");
                return;
            }

            try
            {
                AddEmployeeToDatabase(connectionString, name, role, departmentName, hiringDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee to the database: {ex.Message}");
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        static void AddEmployeeToDatabase(string connectionString, string name, string role, string departmentName, DateTime hiringDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("AddEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@DepartmentName", departmentName);
                    command.Parameters.AddWithValue("@HiringDate", hiringDate);

                    int newEmployeeId = (int)command.ExecuteScalar();

                    Console.WriteLine($"Employee added successfully with ID: {newEmployeeId}");
                }
            }
        }


        static void DisplayEmployeeDetails(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to fetch Employee details along with BasicPay, Bonus, and Deductions from EmployeeSalaryView and Salaries table
                string query = @"
            SELECT e.EmployeeID, e.Name, e.Role, s.BasicPay, s.Bonus, s.Deductions
            FROM EmployeeManagementSystem.dbo.EmployeeSalaryView e
            JOIN EmployeeManagementSystem.dbo.Salaries s ON e.EmployeeID = s.EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine("Employee Details:");
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                string role = reader.GetString(2);
                                decimal basicPay = reader.GetDecimal(3);  // Use decimal for financial data
                                decimal bonus = reader.GetDecimal(4);    // Use decimal for financial data
                                decimal deductions = reader.GetDecimal(5); // Use decimal for financial data

                                // Calculate total salary (BasicPay + Bonus - Deductions)
                                decimal totalSalary = basicPay + bonus - deductions;

                                Console.WriteLine($"ID: {id}, Name: {name}, Role: {role}, Basic Pay: {basicPay:C}, Bonus: {bonus:C}, Deductions: {deductions:C}, Total Salary: {totalSalary:C}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No employees found.");
                        }
                    }
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }



        static void CalculateAndDisplaySalaries(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT e.EmployeeID, e.Name, e.Role, s.BasicPay, s.Bonus, s.Deductions
            FROM EmployeeManagementSystem.dbo.EmployeeSalaryView e
            JOIN EmployeeManagementSystem.dbo.Salaries s ON e.EmployeeID = s.EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // Retrieve the decimal values and calculate the salary
                                decimal basicPay = reader.GetDecimal(3);
                                decimal bonus = reader.GetDecimal(4);
                                decimal deductions = reader.GetDecimal(5);

                                // Calculate salary: BasicPay + Bonus - Deductions
                                decimal salary = basicPay + bonus - deductions;

                                // Convert to double for formatting and display
                                Console.WriteLine($"{reader.GetString(1)} (ID: {reader.GetInt32(0)}) - Salary: {Convert.ToDouble(salary):C}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No employees found.");
                        }
                    }
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }




        static void DisplayTotalPayroll(string connectionString)
        {
            decimal totalPayroll = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to fetch BasicPay, Bonus, and Deductions from EmployeeSalaryView and Salaries table
                string query = @"
            SELECT s.BasicPay, s.Bonus, s.Deductions
            FROM EmployeeManagementSystem.dbo.EmployeeSalaryView e
            JOIN EmployeeManagementSystem.dbo.Salaries s ON e.EmployeeID = s.EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // Retrieve the values as decimal for better precision
                                decimal basicPay = reader.GetDecimal(0);
                                decimal bonus = reader.GetDecimal(1);
                                decimal deductions = reader.GetDecimal(2);

                                // Calculate the salary for the employee and add it to totalPayroll
                                decimal salary = basicPay + bonus - deductions;
                                totalPayroll += salary;
                            }
                        }
                        else
                        {
                            Console.WriteLine("No payroll data found.");
                            return;
                        }
                    }
                }
            }

            // Display total payroll with currency formatting
            Console.WriteLine($"Total Payroll: {totalPayroll:C}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }



        static void UpdateEmployeeSalary(string connectionString)
        {
            Console.Write("Enter Employee ID: ");
            if (!int.TryParse(Console.ReadLine(), out int employeeId))
            {
                Console.WriteLine("Invalid Employee ID.");
                return;
            }

            Console.Write("Enter Basic Pay: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal basicPay))
            {
                Console.WriteLine("Invalid Basic Pay.");
                return;
            }

            Console.Write("Enter Allowances: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal allowances))
            {
                Console.WriteLine("Invalid Allowances.");
                return;
            }

            Console.Write("Enter Deductions: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal deductions))
            {
                Console.WriteLine("Invalid Deductions.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the employee exists in the Employees table
                    string checkEmployeeQuery = "SELECT COUNT(*) FROM Employees WHERE EmployeeID = @EmployeeID";
                    using (SqlCommand checkCommand = new SqlCommand(checkEmployeeQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@EmployeeID", employeeId);
                        int employeeCount = (int)checkCommand.ExecuteScalar();

                        if (employeeCount == 0)
                        {
                            Console.WriteLine("Error: Employee ID does not exist.");
                            return;
                        }
                    }

                    // If the employee exists, proceed to update the salary
                    using (SqlCommand command = new SqlCommand("UpdateSalary", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);
                        command.Parameters.AddWithValue("@BasicPay", basicPay);
                        command.Parameters.AddWithValue("@Allowances", allowances);
                        command.Parameters.AddWithValue("@Deductions", deductions);

                        int rowsAffected = command.ExecuteNonQuery(); // Execute the update

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Salary updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Error: Salary update failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating salary: {ex.Message}");
            }
        }
     
}  
}








using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;

namespace DbClassWork_Clients_Orders
{
    /*
     Задание.
Есть 2 таблицы:
	- Клиент (id, имя, возраст)
	- Заказ (order) (id, описание заказа, client id) - заказ связан с клиентом

Задание: реализовать в виде отдельных процедур с использованием C# и ADO.NET следующие операции:
	1) Добавить нового клиента+
	2) Добавить новый заказ +
	3) Вывести список клиентов +
	4) Вывести список заказов определенного клиента +
	5) Вывести список имен клиентов и количество заказов у каждого из них +
	6) Вывести количество клиентов +
Для тестирование в отдельной процедуре создать 5 клиентов и от 2 до 5 заказов каждому из них (на C#).
     */
    internal class Program
    {
        static SqlConnection OpenDbConnection()
        {
            // обработка исключений будет выполняться выше по стеку
            string connectionString = ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        static void ReadQueryResult(SqlDataReader queryResult)
        {
            // 1. вывести названия столбцов результирующей таблицы (представления)
            for (int i = 0; i < queryResult.FieldCount - 1; i++)
            {
                Console.Write($"{queryResult.GetName(i)} - ");
            }
            Console.WriteLine(queryResult.GetName(queryResult.FieldCount - 1));
            // 2. вывести значения построчно
            bool noRows = true;
            while (queryResult.Read())
            {
                noRows = false;
                for (int i = 0; i < queryResult.FieldCount - 1; i++)
                {
                    Console.Write($"{queryResult[i]} - ");
                }
                Console.WriteLine(queryResult[queryResult.FieldCount - 1]);
            }
            if (noRows)
            {
                Console.WriteLine("No rows in result");
            }
        }
        static void SelectAllClientsRows()//вывод всех клиентов
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand("SELECT * FROM clients_t", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void InsertNewClient(string name, int age)//добавление нового клиента
        {
            SqlConnection connection = null;
            try
            {
                connection = OpenDbConnection();
                string cmdString = $"INSERT INTO clients_t (name_f, age_f) VALUES ('{name}', {age});";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"insert failed, rowsAffected!=1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully inserted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong:{ex.Message}");
            }
            finally
            {
                connection?.Close();
            }
        }
        static void SelectClientOrders(string name)//вывод одного клиента и его заказов
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT name_f AS 'Name', order_description_f AS 'Order' FROM clients_t, orders_t WHERE clients_t.id=orders_t.clientId_f AND name_f='{name}';", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void SelectCountClientOrders()//вывод клиентов и их колво заказов
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT name_f, COUNT(*) AS 'orders count' FROM clients_t, orders_t WHERE clients_t.id=orders_t.clientId_f GROUP BY name_f;", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void SelectCountOfClients()//вывод колва всех клиентов
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT COUNT(*) AS 'count_of_clients' FROM clients_t;", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void InsertNewOrder(string name, string description_order)//добавить новый заказ по имени клиента
        {
            SqlConnection connection = null;
            try
            {
                connection = OpenDbConnection();
                string cmdString = $"INSERT INTO orders_t(order_description_f,clientId_f) VALUES ('{description_order}',(SELECT id FROM clients_t WHERE name_f='{name}'))";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"insert failed, rowsAffected!=1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully inserted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong:{ex.Message}");
            }
            finally
            {
                connection?.Close();
            }
        }
        static void Main(string[] args)
        {
            //OpenDbConnection().Close();//+
            //InsertNewClient("Sara", 29);//+
            //SelectClientOrders("Alex");//+
            //SelectCountClientOrders();//+
            //SelectCountOfClients();//+
            //InsertNewOrder("Alex", "Microphone");//+
            // SelectAllClientsRows();//+

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using HotelOpgave;

namespace HotelDBConnection
{
    class DBClient
    {
        //Database connection string - replace it with the connnection string to your own database 
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Hotel;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private int GetMaxFacility_ID(SqlConnection connection)
        {
            Console.WriteLine("Calling -> GetMaxFacility_ID");

            //This SQL command will fetch one row from the DemoHotel table: The one with the max Hotel_No
            string querystringMaxFacilityID = "SELECT  MAX(Facility_ID)  FROM Facility";
            Console.WriteLine($"SQL applied: {querystringMaxFacilityID}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(querystringMaxFacilityID, connection);
            SqlDataReader reader = command.ExecuteReader();

            //Assume undefined value 0 for max hotel_no
            int MaxFacility_ID = 1;

            //Is there any rows in the query
            if (reader.Read())
            {
                //Yes, get max hotel_no
                try
                {
                    MaxFacility_ID = reader.GetInt32(0); //Reading int fro 1st column
                    
                }
                catch
                {
                    //Just to prevent exception when table is empty
                }
            }

            //Close reader
            reader.Close();

            Console.WriteLine($"Max Facility#: {MaxFacility_ID}");
            Console.WriteLine();

            //Return max hotel_no
            return MaxFacility_ID;
        }

        private int DeleteFacility(SqlConnection connection, int facility_id)
        {
            Console.WriteLine("Calling -> DeleteFacility");

            //This SQL command will delete one row from the DemoHotel table: The one with primary key hotel_No
            string deleteCommandString = $"DELETE FROM Facility  WHERE Facility_ID = {facility_id}";
            Console.WriteLine($"SQL applied: {deleteCommandString}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(deleteCommandString, connection);
            Console.WriteLine($"Deleting Facility #{facility_id}");
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            //Return number of rows affected
            return numberOfRowsAffected;
        }

        private int UpdateFacility(SqlConnection connection, Facility facility)
        {
            Console.WriteLine("Calling -> UpdateFacility");

            //This SQL command will update one row from the DemoHotel table: The one with primary key hotel_No
            string updateCommandString = $"UPDATE Facility SET Name='{facility.Name}' WHERE Facility_ID = {facility.Facility_ID}";
            Console.WriteLine($"SQL applied: {updateCommandString}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(updateCommandString, connection);
            Console.WriteLine($"Updating Facility #{facility.Facility_ID}");
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            //Return number of rows affected
            return numberOfRowsAffected;
        }

        private int InsertFacility(SqlConnection connection, Facility facility)
        {
            Console.WriteLine("Calling -> InsertFacility");

            //This SQL command will insert one row into the DemoHotel table with primary key hotel_No
            string insertCommandString = $"INSERT INTO Facility VALUES({facility.Facility_ID}, '{facility.Name}')";
            Console.WriteLine($"SQL applied: {insertCommandString}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(insertCommandString, connection);

            Console.WriteLine($"Creating Facility #{facility.Facility_ID}");
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            //Return number of rows affected 
            return numberOfRowsAffected;
        }

        private List<Facility> ListAllFacilities(SqlConnection connection)
        {
            Console.WriteLine("Calling -> ListAllFacilities");

            //This SQL command will fetch all rows and columns from the DemoHotel table
            string queryStringAllFacility = "SELECT * FROM Facility";
            Console.WriteLine($"SQL applied: {queryStringAllFacility}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(queryStringAllFacility, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("Listing all Facilities:");

            //NO rows in the query 
            if (!reader.HasRows)
            {
                //End here
                Console.WriteLine("No Facilities in database");
                reader.Close();

                //Return null for 'no hotels found'
                return null;
            }

            //Create list of hotels found
            List<Facility> facilities = new List<Facility>();
            while (reader.Read())
            {
                //If we reached here, there is still one hotel to be put into the list 
                Facility nextFacility = new Facility()
                {
                    Facility_ID = reader.GetInt32(0), //Reading int from 1st column
                    Name = reader.GetString(1),    //Reading string from 2nd column
                    
                };

                //Add hotel to list
                facilities.Add(nextFacility);

                Console.WriteLine(nextFacility);
            }

            //Close reader
            reader.Close();
            Console.WriteLine();

            //Return list of hotels
            return facilities;
        }

        private Facility GetFacility(SqlConnection connection, int Facility_ID)
        {
            Console.WriteLine("Calling -> GetFacility");

            //This SQL command will fetch the row with primary key hotel_no from the DemoHotel table
            string queryStringOneFacility = $"SELECT * FROM Facility WHERE Facility_ID = {Facility_ID}";
            Console.WriteLine($"SQL applied: {queryStringOneFacility}");

            //Prepare SQK command
            SqlCommand command = new SqlCommand(queryStringOneFacility, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine($"Finding Facility#: {Facility_ID}");

            //NO rows in the query? 
            if (!reader.HasRows)
            {
                //End here
                Console.WriteLine("No Facilities in database");
                reader.Close();

                //Return null for 'no hotel found'
                return null;
            }

            //Fetch hotel object from teh database
            Facility facility = null;
            if (reader.Read())
            {
                facility = new Facility()
                {
                    Facility_ID = reader.GetInt32(0), //Reading int fro 1st column
                    Name = reader.GetString(1),    //Reading string from 2nd column
                    
                };

                Console.WriteLine(facility);
            }

            //Close reader
            reader.Close();
            Console.WriteLine();

            //Return found hotel
            return facility;
        }
        public void Start()
        {
            //Apply 'using' to connection (SqlConnection) in order to call Dispose (interface IDisposable) 
            //whenever the 'using' block exits
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Open connection
                connection.Open();

                //List all hotels in the database
                ListAllFacilities(connection);

                //Create a new hotel with primary key equal to current max primary key plus 1
                Facility newFacility = new Facility()
                {
                    Facility_ID = GetMaxFacility_ID(connection) + 1,
                    Name = "Ny svømmehal",
                   
                };

                //Insert the hotel into the database
                InsertFacility(connection, newFacility);

                //List all hotels including the the newly inserted one
                ListAllFacilities(connection);
                

                //Get the newly inserted hotel from the database in order to update it 
                Facility facilityToBeUpdated = GetFacility(connection, newFacility.Facility_ID);

                //Alter Name and Addess properties
                facilityToBeUpdated.Name += "(updated)";
               

                //Update the hotel in the database 
                UpdateFacility(connection, facilityToBeUpdated);

                //List all hotels including the updated one
                ListAllFacilities(connection);

                //Get the updated hotel in order to delete it
                Facility facilityToBeDeleted = GetFacility(connection, facilityToBeUpdated.Facility_ID);

                //Delete the hotel
                DeleteFacility(connection, facilityToBeDeleted.Facility_ID);

                //List all hotels - now without the deleted one
                ListAllFacilities(connection);
            }
        }
    }
}
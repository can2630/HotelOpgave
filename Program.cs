using HotelDBConnection;
using System;

namespace HotelOpgave  
{
    class Program
    {
        static void Main(string[] args)
        {
            DBClient abc = new DBClient();
            abc.Start();

        }

    }
}
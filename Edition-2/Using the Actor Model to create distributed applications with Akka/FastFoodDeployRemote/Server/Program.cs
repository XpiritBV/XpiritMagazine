using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Shared;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var actorSystem = ActorSystem.Create("remotefastfood"))
            {
                //actorSystem.ActorOf<FastFoodEmployee2>("fastfoodemployee");
                Console.ReadLine();
            }
        }
    }
}

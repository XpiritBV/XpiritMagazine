using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ActorSystem actorSystem = ActorSystem.Create("FastFoodRestaurant"))
            {
                //Props props1 = Props.Create(() => new Customer());
                //var props2 = Props.Create<Customer>();

                //IActorRef customerA = actorSystem.ActorOf(props1, "customer");
                //IActorRef customerB = actorSystem.ActorOf(props2);
                //IActorRef customerC = actorSystem.ActorOf<Customer>("customer");
                //IActorRef customerD = actorSystem.ActorOf<Customer>();

                IActorRef customer = actorSystem.ActorOf(Props.Create<Customer2>(), "customer");

                customer.Tell(new MarkHungry());
                Console.Read();
            }
        }
    }
}

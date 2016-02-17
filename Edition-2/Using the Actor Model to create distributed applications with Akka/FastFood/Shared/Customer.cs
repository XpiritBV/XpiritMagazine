using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Customer : TypedActor, IHandle<MarkHungry>, IHandle<Bill>
    {
        protected override void PreStart()
        {
            
        }

        public void Handle(MarkHungry message)
        {
            Console.WriteLine("Customer is hungy let's order some food");
            IActorRef fastFoodEmployee = Context.ActorOf<Employee>("fastfoodemployee");
            //Context.Watch(fastFoodEmployee);

            fastFoodEmployee.Tell(new BurgerMenuRequest());
            Console.WriteLine("Ordered food");
        }

        //Customer
        public void Handle(Bill message)
        {
            Console.WriteLine("Great, menu is ready. Let's pay.");
        }
    }

    public class Customer2 : UntypedActor
    {
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(3, TimeSpan.FromSeconds(5), ex =>
            {
                if (ex is ApplicationException)
                {
                    return Directive.Restart;
                }
                
                return Directive.Escalate;
            });
        }

        protected override void OnReceive(object message)
        {
            if (message is MarkHungry)
            {
                Console.WriteLine("Customer is hungy let's order some food");
                
                IActorRef employee = Context.ActorOf<Employee>("fastfoodemployee");

                employee.Tell(new BurgerMenuRequest());
                Console.WriteLine("Ordered food");
                return;
            }

            else if (message is Bill)
            {
                Console.WriteLine("Great, menu is ready. Let's pay.");
                return;
            }
            else if (message is Terminated)
            {
                Console.WriteLine("Terminated");
            }
        }
    }
}

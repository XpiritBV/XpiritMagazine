using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class FastFoodEmployee : TypedActor, IHandle<BurgerMenuRequest>, IHandle<SaladRequest>
    {
        protected override void PreRestart(Exception reason, object message)
        {
            // Put the message that failed back on the mailbox.
            // It will be picked up when the actor is started.
            Self.Tell(message);

            var selection = Context.ActorSelection("/user/customer");
            selection.Tell(new Bill());
        }

        // FastFoodEmployee
        public void Handle(BurgerMenuRequest message)
        {
            Console.WriteLine("Collect menu");
            //Context.Stop(Context.Self);
            //throw new ApplicationException("testexception");
            Context.Parent.Tell(new Bill());
        }

        public void Handle(SaladRequest message)
        {
            Console.WriteLine("Collect salad");
        }
    }

    public class FastFoodEmployee2 : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            BurgerMenuRequest burgerMenuRequest = message as BurgerMenuRequest;
            if (burgerMenuRequest != null)
            {
                Console.WriteLine("Collect menu");
                return;
            }

            SaladRequest saladRequest = message as SaladRequest;
            if (saladRequest != null)
            {
                Console.WriteLine("Collect salad");
                return;
            }
        }
    }

    public class FastFoodEmployee4 : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is BurgerMenuRequest)
            {
                Console.WriteLine("Collect menu");
                return;
            }
            
            if (message is SaladRequest)
            {
                Console.WriteLine("Collect salad");
                return;
            }
        }
    }


    public class FastFoodEmployee3 : ReceiveActor
    {
        public FastFoodEmployee3()
        {
            Receive<BurgerMenuRequest>(message => {
                Console.WriteLine("Collect menu");
            });
            Receive<SaladRequest>(message => {
                Handle(message);
            });
        }

        public void Handle(SaladRequest message)
        {
            Console.WriteLine("Collect salad");
        }
    }

   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BootcampBarrier
{
    class Program
    {
        static void Main(string[] args)
        {
            var participants = 5;

            Barrier barrier = new Barrier(participants + 1, // add one for the main thread
                b =>
                {  // this method i s only called when all the participants arrived.
                    Console.WriteLine("{0} participants are at the rendevous point {1}.",
                        b.ParticipantCount - 1, //  subtract the main thread)
                        b.CurrentPhaseNumber);
                });

            for(int i=0; i<participants; i++)
            {
                var localCopy = i;
                Task.Run(() =>
                        {
                            Console.WriteLine("Task {0} left point A!", localCopy);
                            Thread.Sleep(1000 * localCopy + 1); // do some "work"
                            if(localCopy %2 ==0)
                            {
                                Console.WriteLine("Task {0} arrived at point B!", localCopy);
                                barrier.SignalAndWait();
                            }
                            else // the odd numbers wish to turn back
                            {
                                Console.WriteLine("Task {0} changed its mind and went back!", localCopy);
                                barrier.RemoveParticipant();
                                return;
                            }
                            Thread.Sleep(1000 * (participants - localCopy)); // do some "more work"
                            Console.WriteLine("Task {0} arrived at point C!", localCopy);
                            barrier.SignalAndWait();
                        });
            }

            Console.WriteLine("Main thread is waiting for {0} tasks!", barrier.ParticipantCount - 1);
            barrier.SignalAndWait(); // waiting at the first phase
            barrier.SignalAndWait(); // waiting at the second phase
            Console.WriteLine("Main thread is done!");
            Console.ReadKey();
        }
    }
}

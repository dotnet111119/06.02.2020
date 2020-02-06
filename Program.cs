using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _0602
{
    class Program
    {
        static void Example1()
        {
            // way 1
            Task t1 = new Task(() =>
            {
                Console.WriteLine(".........");
                Thread.Sleep(1000);
                Console.WriteLine("Hello from the task 1");
                Console.WriteLine($"thread id: {Thread.CurrentThread.ManagedThreadId}");
            }, TaskCreationOptions.LongRunning); // not from thread pool!

            Task t2 = new Task(() =>
            {
                Console.WriteLine(".........");
                Thread.Sleep(1000);
                Console.WriteLine("Hello from the task 2");
                Console.WriteLine($"thread id: {Thread.CurrentThread.ManagedThreadId}");
            }, TaskCreationOptions.LongRunning); // not from thread pool!

            //t1.RunSynchronously(); // like join
            //t1.Status
            //t1.Start();

            t1.Start();
            t2.Start();

            Task.WaitAll(new[] { t1, t2 });
            var task_list = new[] { t1, t2 };
            int index = Task.WaitAny(task_list);
            Console.WriteLine($"Task {task_list[index]} was completed!");
        }
        static void Example2()
        {
            Random r = new Random();
            Task<int> t1 = new Task<int>(() =>
            {
                int number = r.Next(100);
                return number;
            }, TaskCreationOptions.LongRunning); // not from thread pool!

            t1.RunSynchronously();
            Console.WriteLine(t1.Status);
            Console.WriteLine(t1.Result);

            Task<string[]> t2 = new Task<string[]>(() =>
            {
                int number = r.Next(10 * 1000);
                Console.WriteLine(number);
                Thread.Sleep(number);
                Console.WriteLine();
                return new string[] { "Hello", "Tasks", "How are you?" };
            }, TaskCreationOptions.LongRunning); // not from thread pool!

            t2.Start();
            Console.WriteLine(t2.Result);
            //t2.Wait(); // same as join
            Console.WriteLine("...............");
            while (t2.Status != TaskStatus.RanToCompletion)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine(t2.Result);
        }
        static void Main(string[] args)
        {
            /*
            // way 2
            Task.Run(() =>
            {
                Console.WriteLine("Using task.run");
            }); // must be thread pool

            // way 3
            Task t4 = Task.Factory.StartNew(() =>
            {
                
                Console.WriteLine("Using task.factory.run");
                throw new Exception(); // no crash!!!
            }, creationOptions:TaskCreationOptions.LongRunning); // must be thread pool

            // crash...
            //Thread t8 = new Thread(() => { throw new Exception(); });
            //t8.IsBackground = true;
            //t8.Start();
            */

            Task<int> final = Task.Run<int>(() =>
            {
                Console.WriteLine("Using task.run");
                Thread.Sleep(1000);
                //throw new Exception();
                return 5;
            }).ContinueWith((Task<int> parent) =>
            {
                Console.WriteLine(parent.IsFaulted);
                if (parent.IsFaulted == true)
                {
                    Console.WriteLine($"parent is faulted");
                }
                else
                {
                    Console.WriteLine($"parent result = {parent.Result}");
                }
                return 10;
            });
            Console.WriteLine(final.Result); // 10

            Thread.Sleep(1000);
            //Console.WriteLine(t4.IsFaulted);
            Console.ReadLine();


        }
    }
}

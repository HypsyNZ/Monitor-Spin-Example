```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorTest
{
    /// <summary>
    /// Spin
    /// Also the documentation is wrong: https://i.imgur.com/WBnicGe.png
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            StaticAsyncMain();
            Console.ReadLine();
        }

        private static object lockObj = new();
        private static bool lockTaken = false;

        private static async void StaticAsyncMain()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1).ConfigureAwait(false);
                    try
                    {
                        lockTaken = Monitor.TryEnter(lockObj);
                        if (lockTaken)
                        {
                            // Critical Section Starts

                            try
                            {
                                int i = 0;
                                while (true)
                                {
                                    Console.WriteLine(i + "| Blocking2");
                                    i++;

                                    if (i == 100)
                                    {
                                        i = 0;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                Monitor.Exit(lockObj);
                            }

                            // Critical Section Ends
                        }
                        else
                        {
                            // Avoided Critical Section Carry On
                        }
                    }
                    catch
                    {
                        // Some Exception Outside the Lock
                    }
                }
            }).ConfigureAwait(false);

            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1).ConfigureAwait(false);
                    try
                    {
                        lockTaken = Monitor.TryEnter(lockObj);
                        if (lockTaken)
                        {
                            // Critical Section Starts

                            try
                            {
                                int i = 0;
                                while (true)
                                {
                                    Console.WriteLine(i + "| Blocking1");
                                    i++;

                                    if (i == 100)
                                    {
                                        i = 0;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                Monitor.Exit(lockObj);
                            }

                            // Critical Section Ends
                        }
                        else
                        {
                            // Avoided Critical Section Carry On
                        }
                    }
                    catch
                    {
                        // Some Exception Outside the Lock
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
```

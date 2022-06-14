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
            // Task One
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
                                int safeInside = 0;
                                while (true)
                                {
                                    Console.WriteLine(safeInside + "| Blocking Task Two");
                                    safeInside++;

                                    if (safeInside == 100)
                                    {
                                        safeInside = 0;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                // Exception Inside Critical Section
                            }
                            finally
                            {
                                Monitor.Exit(lockObj);
                            }

                            // Critical Section Ends
                        }
                        else
                        {
                            // Avoided Critical Section Continue Normal Execution
                        }
                    }
                    catch
                    {
                        // Some Exception Outside the Critical Section
                    }
                    finally
                    {
                        // DO NOT try Exit the Monitor Here like in the Documentation Example
                    }
                }
            }).ConfigureAwait(false);

            // Task Two
            await Task.Run(async () =>
            {
                int dangerOutside = 0;
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
                                while (true)
                                {
                                    Console.WriteLine(dangerOutside + "| Blocking Task One");
                                    dangerOutside++;

                                    if (dangerOutside == 100)
                                    {
                                        dangerOutside = 0;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                // Exception Inside Critical Section
                            }
                            finally
                            {
                                Monitor.Exit(lockObj);
                            }

                            // Critical Section Ends
                        }
                        else
                        {
                            // Avoided Critical Section Continue Normal Execution
                        }
                    }
                    catch
                    {
                        // Some Exception Outside the Critical Section
                    }
                    finally
                    {
                        // DO NOT try Exit the Monitor Here like in the Documentation Example
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
```

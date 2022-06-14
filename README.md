```cs
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

        private static void StaticAsyncMain()
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
                        }
                        else
                        {
                            // The lock was not acquired.
                        }
                    }
                    catch
                    {
                    }
                }
            });

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
                        }
                        else
                        {
                            // The lock was not acquired.
                        }
                    }
                    catch
                    {
                    }
                }
            });
        }
    }
}
```

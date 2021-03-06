/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

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

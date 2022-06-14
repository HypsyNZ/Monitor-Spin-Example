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

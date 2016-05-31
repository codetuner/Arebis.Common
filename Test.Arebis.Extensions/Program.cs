using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using global::Arebis.Extensions;

namespace Test.Arebis.Extensions
{
    class Program
    {
        static void Main(string[] args)
        {
			string testNameMatcher = "*";

			if (Array.IndexOf(args, "/?") >= 0)
			{
				Console.WriteLine("Syntax: {0} [matchpattern] [/debug]", Environment.GetCommandLineArgs()[0]);
				Environment.Exit(0);
			}

			if (Array.IndexOf(args, "/debug") >= 0)
			{
				System.Diagnostics.Debugger.Launch();
			}

			foreach (string arg in args)
			{
				if (arg.StartsWith("/") == false) 
                {
                    testNameMatcher = arg;
                    Console.WriteLine("Test matching pattern: " + arg);
	    			break;
                }
			}

			int testCount = 0;
			int errorCount = 0;

            foreach (Type testType in Assembly.GetExecutingAssembly().GetTypes())
            {
				bool firstTest = true;

                // Filter out non-test or ignored classes:
                if (testType.IsAbstract) continue;
				if (testType.GetCustomAttributes(typeof(TestClassAttribute), true).Length == 0) continue;
				if (testType.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0) continue;

                // Retrieve TestInitialize & TestCleanup methods:
                MethodInfo testInitialize = null;
                MethodInfo testCleanup = null;
                foreach (MethodInfo method in testType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
                {
                    if (method.GetCustomAttributes(typeof(TestInitializeAttribute), true).Length > 0) testInitialize = method;
                    if (method.GetCustomAttributes(typeof(TestCleanupAttribute), true).Length > 0) testCleanup = method;
                }


                // For each method:
                foreach (MethodInfo testMethod in testType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
                {
                    // Filter out non-test or ignored methods:
					if (testMethod.GetCustomAttributes(typeof(TestMethodAttribute), false).Length == 0) continue;
					if (testMethod.GetCustomAttributes(typeof(IgnoreAttribute), false).Length > 0) continue;

					// Check if method matches testNameMatcher:
					if ((testMethod.DeclaringType.ToString() + "." + testMethod.Name).Like(testNameMatcher) == false) continue;

					if (firstTest)
					{
						Console.WriteLine();
						Console.WriteLine("TestClass '{0}':", testType.ToString());
						Console.WriteLine();
						firstTest = false;
					}

                    // For valid test-methods:
                    try
                    {
                        // Check for expected exception;
                        Type expectedException = null;
                        if (testMethod.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false).Length > 0)
                            expectedException = ((ExpectedExceptionAttribute)testMethod.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false)[0]).ExceptionType;

                        // Create instance:
                        Object testInstance = Activator.CreateInstance(testType);

                        // Run testInitialize method:
                        if (testInitialize != null) testInitialize.Invoke(testInstance, new object[0]);

                        Console.WriteLine("Running test '{0}'...", testMethod.Name);

						// Increase counter:
						testCount++;

                        // Invoke test method:
                        try
                        {
                            testMethod.Invoke(testInstance, new object[0]);

                            if (expectedException == null)
                            {
                                Console.WriteLine("OK");
                            }
                            else
                            {
                                Console.WriteLine("ERROR");
                                errorCount++;
                                Console.WriteLine("  Expected exception unraised: {0}", expectedException.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            if ((ex is TargetInvocationException) && (ex.InnerException != null)) ex = ex.InnerException;

                            if (ex is AssertFailedException)
                            {
                                Console.WriteLine("FAILED: " + ex.Message);
                                errorCount++;
                            }
                            else if (ex is AssertInconclusiveException)
                            {
                                Console.WriteLine("INCONCLUSIVE: " + ex.Message);
                            }
                            else if (expectedException == null)
                            {
                                Console.WriteLine("ERROR");
                                errorCount++;
                                Console.WriteLine("  Unexpected exception: {0}", ex.ToString());
                            }
                            else if (ex.GetType().IsAssignableFrom(expectedException))
                            {
                                Console.WriteLine("OK");
                            }
                            else
                            {
                                Console.WriteLine("ERROR");
                                errorCount++;
                                Console.WriteLine("  Exception does not match expected exception: {0}", ex.ToString());
                            }
                        }

                        // Run testCleanup method:
                        if (testCleanup != null) testCleanup.Invoke(testInstance, new object[0]);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("FAILURE RUNNING TESTS:");
                        Console.WriteLine(ex.ToString());
                    }

                    // Blank line between:
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Tests done: {0} errors / {1} tests.", errorCount, testCount);
            Console.WriteLine();
        }
    }
}

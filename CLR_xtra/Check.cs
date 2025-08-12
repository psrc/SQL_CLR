using System;
using System.Reflection;

namespace YourNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Loading assembly...");
                Assembly assembly = Assembly.LoadFrom("C:\\Users\\mjensen\\projects\\SqlRegEx\\bin\\Debug\\SqlRegEx.dll");

                Console.WriteLine("Assembly loaded successfully.");
                Type[] types = assembly.GetTypes();
                Console.WriteLine(String.Format("Number of types: {0}", types.Length));

                foreach (Type type in types)
                {
                    Console.WriteLine(String.Format("Class: {0}", type.FullName));
                    MethodInfo[] methods = type.GetMethods();
                    Console.WriteLine(String.Format("Number of methods in {0}: {1}", type.FullName, methods.Length));

                    foreach (MethodInfo method in methods)
                    {
                        Console.WriteLine(String.Format("Method: {0}", method.Name));
                    }
                }
                Console.WriteLine("Execution finished.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}

using System.Security.Cryptography;

namespace EyesOnU
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            var dict = new Dictionary<string, string>();
            if (args.Length > 0)
            {
                Uri uri = new Uri(args[0]);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                foreach (string key in queryParams)
                {
                    dict[key] = queryParams[key];
                }
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.


            //var form = new Monitor();
            var form = new DisplayForm(dict);
            Application.Run(form);
        }
    }
}
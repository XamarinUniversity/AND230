using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AndroidKeyStoreSHA1
{
    class Program
    {
        //C:\Program Files (x86)\Java\jdk1.7.0_55\bin\keytool.exe
        //C:\Users\\Adrian\AppData\Local\Xamarin\Mono for Android\debug.keystore
        //"C:\Program Files (x86)\Java\jdk1.7.0_55\bin\keytool.exe" -list -v -keystore "C:\Users\Adrian\AppData\Local\Xamarin\Mono for Android\debug.keystore" -storepass android -keypass android

        static void Main(string[] args)
        {
            // Option "-value" to allow copying the SHA1 directly to the clipboard for pasting elsewhere.
            // On Windows: "DroidSHA1Extractor -value | copy"
            // On Mac: "mono DroidSHA1Extractor -value | pbcopy"
            var outputOnlySha1 = args.Length > 0 ? args[0] == "-value" : false;
            var javaPath = GetJavaPath();
            var keytoolPath = GetKeytoolPath(javaPath);
            var keystorePath = GetDebugKeystorePath();

            if (String.IsNullOrWhiteSpace(javaPath))
            {
                Console.WriteLine("Java wasn't found in Program Files (x86)");
                return;
            }
            else
            {
                if (!outputOnlySha1)
                {
                    Console.WriteLine($"Java folder: {javaPath}");
                }
            }

            if (File.Exists(keytoolPath))
            {
                if (!outputOnlySha1)
                {
                    Console.WriteLine($"Keytool path: {keytoolPath}");
                }
            }
            else
            {
                Console.WriteLine($"Keytool.exe wasn't found at {keytoolPath}");
                return;
            }

            if (File.Exists(keystorePath))
            {
                if (!outputOnlySha1)
                {
                    Console.WriteLine($"Key store path: {keystorePath}");
                }
            }
            else
            {
                Console.WriteLine($"keystore.debug wasn't found at {keystorePath}. Please try deploying any Android app to automatically generate this.");
                return;
            }

            var response = GetSHA1(keytoolPath, keystorePath);
            if (String.IsNullOrWhiteSpace(response.Item1))
            {
                Console.WriteLine($"Error: {response.Item2}");
                return;
            }

            var parsedSHA1 = ParseSHA1(response.Item1);

            if (outputOnlySha1)
            {
                Console.WriteLine(parsedSHA1);
                return;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"SHA1: {parsedSHA1}");
            }
            Console.WriteLine("Press any key to quit");
            Console.ReadKey(); //to keep console open

        }

        static Tuple<string, string> GetFirstLineOfWhichCommand(string name)
        {
            Process myProcess = new Process();
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo("which", name);

            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardError = true;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcess.StartInfo = myProcessStartInfo;
            myProcess.Start();

            var sr = myProcess.StandardOutput;
            var consoleOut = sr.ReadToEnd();
            var keytoolPath = consoleOut.Split('\n').FirstOrDefault() ?? "";

            var srError = myProcess.StandardError;
            var consoleError = srError.ReadToEnd();

            myProcess.Close();

            return new Tuple<string, string>(keytoolPath, consoleError);
        }

        static Tuple<string, string> GetSHA1(string keytoolPath, string keystorePath)
        {
            Process myProcess = new Process();
            var arguments = $"-list -v -keystore \"{keystorePath}\" -storepass android -keypass android";
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(keytoolPath, arguments);

            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardError = true;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcess.StartInfo = myProcessStartInfo;
            myProcess.Start();

            var sr = myProcess.StandardOutput;
            var consoleOut = sr.ReadToEnd();

            var srError = myProcess.StandardError;
            var consoleError = srError.ReadToEnd();

            myProcess.Close();

            return new Tuple<string, string>(consoleOut, consoleError);
        }

        static string GetUserName()
        {
            return Environment.UserName;
        }

        static string GetProgramFilesx86Location()
        {
            if (Environment.Is64BitOperatingSystem == true)
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        static string GetJavaPath()
        {
            string javaPath = null;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    javaPath = GetFirstLineOfWhichCommand("java").Item1;
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    string javaParentPath = Path.Combine(GetProgramFilesx86Location(), "Java");
                    if (!Directory.Exists(javaParentPath))
                        return String.Empty;

                    javaPath = Directory.EnumerateDirectories(javaParentPath).Last();
                    break;
                default:
                    // No idea what platform they are on.
                    throw new Exception("ALL IS LOST!!!");
            }

            return javaPath;
        }

        static string GetKeytoolPath(string javaPath)
        {
            string keytoolPath = null;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    keytoolPath = GetFirstLineOfWhichCommand("keytool").Item1;
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    keytoolPath = Path.Combine(javaPath, "bin\\keytool.exe");
                    break;
                default:
                    // No idea what platform they are on.
                    throw new Exception("ALL IS LOST!!!");
            }

            return keytoolPath;
        }

        static string GetDebugKeystorePath()
        {
            string debugKeystorePath = null;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    debugKeystorePath = $"/Users/{GetUserName()}/.local/share/Xamarin/Mono for Android/debug.keystore";
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    debugKeystorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Xamarin\\Mono for Android\\debug.keystore");
                    break;
                default:
                    // No idea what platform they are on.
                    throw new Exception("ALL IS LOST!!!");
            }

            return debugKeystorePath;
        }

        public static string ParseSHA1(string value)
        {
            if (value.Contains("SHA1:"))
                return value.Substring(value.IndexOf("SHA1:") + 6, 59);

            return "SHA1 not found - something went wrong and we clearly didn't catch it";
        }
    }
}
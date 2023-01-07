using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManifestGenerator {
    internal class Program {

        static string MainDirectory;

        [STAThreadAttribute] static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Please locate the folder in which your SCP: Containment Breach Remastered game files are located in...");
            Thread.Sleep(2000);

            FolderBrowserDialog browser = new FolderBrowserDialog();
            browser.RootFolder = Environment.SpecialFolder.MyComputer;
            browser.Description = "Please locate the folder in which your SCP: Containment Breach Remastered game files are located in.";

            if (browser.ShowDialog() == DialogResult.OK) {
                MainDirectory = browser.SelectedPath;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully located game directory.");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Generating game file manifest to your desktop...");
                Thread.Sleep(2000);
            } else {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to locate directory.");
                Thread.Sleep(2000);
                Environment.Exit(-1);
            }

            List<string> Directories = new List<string> {
                MainDirectory
            };

            List<string> Files = new List<string>();

            int timesFoundNothing = 0;

            while (timesFoundNothing <= 3) {
                List<string> newDirs = new List<string>();
                foreach (string dir in Directories) {
                    string[] dirs = Directory.GetDirectories(dir);
                    foreach (string dir2 in dirs) {
                        if (!Directories.Contains(dir2)) {
                            if (dir2 != MainDirectory + "\\Saves" && dir2 != MainDirectory + "\\SFX\\Radio\\UserTracks")
                                newDirs.Add(dir2);
                        }
                    }
                }

                if (newDirs.Count == 0)
                    timesFoundNothing++;
                
                foreach (string dir in newDirs) {
                    Directories.Add(dir);
                }
            }

            foreach (string dir in Directories) {
                string[] files = Directory.GetFiles(dir);
                Files.Add(dir);

                foreach (string file in files) {
                    if (file != MainDirectory + "\\steam_autocloud.vdf" || file != MainDirectory + "\\options.ini")
                        Files.Add(file);
                }
            }

            Files.Sort((s1, s2) => s1.CompareTo(s2));

            List<string> output = new List<string>();

            foreach (string file in Files) {
                output.Add(file + " | " + CalculateMD5(file));
            }

            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\SCPCBR_DEBUG.txt", output);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully wrote debug information to SCPCBR_DEBUG.txt on your desktop.");
            Thread.Sleep(5000);
        }

        static string CalculateMD5(string filename) {
            try {
                using (var md5 = MD5.Create()) {
                    using (var stream = File.OpenRead(filename)) {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            } catch {
                return "00000000000000000000000000000000";
            }
        }
    }
}

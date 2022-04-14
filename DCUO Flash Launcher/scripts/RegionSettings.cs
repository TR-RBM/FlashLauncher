using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FlashLauncher
{
    /// <summary>
    /// Class for Region operations
    /// </summary>
    public class RegionSettings
    {
        public string CurrentSelection = "EU";
        private string[] AllowedRegions = new string[] { "EU", "US" };
        private string SettingsFilePath = @".\settings.ini";
        private string DefaultRegionStr = "defaultRegion";
        public RegionSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                try
                {
                    File.Create(SettingsFilePath).Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Read the region from settings file
        /// </summary>
        public void LoadSettings()
        {
            string line = "";

            using (StreamReader reader = new StreamReader(SettingsFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    // skip comments symbol: #
                    if (line[0] == '#')
                    {
                        continue;
                    }
                    else if (line.Contains(DefaultRegionStr) && line.Contains('='))
                    {
                        try
                        {
                            string regionStr = line.Split('=')[1].ToUpper();
                            if (!AllowedRegions.Contains(regionStr))
                            {
                                continue;
                            }
                            else
                            {
                                CurrentSelection = regionStr;
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                reader.Close();
            }
        }


        /// <summary>
        /// Write region settings
        /// </summary>
        public void WriteSettings()
        {
            List<string> lines = new List<string>();

            string line = "";
            using (StreamReader reader = new StreamReader(SettingsFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] == '#')
                    {
                        lines.Add(line);
                        continue;
                    }
                    else if(line.Contains(DefaultRegionStr) && line.Contains('='))
                    {
                        lines.Add(DefaultRegionStr + '=' + CurrentSelection);
                        continue;
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }
                reader.Close();
            }
            File.WriteAllLines(SettingsFilePath, lines.ToArray());
        }
    }
}
using StardewValley_WebScraper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using static StardewValley_WebScraper.Utilities.Enums;

namespace StardewValley_WebScraper.Utilities
{
    public static class Saving
    {
        private static Security _security;
        private static KeyValuePair<bool, bool> _canAccessSaveLoad;
        private const string DATA_FILENAME = @".\stardewValleyVillagers.dat";
        private static BinaryFormatter BinaryFormatter;

        public static Security Security
        {
            get
            {
                if (_security == null)
                {
                    _security = new Security();
                }
                return _security;
            }
        }

        /// <summary>
        /// Provides whether it is possible to access the save load base directory (key) and save files (value)
        /// </summary>
        public static KeyValuePair<bool, bool> CanAccessSaveLoad
        {
            get
            {
                if (_canAccessSaveLoad.Key == false || _canAccessSaveLoad.Value == false)
                {
                    bool savedFileAccess =
                             Security.HasAccess(
                                 new FileInfo(DATA_FILENAME),
                                 System.Security.AccessControl.FileSystemRights.Write);

                    bool baseDirectory = 
                            Security.HasAccess(
                             new DirectoryInfo(@".\"),
                             System.Security.AccessControl.FileSystemRights.Write);
                    _canAccessSaveLoad = new KeyValuePair<bool, bool>(baseDirectory, savedFileAccess);
                }
                
                return _canAccessSaveLoad;
            }
        }

        static Saving()
        {
            BinaryFormatter = new BinaryFormatter();
        }

        public static bool Save(ObservableCollection<Villager> villagersCollection)
        {
            try
            {
                if (!File.Exists(DATA_FILENAME))
                {
                    StardewValleyVillagersData stardewValleyVillagersData = new StardewValleyVillagersData
                    {
                        Villagers = new List<Villager_Save>()
                    };

                    foreach (var villager in villagersCollection)
                    {
                        stardewValleyVillagersData.Villagers.Add(new Villager_Save
                        {
                            Address = villager.Address,
                            AvatarUrl = villager.AvatarUrl,
                            BestGifts = villager.BestGifts,
                            Birthday = villager.Birthday,
                            Gender = villager.Gender,
                            LivesIn = villager.LivesIn,
                            Marriage = villager.Marriage,
                            Name = villager.Name
                        });
                    }

                    using (FileStream writerFileStream = new FileStream(DATA_FILENAME, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter.Serialize(writerFileStream, stardewValleyVillagersData);

                        writerFileStream.Close();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write to file:\n\n" + ex.Message);
                return false;
            }
        }

        public static ObservableCollection<Villager> Load()
        {
            ObservableCollection<Villager> villagers = null;
            StardewValleyVillagersData dataStore = null;

            if (File.Exists(DATA_FILENAME) && Security.HasAccess(new FileInfo(DATA_FILENAME), System.Security.AccessControl.FileSystemRights.Write))
            {
                try
                {
                    using (FileStream readerFileStream = new FileStream(DATA_FILENAME, FileMode.Open, FileAccess.Read))
                    {
                        dataStore = (StardewValleyVillagersData)BinaryFormatter.Deserialize(readerFileStream);
                        readerFileStream.Close();
                    }

                    villagers = new ObservableCollection<Villager>();

                    foreach (var villager in dataStore.Villagers)
                    {
                        villagers.Add(new Villager(villager.Name)
                        {
                            Address = villager.Address,
                            AvatarUrl = villager.AvatarUrl,
                            BestGifts = villager.BestGifts,
                            Birthday = villager.Birthday,
                            Gender = villager.Gender,
                            LivesIn = villager.LivesIn,
                            Marriage = villager.Marriage
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load data:\n\n" + ex.Message);
                }
            }

            return villagers;
        }
    }

    [Serializable]
    public class StardewValleyVillagersData
    {
        public List<Villager_Save> Villagers;
    }

    // Need to convert Villager to Save Safe object (no observable).

    [Serializable]
    public class Villager_Save
    {
        public string Name { get; set; }

        public Genders Gender { get; set; }

        internal string AvatarUrl { get; set; }

        public string Birthday { get; set; }

        public string LivesIn { get; set; }

        public string Address { get; set; }

        public string Marriage { get; set; }

        public List<string> BestGifts { get; set; }
    }
}

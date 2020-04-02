using Dev2Be.Toolkit;
using MRULib;
using MRULib.MRU.Interfaces;
using MRULib.MRU.Models.Persist;
using System;
using System.IO;
using System.Reflection;

namespace SDLP
{
    public class RecentFiles
    {
        private static IMRUListViewModel mruListViewModel = null;

        private static readonly string mruFilePath;

        public static IMRUListViewModel MRUListViewModel { get { return mruListViewModel; } }

        static RecentFiles()
        {
            AssemblyInformations assemblyInformation = new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name);

            mruListViewModel = MRU_Service.Create_List();

            mruFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyInformation.Company, assemblyInformation.Product, "mru.xml");

            if (!Directory.Exists(Path.GetDirectoryName(mruFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(mruFilePath));

            if (File.Exists(mruFilePath))
                mruListViewModel = MRUEntrySerializer.Load(mruFilePath);
        }

        public static void UpdateEntry(string filePath) => mruListViewModel.UpdateEntry(MRU_Service.Create_Entry(filePath, DateTime.Now));

        public static void SaveMRU()
        {
            MRUEntrySerializer.Save(mruFilePath, MRUListViewModel);
        }
    }
}

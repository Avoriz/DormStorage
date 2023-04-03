using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DormStorageV1
{
    //
    // Summary:
    //      Integrates an automatic updater that connects to an outsourced link to compare versions and, if needed, download.
    public class Updater
    {
        public string RemoteVersionPath { get; set; }
        public string RemoteVersion { get; set; }
        public string LocalVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo localVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
                return localVersion.FileVersion;
            }
        }

        public Updater()
        {
            Init(null);
        }
        
        public Updater(string remoteVersionPath)
        {
            Init(remoteVersionPath);
        }

        public void Init(string remoteVersionUri)
        {
            while (true)
            {
                if (remoteVersionUri == null)
                {
                    Console.WriteLine("No path specified.");
                    Console.ReadKey();
                    Console.Clear();
                    break;
                }
                RemoteVersionPath = remoteVersionUri;
                FetchRemoteVersion(RemoteVersionPath);
                if (RemoteVersion != LocalVersion)
                {
                    FetchUpdate(RemoteVersionPath);
                }
            }
        }

        public string FetchRemoteVersion(string remoteVersionUri)
        {
            WebClient client = new WebClient();
            string remoteVersion = client.DownloadString(remoteVersionUri);
            return RemoteVersion = remoteVersion;
        }

        public void FetchUpdate(string remoteVersionUri) 
        {

        }
    }
}

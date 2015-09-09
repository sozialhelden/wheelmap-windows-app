using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;

namespace Wheelmap_Windows.Model {
    public class Database : SQLiteConnection {

        private const string DATABASE_NAME = "wheelmap.db";

        private static Database _instance;
        public static Database Instance {
            get {
                if (_instance == null) {
                    var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    var path = Path.Combine(folder.Path.ToString(), DATABASE_NAME);
                    _instance = new Database(path);
                }
                return _instance;
            }
        }

        public Database(string path) : base(new SQLitePlatformWinRT(), path) {
            Log.d(this, "Databasepath: " + path);
            CreateTable<DatabaseVersion>();
            CreateTable<Category>();
            CreateTable<NodeType>();
            CreateTable<Node>();
        }
    }

    // for later
    public class DatabaseVersion {

        [PrimaryKey]
        public int id { get; set; } = 1;

        public int version { get; set; } = 1;

    }
}

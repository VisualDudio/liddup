using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Lite;

using Liddup.Models;
using Xamarin.Forms;

namespace Liddup
{
    public static class SongManager
    {
        public delegate void ReloadDataDelegate(object sender, ReplicationChangeEventArgs e);
        public delegate void DatabaseChangedDelegate(object sender, DatabaseChangeEventArgs e);

        private static Database _database;
        private static Manager _manager;
        private const ushort Port = 5432;
        private const string Scheme = "http";
        public static string Host;
        private const string DatabaseName = "liddupsongs4";

        static SongManager()
        {
            _manager = Manager.SharedInstance;
            _database = _manager.GetDatabase(DatabaseName);
        }

        public static Song GetSong(string id)
        {
            var doc = _database.GetDocument(id);
            var props = doc.UserProperties;
            var song = new Song
            {
                Id = id,
                Title = props["title"].ToString(),
                Description = props["description"].ToString(),
                Uri = props["uri"].ToString(),
                Votes = Convert.ToInt32(props["votes"].ToString())
            };

            return song;
        }

        public static ObservableCollection<Song> GetSongs()
        {
            var query = _database.CreateAllDocumentsQuery();
            var results = query.Run().OrderByDescending(x => x.Document.Properties["votes"]);

            var songs = new ObservableCollection<Song>();

            foreach (var row in results)
            {
                var song = new Song
                {
                    Id = row.DocumentId,
                    Title = row.Document.UserProperties["title"].ToString(),
                    Description = row.Document.UserProperties["description"].ToString(),
                    Uri = row.Document.UserProperties["uri"].ToString(),
                    Votes = Convert.ToInt32(row.Document.UserProperties["votes"].ToString())
                };
                songs.Add(song);
            }

            return songs;
        }

        public static string SaveSong(Song song)
        {
            Document doc;

            if (song.Id == null)
            {
                doc = _database.CreateDocument();
                doc.PutProperties(song.ToDictionary());
                song.Id = doc.Id;
            }
            else
            {
                doc = _database.GetDocument(song.Id);
                try
                {
                    doc.Update(newRevision =>
                    {
                        var props = newRevision.Properties;
                        props["votes"] = song.Votes;
                        props["title"] = song.Title;
                        props["description"] = song.Description;
                        props["uri"] = song.Uri;
                        return true;
                    });
                }
                catch (Exception ex)
                {
                }
            }
            return doc.Id;
        }

        public static void DeleteSong(Song song)
        {
            var doc = _database.GetExistingDocument(song.Id);
            doc?.Delete();
        }

        public static void StartListener()
        {
            DependencyService.Get<INetworkManager>().Start(_manager, Port);
        }

        public static void UpdateUI(DatabaseChangedDelegate updater)
        {
            _database.Changed += (sender, e) =>
            {
                updater?.Invoke(sender, e);
            };
        }

        public static void StartReplications(ReloadDataDelegate refresher)
        {
            var pull = _database.CreatePullReplication(CreateSyncUri());
            var push = _database.CreatePushReplication(CreateSyncUri());

            pull.Continuous = true;
            push.Continuous = true;

            pull.Start();
            push.Start();

            pull.Changed += (sender, e) =>
            {
                refresher?.Invoke(sender, e);
            };

            push.Changed += (sender, e) =>
            {
                refresher?.Invoke(sender, e);
            };
        }

        private static Uri CreateSyncUri()
        {
            Uri syncUri = null;

            try
            {
                var uriBuilder = new UriBuilder(Scheme, Host, Port, DatabaseName);
                syncUri = uriBuilder.Uri;
            }
            catch
            {

            }

            return syncUri;
        }

        public static void DeleteDatabase()
        {
            _database?.Delete();
        }
    }
}

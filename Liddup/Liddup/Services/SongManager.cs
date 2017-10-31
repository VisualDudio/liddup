using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Couchbase.Lite;
using Couchbase.Lite.Listener.Tcp;

using Liddup.Models;

namespace Liddup.Services
{
    public static class SongManager
    {
        public delegate void DatabaseChangedDelegate(object sender, DatabaseChangeEventArgs e);

        private static readonly Database Database;
        private const ushort Port = 5431;
        private const string Scheme = "http";
        public static string Host;
        private const string DatabaseName = "liddupsongs0005";

        static SongManager()
        {
            Database = Manager.SharedInstance.GetDatabase(DatabaseName);
        }

        public static Song GetSong(string id)
        {
            var doc = Database.GetDocument(id);
            var props = doc.UserProperties;
            var song = new Song
            {
                Id = id,
                Title = props["title"].ToString(),
                Uri = props["uri"].ToString(),
                Votes = Convert.ToInt32(props["votes"].ToString()),
                Source = props["source"].ToString()
            };

            return song;
        }

        public static byte[] GetSongContents(Song song)
        {
            return Database.GetDocument(song.Id).CurrentRevision.GetAttachment("contents")?.Content.ToArray();
        }

        public static IEnumerable<Song> GetSongs()
        {
            var query = Database.CreateAllDocumentsQuery();
            var results = query.Run();

            var songs = new ObservableCollection<Song>();

            foreach (var row in results)
            {
                try
                {
                    var song = new Song
                    {
                        Id = row.Document.Id,
                        Title = row.Document.UserProperties["title"].ToString(),
                        Uri = row.Document.UserProperties["uri"].ToString(),
                        Votes = Convert.ToInt32(row.Document.UserProperties["votes"].ToString()),
                        Source = row.Document.UserProperties["source"].ToString()
                    };
                    songs.Add(song);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return songs;
        }

        public static string SaveSong(Song song)
        {
            Document doc;
            
            if (song.Id == null)
            {
                doc = Database.CreateDocument();
                doc.PutProperties(song.ToDictionary());
                
                if (song.Source.Equals("Library"))
                {
                    var newRevision = doc.CurrentRevision.CreateRevision();
                    newRevision.SetAttachment("contents", "audio/mpeg", song.Contents);
                    newRevision.Save();
                }
            }
            else
            {
                doc = Database.GetDocument(song.Id);
                try
                {
                    doc.Update(newRevision =>
                    {
                        var properties = newRevision.UserProperties;
                        if (song.Source.Equals("Library"))
                            newRevision.SetAttachment("contents", "audio/mpeg", song.Contents);
                        properties["votes"] = song.Votes;
                        newRevision.SetUserProperties(properties);
                        return true;
                    });
                }
                catch (CouchbaseLiteException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
            song.Id = doc.Id;
            return doc.Id;
        }

        public static void DeleteSong(Song song)
        {
            var doc = Database.GetExistingDocument(song.Id);
            doc?.Delete();
        }

        public static void StartListener()
        {
            var listener = new CouchbaseLiteTcpListener(Manager.SharedInstance, Port);
            listener.Start();
            
            //var broadcaster = new CouchbaseLiteServiceBroadcaster(null, port)
            //{
            //    Name = "LiddupSession"
            //};
            //broadcaster.Start();

            //var browser = new CouchbaseLiteServiceBrowser(null);
            //browser.ServiceResolved += (sender, e) => {
                
            //};

            //browser.ServiceRemoved += (sender, e) => {
                
            //};
            //browser.Start();
        }

        public static void StartReplications(DatabaseChangedDelegate refresher)
        {
            var pull = Database.CreatePullReplication(CreateSyncUri());
            var push = Database.CreatePushReplication(CreateSyncUri());

            pull.Continuous = true;
            push.Continuous = true;

            Database.Changed += (sender, e) =>
            {
                refresher?.Invoke(sender, e);
            };

            pull.Start();
            push.Start();
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
                System.Diagnostics.Debug.WriteLine("Error in creating sync uri!");
            }

            return syncUri;
        }

        public static void DeleteDatabase()
        {
            Database?.Delete();
        }

        public static void DeleteDatabases()
        {
            var database = Manager.SharedInstance.GetExistingDatabase(DatabaseName);
            foreach (var doc in database.CreateAllDocumentsQuery().Run())
                doc.Document.Delete();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Liddup.Models;
using Liddup.Services;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using Xamarin.Forms;

namespace Liddup
{
    public static class SpotifyWebApiManager
    {
        private static SpotifyWebAPI _spotify;
        private static PrivateProfile _profile;
        private static CancellationTokenSource _tokenSource;

        static SpotifyWebApiManager()
        {
            _spotify = new SpotifyWebAPI
            {
                UseAuth = true,
                TokenType = "Bearer",
                AccessToken = DependencyService.Get<ISpotifyApi>()?.AccessToken
            };
            _profile = _spotify.GetPrivateProfile();
            _profile.Id = Uri.EscapeDataString(_profile.Id);
        }

        public static async Task<List<SimplePlaylist>> GetUserPlaylistsAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var playlists = await _spotify.GetUserPlaylistsAsync(_profile.Id);
            token.ThrowIfCancellationRequested();
            var list = playlists.Items.ToList();

            while (playlists.Next != null)
            {
                token.ThrowIfCancellationRequested();
                playlists = await _spotify.GetUserPlaylistsAsync(_profile.Id, 20, playlists.Offset + playlists.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(playlists.Items);
            }

            return list;
        }

        public static async Task<List<FullTrack>> GetUserPlaylistSongsAsync(string profileId, string playlistId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var savedTracks = await _spotify.GetPlaylistTracksAsync(profileId, playlistId);
            token.ThrowIfCancellationRequested();
            var list = savedTracks.Items.Select(track => track.Track).ToList();

            while (savedTracks.Next != null)
            {
                token.ThrowIfCancellationRequested();
                savedTracks = await _spotify.GetPlaylistTracksAsync(profileId, playlistId, "", 20, savedTracks.Offset + savedTracks.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(savedTracks.Items.Select(track => track.Track));
            }

            return list;
        }

        public static async Task<List<FullTrack>> GetSavedTracks(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var savedTracks = await _spotify.GetSavedTracksAsync();
            token.ThrowIfCancellationRequested();
            var list = savedTracks.Items.Select(track => track.Track).ToList();

            while (savedTracks.Next != null)
            {
                token.ThrowIfCancellationRequested();
                savedTracks = await _spotify.GetSavedTracksAsync(20, savedTracks.Offset + savedTracks.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(savedTracks.Items.Select(track => track.Track));
            }

            return list;
        }

        public static void AddSongToMasterPlaylist(object item, ISongProvider sender)
        {
            var song = new Song
            {
                Uri = ((FullTrack)item).Uri,
                SongSource = "Spotify",
                Title = ((FullTrack)item).Name,
                Votes = 0
            };
            var track = new FullTrack();

            try
            {
                MessagingCenter.Send(sender, "AddSong", song);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}

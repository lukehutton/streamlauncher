﻿using System.Threading.Tasks;
using StreamLauncher.Repositories;

namespace StreamLauncher.MediaPlayers
{
    public interface ILiveStreamer
    {
        void Play(string game, string streamSource, Quality quality);
        void SaveConfig();
    }
}
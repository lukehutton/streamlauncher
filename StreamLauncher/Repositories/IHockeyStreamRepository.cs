﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public interface IHockeyStreamRepository
    {
        Task<IEnumerable<HockeyStream>> GetLiveStreams(DateTime date);
        Task<LiveStream> GetLiveStream(int streamId, string location, Quality quality);
    }
}
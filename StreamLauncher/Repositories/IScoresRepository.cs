using System.Collections.Generic;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public interface IScoresRepository
    {
        IEnumerable<Score> GetScores();
    }
}
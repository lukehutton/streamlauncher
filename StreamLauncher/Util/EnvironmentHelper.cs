using System;

namespace StreamLauncher.Util
{
    public class EnvironmentHelper : IEnvironmentHelper
    {
        public bool Is64BitEnvironment()
        {
            return Environment.Is64BitOperatingSystem;
        }
    }
}
using OpenSilver.Simulator;
using System;

namespace MigrationExamples.OpenSilver.Simulator
{
    internal static class Startup
    {
        [STAThread]
        static int Main(string[] args)
        {
            return SimulatorLauncher.Start(typeof(App));
        }
    }
}

﻿namespace TSGSystemsToolkit.CmdLine.Options
{
    public class VeederRootOptions : OptionsBase
    {
        public string FilePath { get; set; }
        public string OutputPath { get; set; }
        public bool CreateFuelPosFile { get; set; }
        public bool CreateCsv { get; set; }
    }
}

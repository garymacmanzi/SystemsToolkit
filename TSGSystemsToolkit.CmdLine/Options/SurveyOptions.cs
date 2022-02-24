﻿namespace TSGSystemsToolkit.CmdLine.Options
{
    public class SurveyOptions : OptionsBase
    {
        public string FilePath { get; set; }
        public string OutputPath { get; set; }
        public bool FuelPosSurvey { get; set; }
        public bool SerialNumberSurvey { get; set; }
    }
}

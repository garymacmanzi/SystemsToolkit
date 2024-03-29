﻿using System;
using System.Collections.Generic;
using System.IO;

namespace FuelPOS.DebugTools
{
    public class DebugFileCreator
    {
        public string GenerateProcessString(string processName)
        {
            return processName;
        }

        public string GenerateProcessString(string processName, List<string> parameters)
        {
            string output = $"{processName},";
            foreach (var param in parameters)
            {
                output += $"{param} ";
            }

            return output;
        }

        public List<string> GenerateFileString(List<string> processes)
        {
            return GenerateFileString(processes, 24);
        }

        public List<string> GenerateFileString(List<string> processes, int debugTime)
        {
            /* File format:
             * DEBUG_TIME,hh
             * ProcessName,P,a (P = POS number, a = optional params)
             */
            List<string> output = new List<string>
            {
                "; Debug definition file",
                $"; Generated by TSG UK debug tool: {DateTime.Now}",
            };

            output.Add($"DEBUG_TIME,{debugTime}");

            foreach (var process in processes)
            {
                output.Add(process);
            }

            output.Add("; End File");

            return output;
        }

        public void CreateFile(List<string> fileText, string outputPath)
        {
            // Max Filesize to be 5000 bytes
            if (outputPath.EndsWith(@"\"))
            {
                outputPath = outputPath.Remove(outputPath.Length - 1, 1);
            }

            outputPath += @"\BUG_DEF.ASC";

            File.WriteAllLines(outputPath, fileText);
        }
    }
}

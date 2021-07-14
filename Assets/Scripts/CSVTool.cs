using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using CsvHelper;
using System.Globalization;

public class CSVTool
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(TextAsset data)
    {
        var list = new List<Dictionary<string, object>>();

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                if (int.TryParse(value, out int n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out float f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static void Write<T>(List<T> data, bool constant)
    {
        string folder;
        if (Application.isEditor)
        {
            folder = Application.streamingAssetsPath;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }
        else
        {
            folder = Application.persistentDataPath;
        }

        string destination = Path.Combine(folder, DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss") + "_" + (constant ? "constant" : "trigger") + ".csv");

        using StreamWriter writer = new StreamWriter(destination, false);
        using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data);
    }

}
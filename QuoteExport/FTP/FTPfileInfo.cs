﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dimeng.FTP
{
    public enum DirectoryEntryTypes
    {
        File,
        Directory
    }

    public class FTPfileInfo
    {
        public string FullName
        {
            get
            { return Path + Filename; }
        }
        public string Filename
        { get { return _filename; } }
        /// <summary>
        /// Path of file (always ending in /)
        /// </summary>
        /// <remarks>
        /// 1.1: Modifed to ensure always ends in / - with thanks to jfransella for pointing this out
        /// </remarks>
        public string Path
        {
            get
            {
                return _path + (_path.EndsWith("/") ? "" : "/");
            }
        }

        public DirectoryEntryTypes FileType
        {
            get
            {
                return _fileType;
            }
        }

        public long Size
        {
            get
            {
                return _size;
            }
        }

        public DateTime FileDateTime
        {
            get
            {
                return _fileDateTime;
            }
            internal set
            {
                _fileDateTime = value;
            }
        }

        public string Permission
        {
            get
            {
                return _permission;
            }
        }

        public string Extension
        {
            get
            {
                int i = this.Filename.LastIndexOf(".");
                if (i >= 0 && i < (this.Filename.Length - 1))
                {
                    return this.Filename.Substring(i + 1);
                }
                else { return ""; }
            }
        }
        public string NameOnly
        {
            get
            {
                int i = this.Filename.LastIndexOf(".");
                if (i > 0) { return this.Filename.Substring(0, i); }
                else
                { return this.Filename; }
            }
        }

        private string _filename;
        private string _path;
        private DirectoryEntryTypes _fileType;
        private long _size;
        private DateTime _fileDateTime;
        private string _permission;

        public FTPfileInfo(string line, string path)
        {
            //parse line
            Match m = GetMatchingRegex(line);
            if (m == null)
            {
                //failed
                throw (new ApplicationException("Unable to parse line: " + line));
            }
            else
            {
                _filename = m.Groups["name"].Value;
                _path = path;
                Int64.TryParse(m.Groups["size"].Value, out _size);
                _permission = m.Groups["permission"].Value;
                string _dir = m.Groups["dir"].Value;
                if (_dir != "" && _dir != "-")
                {
                    _fileType = DirectoryEntryTypes.Directory;
                }
                else
                {
                    _fileType = DirectoryEntryTypes.File;
                }

                try
                {
                    _fileDateTime = DateTime.Parse(m.Groups["timestamp"].Value);
                }
                catch (Exception)
                {
                    _fileDateTime = Convert.ToDateTime(null);
                }
            }
        }

        private Match GetMatchingRegex(string line) { Regex rx; Match m; for (int i = 0; i <= _ParseFormats.Length - 1; i++) { rx = new Regex(_ParseFormats[i]); m = rx.Match(line); if (m.Success) { return m; } } return null; }

        private static string[] _ParseFormats = new string[] { 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)",
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)",
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)",
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})(\\s+)(?<size>(\\d+))(\\s+)(?<ctbit>(\\w+\\s\\w+))(\\s+)(?<size2>(\\d+))\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{2}:\\d{2})\\s+(?<name>.+)",
            "(?<timestamp>\\d{2}\\-\\d{2}\\-\\d{2}\\s+\\d{2}:\\d{2}[Aa|Pp][mM])\\s+(?<dir>\\<\\w+\\>){0,1}(?<size>\\d+){0,1}\\s+(?<name>.+)" 
        };

    }
}

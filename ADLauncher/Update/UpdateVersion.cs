using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADLauncher
{
    public class UpdateVersion
    {
        public UpdateVersion(string verStr)
        {
            this.VersionNumber = verStr;
            string[] vers = this.VersionNumber.Split('.');

            if (vers.Length == 4)
            {
                this.Year = int.Parse(vers[0]);
                this.Month = int.Parse(vers[1]);
                this.Day = int.Parse(vers[2]);
                this.ReleaseNumber = int.Parse(vers[3]);
            }
            else
            {
                throw new Exception("Wrong version number!");
            }
        }
        public string VersionNumber { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }
        public int ReleaseNumber { get; private set; }

        public bool IsNewerThan(UpdateVersion uv)
        {
            //todo：好傻的
            if (this.Year < uv.Year)
            {
                return false;
            }
            else if (this.Year == uv.Year)
            {
                if (this.Month < uv.Month)
                {
                    return false;
                }
                else if (this.Month == uv.Month)
                {
                    if (this.Day < uv.Day)
                    {
                        return false;
                    }
                    else if (this.Day == uv.Day)
                    {
                        if (this.ReleaseNumber <= uv.ReleaseNumber)
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return true;
            }
        }
    }
}

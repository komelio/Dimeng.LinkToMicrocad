using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    internal partial class SubassemblyChecker
    {
        internal string FileName(Subassembly sub, string librarySubsPath, string projectSubsPath)
        {
            Logger.GetLogger().Debug("Library subassembly path:" + librarySubsPath);
            Logger.GetLogger().Debug("Project subassembly path:" + projectSubsPath);

            string subFileName = string.Format("{0}_({1}){2}.cutx", sub.Parent.Handle, sub.Description, sub.LineNumber);
            string subFileNameCommon = string.Format("{0}.cutx", sub.Description);

            subFileName = Path.Combine(projectSubsPath, subFileName);
            if (File.Exists(subFileName))
            {
                Logger.GetLogger().Info(string.Format("Subassembly file found:{0}", subFileName));
                return subFileName;
            }

            subFileNameCommon = Path.Combine(projectSubsPath, subFileNameCommon);
            if (File.Exists(subFileNameCommon))
            {
                Logger.GetLogger().Info(string.Format("Subassembly file found:{0}", subFileNameCommon));
                return subFileNameCommon;
            }

            Logger.GetLogger().Info("Copying subassembly cutx file from library path");

            string[] files = Directory.GetFiles(librarySubsPath, sub.Description + ".cutx", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                throw new Exception(string.Format("Subassembly {0} not found!", sub.Description));
            }

            Logger.GetLogger().Debug("Library subassembly file path:" + files[0]);

            File.Copy(files[0], subFileNameCommon);
            return files[0];
        }
    }
}

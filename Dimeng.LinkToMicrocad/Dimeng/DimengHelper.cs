/*
 * Steps:
 * 1. Read the current product(AK,AD,AC,etc..) information from registry
 * 2. Find the temp.xml from product installation path.
 *    Read the information from xml file.
 * 3. According to the information from temp.xml,find relative microvellum cutx file and popup the prompt dialog.
 * 4. Draw the block dwg file and refresh the temp.xml
 * 5. Done.
 * 
 */

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    internal class DimengHelper
    {

        internal void ShowPromptAndDrawBlock()
        {
            var productInfo = AKInfo.GetInfo();

            string folderPath = getTempFolderPath(productInfo.Path);
            string tempXMLFilePath = Path.Combine(folderPath, "temp.xml");

            AKProduct product = AKProduct.Load(tempXMLFilePath);
            BlockDrawer drawer = new BlockDrawer(product.Tab.VarX, product.Tab.VarZ, product.Tab.VarY, folderPath);
            drawer.DrawAndSaveAs();
        }

        private string getTempFolderPath(string productPath)
        {
            string folderPath = Path.Combine(productPath, "temp");
            folderPath = Path.Combine(folderPath, "dms");

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException("未找到交换数据的文件夹");
            }

            return folderPath;
        }

        internal void EditAndDrawBlock()
        {
            ShowPromptAndDrawBlock();
        }
    }
}

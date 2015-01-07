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

using Dimeng.WoodEngine.Prompts;
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
            try
            {
                string folderPath = getTempFolderPath(Context.GetContext().AKInfo.Path);
                string tempXMLFilePath = Path.Combine(folderPath, "temp.xml");

                AKProduct product = AKProduct.Load(tempXMLFilePath);
                string dwgFilePath = Path.Combine(folderPath, product.Tab.DWG + ".dwg");

                //Find Microvellum data and call the prompt pop out
                //var mvLibrary = new MVLibrary(Path.Combine(productInfo.Path, "Dimeng", "Library"));

                //test prompt window
                string productCutx = @"c:\mv\di meng chan pin ku_v3\library\01-地柜\a-开门地柜.cutx";
                string globalGvfx = @"c:\mv\di meng chan pin ku_v3\Template\GlobalV7.gvfx";
                string edgeEdgx = @"c:\mv\di meng chan pin ku_v3\Template\封边文件.edgx";
                string cutPartsCtpx = @"c:\mv\di meng chan pin ku_v3\Template\切割板件文件.ctpx";
                string hardwareHwrx = @"c:\mv\di meng chan pin ku_v3\Template\五金件文件.hwrx";
                string doorstyleDsvx = @"c:\mv\di meng chan pin ku_v3\Template\门样式文件.dsvx";

                PromptsViewModel viewmodel = new PromptsViewModel(productCutx, globalGvfx, cutPartsCtpx, edgeEdgx, hardwareHwrx, doorstyleDsvx,
                    product.Tab.Name, product.Tab.Photo, product.Tab.VarX, product.Tab.VarZ, product.Tab.VarY);

                PromptWindow prompt = new PromptWindow();
                prompt.ViewModel = viewmodel;
                prompt.ShowDialog();

                BlockDrawer drawer = new BlockDrawer(product.Tab.VarX, product.Tab.VarZ, product.Tab.VarY, dwgFilePath);
                drawer.DrawAndSaveAs();
            }
            catch (Exception error)
            {
                throw new Exception("Error occured during drawing....", error);
            }
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

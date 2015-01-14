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

using Dimeng.WoodEngine.Entities;
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
                var project = ProjectManager.CreateOrOpenProject(product.GetProjectPath());

                SpecificationGroup specificationGroup;
                string productCutx;

                var mvProduct = project.Products.Find(it => it.Handle == product.Tab.DMID);
                if (mvProduct != null)//if current project have the dmid product from its products,just open the old data
                {
                    productCutx = Path.Combine(project.JobPath, mvProduct.FileName);
                    specificationGroup = project.SpecificationGroups.Find(it => it.Name == mvProduct.MatFile);
                }
                else//if not,find a new one and add a new product to project
                {
                    //Find Microvellum data and call the prompt pop out
                    productCutx = Context.GetContext().MVDataContext.GetProduct(product.Tab.Name);
                    specificationGroup = project.SpecificationGroups[0];
                }

                string globalGvfx = Path.Combine(project.JobPath, specificationGroup.GlobalFileName);
                string cutPartsCtpx = Path.Combine(project.JobPath, specificationGroup.CutPartsFileName);
                string edgeEdgx = Path.Combine(project.JobPath, specificationGroup.EdgeBandFileName);
                string hardwareHwrx = Path.Combine(project.JobPath, specificationGroup.HardwareFileName);
                string doorstyleDsvx = Path.Combine(project.JobPath, specificationGroup.DoorWizardFileName);

                PromptsViewModel viewmodel = new PromptsViewModel(productCutx, globalGvfx, cutPartsCtpx, edgeEdgx, hardwareHwrx, doorstyleDsvx,
                        product.Tab.Name, product.Tab.Photo, product.Tab.VarX, product.Tab.VarZ, product.Tab.VarY);

                PromptWindow prompt = new PromptWindow();
                prompt.ViewModel = viewmodel;
                prompt.ShowDialog();

                ProjectManager.AddProduct(viewmodel.Name, viewmodel.Comments, product.Tab.DMID,
                    project.SpecificationGroups[0], viewmodel.Width, viewmodel.Height, viewmodel.Depth,
                    viewmodel.Book, project);

                BlockDrawer drawer = new BlockDrawer(product.Tab.VarX,
                                                     product.Tab.VarZ,
                                                     product.Tab.VarY,
                                                     Path.Combine(folderPath, product.Tab.DWG + ".dwg"));
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

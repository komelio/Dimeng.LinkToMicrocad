using Dimeng.LinkToMicrocad.Logging;
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
                string folderPath = getTempFolderPath(
                    Context.GetContext().AKInfo.Path);
                AKProduct product = AKProduct.Load(
                    Path.Combine(folderPath, "temp.xml"));

                var project = ProjectManager.CreateOrOpenProject(
                    product.GetProjectPath());

                Product mvProduct = getProductFromProject(product, project);
                string productCutx = mvProduct.GetProductCutxFileName();
                SpecificationGroup specificationGroup = project.SpecificationGroups.Find(it => it.Name == mvProduct.MatFile);

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

        private Product getProductFromProject(AKProduct akProduct, Project project)
        {
            if (project.HasProduct(akProduct.Tab.DMID))
            {
                Logger.GetLogger().Debug("Project has the product:" + akProduct.Tab.Name);
                return project.Products
                              .Find(it => it.Handle.ToUpper() == akProduct.Tab.DMID.ToUpper());
            }
            else
            {
                Logger.GetLogger().Debug("Project do not have the product:" + akProduct.Tab.Name);
                Logger.GetLogger().Debug("Add product to project and copy template from library.");
                return project.AddProduct(akProduct.Tab.Name,
                                          akProduct.Tab.DMID,
                                          akProduct.Tab.VarX,
                                          akProduct.Tab.VarZ,
                                          akProduct.Tab.VarY,
                                          Context.GetContext().MVDataContext.GetProduct(akProduct.Tab.Name));
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

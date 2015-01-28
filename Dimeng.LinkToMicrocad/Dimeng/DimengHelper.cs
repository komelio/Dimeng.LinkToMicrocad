using Dimeng.LinkToMicrocad.Drawing;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Business;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Prompts;
using Microsoft.Win32;
using SpreadsheetGear;
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
                string tempXMLPath = Path.Combine(folderPath,
                    "temp.xml");
                AKProduct product = AKProduct.Load(tempXMLPath);

                var project = ProjectManager.CreateOrOpenProject(
                    product.GetProjectPath());

                Product mvProduct = getProductFromProject(product, project);
                string productCutx = mvProduct.GetProductCutxFileName();
                SpecificationGroup specificationGroup =
                    project.SpecificationGroups.Find(it => it.Name == mvProduct.MatFile);
                var bookset = showPromptWindow(product, project, productCutx, specificationGroup);

                bookset.GetLock();//lock the work book set

                updateProductWHD(product, bookset);

                ProductAnalyst analyst = new ProductAnalyst(
                    Context.GetContext().MVDataContext.GetLatestRelease());
                var errors = analyst.Analysis(mvProduct, bookset);
                bookset.Workbooks["L"].SaveAs(productCutx, FileFormat.OpenXMLWorkbook);

                bookset.ReleaseLock();//release the work book set

                outputErrors(errors);
                (new TempXMLWriter()).WriteFile(tempXMLPath, product);

                ProductDrawer drawer = new ProductDrawer();
                drawer.DrawAndSaveAsDWG(mvProduct,
                    bookset, Path.Combine(folderPath, product.Tab.DWG + ".dwg"));

                //(new TempXMLWriter()).WriteFile(tempXMLPath, product);
            }
            catch (Exception error)
            {
                throw new Exception("Error occured during drawing....", error);
            }
        }

        private void updateProductWHD(AKProduct product, IWorkbookSet bookset)
        {
            //Update the product data
            try
            {
                product.Tab.VarX = double.Parse(bookset.Workbooks["L"].Worksheets["Prompts"].Cells[0, 1].Value.ToString());
                product.Tab.VarZ = double.Parse(bookset.Workbooks["L"].Worksheets["Prompts"].Cells[1, 1].Value.ToString());
                product.Tab.VarY = double.Parse(bookset.Workbooks["L"].Worksheets["Prompts"].Cells[2, 1].Value.ToString());
            }
            catch (Exception error)
            {
                throw new Exception("Error occured when injecting WHD values into product properties.", error);
            }
        }

        private void outputErrors(IEnumerable<ModelError> errors)
        {
            foreach (var error in errors)
            {
                Logger.GetLogger().Fatal(error.Message);
            }
        }

        private static IWorkbookSet showPromptWindow(AKProduct product, Project project, string productCutx, SpecificationGroup specificationGroup)
        {
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

            return viewmodel.BookSet;
        }

        private Product getProductFromProject(AKProduct akProduct, Project project)
        {
            if (project.HasProduct(akProduct.Tab.DMID))
            {
                Logger.GetLogger().Debug("Project has the product:" + akProduct.Tab.Name);
                var _product = project.Products
                                     .Find(it => it.Handle.ToUpper() == akProduct.Tab.DMID.ToUpper());
                _product.Width = akProduct.Tab.VarX;
                _product.Height = akProduct.Tab.VarZ;
                _product.Depth = akProduct.Tab.VarY;

                //TODO:update the info in the project database

                return _product;
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
                SpecificationGroup specificationGroup =
                    project.SpecificationGroups.Find(it => it.Name == mvProduct.MatFile);
                var bookset = showPromptWindow(product, project, productCutx, specificationGroup);

                bookset.GetLock();

                updateProductWHD(product, bookset);

                ProductAnalyst analyst = new ProductAnalyst(
                    Context.GetContext().MVDataContext.GetLatestRelease());
                var errors = analyst.Analysis(mvProduct, bookset);
                bookset.Workbooks["L"].SaveAs(productCutx, FileFormat.OpenXMLWorkbook);



                bookset.ReleaseLock();

                outputErrors(errors);

                ProductDrawer drawer = new ProductDrawer();
                drawer.DrawAndSaveAsDWG(mvProduct,
                    bookset, Path.Combine(folderPath, product.Tab.DWG + ".dwg"));
            }
            catch (Exception error)
            {
                throw new Exception("Error occured during drawing....", error);
            }
        }
    }
}

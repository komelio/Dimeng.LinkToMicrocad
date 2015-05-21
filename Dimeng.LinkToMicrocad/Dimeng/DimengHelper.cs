using Autodesk.AutoCAD.Geometry;
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
using System.Windows;
using System.Xml.Linq;

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

                string mvDataContextPath = product.Tab.CatalogPath;
                mvDataContextPath = Path.Combine(mvDataContextPath, "Library");
                MVDataContext mvContext = MVDataContext.GetContext(mvDataContextPath);
                Context.GetContext().MVDataContext = mvContext;

                var project = ProjectManager.CreateOrOpenProject(
                    product.GetUIVarValue("ManufacturingFolder"));
                Context.GetContext().CurrentProject = project;

                Product mvProduct = getProductFromProject(product, project);
                SpecificationGroup specificationGroup =
                    project.SpecificationGroups.Find(it => it.Name == mvProduct.MatFile);
                var bookset = showPromptWindow(product,
                                               project,
                                               specificationGroup,
                                               mvProduct,
                                               Context.GetContext().MVDataContext.GetLatestRelease());

                bookset.GetLock();//lock the work book set

                updateAKProductWHD(product, bookset);
                mvProduct.Width = product.Tab.VarX;
                mvProduct.Height = product.Tab.VarZ;
                mvProduct.Depth = product.Tab.VarY;
                project.UpdateProduct(mvProduct);

                ProductAnalyst analyst = new ProductAnalyst(
                    Context.GetContext().MVDataContext.GetLatestRelease());
                var errors = analyst.Analysis(mvProduct, bookset);
                bookset.Workbooks["L"].SaveAs(mvProduct.GetProductCutxFileName(),
                                              FileFormat.OpenXMLWorkbook);
                bookset.ReleaseLock();//release the work book set

                outputErrors(errors);

                var offsetVector = new Vector3d(0, 0, 0); //getAcutualWHD(product, mvProduct);
                Logger.GetLogger().Debug("XXXXXX    " + offsetVector.ToString());

                //write the temp.xml back to autodecco
                IEnumerable<string> materialList = getMaterialList(mvProduct);
                (new TempXMLWriter()).WriteFile(tempXMLPath, product, materialList);

                ProductDrawer drawer = new ProductDrawer(offsetVector);
                drawer.DrawAndSaveAsDWG(mvProduct,
                    bookset, Path.Combine(folderPath, product.Tab.DWG + ".dwg"));
            }
            catch (Exception error)
            {
                throw new Exception("Error occured during drawing....", error);
            }
        }

        private Vector3d getAcutualWHD(AKProduct product, Product mvProduct)
        {
            if (mvProduct.CombinedParts.Count == 0)
            {
                return new Vector3d();
            }

            double minx = mvProduct.CombinedParts[0].Point1.X;
            double miny = mvProduct.CombinedParts[0].Point1.Y;
            double minz = mvProduct.CombinedParts[0].Point1.Z;
            double maxx = mvProduct.CombinedParts[0].Point1.X;
            double maxy = mvProduct.CombinedParts[0].Point1.Y;
            double maxz = mvProduct.CombinedParts[0].Point1.Z;

            foreach (var part in mvProduct.CombinedParts)
            {
                List<Point3d> points = new List<Point3d>() { part.Point1, part.Point2, part.Point3, part.Point4, part.Point5, part.Point6, part.Point7, part.Point8 };
                foreach (var pt in points)
                {
                    if (minx > pt.X)
                    { minx = pt.X; }
                    if (miny > pt.Y)
                    { miny = pt.Y; }
                    if (minz > pt.Z)
                    { minz = pt.Z; }
                    if (maxx < pt.X)
                    { maxx = pt.X; }
                    if (maxy < pt.Y)
                    { maxy = pt.Y; }
                    if (maxz < pt.Z)
                    { maxz = pt.Z; }
                }
            }

            product.Tab.VarX = Math.Abs(maxx - minx);
            product.Tab.VarY = Math.Abs(maxy - miny);
            product.Tab.VarZ = Math.Abs(maxz - minz);

            return (new Point3d(-minx, -maxy, -minz)) - Point3d.Origin;
        }

        private IEnumerable<string> getMaterialList(Product mvProduct)
        {
            //todo:combinedparts
            return mvProduct.Parts.Select(it => it.Material.Name).Distinct();
        }

        private void updateAKProductWHD(AKProduct product, IWorkbookSet bookset)
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

        private static IWorkbookSet showPromptWindow(AKProduct product, Project project,
            SpecificationGroup specificationGroup, Product mvProduct, IMVLibrary library)
        {
            string globalGvfx = Path.Combine(project.JobPath, specificationGroup.GlobalFileName);
            string cutPartsCtpx = Path.Combine(project.JobPath, specificationGroup.CutPartsFileName);
            string edgeEdgx = Path.Combine(project.JobPath, specificationGroup.EdgeBandFileName);
            string hardwareHwrx = Path.Combine(project.JobPath, specificationGroup.HardwareFileName);
            string doorstyleDsvx = Path.Combine(project.JobPath, specificationGroup.DoorWizardFileName);

            PromptsViewModel viewmodel = new PromptsViewModel(mvProduct.GetProductCutxFileName(),
                globalGvfx, cutPartsCtpx, edgeEdgx, hardwareHwrx, doorstyleDsvx,
                product.Tab.Name, product.Tab.Photo, product.Tab.VarX, product.Tab.VarZ, product.Tab.VarY, product.Tab.VarElevation, library);

            PromptWindow prompt = new PromptWindow();
            prompt.ViewModel = viewmodel;
            prompt.ShowDialog();

            mvProduct.Comments = viewmodel.Comments;

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
                                          Context.GetContext().MVDataContext.GetProduct(akProduct.Tab.ID));
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

        public void DeleteProduct()
        {
            if (Context.GetContext().CurrentProject == null)
            {
                MessageBox.Show("当前未有打开的任务！");
                return;
            }

            string folderPath = getTempFolderPath(
                    Context.GetContext().AKInfo.Path);
            string tempXMLPath = Path.Combine(folderPath,
                "Delete.xml");

            foreach (string s in readIdFromDeleteXML(tempXMLPath))
            {
                if (Context.GetContext().CurrentProject.HasProduct(s))
                {
                    Logger.GetLogger().Debug("Deleting product id :" + s);
                    Context.GetContext().CurrentProject.DeleteProduct(s);
                }
            }
        }

        private IEnumerable<string> readIdFromDeleteXML(string tempXMLPath)
        {
            List<string> values = new List<string>();

            XElement xml = XElement.Load(tempXMLPath);
            var ids = from e in xml.Elements("D")
                      select e;
            foreach (var x in ids)
            {
                values.Add(x.Attribute("Name").Value);
            }

            return values;
        }
    }
}

using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Drawing;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Business;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Prompts;
using Dimeng.WoodEngine.Spread;
using Microsoft.Win32;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

                if (product.SubInfo == null)//不是组件
                {
                    Product mvProduct = getProductFromProject(product, project);
                    project.Products.Add(mvProduct);

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
                    Logger.GetLogger().Debug("Offset Vector is " + offsetVector.ToString());

                    //write the temp.xml back to autodecco
                    IEnumerable<string> materialList = getMaterialList(mvProduct);
                    (new TempXMLWriter()).WriteFile(tempXMLPath, product, materialList);

                    ProductDrawer drawer = new ProductDrawer(offsetVector, Context.GetContext().MVDataContext.GetLatestRelease());
                    drawer.DrawAndSaveAsDWG(mvProduct,
                        bookset, Path.Combine(folderPath, product.Tab.DWG + ".dwg"));

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerAsync(mvProduct);
                }
                else
                {
                    Product mvProduct = getSubassemblyParentFromProject(product, project);//从组件信息中获取产品
                    SpecificationGroup specificationGroup =
                        project.SpecificationGroups.Find(it => it.Name == mvProduct.MatFile);

                    string subname;
                    var bookset = showSubPromptWindow(product,
                                                   project,
                                                   specificationGroup,
                                                   mvProduct,
                                                   Context.GetContext().MVDataContext.GetLatestRelease(),
                                                   out subname);

                    bookset.Workbooks["S"].SaveAs(subname,  //TODO:组件的文件名
                                                  FileFormat.OpenXMLWorkbook);

                    bookset.GetLock();//lock the work book set

                    ProductAnalyst analyst = new ProductAnalyst(
                        Context.GetContext().MVDataContext.GetLatestRelease());

                    var errors = analyst.Analysis(mvProduct, bookset);
                    bookset.Workbooks["L"].SaveAs(mvProduct.GetProductCutxFileName(),
                                                  FileFormat.OpenXMLWorkbook);

                    bookset.ReleaseLock();//release the work book set

                    outputErrors(errors);

                    var offsetVector = new Vector3d(0, 0, 0); //getAcutualWHD(product, mvProduct);
                    Logger.GetLogger().Debug("Offset Vector is " + offsetVector.ToString());

                    //write the temp.xml back to autodecco
                    IEnumerable<string> materialList = getMaterialList(mvProduct);
                    (new TempXMLWriter()).WriteFile(tempXMLPath, product, materialList);

                    ProductDrawer drawer = new ProductDrawer(offsetVector, Context.GetContext().MVDataContext.GetLatestRelease());
                    drawer.DrawAndSaveAsDWG(mvProduct,
                        bookset, Path.Combine(folderPath, product.Tab.DWG + ".dwg"));

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerAsync(mvProduct);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("绘制过程中发生错误，:(");
                throw new Exception("Error occured during drawing....", error);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Product product = (Product)e.Argument;
                var exporter = new ERPExporter(product);

                Logger.GetLogger().Info("Start exporting excel files....");

                exporter.Output();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + error.StackTrace);
            }
        }

        private IWorkbookSet showSubPromptWindow(AKProduct product, Project project, SpecificationGroup specificationGroup,
            Product mvProduct, IMVLibrary library, out string subFileName)
        {
            string globalGvfx = Path.Combine(project.JobPath, specificationGroup.GlobalFileName);
            string cutPartsCtpx = Path.Combine(project.JobPath, specificationGroup.CutPartsFileName);
            string edgeEdgx = Path.Combine(project.JobPath, specificationGroup.EdgeBandFileName);
            string hardwareHwrx = Path.Combine(project.JobPath, specificationGroup.HardwareFileName);
            string doorstyleDsvx = Path.Combine(project.JobPath, specificationGroup.DoorWizardFileName);

            int lineNumber = 0;
            string subassemblyCutx = project.AddSubToProduct(mvProduct, product, library.Library, ref lineNumber);
            subFileName = subassemblyCutx;

            var bookset = SpreadHelper.GetProductSubassemblyBookSet(mvProduct.GetProductCutxFileName(),
                globalGvfx, cutPartsCtpx, hardwareHwrx, doorstyleDsvx, edgeEdgx, subassemblyCutx);

            PromptsViewModel viewmodel = new PromptsViewModel(bookset,
                product.Tab.Name, product.Tab.VarX, product.Tab.VarZ, product.Tab.VarY,
                product.Tab.VarElevation, library, "S", project.JobPath, mvProduct.Handle);

            PromptWindow prompt = new PromptWindow();
            prompt.ViewModel = viewmodel;
            prompt.ShowDialog();

            mvProduct.Comments = viewmodel.Comments;

            product.Tab.VarElevation = prompt.ViewModel.Elevation;//更新离地高度

            //把组件的长宽高再保存回产品
            bookset.Workbooks["L"].Worksheets["Subassemblies"].Cells[lineNumber, 18].Value
                = bookset.Workbooks["S"].Worksheets["Prompts"].Cells[0, 1].Value;
            bookset.Workbooks["L"].Worksheets["Subassemblies"].Cells[lineNumber, 19].Value
                = bookset.Workbooks["S"].Worksheets["Prompts"].Cells[1, 1].Value;
            bookset.Workbooks["L"].Worksheets["Subassemblies"].Cells[lineNumber, 20].Value
                = bookset.Workbooks["S"].Worksheets["Prompts"].Cells[2, 1].Value;

            return viewmodel.BookSet;
        }

        private Product getSubassemblyParentFromProject(AKProduct akProduct, Project project)
        {
            var product = project.Products.Find(it => it.Handle.ToUpper() == akProduct.SubInfo.MainA.ToUpper());

            if (product == null)
            {
                throw new Exception(string.Format("Product {0} not found in this project!", akProduct.SubInfo.MainA));
            }
            else
            { return product; }
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
            return mvProduct.CombinedParts.Select(it => it.Material.Name).Distinct();
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

            var bookset = SpreadHelper.GetProductBaseicBookSet(mvProduct.GetProductCutxFileName(),
                                                               globalGvfx, cutPartsCtpx, hardwareHwrx, doorstyleDsvx, edgeEdgx);

            PromptsViewModel viewmodel = new PromptsViewModel(bookset,
                product.Tab.Name, product.Tab.VarX, product.Tab.VarZ,
                product.Tab.VarY, product.Tab.VarElevation, library, "L", project.JobPath, mvProduct.Handle);

            PromptWindow prompt = new PromptWindow();
            prompt.ViewModel = viewmodel;
            prompt.ShowDialog();

            mvProduct.Comments = viewmodel.Comments;

            product.Tab.VarElevation = prompt.ViewModel.Elevation;//更新离地高度

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
            string folderPath = getTempFolderPath(
                    Context.GetContext().AKInfo.Path);
            string tempXMLPath = Path.Combine(folderPath,
                "Delete.xml");

            foreach (var s in readIdFromDeleteXML(tempXMLPath))
            {
                var project = ProjectManager.CreateOrOpenProject(s.Path);

                if (project.HasProduct(s.Id.Replace("_", "")))
                {
                    Logger.GetLogger().Debug("Deleting product id :" + s.Id);
                    project.DeleteProduct(s.Id.Replace("_", ""));
                }
                else
                {
                    Logger.GetLogger().Error("Product not found,id :" + s.Id);
                }
            }

            File.Delete(tempXMLPath);
        }

        private IEnumerable<DelItem> readIdFromDeleteXML(string tempXMLPath)
        {
            List<DelItem> values = new List<DelItem>();

            XElement xml = XElement.Load(tempXMLPath);
            var ids = from e in xml.Elements("D")
                      select e;

            try
            {
                foreach (var x in ids)
                {
                    values.Add(new DelItem(x.Attribute("Name").Value, x.Attribute("ManufacturingFolder").Value));
                }
            }
            catch
            {
                MessageBox.Show("删除产品时发生错误，请确认当前ad版本已经为最新版本");
            }
            return values;
        }

        private struct DelItem
        {
            public DelItem(string id, string path)
            {
                this.Id = id;
                this.Path = path;
            }
            public string Id;
            public string Path;
        }
    }
}

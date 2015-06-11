using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpreadsheetGear;
using Dimeng.WoodEngine.SpreadCustomFunctions;

namespace Dimeng.WoodEngine.Spread
{
    public class SpreadHelper
    {
        public static IWorkbookSet GetProductBaseicBookSet(string productCutx, string globalGvfx, string cutpartsCtpx, string hardwareHwrx, string doorstyleDrx, string edgeEdgx)
        {
            IWorkbookSet books = Factory.GetWorkbookSet();
            IWorkbook bookL = books.Workbooks.Open(productCutx);
            bookL.FullName = "L";
            var bookG = books.Workbooks.Open(globalGvfx);
            bookG.FullName = "G";
            var bookM = books.Workbooks.Open(cutpartsCtpx);
            bookM.FullName = "M";
            var bookE = books.Workbooks.Open(edgeEdgx);
            bookE.FullName = "E";
            var bookH = books.Workbooks.Open(hardwareHwrx);
            bookH.FullName = "H";
            var bookD = books.Workbooks.Open(doorstyleDrx);
            bookD.FullName = "D";

            loadCustomFunctions(books);

            return books;
        }

        public static IWorkbookSet GetProductSubassemblyBookSet(string productCutx, string globalGvfx, string cutpartsCtpx, string hardwareHwrx, string doorstyleDrx, string edgeEdgx, string subCutx)
        {
            IWorkbookSet books = Factory.GetWorkbookSet();
            IWorkbook bookL = books.Workbooks.Open(productCutx);
            bookL.FullName = "L";
            var bookG = books.Workbooks.Open(globalGvfx);
            bookG.FullName = "G";
            var bookM = books.Workbooks.Open(cutpartsCtpx);
            bookM.FullName = "M";
            var bookE = books.Workbooks.Open(edgeEdgx);
            bookE.FullName = "E";
            var bookH = books.Workbooks.Open(hardwareHwrx);
            bookH.FullName = "H";
            var bookD = books.Workbooks.Open(doorstyleDrx);
            bookD.FullName = "D";
            var bookS = books.Workbooks.Open(subCutx);
            bookS.FullName = "S";

            loadCustomFunctions(books);

            return books;
        }

        private static void loadCustomFunctions(IWorkbookSet books)
        {
            books.Add(GetEQ1.geteq1);
            books.Add(GetEQ2.geteq2);
            books.Add(GetEQH.geteqh);
            books.Add(GetEQV.geteqv);
            books.Add(Partlength.PL);
            books.Add(Partwidth.pw);
            books.Add(PartQuantity.pq);
            books.Add(PARENTNAME.pname);
            books.Add(PartThickness.PT);
            books.Add(PartThicknessM.PTM);
            books.Add(Radians.radians);
            books.Add(Degrees.degrees);
            //books.Add(fABS.fABS);
            //books.Add(fACOS.fACOS);
            //books.Add(fAnd.fAnd);
            //books.Add(fASin.fASin);
            //books.Add(fAtan.fAtan);
            books.Add(PNT.pnt);
            books.Add(Vectors.vectors);
            books.Add(GETBULGE.getbulge);
            books.Add(GETBULGEA.getbulgea);
            books.Add(GETARCALTITUDE.getarcaltitude);
            books.Add(GETARCANGLE.getarcangle);
            books.Add(GETARCCHORD.getarcchord);
            books.Add(GETARCLENGTH.getarclength);
            books.Add(GETARCRADIUS.getarcradius);
            books.Add(RANGETOSTRING.rangetostring);
            books.Add(MyTitle.myTitle);
        }
    }
}

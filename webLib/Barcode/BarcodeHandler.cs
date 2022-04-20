using eVida.Web.Controls;
using eVida.Web.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace eVida.Web.Barcode {
	class BarcodeHandler : IHttpHandler, IReadOnlySessionState, IRequiresSessionState {

		public bool IsReusable {
			get { return false; }
		}

		public void ProcessRequest(HttpContext context) {
			string strCode = context.Request["CODE"];
			IUsuarioLogado usuario = PageHelper.GetUsuarioLogado(context.Session);

			if (string.IsNullOrEmpty(strCode)) {
				context.Response.Write("Parâmetros inválidos!");
				return;
			}

			Processar(strCode, context.Response);
		}
		public void Processar(string code, HttpResponse response) {
			
			// Multiply the lenght of the code by 40 (just to have enough width)
			int w = code.Length * 50;

			// Create a bitmap object of the width that we calculated and height of 100
			Bitmap oBitmap = new Bitmap(w, 120);

			// then create a Graphic object for the bitmap we just created.
			Graphics oGraphics = Graphics.FromImage(oBitmap);

			// Now create a Font object for the Barcode Font
			// (in this case the IDAutomationHC39M) of 18 point size
			Font oFont = new Font("IDAutomationHC39M", 22);

			// Let's create the Point and Brushes for the barcode
			PointF oPoint = new PointF(2f, 2f);
			SolidBrush oBrushWrite = new SolidBrush(Color.Black);
			SolidBrush oBrush = new SolidBrush(Color.White);

			// Now lets create the actual barcode image
			// with a rectangle filled with white color
			oGraphics.FillRectangle(oBrush, 0, 0, w, 120);

			// We have to put prefix and sufix of an asterisk (*),
			// in order to be a valid barcode
			oGraphics.DrawString("*" + code + "*", oFont, oBrushWrite, oPoint);

			// Then we send the Graphics with the actual barcode
			response.ContentType = "image/jpeg";
			oBitmap.Save(response.OutputStream, ImageFormat.Jpeg);
		}

	}
}

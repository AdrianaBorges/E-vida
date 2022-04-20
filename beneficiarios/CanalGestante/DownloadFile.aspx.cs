using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.CanalGestante {
	public partial class DownloadFile : System.Web.UI.Page {
		protected void Page_Load(object sender, EventArgs e) {
			Response.Clear();
			if (string.IsNullOrEmpty(Request["FID"])) {
				Response.Write("Parâmetro FID inválido!");
				return;
			}

			UsuarioCanalGestanteVO usuario = PageHelper.GetUsuarioLogado(Session) as UsuarioCanalGestanteVO;
			if (usuario == null) {
				Response.Write("Sessão expirada! Tente novamente!");
				return;
			}

			string fid = Request["FID"];
			if (CanalGestanteBenefVO.CARTA_INFO.Equals(fid)) {
				WriteCartaInfo(usuario);
			} else if (CanalGestanteBenefVO.CARTAO_GES.Equals(fid)) {
				WriteCartaoGestante(usuario);
			} else if (CanalGestanteBenefVO.PARTOGRAMA.Equals(fid)) {
				WritePartograma(usuario);
			} else if ("PROTOCOLO".Equals(fid)) {
				WriteProtocolo(usuario);
			} else {
				Response.Write("Parâmetro FID inválido! " + fid);
				return;
			}
		}

		private void WriteCartaInfo(UsuarioCanalGestanteVO usuario) {
			string appPath = Request.PhysicalApplicationPath;
			string path = System.IO.Path.Combine(appPath, "CanalGestante", "CartaInformacao.pdf");
            CanalGestanteBO.Instance.MarcarDownload(usuario.Usuario.Codint, usuario.Usuario.Codemp, usuario.Usuario.Matric, usuario.Usuario.Tipreg, CanalGestanteBenefVO.CARTA_INFO);
			WriteFile(path, "CartaInformacao.pdf");
		}

		private void WriteCartaoGestante(UsuarioCanalGestanteVO usuario) {
			string appPath = Request.PhysicalApplicationPath;
			string path = System.IO.Path.Combine(appPath, "CanalGestante", "CartaoGestante.pdf");
            CanalGestanteBO.Instance.MarcarDownload(usuario.Usuario.Codint, usuario.Usuario.Codemp, usuario.Usuario.Matric, usuario.Usuario.Tipreg, CanalGestanteBenefVO.CARTAO_GES);
			WriteFile(path, "CartaoGestante.pdf");
		}

		private void WritePartograma(UsuarioCanalGestanteVO usuario) {
			string appPath = Request.PhysicalApplicationPath;
			string path = System.IO.Path.Combine(appPath, "CanalGestante", "Partograma.pdf");
            CanalGestanteBO.Instance.MarcarDownload(usuario.Usuario.Codint, usuario.Usuario.Codemp, usuario.Usuario.Matric, usuario.Usuario.Tipreg, CanalGestanteBenefVO.PARTOGRAMA);
			WriteFile(path, "Partograma.pdf");
		}

		private void WriteProtocolo(UsuarioCanalGestanteVO usuario) {
			string pid = Request["PID"];
			if (string.IsNullOrEmpty(pid)) {
				Response.Write("PID inválido!");
				return;
			}
			int id;
			if (!Int32.TryParse(pid, out id)) {
				Response.Write("PID inválido! " + pid);
				return;
			}

			CanalGestanteVO vo = CanalGestanteBO.Instance.GetById(id);
			if (vo == null) {
				Response.Write("Protocolo inexistente " + id);
				return;
			}

            if (vo.Codint.Trim() != usuario.Usuario.Codint.Trim() || vo.Codemp.Trim() != usuario.Usuario.Codemp.Trim() || vo.Matric.Trim() != usuario.Usuario.Matric.Trim() || vo.Tipreg.Trim() != usuario.Usuario.Tipreg.Trim())
            {
				Response.Write("Protocolo inexistente!!!");
				return;
			}
			string nome;
			string path = CanalGestanteBO.Instance.GetFilePath(vo, out nome);
			if (!System.IO.File.Exists(path)) {
				Response.Write("Arquivo inexistente!!!");
				return;
			}
			WriteFile(path, nome);
		}

		private void WriteFile(string path, string nome) {
			string contentType = MimeMapping.GetMimeMapping(nome);
			Response.BufferOutput = true;
			Response.Clear();
			Response.AddHeader("content-disposition", "attachment;filename=\"" + nome + "\"");
			Response.ContentType = contentType;
			Response.WriteFile(path);
		}
	}
}
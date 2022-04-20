using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVida.Web.Upload {
	public class DownloadHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState {
		/// <summary>
		/// You will need to configure this handler in the Web.config file of your 
		/// web and register it with IIS before being able to use it. For more information
		/// see the following link: http://go.microsoft.com/?linkid=8101007
		/// </summary>
		#region IHttpHandler Members

		public bool IsReusable {
			// Return false in case your Managed Handler cannot be reused for another request.
			// Usually this would be false in case you have some state information preserved per request.
			get { return true; }
		}

		public void ProcessRequest(HttpContext context) {
			string strTipo = context.Request["TIPO"];
			string id = context.Request["ID"];
			string strInline = context.Request["INLINE"];
			IUsuarioLogado usuario = PageHelper.GetUsuarioLogado(context.Session);

			if (string.IsNullOrEmpty(strTipo) || string.IsNullOrEmpty(id)) {
				context.Response.Write("Parâmetros inválidos!");
			}
			bool inline = !("N".Equals(strInline));

			FileUtil.FileDir tipo = (FileUtil.FileDir)Enum.Parse(typeof(FileUtil.FileDir), strTipo);
			
			WriteFile(id, usuario, tipo, context.Response, inline);
		}

		#endregion

        private bool CheckUsuarioBeneficiario(IUsuarioLogado usuario, string codint, string codemp, string matric, HttpResponse response)
        {
			UsuarioBeneficiarioVO uVO = usuario as UsuarioBeneficiarioVO;
			if (uVO != null) {
                if (uVO.Codint.Trim() != codint.Trim() || uVO.Codemp.Trim() != codemp.Trim())
                {
					response.Write("Acesso não permitido a este arquivo!");
					return false;
				}
			}
			return true;
		}

        private bool CheckUsuarioBeneficiario(IUsuarioLogado usuario, PUsuarioVO titular, HttpResponse response)
        {
            UsuarioBeneficiarioVO uVO = usuario as UsuarioBeneficiarioVO;
            if (uVO != null)
            {
                if (uVO.Codint != titular.Codint || uVO.Codemp != titular.Codemp || uVO.Matric != titular.Matric)
                {
                    response.Write("Acesso não permitido a este arquivo!");
                    return false;
                }
            }
            return true;
        }

		private bool CheckUsuarioCredenciado(IUsuarioLogado usuario, PRedeAtendimentoVO credenciado, HttpResponse response) {
			UsuarioCredenciadoVO uVO = usuario as UsuarioCredenciadoVO;
			if (uVO != null) {
				if (uVO.RedeAtendimento.Codigo != credenciado.Codigo) {
					response.Write("Acesso não permitido a este arquivo!");
					return false;
				}
			}
			return true;
		}

		private void WriteFile(string id, IUsuarioLogado usuario, FileUtil.FileDir tipo, HttpResponse response, bool inline) {
			bool ok = false;
			switch (tipo) {
				case FileUtil.FileDir.DECLARACAO_UNIVERSITARIO:
					ok = WriteFileUniversitario(id, usuario, response);
					break;
				case FileUtil.FileDir.AUTORIZACAO:
					ok = WriteFileAutorizacao(id, usuario, response);
					break;
				case FileUtil.FileDir.ASSINATURA:
					ok = WriteFileAssinatura(id, usuario, response);
					break;
				case FileUtil.FileDir.REUNIAO:
					ok = WriteFileReuniao(id, usuario, response);
					break;
				case FileUtil.FileDir.CONSELHO:
					ok = WriteFileConselho(id, usuario, response);
					break;
				case FileUtil.FileDir.INDISPONIBILIDADE_REDE:
					ok = WriteFileIndisponibilidade(id, usuario, response, inline);
					break;
				case FileUtil.FileDir.SOLICITACAO_VIAGEM:
					ok = WriteFileViagem(id, usuario, response);
					break;
				case FileUtil.FileDir.MENSALIDADE_IR:
					ok = WriteFileMensalidadeIr(id, usuario, response);
					break;
				case FileUtil.FileDir.MEDICAMENTO_REEMBOLSAVEL:
					ok = WriteFileMedicamentoReembolsavel(id, usuario, response);
					break;
                case FileUtil.FileDir.BOLETIM_OCORRENCIA:
                    ok = WriteFileBoletimOcorrencia(id, usuario, response, inline);
                    break;
			}
			if (!ok)
				response.Write("Parametros invalidos");
		}

		private void WriteFileResponse(string nome, string fileLocation, HttpResponse response, bool inline) {
			//log.Debug("FileLocation: " + fileLocation);

			if (!System.IO.File.Exists(fileLocation)) {
				response.Write("Arquivo físico não encontrato. Contate suporte!");
				return;
			}

			string attachment = "";
			if (!inline)
				attachment = "attachment;";
			else {
				string contentType = MimeMapping.GetMimeMapping(nome);
				response.ContentType = contentType;
			}

			response.AddHeader("content-disposition", attachment + "filename=\"" + nome + "\"");

			response.WriteFile(fileLocation);
		}

		private bool WriteFileUniversitario(string id, IUsuarioLogado usuario, HttpResponse response) {
			DeclaracaoUniversitarioVO vo = DeclaracaoUniversitarioBO.Instance.GetById(Int32.Parse(id));

			if (!CheckUsuarioBeneficiario(usuario, vo.Codint, vo.Codemp, vo.Matric, response)) {
				return false;
			}

			response.AddHeader("content-disposition", "attachment;filename=\"" + vo.NomeArquivo + "\"");
			string dir = FileUtil.GetRepositoryDir(FileUtil.FileDir.DECLARACAO_UNIVERSITARIO);
			response.WriteFile(System.IO.Path.Combine(dir, vo.NomeArquivo));
			return true;
		}

		private bool WriteFileAutorizacao(string id, IUsuarioLogado usuario, HttpResponse response) {
			string[] parameters = id.Split(';');
			if (parameters.Length != 2) {
				return false;
			}
			int idAutorizacao = 0;
			string fileName = parameters[1];

			if (!Int32.TryParse(parameters[0], out idAutorizacao)) {
				return false;
			}
			if (string.IsNullOrEmpty(fileName)) {
				return false;
			}

			AutorizacaoVO vo = AutorizacaoBO.Instance.GetById(idAutorizacao);
			if (vo == null)
				return false;

			if (!CheckUsuarioBeneficiario(usuario, vo.UsuarioTitular, response)) {
				return false;
			}
			if (!CheckUsuarioCredenciado(usuario, vo.RedeAtendimento, response)) {
				return false;
			}
			List<AutorizacaoArquivoVO> lstArquivos = AutorizacaoBO.Instance.ListArquivos(idAutorizacao);

			if (lstArquivos == null || lstArquivos.Count == 0)
				return false;


			response.AddHeader("content-disposition", "attachment;filename=\"" + fileName + "\"");
			string path = FileUtil.GetFilePath(FileUtil.FileDir.AUTORIZACAO, null, vo.Id.ToString(), fileName);
			response.WriteFile(path);
			return true;
		}

		private bool WriteFileAssinatura(string id, IUsuarioLogado usuario, HttpResponse response) {
			usuario = usuario as UsuarioIntranetVO;
			if (usuario == null) {
				response.Write("Sistema inválido para assinatura!");
				return false;
			}
			string loginUsuario = string.Empty;
			loginUsuario = id.FromBase64String();
			if (string.IsNullOrEmpty(loginUsuario)) {
				response.Write("Usuário inválido para assinatura! [" + id + "]");
				return false;
			}
			UsuarioVO vo = UsuarioBO.Instance.GetUsuarioByLogin(loginUsuario);
			if (vo == null) {
				response.Write("Usuário não encontrado! [" + loginUsuario + "]");
				return false;				
			}
			string dir = FileUtil.GetRepositoryDir(FileUtil.FileDir.ASSINATURA);
			string fileName = FileUtil.GetFileFromPathWithoutExtension(FileUtil.FileDir.ASSINATURA, vo.Id.ToString(), "ass");
			if (string.IsNullOrEmpty(fileName)) {
				response.Write("Assinatura de usuário não encontrada!");
				return true;
			}

			string contentType = MimeMapping.GetMimeMapping(fileName);
			response.ContentType = contentType;
			//response.AddHeader("content-disposition", "filename=\"" + System.IO.Path.GetFileName(fileName) + "\"");
			response.WriteFile(fileName);
			return true;
		}

		private bool WriteFileReuniao(string id, IUsuarioLogado usuario, HttpResponse response) {
			usuario = usuario as UsuarioIntranetVO;
			if (usuario == null) {
				response.Write("Sistema inválido!");
				return false;
			}

			string[] parameters = id.Split(';');
			if (parameters.Length != 2) {
				return false;
			}
			int idReuniao = 0;
			int fileId = 0;

			if (!Int32.TryParse(parameters[0], out idReuniao)) {
				return false;
			}
			if (!Int32.TryParse(parameters[1], out fileId)) {
				return false;
			}

			ReuniaoVO vo = ReuniaoBO.Instance.GetById(idReuniao);
			if (vo == null)
				return false;

			UsuarioIntranetVO usuarioIntranet = (UsuarioIntranetVO)usuario;
			if (!usuarioIntranet.HasPermission(Modulo.VISUALIZAR_REUNIAO)) {
				response.Write("Você não possui permissão de visualizar reuniões!");
				return false;
			}
			/*if (!usuarioIntranet.HasPermission(Modulo.GERENCIAR_REUNIAO)) {
				ConselhoVO conselho = ConselhoBO.Instance.GetConselhoByUsuario(usuarioIntranet.Id);
				if (conselho == null || !vo.CodConselho.Equals(conselho.Codigo)) {
					response.Write("Você não pertence ao conselho da reunião!");
					return false;
				}
			}*/

			List<ArquivoReuniaoVO> lstArquivos = ReuniaoBO.Instance.ListarArquivosByReuniao(idReuniao);

			if (lstArquivos == null || lstArquivos.Count == 0)
				return false;

			ArquivoReuniaoVO arqVO = lstArquivos.FirstOrDefault(x =>
				x.IdArquivo == fileId);
			if (arqVO == null)
				return false;

			response.AddHeader("content-disposition", "attachment;filename=\"" + arqVO.NomeArquivo + "\"");
			string path = FileUtil.GetFilePath(FileUtil.FileDir.REUNIAO, null, vo.Id.ToString(), arqVO.NomeArquivo);
			response.WriteFile(path);
			return true;
		}

		private bool WriteFileConselho(string id, IUsuarioLogado usuario, HttpResponse response) {
			usuario = usuario as UsuarioIntranetVO;
			if (usuario == null) {
				response.Write("Sistema inválido!");
				return false;
			}

			string[] parameters = id.Split(';');
			if (parameters.Length != 2) {
				return false;
			}
			string idConselho = string.Empty;
			int fileId = 0;

			idConselho = parameters[0];

			if (!Int32.TryParse(parameters[1], out fileId)) {
				return false;
			}

			ConselhoVO vo = ConselhoBO.Instance.GetConselhoByCodigo(idConselho);
			if (vo == null)
				return false;

			UsuarioIntranetVO usuarioIntranet = (UsuarioIntranetVO)usuario;
			if (!usuarioIntranet.HasPermission(Modulo.VISUALIZAR_REUNIAO)) {
				response.Write("Você não possui permissão de visualizar reuniões!");
				return false;
			}
			/*if (!usuarioIntranet.HasPermission(Modulo.GERENCIAR_REUNIAO)) {
				ConselhoVO conselho = ConselhoBO.Instance.GetConselhoByUsuario(usuarioIntranet.Id);
				if (conselho == null || !vo.Codigo.Equals(conselho.Codigo)) {
					response.Write("Você não pertence ao conselho!");
					return false;
				}
			}*/

			List<ArquivoConselhoVO> lstArquivos = ConselhoBO.Instance.ListarArquivosConselho(idConselho);

			if (lstArquivos == null || lstArquivos.Count == 0)
				return false;

			ArquivoConselhoVO arqVO = lstArquivos.FirstOrDefault(x =>
				x.IdArquivo == fileId);
			if (arqVO == null)
				return false;

			response.AddHeader("content-disposition", "attachment;filename=\"" + arqVO.NomeArquivo + "\"");
			string path = FileUtil.GetFilePath(FileUtil.FileDir.CONSELHO, null, vo.Codigo.ToString(), arqVO.NomeArquivo);
			response.WriteFile(path);
			return true;
		}

		private bool WriteFileIndisponibilidade(string id, IUsuarioLogado usuario, HttpResponse response, bool inline) {
			
			string[] parameters = id.Split(';');
			if (parameters.Length != 2) {
				return false;
			}
			int idIndisponibilidade = 0;
			int fileId = 0;

			if (!Int32.TryParse(parameters[0], out idIndisponibilidade)) {
				return false;
			}
			if (!Int32.TryParse(parameters[1], out fileId)) {
				return false;
			}

			IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(idIndisponibilidade);
			if (vo == null)
				return false;


			if (!CheckUsuarioBeneficiario(usuario, vo.Usuario, response)) {
				return false;
			}

			List<IndisponibilidadeRedeArquivoVO> lstArquivos = IndisponibilidadeRedeBO.Instance.ListarArquivos(idIndisponibilidade);

			if (lstArquivos == null || lstArquivos.Count == 0)
				return false;

			IndisponibilidadeRedeArquivoVO arqVO = lstArquivos.FirstOrDefault(x =>
				x.IdArquivo == fileId);
			if (arqVO == null)
				return false;

			string path = FileUtil.GetFilePath(FileUtil.FileDir.INDISPONIBILIDADE_REDE, null, vo.Id.ToString(), arqVO.NomeArquivo);
			WriteFileResponse(arqVO.NomeArquivo, path, response, inline);
			//response.AddHeader("content-disposition", "attachment;filename=\"" + arqVO.NomeArquivo + "\"");
			//response.WriteFile(path);
			return true;
		}

		private bool WriteFileViagem(string id, IUsuarioLogado usuario, HttpResponse response) {

			string[] parameters = id.Split(';');
			if (parameters.Length != 3) {
				return false;
			}
			int idViagem = 0;
			int idTipo = 0;
			int fileId = 0;

			if (!Int32.TryParse(parameters[0], out idViagem)) {
				return false;
			}
			if (!Int32.TryParse(parameters[1], out idTipo)) {
				return false;
			}
			if (!Int32.TryParse(parameters[2], out fileId)) {
				return false;
			}


			SolicitacaoViagemVO vo = ViagemBO.Instance.GetById(idViagem);
			if (vo == null)
				return false;

			UsuarioIntranetVO usuarioVO = (UsuarioIntranetVO)usuario;
			if (vo.CodUsuarioSolicitante != usuarioVO.Id) {
				if (!usuarioVO.HasPermission(Modulo.VIAGEM_DIRETORIA) &&
					!usuarioVO.HasPermission(Modulo.VIAGEM_FINANCEIRO) &&
					!usuarioVO.HasPermission(Modulo.VIAGEM_SECRETARIA) &&
					!usuarioVO.HasPermission(Modulo.VIAGEM_COORDENADOR)) {
					response.Write("Você não possui permissão de visualizar este arquivo!");
				}
			}

			List<SolicitacaoViagemArquivoVO> lstArquivos = ViagemBO.Instance.ListarArquivos(idViagem);

			if (lstArquivos == null || lstArquivos.Count == 0)
				return false;

			TipoArquivoViagem tipo = (TipoArquivoViagem)idTipo;
			SolicitacaoViagemArquivoVO arqVO = lstArquivos.FirstOrDefault(x => x.TipoArquivo == tipo && x.IdArquivo == fileId);
			if (arqVO == null)
				return false;

			response.AddHeader("content-disposition", "attachment;filename=\"" + arqVO.NomeArquivo + "\"");
			string path = FileUtil.GetFilePath(FileUtil.FileDir.SOLICITACAO_VIAGEM, vo.Id.ToString(), ViagemBO.Instance.GetFileDiskId(arqVO), arqVO.NomeArquivo);
			response.WriteFile(path);
			return true;
		}

		private bool WriteFileMensalidadeIr(string id, IUsuarioLogado usuario, HttpResponse response) {
			UsuarioBeneficiarioVO benefVO = usuario as UsuarioBeneficiarioVO;
			string cartao = null;
			if (benefVO != null) {
				cartao = benefVO.Usuario.Matant;
			} else {
				string[] parameters = id.Split(';');
				if (parameters.Length != 2) {
					return false;
				}
				cartao = parameters[1];
				id = parameters[0];
			}

			int ano = 0;

			if (!Int32.TryParse(id, out ano)) {
				return false;
			}

			string fileName = "MENSALIDADE_COPARTICIPACAO_" + ano + "_" + cartao + ".pdf";

			response.AddHeader("content-disposition", "attachment;filename=\"" + fileName + "\"");

			string path = ExtratoIrBeneficiarioBO.Instance.RelatorioMensalidadeFile(cartao, ano);
			response.WriteFile(path);
			return true;
		}

		private bool WriteFileMedicamentoReembolsavel(string id, IUsuarioLogado usuario, HttpResponse response) {

			string[] parameters = id.Split(';');
			if (parameters.Length != 2) {
				return false;
			}
			string cdMascara = string.Empty;
			int fileId = 0;

			cdMascara = parameters[0];
			if (!Int32.TryParse(parameters[1], out fileId)) {
				return false;
			}

			MedicamentoReembolsavelVO vo = MedicamentoReembolsavelBO.Instance.GetById(cdMascara);
			if (vo == null)
				return false;

			UsuarioIntranetVO usuarioVO = (UsuarioIntranetVO)usuario;
			if (!usuarioVO.HasPermission(Modulo.GESTAO_MEDICAMENTO_REEMBOLSAVEL)) {
				response.Write("Você não possui permissão de visualizar arquivos de medicamentos!");
			}

			List<MedicamentoReembolsavelArqVO> lstArquivos = MedicamentoReembolsavelBO.Instance.ListarArquivos(cdMascara);

			if (lstArquivos == null || lstArquivos.Count == 0)
				return false;

			MedicamentoReembolsavelArqVO arqVO = lstArquivos.FirstOrDefault(x => x.IdArquivo == fileId);
			if (arqVO == null)
				return false;

			string idPasta = cdMascara;

			response.AddHeader("content-disposition", "attachment;filename=\"" + arqVO.NomeArquivo + "\"");
			string path = FileUtil.GetFilePath(FileUtil.FileDir.MEDICAMENTO_REEMBOLSAVEL, idPasta, MedicamentoReembolsavelBO.Instance.GetFileDiskId(arqVO), arqVO.NomeArquivo);
			response.WriteFile(path);
			return true;
		}

        private bool WriteFileBoletimOcorrencia(string id, IUsuarioLogado usuario, HttpResponse response, bool inline)
        {
            string[] parameters = id.Split(';');
            if (parameters.Length != 2)
            {
                return false;
            }
            int idSegViaCarteira = 0;
            int fileId = 0;

            if (!Int32.TryParse(parameters[0], out idSegViaCarteira))
            {
                return false;
            }
            if (!Int32.TryParse(parameters[1], out fileId))
            {
                return false;
            }

            SolicitacaoSegViaCarteiraVO vo = SegViaCarteiraBO.Instance.GetById(idSegViaCarteira);
            if (vo == null)
                return false;

            List<SolicitacaoSegViaCarteiraArquivoVO> lstArquivos = SegViaCarteiraBO.Instance.ListarArquivos(idSegViaCarteira);

            if (lstArquivos == null || lstArquivos.Count == 0)
                return false;

            SolicitacaoSegViaCarteiraArquivoVO arqVO = lstArquivos.FirstOrDefault(x =>
                x.IdArquivo == fileId);
            if (arqVO == null)
                return false;

            string path = FileUtil.GetFilePath(FileUtil.FileDir.BOLETIM_OCORRENCIA, null, vo.CdSolicitacao.ToString(), arqVO.NomeArquivo);
            WriteFileResponse(arqVO.NomeArquivo, path, response, inline);
            //response.AddHeader("content-disposition", "attachment;filename=\"" + arqVO.NomeArquivo + "\"");
            //response.WriteFile(path);
            return true;
        }

	}
}

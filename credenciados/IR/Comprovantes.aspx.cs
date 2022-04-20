using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaCredenciados.Classes;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;

namespace eVidaCredenciados.IR {
	public partial class Comprovantes : PageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string downloadFile = Request["DW_FILE"];
				string tipoPessoa = UsuarioLogado.RedeAtendimento.Tippe;
				string dir = GetIrDir();

				if (string.IsNullOrEmpty(downloadFile)) {

					if (tipoPessoa.Equals(PConstantes.PESSOA_JURIDICA)) {
						ConfigPessoaJuridica(dir);
					} else {
						ConfigPessoaFisica(dir);
					}
				} else {
					int tipo = Int32.Parse(downloadFile);
					WriteFileIr(dir, tipoPessoa);
				}
			}
		}

		private void ConfigPessoaJuridica(string dir) {
			string cnpj = FormatUtil.UnformatCnpj(FormatUtil.FormatCnpj(UsuarioLogado.RedeAtendimento.Cpfcgc.ToString()));

			string fullName = GetRealFileName(dir, cnpj);
			bool ok = !string.IsNullOrEmpty(fullName);
			lnkIr.Enabled = false;
			lnkIr.Enabled = ok;

			if (!lnkIr.Enabled) {
				lnkIr.Text = "Não disponível";
				lnkIr.OnClientClick = "return false;";
			}
		}

		private void ConfigPessoaFisica(string dir) {
			string file = GetPrefixFileNamePF();

			string fullName = GetRealFileName(dir, file);
			bool ok = !string.IsNullOrEmpty(fullName);
			lnkIr.Enabled = ok;

			if (!lnkIr.Enabled) {
				lnkIr.Text = "Não disponível";
				lnkIr.OnClientClick = "return false;";
			}
		}

		private string GetPrefixFileNamePJ() {
			string cnpj = FormatUtil.UnformatCnpj(FormatUtil.FormatCnpj(UsuarioLogado.RedeAtendimento.Cpfcgc.ToString()));
			return cnpj;
		}

		private string GetPrefixFileNamePF() {
            string cpf = FormatUtil.UnformatCpf(FormatUtil.FormatCpf(UsuarioLogado.RedeAtendimento.Cpfcgc));
			return cpf;
		}

		private string GetRealFileName(string dir, string filePrefix) {
			dir = System.IO.Path.GetFullPath(dir);
			IEnumerable<string> files = System.IO.Directory.EnumerateFiles(dir, filePrefix + ".*");
			if (files.Count() == 0) return null;
			foreach (string file in files) {
				if (file.EndsWith("pdf", StringComparison.InvariantCultureIgnoreCase)) {
					return file;
				}
			}
			return files.First();
		}

		private string GetIrDir() {
			string dir = ParametroUtil.IrFolderCredenciado;
			dir = System.IO.Path.Combine(dir, (DateTime.Now.Year - 1).ToString());
			return dir;
		}

		private void WriteFileIr(string dir, string tipoPessoa) {
			string file = null;
			
			if (tipoPessoa.Equals(PConstantes.PESSOA_JURIDICA)) {
				file = GetPrefixFileNamePJ();
			} else {
				file = GetPrefixFileNamePF();
			}

			string fullName = GetRealFileName(dir, file);
			Response.Clear();
			if (System.IO.File.Exists(fullName)) {
				string name = System.IO.Path.GetFileName(fullName);
				Response.AddHeader("content-disposition", "attachment;filename=\"" + name + "\"");
				Response.WriteFile(fullName);
			} else {
				Response.Write("Arquivo inválido!");
			}
			Response.End();
		}

	}
}
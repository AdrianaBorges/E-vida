using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.FormsPop {
	public partial class PopCancelGeneric : PopUpPageBase {
		public enum CancelObject {
			CartaPositiva = 1
		}

		struct ResMsg {
			public string TitleText { get; set; }
			public string ButtonText { get; set; }
			public string QuestionText { get; set; }
			public string PromptText { get; set; }
			public string IdText { get; set; }
			public Modulo Modulo { get; set; }
		}

		[Serializable]
		struct ObjectId {
			public string Id { get; set; }
			public CancelObject Type { get; set; }
		}

		protected override void PageLoad(object sender, EventArgs e) {
			CancelObject type;

			string id = Request["ID"];
			string reqType = Request["TYPE"];

			if (string.IsNullOrEmpty(reqType)) {
				ShowOpenError("Tipo de requisição inválido!");
				return;
			}
			if (string.IsNullOrEmpty(id)) {
				this.ShowOpenError("O ID de requisição está ausente!");
				return;
			}

			if (!Enum.TryParse<CancelObject>(reqType, out type)) {
				ShowOpenError("Tipo de requisição - " + reqType + " - inválido!");
				return;
			}

			ObjectId objId = new ObjectId();
			objId.Type = type;
			objId.Id = id;

			Id = objId;
			ResMsg msg = GetResource(objId);
			litQuestion.Text = msg.QuestionText;
			btnSalvar.Text = msg.ButtonText;
			litTitle.Text = msg.TitleText;

			if (!HasPermission(msg.Modulo)){
				ShowAcessoNegado();
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		ObjectId Id { get { return (ObjectId)ViewState["ID"]; } set { ViewState["ID"] = value; } }

		private void ShowOpenError(string msg) {
			this.ShowError(msg);
			this.btnSalvar.Visible = false;
			return;
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCancelamento.Text)) {
				this.ShowError("Informe o motivo de cancelamento!");
				return;
			}
			try {
				if (DoCancel())
					CallDefaultCallback(Id.Id);
			} catch (Exception ex) {
				this.ShowError("Erro ao executar operação!", ex);
			}
		}

		private ResMsg GetResource(ObjectId id) {
			ResMsg msg = new ResMsg();
			msg.PromptText = "Informe o motivo de cancelamento abaixo";
			msg.ButtonText = "Cancelar solicitação";
			msg.QuestionText = "Deseja realmente cancelar o formulário de protocolo " + id.Id;

			switch (id.Type) {
				case CancelObject.CartaPositiva:
					msg.TitleText = "Carta Positiva CRA";
					msg.Modulo = eVidaGeneralLib.VO.Modulo.CARTA_POSITIVA_CRA_APROVAR;
					break;
				default:
					break;
			}

			return msg;
		}

		private bool DoCancel() {
			int id = Int32.Parse(Id.Id);
			CartaPositivaCraVO vo = CartaPositivaCraBO.Instance.GetById(id);
			if (vo.Status == CartaPositivaCraStatus.CANCELADO) {
				return true;
			}

			vo.MotivoCancelamento = txtCancelamento.Text;
			CartaPositivaCraBO.Instance.CancelarSolicitacao(vo.Id, txtCancelamento.Text, UsuarioLogado.Usuario);
			return true;
		}
	}
}
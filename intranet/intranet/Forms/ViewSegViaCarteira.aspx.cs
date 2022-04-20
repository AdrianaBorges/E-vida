using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.Forms
{
    public partial class ViewSegViaCarteira : PageBase
    {

        protected override void PageLoad(object sender, EventArgs e)
        {

            try
            {
                if (!IsPostBack)
                {
                    int id = 0;
                    if (!Int32.TryParse(Request["ID"], out id))
                    {
                        this.ShowError("Solicitação inválida!");
                        return;
                    }

                    SolicitacaoSegViaCarteiraVO vo = Buscar(id);
                    DataTable dtBeneficiarios = BuscarBeneficiarios(vo);

                    dtBeneficiarios.Columns.Add("rownum", typeof(int));
                    dtBeneficiarios.Columns.Add("motivo", typeof(string));
                    dtBeneficiarios.Columns.Add("nome", typeof(string));
                    dtBeneficiarios.Columns.Add("parentesco", typeof(string));

                    int i = 0;
                    foreach (DataRow dr in dtBeneficiarios.Rows)
                    {
                        dr["rownum"] = ++i;
                        dr["NOME"] = Convert.ToString(dr["nm_beneficiario"]);
                        dr["PARENTESCO"] = Convert.ToString(dr["BRP_DESCRI"]);

                        MotivoSegVia motivo = (MotivoSegVia)Convert.ToChar(dr["TP_MOTIVO"]);
                        string dsMotivo = SolicitacaoSegViaCarteiraEnumTradutor.TraduzMotivo(motivo);
                        dr["Motivo"] = dsMotivo;
                    }

                    gdvDependentes.DataSource = dtBeneficiarios;
                    gdvDependentes.DataBind();

                    PUsuarioVO funcVO = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
                    lblData.Text = vo.Criacao.ToString("dd \\de MMMM \\de yyyy");
                    txtEmailTitular.Text = funcVO.Email;
                    txtCartao.Text = funcVO.Matant;
                    txtNomeTitular.Text = funcVO.Nomusr;
                    lblProtocolo.Text = vo.CdSolicitacao.ToString("0000000000");
                    lblProtocoloAns.Text = vo.ProtocoloAns;
                    txtLocal.Text = vo.Local;

                    // ---------------------- Arquivos ------------------------------
                    List<SolicitacaoSegViaCarteiraArquivoVO> lstArquivos = SegViaCarteiraBO.Instance.ListarArquivos(vo.CdSolicitacao);

                    if (lstArquivos != null && lstArquivos.Count > 0)
                    {
                        btnArquivo.OnClientClick = "return openDownload(" + vo.CdSolicitacao + "," + lstArquivos[0].IdArquivo + ", false)";
                    }
                    else
                    {
                        btnArquivo.Visible = false;
                    }
                    // --------------------------------------------------------------


                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
                Log.Error("Erro ao abrir tela de segunda via de carteira!", ex);
            }
        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.GESTAO_SEG_VIA; }
        }

        private static DataTable BuscarBeneficiarios(SolicitacaoSegViaCarteiraVO vo)
        {
            return SegViaCarteiraBO.Instance.BuscarBeneficiarios(vo.CdSolicitacao);
        }

        private static SolicitacaoSegViaCarteiraVO Buscar(int id)
        {
            SolicitacaoSegViaCarteiraVO vo = SegViaCarteiraBO.Instance.GetById(id);
            return vo;
        }

    }
}
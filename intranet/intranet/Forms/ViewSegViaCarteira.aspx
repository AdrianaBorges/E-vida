<%@ Page Title="Visualização da Solicitação de 2ª Via de Carteira" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ViewSegViaCarteira.aspx.cs" Inherits="eVidaIntranet.Forms.ViewSegViaCarteira" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openPdf() {
            openReport('<%= eVida.Web.Report.ReportHandler.EnumRelatorio.SEGUNDA_VIA %>', 'ID=<%= Request["ID"] %>');
    	    return false;
    	}

    	function openDownload(idSegViaCarteira, fId, isNew) {

    	    if (isNew) {
    	        alert('Arquivos Novos não podem ser visualizados. O formulário deve ser salvo para disponibilizar a visualização!');
    	        return false;
    	    }
    	    openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.BOLETIM_OCORRENCIA %>', "ID=" + idSegViaCarteira + ";" + fId);

            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo" style="margin-left: 15px">
        <b>Protocolo:</b> <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true" /><br />
        <b>Protocolo ANS: </b><asp:Label ID="lblProtocoloAns" runat="server" Font-Bold="true" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
                <td rowspan="2" valign="top">
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:Label ID="txtCartao" runat="server" Width="150px"/>
                </td>
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:Label ID="txtEmailTitular" runat="server" Width="400px" />
                </td>
            </tr>
        </table>
        <br />
        <asp:GridView ID="gdvDependentes" runat="server" Width="100%" AutoGenerateColumns="false"
            DataKeyNames="cd_beneficiario" CssClass="tabelaBorda">
            <Columns>
                <asp:BoundField HeaderText="Seq." DataField="rownum"/>
                <asp:BoundField HeaderText="Nome Beneficiário" DataField="nome" />
                <asp:BoundField HeaderText="Parentesco" DataField="parentesco" />
                <asp:BoundField HeaderText="Plano Vinculado" DataField="BI3_DESCRI" />
                <asp:BoundField HeaderText="Motivo para emissão da segunda via" DataField="motivo" />
            </Columns>
        </asp:GridView>

        <h2>2 - INFORMAÇÕES</h2>
	    <div class="observacao">
		    <p>Em casos de Roubo ou Furto o titular deve apresentar em conjunto com esta solicitação cópia do Boletim de Ocorrência Policial. Nestes casos não haverá cobrança para emissão da segunda via.</p>
	    </div>

        <h2>3 - DECLARACAO</h2>
	    <div class="observacao">
		    <p>Declaro estar ciente que pagarei pelo valor integral do custo para emissão da 2ª via de cada carteira solicitada, nos casos de Quebra ou Perda do Cartão, conforme estabelecido nos normativos dos planos da E-VIDA.</p>
            <p>Em caso de uso indevido, estou ciente que participarei integralmente nas despesas referentes à utilização dos planos a que estou vinculado.</p>
	    </div>
        
        <div>
            <br />
	        (Local e Data) <asp:Label ID="txtLocal" runat="server" Width="150px" CssClass="inputInline" />, <asp:Label ID="lblData" runat="server" />
	        <br />            
	    </div>

        <div>
            <table width="100%">
                <tr>                    
                    <td align="center"><asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClientClick="history.back(-1)" /></td>
                    <td align="center"><asp:ImageButton ID="btnPdf" ImageUrl="../img/pdf.png" runat="server" ToolTip="Gerar PDF" Width="50px" CssClass="printer" OnClientClick="openPdf()" /></td>
                    <td align="center"><asp:ImageButton ID="btnArquivo" ImageUrl="../img/anexo.png" runat="server" ToolTip="Boletim de Ocorrência" Width="50px" /></td>
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>

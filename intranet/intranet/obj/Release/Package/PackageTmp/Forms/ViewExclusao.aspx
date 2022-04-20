<%@ Page Title="Visualização da Solicitação de Exclusão de Beneficiário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ViewExclusao.aspx.cs" Inherits="eVidaIntranet.Forms.ViewExclusao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function openPdf() {
    		openReport(RELATORIO_EXCLUSAO, 'ID=<%= Request["ID"] %>');
    		return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo" style="margin-left: 15px">
        <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
                <td rowspan="2" valign="top">
                    <asp:Label ID="lblMatricula" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:Label ID="txtMatricula" runat="server" Width="150px"/>
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
        

        <h2>2 - Beneficiários para Exclusão</h2>
	    <asp:GridView ID="gdvDependentes" runat="server" Width="100%" AutoGenerateColumns="false"
            DataKeyNames="Cdusuario" OnRowDataBound="gdvDependentes_RowDataBound" >
            <AlternatingRowStyle CssClass="tbDependenteAlt" />
            <RowStyle CssClass="tbDependente" BorderWidth="1px" />
            
            <Columns>
                <asp:BoundField HeaderText="Seq." />
                <asp:BoundField HeaderText="Nome Beneficiário" DataField="Nome" />
                <asp:BoundField HeaderText="Parentesco" DataField="Parentesco" />
                <asp:BoundField HeaderText="Plano Vinculado" DataField="Plano" />
            </Columns>
        </asp:GridView>

        <h2>3 - DECLARACAO</h2>
	    <div class="observacao">
		    <p>Compete ao beneficiário titular a devolução de sua carteira de identificação no momento do seu desligamento do plano, 
				bem como a devolução das carteiras de identificação dos seus dependentes vinculados aos planos de saúde quando perderem essa condição.</p>
            <p>Em caso de uso indevido e da não devolução das carteiras de identificação, declaro que estou ciente que participarei 
				integralmente nas despesas referentes à utilização do plano de saúde.
            </p>
			<p>O desligamento do titular ou de seus dependentes não exime o beneficiário titular de quitar eventuais débitos com a 
				E-VIDA, incluindo os valores de contribuição mensal e de co-participação.
			</p>
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
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>

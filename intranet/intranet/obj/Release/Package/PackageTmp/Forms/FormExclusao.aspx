<%@ Page Title="SOLICITAÇÃO DE EXCLUSÃO DE BENEFICIÁRIOS" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormExclusao.aspx.cs" Inherits="eVidaIntranet.Forms.FormExclusao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">
    	function ConfigControlsCustom() {
    		$("#<%= tbSelecao.ClientID %> ").css({ "width": "100%" });
        	$("#<%= tbSelecao.ClientID %> td").css({ "padding": "2px" });
        }
        function goNova() {
            window.location = "FormExclusao.aspx";
            return false;
        }
        function confirmSave() {
        	return true;
        }
        function openPdf() {
        	var id = $('#<%= litProtocolo.ClientID%>').val();
        	openReport(RELATORIO_EXCLUSAO, "ID=" + id);
        	return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    <div id="conteudo">
        <asp:HiddenField ID="litProtocolo" runat="server" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
                <td rowspan="2" valign="top">
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtCartao" runat="server" Width="150px" AutoPostBack="true" OnTextChanged="txtCartao_TextChanged"/>
                </td>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtEmailTitular" runat="server" Width="400px" MaxLength="150" />
                </td>
            </tr>
        </table>
        <br />
        
        <h2>2 - Beneficiários para Exclusão</h2>
	    <table id="tbSelecao" runat="server" border="1" class="tabelaForm">
            <tr>
                <td colspan="3"><asp:DropDownList ID="dpdDependente" runat="server" DataValueField="Key" DataTextField="Value" OnSelectedIndexChanged="dpdDependente_SelectedIndexChanged" AutoPostBack="true" Width="500px" /></td>
				<td rowspan="2" style="width:300px"><asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" /></td>
            </tr>
            <tr style="height:50px">
                <td><b>PARENTESCO</b><br />
                    <asp:Label ID="txtParentesco" runat="server" Width="250px"/>
                </td>
                <td><b>PLANO</b><br />
                    <asp:Label ID="txtPlano" runat="server" Width="250px" />
                </td>
                <td><b>IDADE</b><br />
                    <asp:Label ID="txtIdade" runat="server" Width="80px" />
                </td>
            </tr>            
	    </table>
        <br />
        <asp:GridView ID="gdvDependentes" runat="server" Width="100%" AutoGenerateColumns="false" 
            OnRowDataBound="gdvDependentes_RowDataBound" OnRowCommand="gdvDependentes_RowCommand"
            DataKeyNames="Cdusuario, CodPlano" CssClass="tabela">
            <AlternatingRowStyle CssClass="tbDependenteAlt" />
            <RowStyle CssClass="tbDependente" BorderWidth="1px" />
            
            <Columns>
                <asp:TemplateField HeaderText="Seq.">
                    <ItemTemplate>
                        <asp:Label ID="lblRowNum" runat="server" />
                        <asp:ImageButton ID="lnkRemoverDependente" runat="server" ImageUrl="~/img/remove.png" CommandName="Excluir" CommandArgument='<%# Container.DataItemIndex  %>' AlternateText="Remover" ToolTip="Remover" /> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Nome Beneficiário" DataField="Nome" />
                <asp:BoundField HeaderText="Parentesco" DataField="Parentesco" />               
                <asp:BoundField HeaderText="Plano Vinculado" DataField="Plano" />
            </Columns>
        </asp:GridView>
        <asp:Button ID="btnLimparDep" runat="server" Text="Limpar seleção" OnClick="btnLimparDep_Click" />

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
            <b>Observações: </b> (incluir link da chamada J4Call) <br />
		    <asp:TextBox ID="txtObs" runat="server" Width="90%" TextMode="MultiLine" Rows="3" />
	    </div>
        <div>
            <br />
	        (Local e Data) <asp:TextBox ID="txtLocal" runat="server" Width="150px" CssClass="inputInline" />, <asp:Literal ID="ltData" runat="server" /> 
	        <br />            
	    </div>
		<div style="text-align: center">
			<br /><br /><br /><br />
			____________________________________________________________<br />
			ASSINATURA DO TITULAR
		</div>
        <div>
            <table width="100%">
                <tr>
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" OnClientClick="return confirmSave()" />
					<asp:ImageButton ID="btnPdf" runat="server" Text="Imprimir" OnClientClick="return openPdf()" Visible="false" ImageUrl="~/img/PDF.png" Height="30px" />
                    <asp:Button ID="btnNova" runat="server" Text="Nova Solicitação" OnClientClick="return goNova()" Visible="false" />
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>

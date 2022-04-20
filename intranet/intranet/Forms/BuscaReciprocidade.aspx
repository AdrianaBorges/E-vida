<%@ Page Title="Requerimentos de Reciprocidade" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaReciprocidade.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaReciprocidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">        
    	var POP_CANCELAR = 0;
    	var POP_APROVAR = 1;

    	function ConfigControlsCustom() {
    	    $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
            $('#<%= txtProtocoloANS.ClientID %>').ForceNumericOnly();
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

        	createLocator(550, 450, dlgOpen, null, defaultDlgCallback);
        }

    	function dlgOpen(handler, ev, ui) {
    		var src = "";
    		switch (handler.tipo) {
    			case POP_CANCELAR: src = 'PopCancelReciprocidade.aspx?TIPO=CANCEL'; break;
    			case POP_APROVAR: src = 'PopAprovReciprocidade.aspx?'; break;
    			case POP_NEGAR: src = 'PopAutorizacaoChanger.aspx?TIPO=NEGAR'; break;
    			case POP_SERVICO: src = '../GenPops/PopServico.aspx?SHOWT=true'; break;
    			case POP_DOC: src = 'PopAutorizacaoChanger.aspx?TIPO=SOL_DOC'; break;
    		}
    		src += '&ID=' + handler.id;
    		setLocatorUrl(src);
    	}
		
    	function openPop(tipo, obj, id, row) {
    		var handler = new LocatorHandler(tipo, id, row, obj);

    		var titulo = tipo == POP_APROVAR ? "Aprovar Reciprocidade" : "Negar Reciprocidade";

    		openLocator(titulo, handler);
    		return false;
    	}

    	function openPdf(id) {
    		openReport(RELATORIO_SOL_RECIPROCIDADE, 'ID=' + id);
		    return false;
		}

        function openCancelar(obj, id, row) {
            openPop(POP_CANCELAR, obj, id, row);
            return false;
        }
        function openAprovar(obj, id, row) {
            openPop(POP_APROVAR, obj, id, row);
            return false;
        }
		
        function openEdit(id) {
        	window.location = "FormReciprocidade.aspx?id=" + id;
        	return false;
        }

        function openView(id) {
            window.open('PopViewReciprocidade.aspx?ID=' + id, "_negativa", "width=880px, height=700px,scrollbars=yes", true);
            return false;
        }
        function goView(id) {
            window.location = 'ViewReciprocidade.aspx?ID=' + id;
            return false;
        }
        function goNova(id) {
        	window.location = 'FormNewReciprocidade.aspx';
        	return false;
        }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
<table width="100%" cellspacing="10px" class="tabelaForm">
	<tr>
		<td colspan="6"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:80px; text-align: right">Protocolo:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
        <td style="width:100px; text-align: right">Protocolo ANS:</td>
		<td align="left"><asp:TextBox ID="txtProtocoloANS" runat="server" Width="220px" MaxLength="20"  /></td>
        <td style="width:80px; text-align: right">Matrícula:</td>
        <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
    </tr>
	<tr>
		<td style="width:80px; text-align: right">Situação: </td>
		<td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="220px" /></td>
        <td colspan="4"></td>
	</tr>    
	<tr>
		<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			<asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClientClick="return goNova();" />
		</td>
	</tr>
    </table>
    <table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
				AllowSorting="false" CssClass="gridView" Width="1400px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
                AllowPaging="True" PageSize="25" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging">
                <Columns>
			        <asp:TemplateField HeaderText="Situação">
				        <ItemTemplate>
					        <asp:Image ID="imgSituacao" runat="server" />
				        </ItemTemplate>
			        </asp:TemplateField>
					<asp:BoundField HeaderText="Requerimento" DataField="CD_SOLICITACAO" DataFormatString="{0:000000000}" />
                    <asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" />
					<asp:BoundField HeaderText="Empresa" DataField="BA3_CODEMP" />
					<asp:BoundField HeaderText="Matrícula" DataField="BA3_MATEMP" />
					<asp:BoundField HeaderText="Data da Solicitação" DataField="dt_criacao" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="Situação" DataField="CD_STATUS" />
                    <asp:BoundField HeaderText="Usuário Envio" DataField="NM_USUARIO_ENVIO" />
                    <asp:BoundField HeaderText="Data do Envio" DataField="dt_envio" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Usuário Aprovação" DataField="NM_USUARIO_APROVACAO" />
                    <asp:BoundField HeaderText="Data de Aprovação" DataField="DT_APROVACAO" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField>
                        <ItemStyle Width="125px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Gerar PDF" ToolTip="Gerar PDF" />
                            <asp:ImageButton ID="bntView" runat="server" ImageUrl="~/img/lupa.gif" Height="30px" OnClientClick='<%# "return goView("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Visualizar" ToolTip="Visualizar" />
                            <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="25px" OnClientClick='<%# "return openEdit("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Enviar Solicitação" ToolTip="Enviar Solicitação" />
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdAprovar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openAprovar") %>'>
                                <asp:Image ID="imgAprovar"  runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Aprovar Solicitação" ToolTip="Aprovar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnNegar" runat="server" CommandName="CmdCancelar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgNegar"  runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Negar Solicitação" ToolTip="Negar Solicitação" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>

<%@ Page Title="Negativas" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaFormNegativa.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaFormNegativa" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_CANCELAR = 1;

        function ConfigControlsCustom() {
            $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
            $('#<%= txtProtocoloANS.ClientID %>').ForceNumericOnly();
            createLocator(550, 450, dlgOpen, dlgClose, defaultDlgCallback);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CANCELAR: src = '../FormsPop/PopCancelNegativa.aspx?'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function dlgClose(ev, ui) {

        }
        function openPop(tipo, obj, id, row) {
            var handler = new LocatorHandler(tipo, id, row, obj);
            var titulo = "";
            switch (tipo) {
                case POP_CANCELAR: titulo = "Cancelar Negativa"; break;
            }
            openLocator(titulo, handler);
            return false;
        }

    	function openPdf(id) {
    		openReport(RELATORIO_NEGATIVA, 'ID=' + id, true);
    		return false;
    	}

    	function openCancelar(obj, id, row) {
    	    openPop(POP_CANCELAR, obj, id, row);
    	    return false;
    	}

    	function openReanalise(obj, id, row) {
    	    window.location = "FormNegativaReanalise.aspx?id=" + id;
    	    return false;
    	}

        function openEdit(id) {
            window.location = "FormNegativa.aspx?id=" + id;
            return false;
        }

        function openView(id) {
            open('../FormsPop/PopViewFormNegativa.aspx?ID=' + id, "_negativa", "width=880px, height=700px,scrollbars=yes", true);
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
        <td style="width:100px; text-align: right">Protocolo:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
        <td style="width:150px; text-align: right">Protocolo ANS:</td>
		<td align="left"><asp:TextBox ID="txtProtocoloANS" runat="server" Width="220px" MaxLength="20"  /></td>
        
        <td style="width:100px; text-align: right">Tipo:</td>
		<td align="left"><asp:DropDownList ID="dpdTipo" runat="server" Width="220px">
            <asp:ListItem Value="" Text="TODOS" />
        <asp:ListItem Value="CRED" Text="REDE CREDENCIADA" />
        <asp:ListItem Value="LIVR" Text="LIVRE ESCOLHA" />
        </asp:DropDownList></td>		
    </tr>
    <tr>
        <td style="width:150px; text-align: right">Nome Beneficiário:</td>
		<td align="left" colspan="5">
            <asp:TextBox ID="txtNomeBeneficiario" runat="server" Width="590px" MaxLength="200" />
		</td>		
    </tr>
	<tr>
		<td style="width:150px; text-align: right">Máscara Serviço: </td>
		<td align="left"><asp:TextBox ID="txtMascara" runat="server" Width="120px" MaxLength="10" /></td>
        <td align="right">Descrição Serviço:</td>
        <td colspan="3"><asp:TextBox ID="txtDesServico" runat="server" Width="430px" MaxLength="100" /></td>
	</tr>
    <tr>
        <td style="width:150px; text-align: right">Situação da Negativa: </td>
		<td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="180px" /></td>
        <td align="right">Situação da Reanálise:</td>
        <td colspan="3"><asp:DropDownList ID="dpdSituacaoReanalise" runat="server" Width="200px" /></td>
    </tr>    
	<tr>
		<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
            <asp:Button ID="btnNovo" runat="server" Text="Novo" OnClick="btnNovo_Click" />
		</td>
	</tr>
</table>
<table>
	<tr style="height:300px">
		<td colspan="6">
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
				AllowSorting="false" CssClass="gridView" Width="1200px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
				PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="CD_SOLICITACAO" DataFormatString="{0:000000000}" />
                    <asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS"/>
					<asp:BoundField HeaderText="Beneficiário" DataField="BA1_NOMUSR" />
					<asp:BoundField HeaderText="Situação da Negativa" DataField="CD_STATUS" />
                    <asp:BoundField HeaderText="Situação da Reanálise" DataField="CD_STATUS_REANALISE" />
                    <asp:TemplateField>
                        <ItemStyle Width="140px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Gerar PDF" ToolTip="Gerar PDF" />
                            <asp:ImageButton ID="btnPrint" runat="server" ImageUrl="~/img/print.png" Height="30px" OnClientClick='<%# "return openView("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Imprimir" ToolTip="Imprimir" />
                            <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="25px" OnClientClick='<%# "return openEdit("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Editar Solicitação" ToolTip="Editar Solicitação" />
                            <asp:LinkButton  ID="btnReanalise" runat="server" CommandName="CmdReanalizar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openReanalise") %>'>
                                <asp:Image ID="imgReanalise" runat="server" ImageUrl="~/img/refresh.png" Height="25px" AlternateText="Reanálise da Solicitação" ToolTip="Reanálise da Solicitação" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>                    
					<asp:TemplateField HeaderText="Ações">
						<ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdFinalizar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' >
                                <asp:Image ID="imgAprovar" runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Aprovar Negativa" ToolTip="Aprovar Negativa" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnCancelar" runat="server" CommandName="CmdCancelar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgCancelar" runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Cancelar Solicitação" ToolTip="Cancelar Solicitação" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>

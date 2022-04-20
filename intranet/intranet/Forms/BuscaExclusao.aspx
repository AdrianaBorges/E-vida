<%@ Page Title="Exclusão de beneficiários" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaExclusao.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaExclusao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var POP_CANCELAR = 0;
        var POP_APROVAR = 1;

        function ConfigControlsCustom() {
            $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

            createLocator(550, 450, dlgOpen, null, defaultDlgCallback);
        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CANCELAR: src = 'PopCancelExclusao.aspx?'; break;
                case POP_APROVAR: src = 'A.aspx?ID='; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function openPop(tipo, obj, id, row) {
            var handler = new LocatorHandler(tipo, id, row, obj);

            var titulo = tipo == POP_APROVAR ? "Aprovar Exclusão" : "Negar Exclusão";

            openLocator(titulo, handler);
            return false;
        }


    	function openPdf(id) {
    		openReport(RELATORIO_EXCLUSAO, 'ID=' + id, true);
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
        function openSolDoc(obj, id, row) {
            return confirm("Deseja mudar a situação da solicitação para Aguardando Documentação?");
        }

        function goView(id) {
        	window.location = 'ViewExclusao.aspx?ID=' + id;
            return false;
        }
        function goNova() {
            window.location = 'FormExclusao.aspx';
            return false;
        }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
<table width="100%" cellspacing="10px" class="tabelaForm">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">Protocolo:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
        <td style="width:200px; text-align: right">Matrícula:</td>
        <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
    </tr>
	<tr>
		<td style="width:150px; text-align: right">Situação: </td>
		<td align="left" colspan="3"><asp:DropDownList ID="dpdSituacao" runat="server" Width="300px" /></td>        
	</tr>    
	<tr>
		<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
            <asp:Button ID="btnNovo" runat="server" Text="Nova solicitação" OnClientClick="return goNova()" />
		</td>
	</tr>
</table>
<table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
				AllowSorting="false" CssClass="gridView" Width="1200px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand">
                <Columns>
					<asp:BoundField HeaderText="Requerimento" DataField="CD_SOLICITACAO" DataFormatString="{0:000000000}" />
                    <asp:BoundField HeaderText="Empresa" DataField="BA3_CODEMP" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Matrícula" DataField="BA3_MATEMP" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Data da Solicitação" DataField="dt_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="160px" HeaderStyle-HorizontalAlign="Center" />
					<asp:BoundField HeaderText="Data Alteração" DataField="dt_alteracao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="160px" HeaderStyle-HorizontalAlign="Center" />
					<asp:BoundField HeaderText="Situação" DataField="CD_STATUS" />
                    <asp:TemplateField>
                        <ItemStyle Width="155px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Gerar PDF" ToolTip="Gerar PDF" />
                            <asp:ImageButton ID="bntView" runat="server" ImageUrl="~/img/lupa.gif" Height="30px" OnClientClick='<%# "return goView("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Visualizar" ToolTip="Visualizar" />                                                        
                            <asp:LinkButton  ID="btnSolDoc" runat="server" CommandName="CmdDoc" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openSolDoc") %>'>
                                <asp:Image ID="imgSolDoc" runat="server" ImageUrl="~/img/newDoc.png" Height="25px" AlternateText="Aguardar Documentação" ToolTip="Aguardar Documentação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdAprovar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' >
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

<%@ Page Title="Busca Viagem" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaViagem.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaViagem" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_ENCERRAR = 1;

        function ConfigControlsCustom() {
            $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
            //$('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

            createLocator(650, 550, dlgOpen, null, defaultDlgCallback);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ENCERRAR: src = '../GenPops/PopGenericCancel.aspx?tipo=VIAGEM&ID=' + handler.id; break;
            }
            setLocatorUrl(src);
        }

        function openEncerrar(obj, id, row) {
            var handler = new LocatorHandler(POP_ENCERRAR, id, row, obj);
            openLocator("Encerrar Solicitação", handler);
            return false;
        }

        function reloadPage() {
            window.location.reload(true);
        }

        function openEdit(btn, id) {
            window.location = "FormViagem.aspx?id=" + id;
            return false;
        }

        function openPDF(id) {
            openReport(RELATORIO_VIAGEM, "ID=" + id, false);
            return false;
        }

        function goNova(id) {
            window.location = 'FormViagem.aspx';
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	
<table width="950px" cellspacing="10px" class="tabelaForm">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">Protocolo:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
        <td style="width:200px; text-align: right">Matrícula (E-VIDA):</td>
        <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
    </tr>
	<tr>
		<td style="width:150px; text-align: right">Situação: </td>
		<td align="left" colspan="3"><asp:DropDownList ID="dpdSituacao" runat="server" Width="420px" /></td>        
	</tr>    
	<tr>
		<td colspan="4" style="text-align:center">
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			<asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClientClick="return goNova();" />
		</td>
	</tr>
    </table>

    <table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_VIAGEM"
				AllowSorting="false" CssClass="gridView" Width="1200px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand">
                <Columns>
					<asp:BoundField HeaderText="Requerimento" DataField="CD_VIAGEM" DataFormatString="{0:000000000}" />
                    <asp:BoundField HeaderText="Tipo" DataField="ISEXTERNO" />
					<asp:BoundField HeaderText="Matrícula" DataField="CD_MAT_EMPREGADO" />
					<asp:BoundField HeaderText="Nome" DataField="NM_EMPREGADO" />
					<asp:BoundField HeaderText="Data da Solicitação" DataField="DT_SOLICITACAO" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="Situação" DataField="ID_SITUACAO" />
                    <asp:TemplateField>
                        <ItemStyle Width="125px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="25px" OnClientClick='<%# "return openEdit(this, "+ Eval("CD_VIAGEM") + ");" %>' AlternateText="Editar Solicitação" ToolTip="Editar Solicitação" />                            
                            <asp:LinkButton  ID="btnCancelar" runat="server" CommandName="CmdEncerrar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openEncerrar") %>'>
                                <asp:Image ID="imgEncerrar"  runat="server" ImageUrl="~/img/cancel.png" Height="25px" AlternateText="Cancelar Solicitação" ToolTip="Cancelar Solicitação" />
                            </asp:LinkButton>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPDF("+ Eval("CD_VIAGEM") + ");" %>' ToolTip="Relatório" />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
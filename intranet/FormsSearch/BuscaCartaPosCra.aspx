<%@ Page Title="Carta Positiva CRA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaCartaPosCra.aspx.cs" Inherits="eVidaIntranet.FormsSearch.BuscaCartaPosCra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_CANCELAR = 0;

        function ConfigControlsCustom() {
            $('#<%= txtId.ClientID %>').ForceNumericOnly();
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();


            createLocator(650, 550, dlgOpen, dlgClose, defaultDlgCallback);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CANCELAR: src = '../FormsPop/PopCancelGeneric.aspx?TYPE=<%: eVidaIntranet.FormsPop.PopCancelGeneric.CancelObject.CartaPositiva %>'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }
        
        function dlgClose(ev, ui) {

        }

        function openCancelar(obj, id, row) {
            return defaultOpenPop(POP_CANCELAR, obj, id, row, "Cancelar Solicitação");
        }
        
        function openEdit(btn, id) {
            window.location = "../Forms/FormCartaPosCra.aspx?id=" + id;
            return false;
        }
        function goNova(id) {
            window.location = '../Forms/FormCartaPosCra.aspx';
            return false;
        }
        function openPdf(id) {
            openReport(RELATORIO_CARTA_POSITIVA_CRA, 'ID=' + id, true);
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<table width="800px" cellpadding="10px" class="tabelaForm" style="margin-left:20px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">ID:</td>
		<td align="left"><asp:TextBox ID="txtId" runat="server" Width="120px" MaxLength="10"  /></td>
        <td style="width:200px; text-align: right">Matrícula Funcionário:</td>
        <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
    </tr>
    <tr>
        <td style="width:100px; text-align: right">Protocolo CRA:</td>
		<td align="left"><asp:TextBox ID="txtProtocoloCra" runat="server" Width="220px" MaxLength="50"  /></td>
        <td style="width:200px; text-align: right">Cartão:</td>
        <td align="left"><asp:TextBox ID="txtCarteirinha" runat="server" Width="180px" MaxLength="50"  /></td>        
    </tr>
    <tr>
        <td style="text-align:right">Situação:</td>
        <td colspan="3"><asp:DropDownList ID="dpdSituacao" runat="server" /></td>
    </tr>
	<tr>
		<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
			<asp:Button ID="btnNovo" runat="server" Text="Novo Registro" OnClientClick="return goNova();" />
		</td>
	</tr>
    </table>
    <table>
	    <tr style="height:300px">
		    <td>
                <asp:Label ID="lblCount" runat="server" />
			    <asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
				    AllowSorting="false" CssClass="gridView" Width="1600px" 
                    OnRowCommand="gdvRelatorio_RowCommand" OnRowDataBound="gdvRelatorio_RowDataBound"
                    PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                    <Columns>
					    <asp:BoundField HeaderText="Id" DataField="CD_SOLICITACAO"/>
                        <asp:BoundField HeaderText="Protocolo CRA" DataField="DS_PROTOCOLO_CRA" />
					    <asp:BoundField HeaderText="Cartão" DataField="BA1_MATANT" ItemStyle-Wrap="false"/>
					    <asp:BoundField HeaderText="Beneficiário" DataField="BA1_NOMUSR" ItemStyle-Width="200px" />
                        <asp:BoundField HeaderText="Credenciado" DataField="BAU_NOME" ItemStyle-Width="200px" />
                        <asp:BoundField HeaderText="CPF/CNPJ Credenciado" DataField="BAU_CPFCGC"/>
					    <asp:BoundField HeaderText="Situação" DataField="CD_STATUS" />
                        <asp:BoundField HeaderText="Data de Solicitação" DataField="dt_solicitacao" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField HeaderText="Usuário Criação" DataField="usuario_criacao" />
                        <asp:BoundField HeaderText="Data de Alteração" DataField="dt_alteracao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                        <asp:BoundField HeaderText="Usuário Alteração" DataField="usuario_alteracao" />
                        <asp:BoundField HeaderText="Data de Aprovação" DataField="dt_aprovacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                        <asp:BoundField HeaderText="Usuário Aprovação" DataField="usuario_aprovacao" />
                        <asp:TemplateField>
                        <ItemStyle Width="125px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="25px" OnClientClick='<%# "return openEdit(this, "+ Eval("CD_SOLICITACAO") + ");" %>' AlternateText="Editar Carta" ToolTip="Editar Carta" />
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdAprovar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' >
                                <asp:Image ID="imgAprovar" runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Aprovar" ToolTip="Aprovar" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnCancelar" runat="server" CommandName="CmdCancelar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgCancelar" runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Cancelar Solicitação" ToolTip="Cancelar Solicitação" />
                            </asp:LinkButton>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Gerar PDF" ToolTip="Gerar PDF" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>

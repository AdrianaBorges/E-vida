<%@ Page Title="2ª Via de Carteira" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="SegViaCarteira.aspx.cs" Inherits="eVidaIntranet.Forms.SegViaCarteira" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var POP_CANCELAR = 0;

        function ConfigControlsCustom() {
            $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
            $('#<%= txtProtocoloANS.ClientID %>').ForceNumericOnly();
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

            createLocator(550, 450, dlgOpen, null, defaultDlgCallback);
        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CANCELAR: src = 'PopCancelSegViaCarteira.aspx?'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function openPop(tipo, obj, id, row) {
            var handler = new LocatorHandler(tipo, id, row, obj);

            var titulo = "Cancelar Segunda Via";

            openLocator(titulo, handler);
            return false;
        }
        function openPdf(id) {
            openReport(RELATORIO_SEGUNDA_VIA, 'ID=' + id);
            return false;
        }

        function openCancelar(obj, id, row) {
            openPop(POP_CANCELAR, obj, id, row);
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
<table width="100%" cellspacing="10px" class="tabelaForm">
	<tr>
		<td colspan="6"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">Protocolo:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px"/></td>
        <td style="width:100px; text-align: right">Protocolo ANS:</td>
		<td align="left"><asp:TextBox ID="txtProtocoloANS" runat="server" Width="220px" MaxLength="20"/></td>
		<td style="width:80px; text-align: right">Matrícula:</td>
		<td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px"/></td>
    </tr>
	<tr>
        <td style="width:80px; text-align: right">Situação:</td>
		<td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="120px">
            <asp:ListItem Value="" Text="Todos" />
            <asp:ListItem Value="P" Text="Pendentes" Selected="True" />
			<asp:ListItem Value="C" Text="Cancelados" />
            <asp:ListItem Value="F" Text="Finalizados" />
            </asp:DropDownList></td>
		<td style="width:80px; text-align: right"><b>Criado </b> de </td>
		<td align="left" colspan="3"><asp:TextBox ID="txtInicio" runat="server" Width="120px" MaxLength="10" CssClass="calendario"  /> 
            até
            <asp:TextBox ID="txtFim" runat="server" Width="120px" MaxLength="10" CssClass="calendario" /></td>
	</tr>    
	<tr>
		<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" Visible="false" OnClick="btnExportar_Click" />
		</td>
	</tr>
</table>
<table width="100%">
	<tr style="height:300px">
		<td colspan="6">
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
				AllowSorting="false" CssClass="gridView" Width="1200px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="CD_SOLICITACAO" DataFormatString="{0:000000000}" />
					<asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" />
                    <asp:BoundField HeaderText="Empresa" DataField="BA3_CODEMP" />
					<asp:BoundField HeaderText="Matrícula" DataField="BA1_MATEMP" />
					<asp:BoundField HeaderText="Nome Funcionário" DataField="BA1_NOMUSR" />
                    <asp:BoundField HeaderText="Data Criação" DataField="DT_CRIACAO" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"/>
                    <asp:BoundField HeaderText="Data Alteração" DataField="DT_ALTERACAO" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"/>
                    <asp:BoundField HeaderText="Situação" DataField="TP_STATUS" />
                    <asp:TemplateField HeaderText="BO" ItemStyle-HorizontalAlign="Center" >
                        <ItemTemplate>
                            <asp:ImageButton ID="btnArquivo" runat="server" ImageUrl="~/img/anexo.png" Height="30px" />
                        </ItemTemplate>
                    </asp:TemplateField>                    
                    <asp:HyperLinkField HeaderText="" DataNavigateUrlFields="CD_SOLICITACAO" DataNavigateUrlFormatString="ViewSegViaCarteira.aspx?id={0}"
						    Text="&lt;img src='../img/lupa.gif' alt='Visualizar solicitação' border='0'/&gt;" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "openPdf("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Visualizar Solicitação" />

                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdFinalizar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>
                                <asp:Image ID="imgAprovar"  runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Finalizar Solicitação" ToolTip="Finalizar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnNegar" runat="server" CommandName="CmdCancelar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgNegar"  runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Cancelar Solicitação" ToolTip="Cancelar Solicitação" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>

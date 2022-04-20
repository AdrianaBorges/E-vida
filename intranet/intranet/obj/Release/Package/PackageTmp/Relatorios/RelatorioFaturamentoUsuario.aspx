<%@ Page Title="Faturamento por Usuário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioFaturamentoUsuario.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioFaturamentoUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
            $('#popUsuario').dialog({
                autoOpen: false,
                width: 600,
                height: 600,
                draggable: false,
                modal: true,
                resizable: false,
                open: function (ev, ui) {
                    $('#frmUsuario').attr('src', 'PopUsuarioAtendimento.aspx');
                },
                close: function (ev, ui) {
                    $('#frmUsuario').attr('src', '../img/progress.gif');
                },

                show: { effect: 'fade', duration: 500 },
                hide: {effect: 'fade', duration: 500}
            });
        }

        function abrirUsuario() {
            $("#popUsuario").dialog('open');
            return false;
        }


        function addUsuario(id, nome) {
            $('#popUsuario').dialog('close');
            $('#<%=hidCdUsuario.ClientID%>').val(id);
            $('#<%=hidNmUsuario.ClientID%>').val(nome);
            <%= ClientScript.GetPostBackEventReference(btnAdicionarUsuario, "") %>
        }
        function addUsuarios(ids) {
            $('#popUsuario').dialog('close');
            $('#<%=hidCdUsuario.ClientID%>').val(ids);
            $('#<%=hidNmUsuario.ClientID%>').val("--");
            <%= ClientScript.GetPostBackEventReference(btnAdicionarUsuario, "") %>
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    <table width="850px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:140px; text-align: right">Contabilizar por:</td>
        <td colspan="3"><asp:DropDownList ID="dpdContabilizacao" runat="server" Width="120px">
            <asp:ListItem Value="0" Text="GUIA" Selected="True" />
            <asp:ListItem Value="1" Text="ITEM" />
            </asp:DropDownList></td>
    </tr>
	<tr>
		<td style="width:140px; text-align: right">Data Inicial:</td>
		<td align="left"><asp:TextBox ID="txtDataInicial" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  /></td>
        <td style="width:140px; text-align: right">Data Final:</td>
		<td align="left"><asp:TextBox ID="txtDataFinal" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  /></td>
	</tr>
    <tr>
		<td style="text-align: right">Usuários:</td>
		<td align="left" colspan="3">
            <asp:GridView ID="gdvUsuarios" runat="server" Width="500px" CssClass="tabela"
                AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"
                OnRowCommand="gdvUsuarios_RowCommand">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnRemover" runat="server" ImageUrl="~/img/remove.png" CommandName="RemoverUsuario" CommandArgument='<%# Container.DataItemIndex %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="cd_usuario" HeaderText="USUÁRIO" />
                    <asp:BoundField DataField="nm_usuario" HeaderText="NOME" />
                </Columns>
            </asp:GridView>
            <asp:HiddenField ID="hidCdUsuario" runat="server" />
            <asp:HiddenField ID="hidNmUsuario" runat="server" />
            <asp:Button ID="btnAdicionarUsuario" runat="server" OnClientClick="return abrirUsuario()" Text="Adicionar Usuário" OnClick="btnAdicionarUsuario_Click" />
            <asp:Button ID="btnLimparUsuario" runat="server" Text="Limpar Usuários" OnClick="btnLimparUsuario_Click" Visible="false" />
		</td>
    </tr>
    <tr>
        <td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
                <asp:Button ID="btnGraficoProd" runat="server" Text="Gráfico do Período" OnClick="btnGraficoProd_Click" Visible="false" />
                <asp:Button ID="btnGraficoProdDia" runat="server" Text="Gráfico Usuário/Dia" OnClick="btnGraficoProd_Click" Visible="false" />
                <asp:Button ID="btnGraficoProtocolo" runat="server" Text="Gráfico Usuário/Protocolo" OnClick="btnGraficoProtocolo_Click" Visible="false" />
		    </td>        
	</tr>
    </table>


    <table>
	<tr style="height:300px">
		<td>
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="1000px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="PROTOCOLO" DataField="nr_protocolo" SortExpression="nr_protocolo" />
					<asp:BoundField HeaderText="TIPO" DataField="tp_sistema_atend" SortExpression="tp_sistema_atend" />
					<asp:BoundField HeaderText="GUIA" DataField="nr_atendimento" SortExpression="nr_atendimento" />
					<asp:BoundField HeaderText="QTD ITENS" DataField="qtd_item" SortExpression="qtd_item" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="USUÁRIO ISA" DataField="user_update" SortExpression="user_update" />
                    <asp:BoundField HeaderText="NOME" DataField="nm_usuario" SortExpression="nm_usuario" />
                    <asp:BoundField HeaderText="ORIGEM" DataField="tp_origem" SortExpression="tp_origem" />
                    <asp:BoundField HeaderText="OPERACAO" DataField="tp_operacao" />
                    <asp:BoundField HeaderText="DATA E HORA" DataField="date_update" SortExpression="date_update" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
    <div id="popUsuario" title="Busca">
        <iframe id="frmUsuario" src="../img/progress.gif" width="570" height="500px" frameborder="0"></iframe>
    </div>
</asp:Content>

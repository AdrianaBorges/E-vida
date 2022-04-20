<%@ Page Title="Declaração Anual de Débito" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaDebitoAnual.aspx.cs" Inherits="eVidaIntranet.Gestao.BuscaDebitoAnual" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function confirmEnvioLote() {
            return confirm("Envio em lote (seleção na página) será feito de forma assíncrona. Após alguns segundos os registros marcados serão processados. Deseja prosseguir?");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

<table width="880px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:120px; text-align: right">Período (ano):</td>
		<td align="left"><asp:DropDownList ID="dpdAno" runat="server" Width="80px" /></td>
        <td style="width:160px; text-align: right">Plano:</td>
        <td><asp:DropDownList ID="dpdPlano" runat="server" Width="100%" DataTextField="DsPlano" DataValueField="CdPlano" /></td>
	</tr>
    <tr>
        <td style="text-align: right">Empresa:</td>
        <td><asp:DropDownList ID="dpdEmpresa" runat="server" Width="150px" DataValueField="Id" DataTextField="Nome"/></td>
        <td style="text-align: right">Matrícula:</td>
        <td><asp:TextBox ID="txtMatricula" runat="server" Width="150px" MaxLength="10"/></td>
    </tr>
    <tr>		
        <td style="text-align: right">Situação:</td>
        <td><asp:DropDownList ID="dpdSituacao" runat="server">
            <asp:ListItem Value="" Text="Todos" />
            <asp:ListItem Value="-1" Text="NÃO SOLICITADO" />
            <asp:ListItem Value="0" Text="SOLICITADO" />
            <asp:ListItem Value="1" Text="GERADO" />
            <asp:ListItem Value="2" Text="ENVIADO" />
            <asp:ListItem Value="3" Text="ERRO" />
            </asp:DropDownList></td>
        <td style="text-align: right">Pendencia:</td>
        <td><asp:DropDownList ID="dpdPendencia" runat="server">
            <asp:ListItem Value="" Text="Todos" />
            <asp:ListItem Value="N" Text="COM PENDENCIA" />
            <asp:ListItem Value="S" Text="TODOS QUITADOS" />
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td colspan="4" style="text-align:center">
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />    
            <asp:Button ID="btnEnviarSelecao" runat="server" Text="Enviar para seleção" OnClientClick="return confirmEnvioLote()" OnClick="btnEnviarSelecao_Click" />
		</td>        
	</tr>
</table>

    
<table>
	<tr style="height:300px">
		<td colspan="4">
			<asp:UpdatePanel ID="pnlGrid" runat="server" UpdateMode="Conditional">
				<ContentTemplate>

            <asp:Label ID="lblCount" runat="server" Visible="false" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="1200px" OnSorting="gdvRelatorio_Sorting"
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging"
                DataKeyNames="CD_BENEFICIARIO, ANO" OnRowDataBound="gdvRelatorio_RowDataBound" >
                <PagerStyle CssClass="paginacao" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkHeader" runat="server" OnCheckedChanged="chkHeader_CheckedChanged" AutoPostBack="true" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelecionar" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="NOME BENEFICIÁRIO" DataField="nm_beneficiario" />
                    <asp:BoundField HeaderText="MATRÍCULA" DataField="cd_funcionario" />
                    <asp:BoundField HeaderText="DATA SOLICITAÇÃO" DataField="DT_SOLICITACAO" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField HeaderText="SITUAÇÃO" DataField="CD_STATUS" />
                    <asp:BoundField HeaderText="DATA SITUAÇÃO" DataField="DT_STATUS" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField HeaderText="DATA ENVIO" DataField="DT_ENVIO" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnEnviar_Click" />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
					</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>

</table>
</asp:Content>

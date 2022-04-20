<%@ Page Title="Endereços de Cobrança" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaEnderecoCobranca.aspx.cs" Inherits="eVidaIntranet.Admin.BuscaEnderecoCobranca" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function openEdit(btn, id) {
            window.location = "FormEnderecoCobranca.aspx?id=" + id;
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
        <td style="width:200px; text-align: right">CPF:</td>
        <td align="left"><asp:TextBox ID="txtCpf" runat="server" Width="140px" MaxLength="11"  /></td>
		<td style="width:150px; text-align: right">Preenchido: </td>
		<td align="left"><asp:DropDownList ID="dpdPreenchido" runat="server" Width="140px" /></td>
    </tr>
	<tr>
        <td style="width:100px; text-align: right">Nome:</td>
		<td colspan="3" align="left"><asp:TextBox ID="txtNome" runat="server" Width="340px" MaxLength="255"  /></td>
	</tr>    
	<tr>
		<td colspan="4" style="text-align:center">
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
		</td>
	</tr>
</table>
<table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="A1_COD"
				AllowSorting="false" CssClass="gridView" Width="1200px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand" PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                <Columns>
					<asp:BoundField HeaderText="CPF" DataField="A1_CGC" DataFormatString="{0:000000000}" />
                    <asp:BoundField HeaderText="Nome" DataField="A1_NOME" />
                    <asp:BoundField HeaderText="CEP" DataField="A1_CEP" />
                    <asp:BoundField HeaderText="Endereço" DataField="A1_END" />
                    <asp:BoundField HeaderText="Número" DataField="A1_NR_END" />
                    <asp:BoundField HeaderText="Complemento" DataField="A1_COMPLEM" />
                    <asp:BoundField HeaderText="Bairro" DataField="A1_BAIRRO" />
                    <asp:BoundField HeaderText="Endereço de Cobrança" DataField="A1_ENDCOB" />
                    <asp:TemplateField>
                        <ItemStyle Width="155px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEdit") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>


</asp:Content>

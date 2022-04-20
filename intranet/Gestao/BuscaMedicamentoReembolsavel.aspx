<%@ Page Title="Medicamento Reembolsável" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaMedicamentoReembolsavel.aspx.cs" Inherits="eVidaIntranet.Gestao.BuscaMedicamentoReembolsavel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        
        function openEdit(btn, id, idx) {
            window.location = "MedicamentoReembolsavel.aspx?id=" + id;
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="100%" cellspacing="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:100px; text-align: right">Máscara:</td>
			<td align="left"><asp:TextBox ID="txtSMascara" runat="server" Width="150px" CssClass="inteiro" /></td>
			<td style="width:200px; text-align: right">Medicamento / Princípio Ativo:</td>
			<td align="left"><asp:TextBox ID="txtSDescricao" runat="server" MaxLength="150" /></td>
		</tr>

		<tr>
			<td style="width:100px; text-align: right">Reembolsável:</td>
			<td align="left">
                <asp:DropDownList ID="dpdReembolsavel" runat="server" Width="160px" >
				    <asp:ListItem Text="TODOS" Value="" />
					<asp:ListItem Text="SIM" Value="S" />
					<asp:ListItem Text="NÃO" Value="N" />
				</asp:DropDownList>
			</td>
			<td style="width:200px; text-align: right">Plano:</td>
			<td align="left">
                <asp:DropDownList ID="dpdPlano" runat="server" Width="410px" ></asp:DropDownList>
			</td>
		</tr>

		<tr>
			<td style="width:100px; text-align: right">Contínuo:</td>
			<td align="left">
                <asp:DropDownList ID="dpdContinuo" runat="server" Width="160px" >
				    <asp:ListItem Text="TODOS" Value="" />
					<asp:ListItem Text="SIM" Value="S" />
					<asp:ListItem Text="NÃO" Value="N" />
				</asp:DropDownList>
			</td>
		</tr>

		<tr>
			<td colspan="4" style="text-align:center">
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			</td>
		</tr>
    </table>
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="cd_mascara"
					AllowSorting="false" CssClass="gridView" Width="100%" 
					OnRowDataBound="gdvRelatorio_RowDataBound" >
					<Columns>
	                    <asp:BoundField HeaderText="Máscara" DataField="cd_mascara" />
	                    <asp:BoundField HeaderText="Medicamento" DataField="ds_servico" />
	                    <asp:BoundField HeaderText="Princípio Ativo" DataField="DS_PRINCIPIO_ATIVO" />
	                    <asp:BoundField HeaderText="Reembolsável" DataField="CK_REEMBOLSAVEL" />
	                    <asp:BoundField HeaderText="Contínuo" DataField="CK_USO_CONTINUO" />
	                    <asp:BoundField HeaderText="Planos" DataField="DS_LST_PLANO" />
						<asp:TemplateField>
							<ItemStyle Width="95px" />
							<ItemTemplate>
								<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/lupa.gif" Height="25px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEdit") %>' AlternateText="Detalhar" ToolTip="Detalhar" />
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>

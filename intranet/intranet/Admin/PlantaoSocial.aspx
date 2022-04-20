<%@ Page Title="Gerenciar Plantões Sociais" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="PlantaoSocial.aspx.cs" Inherits="eVidaIntranet.Admin.PlantaoSocial" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		
		function confirmExclusao() {
			return confirm("Deseja realmente remover os dados de plantão social?");
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="100%">
        <tr>
            <td align="center">
				<asp:UpdatePanel runat="server" ID="updUf" UpdateMode="Conditional">
					<ContentTemplate>
						<table class="tabelaForm" width="500px">
							<tr>
								<th colspan="6"><h2>Incluir/Alterar Plantao Social</h2><br /></th>
							</tr>
							<tr>
								<td><b>UF:</b></td>
								<td><asp:DropDownList ID="dpdUf" runat="server" DataValueField="Sigla" DataTextField="Nome" 
									OnSelectedIndexChanged="dpdUf_SelectedIndexChanged" AutoPostBack="true" /></td>
								<td><b>Cidade:</b></td>
								<td><asp:DropDownList ID="dpdMunicipio" runat="server" Width="300px" DataValueField="Codmun" DataTextField="Mun" /></td>
								<td><b>Telefone:</b></td>
								<td><asp:TextBox ID="txtTelefone" runat="server" Width="150px" /></td>
							</tr>
							<tr>
								<td colspan="6" align="center"><br />
									<asp:Button ID="btnNovo" runat="server" Text="Limpar/Novo" OnClick="btnNovo_Click" Visible="false" />
									<asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
								</td>
							</tr>
						</table>
					</ContentTemplate>
				</asp:UpdatePanel>
            </td>
        </tr>
	    <tr style="height:300px; vertical-align: top">
		    <td align="center">
				<asp:UpdatePanel runat="server" ID="updLocais" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:ListView ID="ltvPlantaoSocial" runat="server" ItemPlaceholderID="row"
							ItemType="eVidaGeneralLib.VO.PlantaoSocialLocalVO" DataKeyNames="Id">
							<EmptyDataTemplate><p style="text-align:center"><b>Não existem plantões sociais cadastrados.</b></p></EmptyDataTemplate>
							<LayoutTemplate>
								<table class="tabela" style="width:800px">
									<tr>
										<th style="width:50px"></th>	
										<th style="width:50px">UF</th>
										<th>Cidade</th>
										<th>Telefone</th>
									</tr>
									<tr id="row" runat="server"></tr>
								</table>
							</LayoutTemplate>
							<ItemTemplate>
								<tr>
									<td><asp:ImageButton ID="bntExcluir" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
											OnClick="bntExcluir_Click" OnClientClick='<%# "return confirmExclusao();" %>' />
										<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px"
											OnClick="btnEditar_Click" />
									</td>
									<td><b><%# Item.Uf %></b></td>
									<td><%# Item.Cidade %></td>
									<td><%# Item.Telefone %></td>
								</tr>
							</ItemTemplate>
						</asp:ListView>
						</ContentTemplate>
				</asp:UpdatePanel>
		    </td>
	    </tr>
    </table>
</asp:Content>

<%@ Page Title="Motivos de Pendencia" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="MotivosPendencia.aspx.cs" Inherits="eVidaIntranet.Admin.MotivosPendencia" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		
		function confirmExclusao() {
			return confirm("Deseja realmente remover o motivo de pendência?");
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
								<th colspan="4"><h2>Incluir/Alterar Motivo de Pendencia</h2><br /></th>
							</tr>
							<tr>
                                <td><b>ID:</b></td>
                                <td colspan="3"> <asp:Literal ID="litId" runat="server" /></td>
                            </tr>
                            <tr>
								<td><b>Nome:</b></td>
								<td><asp:TextBox ID="txtNome" runat="server" Width="300px" MaxLength="255" /></td>
								<td><b>Tipo:</b></td>
								<td><asp:DropDownList ID="dpdTipo" runat="server" Width="300px" /></td>
							</tr>
							<tr>
								<td colspan="4" align="center"><br />
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
				<asp:UpdatePanel runat="server" ID="updMotivos" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:ListView ID="ltvMotivosPendencia" runat="server" ItemPlaceholderID="row"
							ItemType="eVidaGeneralLib.VO.MotivoPendenciaVO" DataKeyNames="Id">
							<EmptyDataTemplate><p style="text-align:center"><b>Não existem motivos cadastrados.</b></p></EmptyDataTemplate>
							<LayoutTemplate>
								<table class="tabela" style="width:800px">
									<tr>
										<th style="width:50px"></th>	
										<th style="width:50px">Tipo</th>
										<th>Nome</th>
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
									<td><b><%# Item.Tipo %></b></td>
									<td><%# Item.Nome %></td>
								</tr>
							</ItemTemplate>
						</asp:ListView>
						</ContentTemplate>
				</asp:UpdatePanel>
		    </td>
	    </tr>
    </table>
</asp:Content>

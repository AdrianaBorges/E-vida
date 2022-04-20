<%@ Page Title="Ajuste Autorização ISA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="AjusteAutorizacao.aspx.cs" Inherits="eVidaIntranet.Gestao.AjusteAutorizacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    <table width="100%" cellspacing="10px" style="vertical-align:top;">
		<tr>
			<td>
				<asp:Panel ID="pnlDados" runat="server" GroupingText="Buscar" DefaultButton="btnPesquisar">
					<table cellspacing="10px" >
						<tr>
							<td >Autorização:</td>
							<td align="left" colspan="3"><asp:TextBox ID="txtNroAutorizacao" runat="server" Width="200px"/></td>
							<td><asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" OnClick="btnPesquisar_Click" /></td>
						</tr>
					</table>
				</asp:Panel>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Panel ID="pnlOrigem" runat="server" GroupingText="Dados atuais">
					<table cellspacing="10px" >
						<tr>
							<td>Autorização:</td>
							<td><asp:Label ID="lblNroAutorizacao" runat="server" Font-Bold="true" /></td>
						</tr>
						<tr>
							<td >Data de início:</td>
							<td align="left"><asp:Label ID="lblInicio" runat="server" Width="120px" Font-Bold="true" /></td>
							<td> Data de término:</td>
							<td align="left"><asp:Label ID="lblTermino" runat="server" Width="120px" Font-Bold="true" /></td>
							<td>Dias autorizados:</td>
							<td align="left"><asp:Label ID="lblDias" runat="server" Width="80px" Font-Bold="true" /></td>
						</tr>
					</table>
				</asp:Panel>
			</td>
		</tr>
	    <tr>
			<td>
				<asp:Panel ID="pnlDadosNovos" runat="server" GroupingText="Alterar para">
					<table cellspacing="10px" >
						<tr>
							<td >Data de início:</td>
							<td align="left"><asp:TextBox ID="txtInicio" runat="server" Width="120px" CssClass="calendario" AutoPostBack="true" OnTextChanged="data_TextChanged" /></td>
							<td> Data de término:</td>
							<td align="left"><asp:TextBox ID="txtFim" runat="server" Width="120px" CssClass="calendario" AutoPostBack="true" OnTextChanged="data_TextChanged" /></td>
							<td>Dias autorizados:</td>
							<td align="left"><asp:Label ID="lblDias2" runat="server" Width="80px" Font-Bold="true" /></td>
						</tr>
					</table>
				</asp:Panel>
			</td>
		</tr>
        <tr>
            <td colspan="4" style="text-align:center">
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
            </td>
        </tr>
    </table>
    
</asp:Content>

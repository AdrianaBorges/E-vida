<%@ Page Title="Alterar Senha SCL" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="AlterarSenhaUsuarioScl.aspx.cs" Inherits="eVidaIntranet.Admin.AlterarSenhaUsuarioScl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
        <tr>
		    <td style="text-align: right"><b>Login:</b></td>
		    <td align="left"><asp:Label ID="lblLogin" runat="server" /></td>            
		    <td style="text-align: right"><b>Ativo:</b></td>
		    <td align="left"><asp:CheckBox ID="chkAtivo" runat="server" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right"><b>Nome:</b></td>
		    <td colspan="3" align="left"><asp:Label ID="lblNome" runat="server" /></td>
	    </tr>
        <tr>
            <td style="text-align: right"><b>Data de Cadastro:</b></td>
		    <td align="left"><asp:Label ID="lblDataCadastro" runat="server" /></td>
            <td style="text-align: right"><b>Data de Expiração:</b></td>
		    <td align="left"><asp:TextBox ID="txtDataExpiracao" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>
        </tr>
        <tr>
            <td style="text-align: right"><b>Usuário alteração:</b></td>
		    <td align="left"><asp:Label ID="lblUserUpdate" runat="server" /></td>
            <td style="text-align: right"><b>Data de alteração:</b></td>
		    <td align="left"><asp:Label ID="lblDateUpdate" runat="server" /></td>
        </tr>
        <tr>
		    <td style="text-align: right"><b>Senha:</b></td>
		    <td colspan="2" align="left"><asp:TextBox ID="txtSenha" runat="server" Width="220px" TextMode="Password" /></td>
        </tr>
        <tr>
		    <td style="text-align: right"><b>Confirmação de Senha:</b></td>
		    <td colspan="3" align="left"><asp:TextBox ID="txtConfSenha" runat="server" Width="220px" TextMode="Password" />
                <asp:CompareValidator ID="cmpValConfSenha" runat="server" ErrorMessage="As senhas estão diferentes!" CssClass="ui-state-error"
                    Type="String" ControlToCompare="txtSenha" ControlToValidate="txtConfSenha" Operator="Equal"></asp:CompareValidator>
		    </td>
	    </tr>
        <tr>
            <td></td>
		    <td colspan="4" ><b>Perfis:</b><br />
                <asp:GridView ID="gdvPerfil" runat="server" Width="420px" CssClass="tabela"
                    AllowSorting="false" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField HeaderText="SISTEMA" DataField="CdSistema" />
                        <asp:BoundField HeaderText="PERFIL" DataField="NmGrupo" />
                    </Columns>
		     </asp:GridView></td>
	    </tr>
        <tr><td></td>
		    <td colspan="4" ><b>Domínios:</b><br />
                <asp:GridView ID="gdvDominio" runat="server" Width="420px" CssClass="tabela"
                    AllowSorting="false" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField HeaderText="DOMÍNIO" DataField="IdDominio" />
                        <asp:BoundField HeaderText="CD_AUTORIZACAO_I" DataField="CdAutorizacaoI" />
                    </Columns>
		     </asp:GridView></td>
	    </tr>
        <tr>
            <td><br /></td>
	    </tr>
        <tr>
		<td colspan="4" style="text-align:center"><asp:Button ID="btnSalvar" runat="server" Text="Alterar dados" OnClick="btnSalvar_Click" />
		</td>
	</tr>
    </table>
</asp:Content>

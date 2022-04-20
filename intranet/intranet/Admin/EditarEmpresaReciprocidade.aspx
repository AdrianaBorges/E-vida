<%@ Page Title="Empresa Conveniada para Reciprocidade" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="EditarEmpresaReciprocidade.aspx.cs" Inherits="eVidaIntranet.Admin.EditarEmpresaReciprocidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
		    $('#<%= txtCep.ClientID %>').mask("99.999-999", { placeholder: " " });

		    $('#<%= txtCodigo.ClientID %>').ForceNumericOnly();

            $('#<%= txtTelefone.ClientID %>').mask("(99) 9999-9999");
            $('#<%= txtFax.ClientID %>').mask("(99) 9999-9999");
            $('#<%= txtFax2.ClientID %>').mask("(99) 9999-9999");

            $("input:text").css("text-transform", "uppercase");

            $("#<%=txtUrlGuia.ClientID%>").css("text-transform", "lowercase");
        }

        function openNova() {
            window.location = './EditarEmpresaReciprocidade.aspx';
        }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table id="conteudo" runat="server" width="800px" cellspacing="10" style="vertical-align:top">
	    <tr>
            <td >Código:</td>
            <td><asp:TextBox ID="txtCodigo" runat="server" Width="75px" MaxLength="2" /></td>	    
		    <td >Nome:</td>
		    <td align="left"><asp:TextBox ID="txtNome" runat="server" Width="220px" MaxLength="60" /></td>
	    </tr>
        <tr>
		    <td >Área de atuação:</td>
		    <td align="left" colspan="3"><asp:CheckBoxList ID="chkLstArea" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                CellPadding="3" CellSpacing="10"
                DataValueField="sigla" DataTextField="nome" /></td>
	    </tr>
        <tr>
            <td colspan="4">
                <table width="100%" cellpadding="5px" cellspacing="5px" style="border: 1px solid black">
                    <tr>
                        <td>
                            <b>Endereço</b>
                        </td>
                    </tr>
                    <tr>
                        <td>CEP:</td>
                        <td><asp:TextBox ID="txtCep" runat="server" MaxLength="14" Width="120px" AutoPostBack="true" OnTextChanged="txtCep_TextChanged" /></td>
                    </tr>
                    <tr>
                        <td>Endereço:</td>
                        <td><asp:TextBox ID="txtEndereco" runat="server" Width="400" Enabled="false" /></td>
			            <td>Número:</td>
                        <td><asp:TextBox ID="txtNumero" runat="server" MaxLength="10" Width="80px" /></td>
                    </tr>
                    <tr>
                        <td>Complemento:</td>
			            <td><asp:TextBox ID="txtComplemento" runat="server" MaxLength="50" Width="400px" /></td>
                    </tr>
                    <tr>
                        <td>Bairro/Distrito:</td>
                        <td><asp:TextBox ID="txtBairro" runat="server" Width="220px" Enabled="false" /></td>
                    </tr>
                    
                    <tr>
			
			            <td>Cidade:</td>
                        <td><asp:TextBox ID="txtCidade" runat="server" Width="220px" Enabled="false" /></td>
			            <td>UF:</td>
                        <td><asp:TextBox ID="txtUf" runat="server" MaxLength="2" Width="50px" Enabled="false" /></td>

		            </tr>
                </table>
            </td>
		</tr>
        <tr>
		    <td >Email Principal:</td>
		    <td align="left" colspan="3"><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="450px" /></td>
        </tr>
        <tr>
		    <td >Email 2:</td>
		    <td align="left" colspan="3"><asp:TextBox ID="txtEmail2" runat="server" MaxLength="100" Width="450px" /></td>
        </tr>
        <tr>
		    <td >Email 3:</td>
		    <td align="left" colspan="3"><asp:TextBox ID="txtEmail3" runat="server" MaxLength="100" Width="450px" /></td>
        </tr>
        <tr>
		    <td >Email 4:</td>
		    <td align="left" colspan="3"><asp:TextBox ID="txtEmail4" runat="server" MaxLength="100" Width="450px" /></td>
        </tr>
        <tr>
		    <td >Email 5:</td>
		    <td align="left" colspan="3"><asp:TextBox ID="txtEmail5" runat="server" MaxLength="100" Width="450px" /></td>
        </tr>
        <tr>
		    <td >Email 6:</td>
		    <td align="left" colspan="3"><asp:TextBox ID="txtEmail6" runat="server" MaxLength="100" Width="450px" /></td>
        </tr>
        <tr>
		    <td >Telefone Principal:</td>
		    <td align="left"><asp:TextBox ID="txtTelefone" runat="server" MaxLength="14" Width="100px" /></td>
            <td >Fax Principal:</td>
		    <td align="left"><asp:TextBox ID="txtFax" runat="server" MaxLength="14" Width="100px" /></td>
	    </tr>
        <tr>
		    <td >Telefone 2 (formato livre):</td>
		    <td align="left"><asp:TextBox ID="txtTelefone2" runat="server" MaxLength="14" Width="100px" /></td>
            <td >Fax 2:</td>
		    <td align="left"><asp:TextBox ID="txtFax2" runat="server" MaxLength="14" Width="100px" /></td>
	    </tr>
        <tr>
		    <td >Telefone 3 (formato livre):</td>
		    <td align="left"><asp:TextBox ID="txtTelefone3" runat="server" MaxLength="14" Width="100px" /></td>
	    </tr>
        <tr>
            <td>URL para Guia Médico:</td>
            <td colspan="3"><asp:TextBox ID="txtUrlGuia" runat="server" MaxLength="255" Width="400px" /></td>
        </tr>
        <tr>
            <td>Contato:</td>
            <td colspan="3"><asp:TextBox ID="txtContato" runat="server" MaxLength="60" Width="300px" /></td>
        </tr>
        <tr>
            <td>Área do Contato:</td>
            <td><asp:TextBox ID="txtAreaContato" runat="server" MaxLength="60" Width="200px" /></td>
            <td>Função do Contato:</td>
            <td><asp:TextBox ID="txtFuncaoContato" runat="server" MaxLength="60" Width="200px" /></td>
        </tr>
        <tr>
            <td><br /></td>
	    </tr>
        <tr>
		<td colspan="4" style="text-align:center">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
            <asp:Button ID="btnNova" runat="server" Text="Incluir Nova" OnClientClick="openNova()" Visible="false" />
		</td>
	</tr>
    </table>
</asp:Content>

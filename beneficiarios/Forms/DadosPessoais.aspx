<%@ Page Title="Dados Pessoais" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="DadosPessoais.aspx.cs" Inherits="eVidaBeneficiarios.Forms.DadosPessoais" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><script type="text/javascript">
        function ConfigControlsCustom() {
            $('#<%= txtCep.ClientID %>').mask("99.999-999", { placeholder: " " });
            $('#<%= txtDddCelular.ClientID %>').ForceNumericOnly();
            $('#<%= txtDddComercial.ClientID %>').ForceNumericOnly();
            $('#<%= txtDddResidencial.ClientID %>').ForceNumericOnly();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo">
        <h2>Dados Principais</h2><br />
        <table width="900px">
            <tr>
                <td><b>EMPRESA</b><br />
                    <asp:Label ID="lblEmpresa" runat="server" />
                </td>
                <td><b>MATRÍCULA</b><br />
                    <asp:Label ID="txtMatricula" runat="server" />
                </td>
                <td width="400px">
                    <b>NOME</b><br />
                    <asp:Label ID="txtNome" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="3"><b>E-MAIL</b><br />
                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" />

                </td>
            </tr>
        </table>
        <h2>Endereço</h2>
        <table width="900px">
            <tr>
                <td><b>CEP</b><br />
                    <asp:TextBox ID="txtCep" runat="server" MaxLength="14" Width="120px" AutoPostBack="true" OnTextChanged="txtCep_TextChanged" />
                </td>
                <td colspan="2"><b>ENDEREÇO</b><br />
                    <asp:TextBox ID="txtEndereco" runat="server" Width="400" MaxLength="80" Enabled="false" />
                </td>
                <td><b>Nº</b><br />
                    <asp:TextBox ID="txtNumero" runat="server" MaxLength="6" Width="80px" />
                </td>
            </tr>
            <tr>
                <td><b>COMPLEMENTO</b><br />
                    <asp:TextBox ID="txtComplemento" runat="server" Width="220px" MaxLength="40" />
                </td>
                <td><b>BAIRRO</b><br />
                    <asp:TextBox ID="txtBairro" runat="server" Width="200" MaxLength="70" Enabled="false" />
                </td>
                <td><b>UF</b><br />
                    <asp:DropDownList ID="dpdUf" runat="server" Width="100px" DataTextField="sigla" DataValueField="sigla" AutoPostBack="true" OnSelectedIndexChanged="dpdUf_SelectedIndexChanged" Enabled="false" />
                </td>
                <td><b>MUNÍCIPIO</b><br />
                    <asp:DropDownList ID="dpdMunicipio" runat="server" Width="230px" DataTextField="BID_DESCRI" DataValueField="BID_CODMUN" Enabled="false" />
                </td>
            </tr>
        </table>
        
        <h2>Telefones</h2>
        <table width="900px">
            <tr>
                <td><b>Residencial</b><br />
                    <asp:TextBox ID="txtDddResidencial" runat="server" MaxLength="3" CssClass="inteiro" Width="50" />
                    <asp:TextBox ID="txtTelResidencial" runat="server" MaxLength="9" CssClass="inteiro" Width="150" />
                </td><td><b>Comercial</b><br />
                    <asp:TextBox ID="txtDddComercial" runat="server" MaxLength="3" CssClass="inteiro" Width="50" />
                    <asp:TextBox ID="txtTelComercial" runat="server" MaxLength="9" CssClass="inteiro" Width="150" />
                </td><td><b>Celular</b><br />
                    <asp:TextBox ID="txtDddCelular" runat="server" MaxLength="3" CssClass="inteiro" Width="50" />
                    <asp:TextBox ID="txtTelCelular" runat="server" MaxLength="9" CssClass="inteiro" Width="150" />
                </td>
            </tr>
        </table>

        <%--
	    <h2>Dados Bancários</h2>
	    <table width="900px">
		    <tr>
			    <td>
				    <b>BANCO</b><br />
				    <asp:DropDownList ID="dpdBanco" runat="server" Width="350px" />
			    </td>
			    <td>
				    <table>
					    <tr>
						    <td colspan="2">
							    <b>AGÊNCIA</b>
						    </td>
						    <td>
							    <b>DV</b>
						    </td>
					    </tr>
					    <tr>
						    <td><asp:TextBox ID="txtAgencia" runat="server" MaxLength="20" Width="150px" /></td>
						    <td>-</td>
						    <td><asp:TextBox ID="txtDvAgencia" runat="server" MaxLength="1" Width="50px" /></td>
					    </tr>
				    </table>				
			    </td>
			    <td>
				    <table>
					    <tr>
						    <td colspan="2">
							    <b>CONTA CORRENTE</b>
						    </td>
						    <td>
							    <b>DV</b>
						    </td>
					    </tr>
					    <tr>
						    <td><asp:TextBox ID="txtConta" runat="server" MaxLength="20" Width="150px" OnTextChanged="txtConta_TextChanged" AutoPostBack="true" /></td>
						    <td>-</td>
						    <td><asp:TextBox ID="txtDvConta" runat="server" MaxLength="1" Width="50px" OnTextChanged="txtDvConta_TextChanged" AutoPostBack="true" /></td>
					    </tr>
				    </table>
			    </td>
		    </tr>
	    </table>
        --%>


        <br />
        <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
    </div>
</asp:Content>
